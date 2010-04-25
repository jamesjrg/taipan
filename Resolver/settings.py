class Settings:
    #global settings
    connectString = "Driver={SQL Server};Server=DAPHNE-DURON\\SQLEXPRESS;Database=TaiPan;UID=taipan-r;PWD=fakepass;"
    
    #Settings common to Commodity Prices, Stock Prices, FX Rates:	
    mainLoopTick = 5000
    nTopUpdate = 30
    
    #Geometric Brownian Motion constants
    tickVolatility = 0.005
    gbmNTicks = 10
