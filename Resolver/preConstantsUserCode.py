from System import Array
from System.Data import DataSet
from System.Data.Odbc import OdbcConnection, OdbcDataAdapter

connectString = "Driver={SQL Server};Server=DAPHNE-DURON\\SQLEXPRESS;Database=TaiPan;UID=taipan-r;PWD=fakepass;"
fxSheet = workbook['FX Rates']

currency1 = 1
currency2 = 0
currency3 = 0

def queryDb(query):
    print 'DB query: %s' % query
    connection = OdbcConnection(connectString)
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

def updateFXRates():
    data = queryDb("select top %d ValueDate, USDValue from HistoricalCurrencyPrice where CurrencyID = %d order by ValueDate ASC" % (nTopUpdate, currency1))
    fxSheet.FillRange(data, 1, 3, 2, nTopUpdate + 1)

def queryCountrySummary():
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
    
    
    
