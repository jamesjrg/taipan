loadAssembly("../AlgoServer/AlgoService/bin/Debug/AlgoService.dll")
from AlgoService import AlgoService

algoService = AlgoService()

def runSort():
    arr = Array[int]([3,2,4,7,1,2])
    ret = algoService.Sort("insertion", arr)
    print ret.time
    print ret.sortedData
