import sys
#for ssl needed by ZSI
sys.path.extend([Settings.pythonLibDir, Settings.pythonDllDir, Settings.pythonSiteDir]) 
import ironclad
import bz2
import ZSI
#otherwise directories get multiplied each time you recalculate
sys.path.remove(Settings.pythonLibDir).remove(Settings.pythonDllDir).remove(Settings.pythonSiteDir)

def runSort():
    arr = Array[int]([3,2,4,7,1,2])
    #ret = algoService.Sort("insertion", arr)
    #print ret.time
    #print ret.sortedData
    #ret, specified = algoService.Search(1, True, 1, True)
    #print ret
    
