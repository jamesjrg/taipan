using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolverBindingClasses
{
    public class Arc
    {
        public Arc(int City1, int City2, double Distance)
        {
            this.City1 = City1;
            this.City2 = City2;
            this.Distance = Distance;
        }

        public int City1 { get; set; }
        public int City2 { get; set; }
        public double Distance { get; set; }
    }
}
