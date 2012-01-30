def futuresUpdate():
    data = queryDb("""select * from
    
    (SELECT TOP %d
    Commodity.Name as CommodName, f.Quantity, f.PurchaseTime, f.ActualSetTime,
    bp.Name as BPName, dbo.funcGetUSDValueAtDate(f.LocalPrice, f.PortID, f.ActualSetTime) as BPrice,
    transaction.SaleTime, sp.Name as SPName,
    dbo.funcGetUSDValueAtDate(transaction.SalePrice, transaction.SalePortID, transaction.SaleTime) as SPrice,
    tcomp.Name as TradName, scomp.Name as ShipName
    
    from Commodity
    join FuturesContract as f on f.CommodityID = Commodity.ID
    join Port as bp on bp.ID = f.PortID
    join CommodityTransaction as transaction on transaction.FuturesContractID = f.ID
    join Port as sp on sp.ID = transaction.SalePortID
    join Trader on Trader.CompanyID = f.TraderID
    join Company tcomp on tcomp.ID = Trader.CompanyID
    join CommodityTransport on CommodityTransport.CommodityTransactionID = transaction.ID
    join ShippingCompany on ShippingCompany.CompanyID = CommodityTransport.ShippingCompanyID
    join Company scomp on scomp.ID = ShippingCompany.CompanyID
    
    order by f.PurchaseTime DESC) as foo order by PurchaseTime ASC""" % (Settings.nTopUpdate))
    
def purchasesUpdate():
    # data = queryDb("""select * from

    # (SELECT TOP %d
    # Commodity.Name as CommodName, transaction.Quantity, transaction.PurchaseTime, bp.Name as BPName, dbo.funcGetUSDValueAtDate(x.BuyPrice), x.TimeSold, sp.Name as SPName, x.profitloss, tcomp.Name as TradName, scomp.Name as ShipName
    
    # from Commodity
    # join CommodityTransaction as transaction on transaction.CommodityID = Commodity.ID
    # join CommodityTransport as transport
    # join Company as tcomp
    # join Company as scomp
    
    # some joining on transaction.BuyPortID
    # some joining on transaction.SalePortID
    # some joining on transport.ShippingCompanyID
    
    # order by ct.PurchaseTime DESC) as foo order by PurchaseTime ASC""" % (Settings.nTopUpdate))
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
    
    
def shippingUpdate():
    # data = queryDb("""select * from

    # (SELECT TOP %d
    # Commodity.Name as CommodName, transaction.Quantity, x.startport, x.endport, transport.DepartureTime, transport.ArrivalTime, x.shippingcomp, x.tradername
    
    # from Commodity
    # join CommodityTransaction as transaction on transaction.CommodityID = Commodity.ID
    # join CommodityTransport as transport
    # join Company as tcomp
    # join Company as scomp
    
    # some joining on transaction.BuyPortID
    # some joining on transaction.SalePortID
    # some joining on transport.ShippingCompanyID
    
    # order by transport.DepartureTime DESC) as foo order by DepartureTime ASC""" % (Settings.nTopUpdate))
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")

def traderSumUpdate():
# Company:		Country:		Currency:
		# Balance:						
	# Date range	Date range	Date range	Date range
    # Number of futures bought
# Number of futures settled
# Number of purchases
# Number of sales
# Profit/Loss
# Balance at end of period
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
    
def shippingSumUpdate():
# Company:		Country:		Currency:
		# Stock price:		Balance:				
	# Date range	Date range	Date range	Date range
    # Number of voyages
# Number of ports served
# Min quantity per voyage
# Max quantity per voyage
# Av quantity per voyage
# Profit/Loss
# Balance at end of period
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
    
def countrySumUpdate():
# Country:		Currency:	
		# Number of ports:				
	# Date range	Date range	Date range
    # Number of imports
# Number of exports
# Total import value (USD)
# Total export value (USD)
# Average currency value (USD)
    data = queryDb("SELECT TOP 10 Name FROM Country ORDER BY Name DESC")
