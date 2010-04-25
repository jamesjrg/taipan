#many settings are read in from the XML config file shared with C#
class Settings:
    xmlConfigFile = '../Common/Common.config'
        
    #Settings common to Commodity Prices, Stock Prices, FX Rates:	
    nTopUpdate = 20
    
    #Geometric Brownian Motion constants
    gbmNTicks = 10
