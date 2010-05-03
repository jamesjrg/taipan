#This code based on work by Nathan Brixius, see README

#Solver Foundation won't let you bind parameters to IronPython objects
#N.B. assume SolverBindingClasses.dll already loaded
from SolverBindingClasses import Arc

import math

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
       
# Burma14 from TSPLIB. Optimal tour = 3323.
TSPTestData = [
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

def getTestData():
    arcs = []
    for p1 in TSPTestData:
        for p2 in TSPTestData:
            arcs.append(Arc(p1.name, p2.name, p1.distance(p2)))
            
    return arcs, TSPTestData
    
    