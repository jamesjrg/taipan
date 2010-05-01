from System import Array
from System.Data import DataSet
from System.Data.Odbc import OdbcConnection, OdbcDataAdapter
from System.Xml import XmlReader, XmlNodeType

#load in some assemblies
from System.Reflection import Assembly
import clr
def loadAssembly(relPath):
    dll = os.path.dirname(__file__) + '/' + relPath
    print "Loading: %s" % dll
    assembly = Assembly.LoadFile(dll)
    clr.AddReference(assembly)

loadAssembly("../StatsLib/bin/Debug/StatsLib.dll")
from TaiPan.StatsLib import StatsLib

loadAssembly("../AlgoServer/AlgoService/bin/Debug/AlgoService.dll")
from AlgoService import AlgoService

#init some variables

stats = StatsLib()
algoService = AlgoService()

commoditySheet = workbook['Commodity Prices']
fxSheet = workbook['FX Rates']

futuresSheet = workbook['Futures']
shippingSheet = workbook['Shipping']
countrySheet = workbook['Country summary']
shippingSumSheet = workbook['Shipping summary']

TFPSheet = workbook['Travelling Freighter Problem']
sortingSheet = workbook['Sorting Algorithms']

#util
def ItaliciseRange(startCellStr, endCellStr):
    start = getattr(fxSheet.Cells, "%s" % (startCellStr))
    end = getattr(fxSheet.Cells, "%s" % (endCellStr))
    CellRange(start, end).Italic = True
    
def getForecastStartRow():
    return Settings.nTopUpdate + 2
    
def getForecastEndRow():
    return Settings.nTopUpdate + Settings.gbmNTicks + 2
    
#db functions

def queryDb(query):
    print 'DB query: %s' % query
    connection = OdbcConnection(Settings.connectString)
    adaptor = OdbcDataAdapter(query, connection)
    dataSet = DataSet()
    connection.Open()
    adaptor.Fill(dataSet)    
    connection.Close()
    if len(dataSet.Tables) > 0 and len(dataSet.Tables[0].Rows) > 0:
        ret = Array.CreateInstance(object, len(dataSet.Tables[0].Rows[0].ItemArray), len(dataSet.Tables[0].Rows))
        for rowIndex, row in enumerate(dataSet.Tables[0].Rows):
            for colIndex, val in enumerate([item for item in row.ItemArray]):
                ret[colIndex, rowIndex] = val
        return ret

#init
def readConfig():
    try:
        reader = XmlReader.Create(Settings.xmlConfigFile)
    except:
        raise Exception("Couldn't find Common.config at %s" % Settings.xmlConfigFile)
        
    while reader.Read():
        if reader.NodeType == XmlNodeType.Element and reader.Name == 'add' and reader.HasAttributes:
        
            attribs = {}
            while reader.MoveToNextAttribute():
                attribs[reader.Name] = reader.Value
                
            if 'key' in attribs:
                key = attribs['key']
                if key == 'TickVolatility':
                    Settings.config[key] = float(attribs['value'])
                else:
                    Settings.config[key] = int(attribs['value'])
            elif 'name' in attribs and attribs['name'] == 'taipan-r':
                Settings.connectString = 'Driver={SQL Server};' + attribs['connectionString'].replace(' ', '')
               
#GBM functions        
        
def createBrownian(currentPrice):
    print 'createBrownian: firstVal:%f, volatility:%f nticks:%d' % (currentPrice, Settings.config['TickVolatility'], Settings.gbmNTicks)
    seq = stats.GBMSequence(currentPrice, Settings.config['TickVolatility'], Settings.gbmNTicks)
    for i in range(len(seq)):
        seq[i] = round(seq[i], Settings.config['CurrencyAccuracy'])
    return seq

# Commodity Prices

portIDs = [None,None,None]

def setPortNames():
    data = queryDb("select name from Commodity order by name asc")
    commodNames = [row for row in data]
    
    commoditySheet.Cells.I1.DropdownItems = commodNames
    
    data = queryDb("select name from Port order by name asc")
    portNames = [row for row in data]    

    commoditySheet.Cells.I2.DropdownItems = portNames
    commoditySheet.Cells.I3.DropdownItems = portNames
    commoditySheet.Cells.I4.DropdownItems = portNames
    
