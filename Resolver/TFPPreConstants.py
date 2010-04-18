import clr
clr.AddReference('Microsoft.Solver.Foundation')
clr.AddReference('System.Data')
clr.AddReference('System.Data.DataSetExtensions')

from Microsoft.SolverFoundation.Services import *
from System.Data import DataSet,DataTable,DataRow
from System.Data.DataTableExtensions import *
from System.IO import *
from random import randint

# OML Model string
strModel = "Model[Parameters[Sets,I,J],Parameters[Integers,d[I,J]],Decisions[Integers[1,9],x[I,J]],Constraints[Foreach[{i,I},{j,J},x[i,j]==d[i,j] | d[i,j]==0],Foreach[{i,I},Unequal[Foreach[{j,J},x[i,j]]]],Foreach[{j,J},Unequal[Foreach[{i,I},x[i,j]]]],Foreach[{ib,3},Foreach[{jb,3},Unequal[Foreach[{i,ib*3,ib*3+3},{j,jb*3,jb*3+3},x[i,j]]]]]]]"
context = SolverContext.GetContext();
context.LoadModel(FileFormat.OML, StringReader(strModel));

def solveTFP():
    p = context.CurrentModel.Parameters
    for param in p:
        if param.Name == "d":
            ds = DataSet()
            table = ds.Tables.Add("board")
            table.Columns.Add("Row",int)
            table.Columns.Add("Col",int)
            table.Columns.Add("Value",int)
            #the grid read logic and update it
            sheet1 = workbook['Sheet1']
            
            for i in range(2,11):
                for j in range(6,15):
                    if sheet1.Cells[i,j].Value == Empty:
                        value = 0
                    else:
                        value = sheet1.Cells[i,j].Value
                    table.Rows.Add(i-2,j-6,value)
            param.SetBinding[DataRow](AsEnumerable(table),"Value","Row","Col")
    
    solution = context.Solve()
    r = solution.Decisions
    for w in r:
        e = w
    s = e.ToString().split(', ')
    import re
    pattern = re.compile(r'^\D*(\d{1})\D*(\d{1})\D*(\d{1})\D*(\d*)$')
    for f in s:
        [i,j,v,a] = pattern.search(f).groups()
        sheet1.Cells[int(i)+2,int(j)+17].Value = int(v)
        
        
        
        