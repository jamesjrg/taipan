class Settings:
    #global settings
    connectString = "Driver={SQL Server};Server=DAPHNE-DURON\\SQLEXPRESS;Database=TaiPan;UID=taipan-r;PWD=fakepass;"
    
    #Settings common to Commodity Prices, Stock Prices, FX Rates:	
    constantUpdateInterval = 1000
    nTopUpdate = 30
    
    #Geometric Brownian Motion constants
    percentageDrift = 1
    percentageVolatility = 10
    gbmTickLength = 5
    gbmNTicks = 10
