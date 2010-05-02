wsdl.exe  /out:AlgoServiceProxy.cs /namespace:AlgoService http://localhost:8731/Design_Time_Addresses/AlgoService/
csc.exe /target:library AlgoServiceProxy.cs