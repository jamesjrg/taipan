class Settings:
    #global settings
    connectString = "Driver={SQL Server};Server=DAPHNE-DURON\\SQLEXPRESS;Database=TaiPan;UID=taipan-r;PWD=fakepass;"
    
    #Settings common to Commodity Prices, Stock Prices, FX Rates:	
    constantUpdateInterval = 1000
    nTopUpdate = 30
    monteCarloTickLength = 500
	
