loadAssembly("AlgoServiceProxy.dll")
from AlgoService import AlgoService

import random

algoService = AlgoService()

#ignore this function, it is for if you wish to toy with the data structures provided by the service
def search():
    #"specified" return value and the booleans including as arguments are some silliness forced by the SOAP proxy generation (to do with use of .NET value types)
    ret, specified = algoService.Search(1, True, 1, True)
    print ret, specified

def runSort():
    sortingSheet.B1 = sortingSheet.G1
    sortingSheet.C1 = sortingSheet.G2
    sortingSheet.D1 = sortingSheet.G3
    
    arr = range(sortingSheet.G5)
    random.seed()
    random.shuffle(arr)
    
    iterations = sortingSheet.G6
    
    runTimes = []
    for i in range(0,3):
        alg = getattr(sortingSheet.Cells, "G%d" % (i+1)).Value
        #boolean including as argument is some silliness forced by the SOAP proxy generation (to do with use of .NET value types)
        ret = algoService.TimeSort(alg, Array[int](arr), iterations, True)
        runTimes.append(ret.time)
        print 'Iteration took: %d' % ret.time
    
    for time, letter in zip(runTimes, ('B', 'C', 'D')):
        sortingSheet[letter, 2] = time        
    
def setAlgDropdowns():
    possibleNames = ['insertion', 'merge', 'heap', 'quick', 'randomizedquick', 'counting']
    for i in range(1,4):
        getattr(sortingSheet.Cells, "G%d" % i).DropdownItems = possibleNames
        
setAlgDropdowns()