def futuresUpdate():
    #needs to be USD adjusted
    #join HistoricalPortCommodityPrice as h
    data = queryDb("""select * from
    
    (SELECT TOP %d
    Commodity.Name as ComodName, f.Quantity, f.PurchaseTime, f.ActualSetTime,
    bp.Name as BPName, dbo.funcGetUSDValueAtDate(f.LocalPrice, f.PortID, f.ActualSetTime) as BPrice,
    wc.SaleTime, sp.Name as SPName, dbo.funcGetUSDValueAtDate(wc.SalePrice, wc.SalePortID, wc.SaleTime) as SPrice,
    SPrice - BPrice as Profit,
    tcomp.Name as TradName, scomp.Name as ShipName
    from Commodity
    join FuturesContract as f on f.CommodityID = Commodity.ID
    join Port as bp on bp.ID = f.PortID
    join WarehousedCommodity as wc on wc.FuturesContractID = f.ID
    join Port as sp on sp.ID = wc.SalePortID
    join Trader on Trader.CompanyID = f.TraderID
    join Company tcomp on tcomp.ID = Trader.CompanyID
    join CommodityTransport on CommodityTransport.WarehousedCommodityID = wc.ID
    join ShippingCompany on ShippingCompany.CompanyID = CommodityTransport.ShippingCompanyID
    join Company scomp on scomp.ID = ShippingCompany.CompanyID
    
    order by f.PurchaseTime DESC) as foo order by PurchaseTime ASC""" % (Settings.nTopUpdate))
    
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
