loadAssembly("..\\StatsLib\\bin\\Debug\\StatsLib.dll")
from TaiPan.StatsLib import StatsLib

from rslWPFChart import *

#globals
stats = StatsLib()

portIDs = [None,None,None]
commodChart = None

USDID = None
currencyIds = [None,None,None]
fxChart = None

#util
def ItaliciseRange(sheet, startCellStr, endCellStr):
    start = getattr(sheet.Cells, "%s" % (startCellStr))
    end = getattr(sheet.Cells, "%s" % (endCellStr))
    CellRange(start, end).Italic = True
    
def getForecastStartRow():
    return Settings.nTopUpdate + 2
    
def getForecastEndRow():
    return Settings.nTopUpdate + Settings.gbmNTicks + 2    
            
#Commodity/FX shared
        
def createBrownian(currentPrice):
    print 'createBrownian: firstVal:%f, volatility:%f nticks:%d' % (currentPrice, Settings.config['TickVolatility'], Settings.gbmNTicks)
    seq = stats.GBMSequence(currentPrice, Settings.config['TickVolatility'], Settings.gbmNTicks)
    for i in range(len(seq)):
        seq[i] = round(seq[i], Settings.config['CurrencyAccuracy'])
    return seq

def addForecastTimes(sheet, startRow, endRow):
    #times
    currTime = getattr(sheet, "A%d" % (Settings.nTopUpdate + 1))
    times = []
    for i in range(Settings.gbmNTicks):
        currTime = currTime.AddSeconds(Settings.config['MainLoopTick'] / 1000)
        times.append(currTime)
    sheet.FillRange(times, 1, startRow, 1, endRow)
    ItaliciseRange(sheet, "A%d" % (startRow), "A%d" % (endRow))

# Commodity Prices

def setPortNames():
    data = queryDb("select name from Commodity order by name asc")
    commodNames = [row for row in data]
    
    commoditySheet.Cells.I1.DropdownItems = commodNames
    
    #port names set globally  

    commoditySheet.Cells.I2.DropdownItems = portNames
    commoditySheet.Cells.I3.DropdownItems = portNames
    commoditySheet.Cells.I4.DropdownItems = portNames
    
def updateCommodityPrices():
    commoditySheet.B1 = commoditySheet.I2
    commoditySheet.C1 = commoditySheet.I3
    commoditySheet.D1 = commoditySheet.I4
    
    commodityID = queryDb("select ID from Commodity where Name= @CommodName",
    {"CommodName": commoditySheet.I1})[0,0]
    ids = queryDb("select ID from Port where Name in (@Name1, @Name2, @Name3) order by name ASC",
    {"Name1": commoditySheet.I2, "Name2": commoditySheet.I3, "Name3": commoditySheet.I4})
    portIDs[0] = ids[0, 0]
    portIDs[1] = ids[0, 1]
    portIDs[2] = ids[0, 2]
    
    #first query includes dates
    data = queryDb("select ValueDate, Value from (select top (@Limit) ValueDate, dbo.funcGetUSDValueAtDate(LocalPrice, @PortID, ValueDate) as Value from HistoricalPortCommodityPrice where PortID = PortID and CommodityID = @CommodID order by ValueDate DESC) as foo order by ValueDate ASC",
    {"Limit": Settings.nTopUpdate, "PortID": portIDs[0], "CommodID": commodityID})    
    commoditySheet.FillRange(data, 1, 2, 2, Settings.nTopUpdate + 1)
    
    for i in range(1, 3):        
        data = queryDb("select Value from (select (@Limit) ValueDate, dbo.funcGetUSDValue(LocalPrice, @PID) as Value from HistoricalPortCommodityPrice where PortID = @PID and CommodityID = @CID order by ValueDate DESC) as foo order by ValueDate ASC",
        {"Limit": Settings.nTopUpdate, "PID": portIDs[i], "CID": commodityID})
        commoditySheet.FillRange(data, i + 2, 2, i + 2, Settings.nTopUpdate + 1)
    
def commodForecast():
    startRow = getForecastStartRow()
    endRow = getForecastEndRow()
    
    addForecastTimes(commoditySheet, startRow, endRow)
    
    #prices
    for i, id, letter in zip(range(3), portIDs, ['B', 'C', 'D']):   
        currentPrice = getattr(commoditySheet, "%s%d" % (letter, Settings.nTopUpdate + 1))
        forecast = createBrownian(currentPrice)    
        commoditySheet.FillRange(forecast, 2 + i, startRow, 2 + i, endRow)
        
        #style text
        ItaliciseRange(commoditySheet, "%s%d" % (letter, startRow), "%s%d" % (letter, endRow))

