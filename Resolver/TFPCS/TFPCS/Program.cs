using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SolverFoundation.Services;

namespace Microsoft.SolverFoundation.Samples.TravelingSalesman
{
    public static class ModelingExtensions
    {
        public static void AssignmentConstraintsNoDiag(this Model model, Set s, Decision assign)
        {
            model.AddConstraint("A1", Model.ForEach(s, i => Model.Sum(Model.ForEachWhere(s, j => assign[i, j], j => i != j)) == 1));
            model.AddConstraint("A2", Model.ForEach(s, j => Model.Sum(Model.ForEachWhere(s, i => assign[i, j], i => i != j)) == 1));
        }
    }

    class Program
    {
        // TSP coordinate.
        public class Coordinate
        {
            public int Name { get; set; }

            // X-coordinate (from TSPLIB)
            public double X { get; set; }

            // Y-coordinate (from TSPLIB)
            public double Y { get; set; }

            public Coordinate(int name, double x, double y)
            {
                Name = name;
                X = x;
                Y = y;
            }

            // Latitude in radians.
            public double Latitude
            {
                get { return Math.PI * (Math.Truncate(X) + 5 * (X - Math.Truncate(X)) / 3) / 180; }
            }

            // Longitude in radians.
            public double Longitude
            {
                get { return Math.PI * (Math.Truncate(Y) + 5 * (Y - Math.Truncate(Y)) / 3) / 180; }
            }

            // Geographic distance between two points (as an integer).
            public int Distance(Coordinate p)
            {
                double q1 = Math.Cos(Longitude - p.Longitude);
                double q2 = Math.Cos(Latitude - p.Latitude);
                double q3 = Math.Cos(Latitude + p.Latitude);
                // There may rounding difficulties her if the points are close together...just sayin'.
                return (int)(6378.388 * Math.Acos(0.5 * ((1 + q1) * q2 - (1 - q1) * q3)) + 1);
            }
        }

        // TSP city-city arc.
        // 
        public class Arc
        {
            public int City1 { get; set; }
            public int City2 { get; set; }
            public double Distance { get; set; }
        }

        // Burma14 from TSPLIB. Optimal tour = 3323.
        private static Coordinate[] data = new Coordinate[] {
          new Coordinate(0, 16.47, 96.10),
          new Coordinate(1, 16.47, 94.44),
          new Coordinate(2, 20.09, 92.54),
          new Coordinate(3, 22.39, 93.37),
          new Coordinate(4, 25.23, 97.24),
          new Coordinate(5, 22.00, 96.05),
          new Coordinate(6, 20.47, 97.02),
          new Coordinate(7, 17.20, 96.29),
          new Coordinate(8, 16.30, 97.38),
          new Coordinate(9, 14.05, 98.12),
          new Coordinate(10, 16.53, 97.38),
          new Coordinate(11, 21.52, 95.59),
          new Coordinate(12, 19.41, 97.13),
          new Coordinate(13, 20.09, 94.55)
        };

        static void Main(string[] args)
        {
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            // ------------
            // Parameters
            Set city = new Set(Domain.IntegerNonnegative, "city");
            Parameter dist = new Parameter(Domain.Real, "dist", city, city);
            var arcs = from p1 in data
                       from p2 in data
                       select new Arc { City1 = p1.Name, City2 = p2.Name, Distance = p1.Distance(p2) };
            dist.SetBinding(arcs, "Distance", "City1", "City2");
            model.AddParameters(dist);

            // ------------
            // Decisions
            Decision assign = new Decision(Domain.IntegerRange(0, 1), "assign", city, city);
            Decision rank = new Decision(Domain.RealNonnegative, "rank", city);
            model.AddDecisions(assign, rank);

            // ------------
            // Goal: minimize the length of the tour.
            Goal goal = model.AddGoal("TourLength", GoalKind.Minimize,
              Model.Sum(Model.ForEach(city, i => Model.ForEachWhere(city, j => dist[i, j] * assign[i, j], j => i != j))));

            // ------------
            // Enter and leave each city only once.
            int N = data.Length;
            model.AddConstraint("assign1",
              Model.ForEach(city, i => Model.Sum(Model.ForEachWhere(city, j => assign[i, j],
                j => i != j)) == 1));
            model.AddConstraint("assign2",
              Model.ForEach(city, j => Model.Sum(Model.ForEachWhere(city, i => assign[i, j], i => i != j)) == 1));
            model.AssignmentConstraintsNoDiag(city, assign);

            // Forbid subtours (Miller, Tucker, Zemlin - 1960...)
            model.AddConstraint("no_subtours",
              Model.ForEach(city,
                i => Model.ForEachWhere(city,
                  j => rank[i] + 1 <= rank[j] + N * (1 - assign[i, j]),
                  j => Model.And(i != j, i >= 1, j >= 1)
                )
              )
            );

            Solution solution = context.Solve();

            // Retrieve solution information.
            Console.WriteLine("Cost = {0}", goal.ToDouble());
            Console.WriteLine("Tour:");
            var tour = from p in assign.GetValues() where (double)p[0] > 0.9 select p[2];
            foreach (var i in tour.ToArray())
            {
                Console.Write(i + " -> ");
            }
            Console.WriteLine();
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }
    }
}