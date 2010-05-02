svcutil.exe /config:AlgoService.config http://localhost:8731/Design_Time_Addresses/AlgoService/
csc.exe /out:AlgoServiceProxy.dll /target:library AlgoService.cs