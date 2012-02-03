def futuresUpdate():
    data = queryDb("""select 
    CommodName, Quantity, PurchaseTime, ActualSetTime, BPName, HistUSDBuyPrice, SaleTime, SPName, HistUSDSalePrice,
    HistUSDSalePrice - HistUSDBuyPrice as Profit,
    TradName, ShipName
    
    FROM    
    
    (SELECT TOP (@Limit)
    Commodity.Name as CommodName, f.Quantity, f.PurchaseTime, f.ActualSetTime,
    bp.Name as BPName, dbo.funcGetUSDValueAtDate(f.LocalPrice, f.PortID, f.ActualSetTime) as HistUSDBuyPrice,
    ctrans.SaleTime, sp.Name as SPName,
    dbo.funcGetUSDValueAtDate(ctrans.SalePrice, ctrans.SalePortID, ctrans.SaleTime) as HistUSDSalePrice,
    tcomp.Name as TradName, scomp.Name as ShipName
    
    FROM Commodity
    join FuturesContract as f on f.CommodityID = Commodity.ID
    join Port as bp on bp.ID = f.PortID
    join CommodityTransaction as ctrans on ctrans.FuturesContractID = f.ID
    join Port as sp on sp.ID = ctrans.SalePortID
    join Trader on Trader.CompanyID = f.TraderID
    join Company tcomp on tcomp.ID = Trader.CompanyID
    join CommodityTransport on CommodityTransport.CommodityTransactionID = ctrans.ID
    join ShippingCompany on ShippingCompany.CompanyID = CommodityTransport.ShippingCompanyID
    join Company scomp on scomp.ID = ShippingCompany.CompanyID
    
    order by f.PurchaseTime DESC) as foo order by PurchaseTime ASC""", {"Limit": Settings.nTopUpdate})
    
    futuresSheet.FillRange(data, 1, 4, 12, Settings.nTopUpdate + 1)
    
def purchasesUpdate():
    data = queryDb("""select
    CommodName, Quantity, PurchaseTime, BPName, HistUSDBuyPrice, SaleTime, SPName, HistUSDSalePrice,
    HistUSDSalePrice - HistUSDBuyPrice as Profit,
    TradName, ShipName
    
    FROM
    
    (SELECT TOP (@Limit)
    Commodity.Name as CommodName, ctrans.Quantity, ctrans.PurchaseTime, bp.Name as BPName,
    dbo.funcGetUSDValueAtDate(ctrans.PurchasePrice, ctrans.BuyPortID, ctrans.PurchaseTime) as HistUSDBuyPrice,
    ctrans.SaleTime, sp.Name as SPName,
    dbo.funcGetUSDValueAtDate(ctrans.SalePrice, ctrans.SalePortID, ctrans.SaleTime) as HistUSDSalePrice,
    tcomp.Name as TradName, scomp.Name as ShipName
    
    FROM Commodity
    join CommodityTransaction as ctrans on ctrans.CommodityID = Commodity.ID
    join CommodityTransport as transport on transport.CommodityTransactionID = ctrans.ID
    join Company as tcomp on tcomp.ID = ctrans.TraderID
    join Company as scomp on scomp.ID = transport.ShippingCompanyID
    join Port bp on bp.ID = ctrans.BuyPortID
    join Port sp on sp.ID = ctrans.SalePortID
    
    order by ctrans.PurchaseTime DESC) as foo order by PurchaseTime ASC""", {"Limit": Settings.nTopUpdate})
    
    purchasesSheet.FillRange(data, 1, 4, 12, Settings.nTopUpdate + 1)
    
    
def shippingUpdate():
    # data = queryDb("""select * from

    # (SELECT TOP %d
    # Commodity.Name as CommodName, ctrans.Quantity, x.startport, x.endport, transport.DepartureTime, transport.ArrivalTime, x.shippingcomp, x.tradername
    
    # from Commodity
    # join CommodityTransaction as ctrans on ctrans.CommodityID = Commodity.ID
    # join CommodityTransport as transport
    # join Company as tcomp
    # join Company as scomp
    
    # some joining on ctrans.BuyPortID
    # some joining on ctrans.SalePortID
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
