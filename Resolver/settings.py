﻿from System.Xml import XmlReader, XmlNodeType
from System.Data import DataSet
from System.Data.Odbc import OdbcConnection, OdbcDataAdapter
from System import Array

from System.Reflection import Assembly
import clr

commoditySheet = workbook['Commodity Prices']
fxSheet = workbook['FX Rates']

futuresSheet = workbook['Futures']
purchasesSheet = workbook['Purchases']
shippingSheet = workbook['Shipping']

traderSumSheet = workbook['Trader summary']
shippingSumSheet = workbook['Shipping summary']
countrySumSheet = workbook['Country summary']

TFPSheet = workbook['Travelling Freighter Problem']
sortingSheet = workbook['Sorting Algorithms']

class Settings:
    xmlConfigFile = '../Common/Common.config'
    pythonLibDir = "F:/Python26/Lib"
    pythonSiteDir = "F:/Python26/Lib/site-packages"
    pythonDllDir = 'F:/Python26/DLLs'
    
    #these settings are read in from the XML config file shared with C#
    config = {}
    connectString = None
        
    #Settings common to Commodity Prices, Stock Prices, FX Rates:	
    nTopUpdate = 20
    
    #Geometric Brownian Motion constants
    gbmNTicks = 10

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

#assembly loading
def loadAssembly(relPath):
    dll = os.path.dirname(__file__) + '/' + relPath
    print "Loading: %s" % dll
    assembly = Assembly.LoadFile(dll)
    clr.AddReference(assembly)

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

readConfig()
