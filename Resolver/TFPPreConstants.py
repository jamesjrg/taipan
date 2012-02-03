#This code based on work by Nathan Brixius, see README

from System.Data import DataSet,DataTable,DataRow

#for AsEnumerable
clr.AddReference('System.Data.DataSetExtensions')
from System.Data.DataTableExtensions import *

import clr
clr.AddReference('Microsoft.Solver.Foundation')
from Microsoft.SolverFoundation.Services import *

from TSPTestData import getTestData
from arc import Arc

#taken from Solver Foundation bankshiftsheduling example - use .NET implicit cast from built in type to Term
def const(i):
	return Term.op_Implicit(i)

def solveTFP():
    #get parameters from spreadsheet and database:
    
    arcs, cityList, portsNamesMap = getPortArcs()
    
    #alternative static test data
    #arcs, cityList = getTestData()
    
    #make arcs into a typed array so .NET will let us do binding:
    
    ds = DataSet()
    table = ds.Tables.Add("arcs")
    table.Columns.Add("City1",int)
    table.Columns.Add("City2",int)
    table.Columns.Add("Distance",float)
    
    for a in arcs:
        table.Rows.Add(a.city1, a.city2, a.distance)
        
    #And now actual Solver calls:

    #Set up model
    context = SolverContext.GetContext();
    model = context.CreateModel();
    
    #Parameters
    city = Set(Domain.IntegerNonnegative, "city")
    dist = Parameter(Domain.Real, "dist", city, city)
    
    dist.SetBinding[DataRow](AsEnumerable(table), "Distance", "City1", "City2")
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
    
    print " ->".join([str(t) for t in tours])
    
    tourNames = [portsNamesMap[t] for t in tours]
        
    TFPSheet.FillRange(tourNames, 2, 11, 2, len(tourNames) + 11)    
 
def setPortDropdowns():
    possibleNames = ['']
    possibleNames.extend(portNames)
    for i in range(1,9):
        getattr(TFPSheet.Cells, "B%d" % i).DropdownItems = possibleNames
        
def getPortArcs():
    portsList = []
    portsNamesMap = {}
    portsIDsMap = {}
       
    portsRange = TFPSheet.Cells.B1.to.B8
    for port in portsRange:
        if port:
            portsList.append(port)
    
    #convert to continuous range of ints for Solver
    for i, port in enumerate(portsList):
        portsNamesMap[i] = port
        portsIDsMap[port] = i
    
    #add quotes for SQL
    portsList = ["'%s'" % port for port in portsList]
    portsStr = ", ".join(portsList)
    
    arcs = []
    
    for pName in portsList:
        data = queryDb(
"""select p.Name, o.Name, p.Location.STDistance(o.Location)
from Port p, Port o
where p.Name = @Name and p.ID != o.ID""", {"Name": pName})
              
        #So [0, x] is p.Name, etc
        for i in range(data.GetLength(1)):
            arcs.append(Arc(portsIDsMap[data[0, i]], portsIDsMap[data[1, i]], data[2, i]))

    return arcs, portsList, portsNamesMap
        
setPortDropdowns()


