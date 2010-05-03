#This code based on work by Nathan Brixius, see README

import clr
clr.AddReference('Microsoft.Solver.Foundation')
from Microsoft.SolverFoundation.Services import *

#Solver Foundation won't let you bind parameters to IronPython objects
loadAssembly(".\\SolverBindingClasses\\SolverBindingClasses\\bin\\Debug\\SolverBindingClasses.dll")
from SolverBindingClasses import Arc

from TSPTestData import getTestData

###taken from Solver Foundation bankshiftsheduling example
def const(i):
	return Term.op_Implicit(i)

def solveTFP():
    #get parameters from spreadsheet and database
    #arcs, cityList = getPortArcs()
    
    #alternative static test data
    arcs, cityList = getTestData()
    
    #make it typed so .NET will let us do binding
    arcs = Array[Arc](arcs)

    #Set up model
    context = SolverContext.GetContext();
    model = context.CreateModel();
    
    #Parameters
    city = Set(Domain.IntegerNonnegative, "city")
    dist = Parameter(Domain.Real, "dist", city, city)
    
    dist.SetBinding(arcs, "Distance", "City1", "City2")
    model.AddParameters(dist)
    
    #Decisions
    assign = Decision(Domain.IntegerRange(0, 1), "assign", city, city)
    rank = Decision(Domain.RealNonnegative, "rank", city)
    model.AddDecisions(assign, rank)
    
    #Goal: minimize length of tour
    goal = model.AddGoal("TourLength", GoalKind.Minimize,
              Model.Sum(Model.ForEach(city, lambda i: Model.ForEachWhere(city, lambda j: dist[i, j] * assign[i, j], lambda j: i != j))))

    #Enter and leave each city exactly once
    #note need const (defined above) to convert ints to Solver Term class
    model.AddConstraint("assign1",
              Model.ForEach(city, lambda i: Model.Sum(Model.ForEachWhere(city, lambda j: assign[i, j],
                lambda j: i != j)) == const(1)))
    model.AddConstraint("assign2",
              Model.ForEach(city, lambda j: Model.Sum(Model.ForEachWhere(city, lambda i: assign[i, j],
                lambda i: i != j)) == const(1)))
    
    #no subtours
    nCities = len(cityList)
    
    model.AddConstraint("no_subtours",
              Model.ForEach(city,
                lambda i: Model.ForEachWhere(city,
                  lambda j: rank[i] + const(1) <= rank[j] + const(nCities) * (const(1) - assign[i, j]),
                  lambda j: Model.And(i != j, i >= const(1), j >= const(1))
                )
              )
            )
    
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
        """select p.ID, o.ID, p.Location.STDistance(o.Location), p.Name, o.Name from Port p, Port o
where p.Name = %s and o.Name in (%s)""" % (p, otherPorts))
              
        #So [0, x] is p.ID, [1,x] is o.ID, [2,x] is distance, [3,x] is p.Name, [4,x] is o.Name, 
        for i in range(data.GetLength(1)):
            arcs.append(Arc(data[0, i], data[1, i], data[2, i]))

    return arcs, portsList
        
setPortDropdowns()