def updateCommodityPrices():
    commoditySheet.B1 = commoditySheet.I2
    commoditySheet.C1 = commoditySheet.I3
    commoditySheet.D1 = commoditySheet.I4
    
    commodityID = queryDb("select ID from Commodity where Name= '%s'" % commoditySheet.I1)[0,0]
    ids = queryDb("select ID from Port where Name in ('%s', '%s', '%s') order by name ASC" % (commoditySheet.I2, commoditySheet.I3, commoditySheet.I4))
    portIDs[0] = ids[0, 0]
    portIDs[1] = ids[0, 1]
    portIDs[2] = ids[0, 2]
    
    data = queryDb("select * from (select top %d ValueDate, LocalPrice from HistoricalPortCommodityPrice where PortID = %d and CommodityID = %d order by ValueDate DESC) as foo order by ValueDate ASC" % (Settings.nTopUpdate, portIDs[0], commodityID))
    commoditySheet.FillRange(data, 1, 2, 2, Settings.nTopUpdate + 1)
    
def commodForecast():
    currentPrice = 100
    forecast = createBrownian(currentPrice)
    commoditySheet.FillRange(forecast, 1, 2, 1, Settings.gbmNTicks + 1)

def commodGraph():
        pass
    
# FX rates

USDID = None
currencyIds = [None,None,None]
   
def setFXNames():
    data = queryDb("select name from Currency order by name asc")
    fxNames = [row for row in data]
    
    fxSheet.Cells.I1.DropdownItems = fxNames
    fxSheet.Cells.I2.DropdownItems = fxNames
    fxSheet.Cells.I3.DropdownItems = fxNames
    
def updateFXRates():
    idsAndCodes = queryDb("select ID, ShortName from Currency where Name in ('%s', '%s', '%s')" % (fxSheet.I1, fxSheet.I2, fxSheet.I3))
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
    
    data = queryDb("select * from (select top %d ValueDate, USDValue from HistoricalCurrencyPrice where CurrencyID = %d order by ValueDate DESC) as foo order by ValueDate ASC" % (Settings.nTopUpdate, currencyIds[0]))
    fxSheet.FillRange(data, 1, 2, 2, Settings.nTopUpdate + 1)
    data = queryDb("select USDValue from (select top %d ValueDate, USDValue from HistoricalCurrencyPrice where CurrencyID = %d order by ValueDate DESC) as foo order by ValueDate ASC" % (Settings.nTopUpdate, currencyIds[1]))
    fxSheet.FillRange(data, 3, 2, 3, Settings.nTopUpdate + 1)
    data = queryDb("select USDValue from (select top %d ValueDate, USDValue from HistoricalCurrencyPrice where CurrencyID = %d order by ValueDate DESC) as foo order by ValueDate ASC" % (Settings.nTopUpdate, currencyIds[2]))
    fxSheet.FillRange(data, 4, 2, 4, Settings.nTopUpdate + 1)
 
def fxForecast():
    startRow = getForecastStartRow()
    endRow = getForecastEndRow()
    
    #times
    currTime = getattr(fxSheet, "A%d" % (Settings.nTopUpdate + 1))
    times = []
    for i in range(Settings.gbmNTicks):
        currTime = currTime.AddSeconds(Settings.config['MainLoopTick'] / 1000)
        times.append(currTime)
    fxSheet.FillRange(times, 1, startRow, 1, endRow)
    ItaliciseRange("A%d" % (startRow), "A%d" % (endRow))

    #prices
    #should probably use zip for letters, but I hate unreadable Python one liners
    for i, id in enumerate(currencyIds):
        #USD doesn't fluctuate vs USD
        if id == USDID:
            continue
            
        if i == 0:
            letter = 'B'
        elif i == 1:
            letter = 'C'
        else:
            letter = 'D'         
    
        currentPrice = getattr(fxSheet, "%s%d" % (letter, Settings.nTopUpdate + 1))
        forecast = createBrownian(currentPrice)    
        fxSheet.FillRange(forecast, 2 + i, startRow, 2 + i, endRow)
        
        #style text
        ItaliciseRange("%s%d" % (letter, startRow), "%s%d" % (letter, endRow))
    
fxChart = None
def fxGraph():
    global fxChart
    from rslWPFChart import *
    
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
        
# Country summary
def queryCountrySummary():
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
   
    
# sorting algorithms

def runSort():
    arr = Array[int]([3,2,4,7,1,2])
    ret = algoService.Sort("insertion", arr)
    print ret.time
    print ret.sortedData



readConfig()
setPortNames()
setFXNames()
    
    
    
    