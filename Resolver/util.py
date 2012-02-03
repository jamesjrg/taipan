from System.Reflection import Assembly
import clr
import os

from System.Data import *
from System import Array

#assembly loading
def loadAssembly(relPath):
    #NOTE NOTE NOTE if you include a forward slash here then the path fails to match up with the path the dll considers itself to live at,
    #which means that if the dll is a web service proxy it will fall over with a cryptic error message
    dll = os.path.dirname(__file__) + "\\" + relPath
    print "Loading: %s" % dll
    assembly = Assembly.LoadFile(dll)
    clr.AddReference(assembly)

      
        
        