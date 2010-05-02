def futuresUpdate():
    data = queryDb("""select * from
    (SELECT TOP %d
    c.Name, f.Quantity, f.PurchaseTime, p.Name
    from Commodity as c
    join FuturesContract as f
    join Port as p
    join Trader as t on FuturesContract.TraderID = Trader.ID
    join HistoricalPortCommodityPrice as h
    order by ValueDate DESC) as foo order by ValueDate ASC""" % (Settings.nTopUpdate))
    
def purchasesUpdate():
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
    
def shippingUpdate():
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")

def traderSumUpdate():
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
    
def shippingSumUpdate():
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
    
def countrySumUpdate():
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
