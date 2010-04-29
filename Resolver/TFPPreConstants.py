#This code based on work by Nathan Brixius, see README

import clr
clr.AddReference('Microsoft.Solver.Foundation')
from Microsoft.SolverFoundation.Services import *

import math

class Coordinate:
    def __init__(self, name, x, y):
        self.name = name
        self.x = x
        self.y = y

    #Latitude in radians.
    def latitude():
        return math.pi * (math.trunc(self.x) + 5 * (self.x - math.trunc(self.x)) / 3) / 180

    #Longitude in radians.
    def longitude():
        return math.pi * (math.trunc(self.y) + 5 * (self.y - math.trunc(self.y)) / 3) / 180

    #Geographic distance between two points (as an integer).
    def distance(p):
        q1 = math.cos(self.longitude() - p.longitude())
        q2 = math.cos(self.latitude() - p.latitude())
        q3 = math.cos(self.latitude() + p.latitude())
        #There may rounding difficulties her if the points are close together...just sayin'.
        return int(6378.388 * math.acos(0.5 * ((1 + q1) * q2 - (1 - q1) * q3)) + 1)

class Arc:
    city1 = None
    city2 = None
    distance = None
    
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
    
    #Solve
    solution = context.Solve()
    
    #Print result
    print "Cost = %f" % goal.ToDouble()
    print "Tour:"
    
    #XXX should probably be a generator, maybe
    tours = [p[2] for p in assign.GetValues() if p[0] > 0.9]
    
    for tour in tours:
        print "%s ->" % tour
         
        
        
        