from System import Array
from System.Data import DataSet
from System.Data.Odbc import OdbcConnection, OdbcDataAdapter

commoditySheet = workbook['Commodity Prices']
fxSheet = workbook['FX Rates']

commodityId = 1
commodityPortId1 = 1
commodityPortId2 = 2
commodityPortId3 = 3

currencyId1 = 1
currencyId2 = 2
currencyId3 = 3

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

def queryCurrencies():
    data = queryDb("select Name from Currency ORDER BY Name ASC")

def setCurrency(which, id):
    data = queryDb("select ShortName from Currency where ID = %d" % id)

def updateCommodityPrices():
    data = queryDb("select top %d ValueDate, Price from HistoricalPortCommodityPrice where PortID = %d and CommodityID = %d order by ValueDate ASC" % (Settings.nTopUpdate, commodityPortId1, commodityId))
    commoditySheet.FillRange(data, 1, 3, 2, Settings.nTopUpdate + 1)
   
def updateFXRates():
    data = queryDb("select top %d ValueDate, USDValue from HistoricalCurrencyPrice where CurrencyID = %d order by ValueDate ASC" % (Settings.nTopUpdate, currencyId1))
    fxSheet.FillRange(data, 1, 3, 2, Settings.nTopUpdate + 1)

def queryCountrySummary():
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
    
    
    
