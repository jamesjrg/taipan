wsdl.exe  /out:AlgoServiceProxy.cs /namespace:AlgoService http://localhost:8731/Design_Time_Addresses/AlgoService/
:: Resolver needs assemblies to be built with 3.5
C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe /target:library AlgoServiceProxy.cs