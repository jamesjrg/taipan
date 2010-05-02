loadAssembly("AlgoServiceProxy.dll")
from AlgoService import AlgoService

algoService = AlgoService()

def runSort():
    arr = Array[int]([3,2,4,7,1,2])
    ret, specified = algoService.Search(1, True, 1, True)
    print ret, specified
    
    #ret = algoService.Sort("insertion", arr)
    #print ret.time
    #print ret.sortedData
    #ret, specified = algoService.Search(1, True, 1, True)
    #print ret
    
