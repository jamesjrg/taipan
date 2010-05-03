#This code based on work by Nathan Brixius, see README

import clr
clr.AddReference('Microsoft.Solver.Foundation')
from Microsoft.SolverFoundation.Services import *
from TSPTestData import TSPTestData

#should be extension method
def AssignmentConstraintsNoDiag(model, s, assign):
        model.AddConstraint("A1", Model.ForEach(lambda s, i: Model.Sum(Model.ForEachWhere(lambda s, j: assign[i, j], lambda j: i != j)) == 1))
        model.AddConstraint("A2", Model.ForEach(lambda s, i: Model.Sum(Model.ForEachWhere(lambda s, j: assign[i, j], lambda i: i != j)) == 1))

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
        
    def ToString(self):
        return ", ". join((self.city1, self.city2, str(self.distance)))
        
def solveTFP():
    #get input data
    arcs = getPortArcs()
    return

    #Set up model
    context = SolverContext.GetContext();
    model = context.CreateModel();
    
    #Parameters
    city = Set(Domain.IntegerNonnegative, "city")
    dist = Parameter(Domain.Real, "dist", city, city)
    
    #Add all the arcs
    arcs = []
    for p1 in TSPTestData:
        for p2 in TSPTestData:
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
    possibleNames = ['']
    possibleNames.extend(portNames)
    for i in range(1,9):
        getattr(TFPSheet.Cells, "B%d" % i).DropdownItems = possibleNames
        
def getPortArcs():
    portsList = []    
    ports = TFPSheet.Cells.B1.to.B8
    for port in ports:
        if port:
            portsList.append("'%s'" % port)
    portsStr = ", ".join(portsList)
    
    arcs = []
    
    for p in portsList:
        otherPorts = portsList[:]
        otherPorts.remove(p)
        otherPorts = ", ".join(otherPorts)        
        data = queryDb(
        """select p.Name, o.Name, p.Location.STDistance(o.Location) from Port p, Port o
where p.Name = %s and o.Name in (%s)""" % (p, otherPorts))
              
        #So [0, x] is p.Name, [1,x] is o.Name, [2,x] is distance
        for i in range(data.GetLength(1)):
            arcs.append(Arc(data[0, i], data[1, i], data[2, i]))

    return arcs
        
setPortDropdowns()


