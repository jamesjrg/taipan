#This code based on work by Nathan Brixius, see README

import clr
clr.AddReference('Microsoft.Solver.Foundation')
from Microsoft.SolverFoundation.Services import *

import math

#should be extension method
def AssignmentConstraintsNoDiag(model, s, assign):
        model.AddConstraint("A1", Model.ForEach(lambda s, i: Model.Sum(Model.ForEachWhere(lambda s, j: assign[i, j], lambda j: i != j)) == 1))
        model.AddConstraint("A2", Model.ForEach(lambda s, i: Model.Sum(Model.ForEachWhere(lambda s, j: assign[i, j], lambda i: i != j)) == 1))
    
class Coordinate:
    def __init__(self, name, x, y):
        self.name = name
        self.x = x
        self.y = y

    #Latitude in radians.
    def latitude(self):
        return math.pi * (math.trunc(self.x) + 5 * (self.x - math.trunc(self.x)) / 3) / 180

    #Longitude in radians.
    def longitude(self):
        return math.pi * (math.trunc(self.y) + 5 * (self.y - math.trunc(self.y)) / 3) / 180

    #Geographic distance between two points (as an integer).
    def distance(self, p):
        q1 = math.cos(self.longitude() - p.longitude())
        q2 = math.cos(self.latitude() - p.latitude())
        q3 = math.cos(self.latitude() + p.latitude())
        #There may rounding difficulties her if the points are close together...just sayin'.
        return int(6378.388 * math.acos(0.5 * ((1 + q1) * q2 - (1 - q1) * q3)) + 1)

class Arc:
    def __init__(self, city1, city2, distance):
        self.__city1 = city1
        self.__city2 = city2
        self.__distance = distance
    
    @property 
    def city1(self):
        return self.__city1
        
    @property
    def city2(self):
        return self.__city2
    
    @property 
    def distance(self):
        return self.__distance
        
# Burma14 from TSPLIB. Optimal tour = 3323.
data = [
  Coordinate(0, 16.47, 96.10),
  Coordinate(1, 16.47, 94.44),
  Coordinate(2, 20.09, 92.54),
  Coordinate(3, 22.39, 93.37),
  Coordinate(4, 25.23, 97.24),
  Coordinate(5, 22.00, 96.05),
  Coordinate(6, 20.47, 97.02),
  Coordinate(7, 17.20, 96.29),
  Coordinate(8, 16.30, 97.38),
  Coordinate(9, 14.05, 98.12),
  Coordinate(10, 16.53, 97.38),
  Coordinate(11, 21.52, 95.59),
  Coordinate(12, 19.41, 97.13),
  Coordinate(13, 20.09, 94.55)]
        
def solveTFP():
    #Set up model
    context = SolverContext.GetContext();
    model = context.CreateModel();
    
    #Parameters
    city = Set(Domain.IntegerNonnegative, "city")
    dist = Parameter(Domain.Real, "dist", city, city)
    
    #Add all the arcs
    arcs = []
    for p1 in data:
        for p2 in data:
            arcs.append(Arc(p1.name, p2.name, p1.distance(p2)))
        
    dist.SetBinding(arcs, "distance", "city1", "city2")
    model.AddParameters(dist)
    
    #Decisions
    assign = Decision(Domain.IntegerRange(0, 1), "assign", city, city)
    rank = Decision(Domain.RealNonnegative, "rank", city)
    model.AddDecisions(assign, rank)
    
    #Goal: minimize length of tour
    goal = model.AddGoal("TourLength", GoalKind.Minimize,
              Model.Sum(Model.ForEach(lambda city, i: Model.ForEachWhere(city, lambda j: dist[i, j] * assign[i, j], lambda j: i != j))))
              
    N = data.Length
    model.AddConstraint("assign1",
              Model.ForEach(lambda city, i: Model.Sum(Model.ForEachWhere(lambda city, j: assign[i, j],
                lambda j: i != j)) == 1))
    model.AddConstraint("assign2",
              Model.ForEach(lambda city, j: Model.Sum(Model.ForEachWhere(lambda city, i: assign[i, j], lambda i: i != j)) == 1))
    model.AssignmentConstraintsNoDiag(city, assign)
              
    #Solve
    solution = context.Solve()
    
    #Print result
    print "Cost = %f" % goal.ToDouble()
    print "Tour:"
    
    tours = [p[2] for p in assign.GetValues() if p[0] > 0.9]
    
    for tour in tours:
        print "%s ->" % tour
        
    TFPSheet.FillRange(tours, 2, 11, 2, len(tours) + 11)    
 
def setPortDropdowns():
    possibleNames = ['None']
    possibleNames.extend(portNames)
    for i in range(1,9):
        getattr(TFPSheet.Cells, "B%d" % i).DropdownItems = possibleNames
        
setPortDropdowns()