def commodGraph():
    global commodChart
    
    title = "Commodity Prices"
    yLabel = "USD"
    
    names = [commoditySheet.B1,commoditySheet.C1,commoditySheet.D1]
    
    startRow = getForecastStartRow()
    endRow = getForecastEndRow()
    
    times = []
    for row in commoditySheet.Rows[2:]:
        times.Add(str(row[1]))
    
    USDValues = [[],[],[]]
    for row in commoditySheet.Rows[2:]:
        USDValues[0].Add(row[2])
        USDValues[1].Add(row[3])
        USDValues[2].Add(row[4])
    
    commodChart = rslWPFChart(title, yLabel, names, times, USDValues)
    commodChart.Start()
    
# FX rates
  
def setFXNames():
    data = queryDb("select name from Currency order by name asc")
    fxNames = [row for row in data]
    
    fxSheet.Cells.I1.DropdownItems = fxNames
    fxSheet.Cells.I2.DropdownItems = fxNames
    fxSheet.Cells.I3.DropdownItems = fxNames
    
def updateFX():
    #xxx do this without string interpolation, though a real pain with .NET
    idsAndCodes = queryDb("select ID, ShortName from Currency where Name in (@Name1, @Name2, @Name3)",
    {"Name1": fxSheet.I1, "Name2": fxSheet.I2, "Name3": fxSheet.I3})
    fxSheet.B1 = idsAndCodes[1,0]
    fxSheet.C1 = idsAndCodes[1,1]
    fxSheet.D1 = idsAndCodes[1,2]
   
    currencyIds[0] = idsAndCodes[0,0]
    currencyIds[1] = idsAndCodes[0,1]
    currencyIds[2] = idsAndCodes[0,2]
    
    for i in range(3):
        if idsAndCodes[1,i] == 'USD':
            USDID = idsAndCodes[0,i]
            break
    
    #first query includes dates
    data = queryDb("select * from (select top (@Limit) ValueDate, USDValue from HistoricalCurrencyPrice where CurrencyID = @ID order by ValueDate DESC) as foo order by ValueDate ASC",
    {"Limit": Settings.nTopUpdate, "ID": currencyIds[0]})
    fxSheet.FillRange(data, 1, 2, 2, Settings.nTopUpdate + 1)
    
    for i in range(1, 3):        
        data = queryDb("select USDValue from (select top (@Limit) ValueDate, USDValue from HistoricalCurrencyPrice where CurrencyID = @ID order by ValueDate DESC) as foo order by ValueDate ASC",
        {"Limit": Settings.nTopUpdate, "ID": currencyIds[i]})
        fxSheet.FillRange(data, i + 2, 2, i + 2, Settings.nTopUpdate + 1)
 
def fxForecast():
    startRow = getForecastStartRow()
    endRow = getForecastEndRow()
    
    #times
    addForecastTimes(fxSheet, startRow, endRow)

    #prices
    for i, id, letter in zip(range(3), currencyIds, ['B', 'C', 'D']):
        #USD doesn't fluctuate vs USD
        if id == USDID:
            continue
    
        currentPrice = getattr(fxSheet, "%s%d" % (letter, Settings.nTopUpdate + 1))
        forecast = createBrownian(currentPrice)    
        fxSheet.FillRange(forecast, 2 + i, startRow, 2 + i, endRow)
        
        #style text
        ItaliciseRange(fxSheet, "%s%d" % (letter, startRow), "%s%d" % (letter, endRow))
    
def fxGraph():
    global fxChart
    title = "FX Prices"
    yLabel = "USD"
    
    names = [fxSheet.B1,fxSheet.C1,fxSheet.D1]
    
    startRow = getForecastStartRow()
    endRow = getForecastEndRow()
    
    times = []
    for row in fxSheet.Rows[2:]:
        times.Add(str(row[1]))

    USDValues = [[],[],[]]
    for row in fxSheet.Rows[2:]:
        USDValues[0].Add(row[2])
        USDValues[1].Add(row[3])
        USDValues[2].Add(row[4])
    
    fxChart = rslWPFChart(title, yLabel, names, times, USDValues)
    fxChart.Start()
        
setPortNames()
setFXNames()
    
    
    
    