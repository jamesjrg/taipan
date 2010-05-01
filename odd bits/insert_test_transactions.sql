INSERT INTO dbo.FuturesContract
           (TraderID, CommodityID, PortID, Price, Quantity, PurchaseTime, SettlementTime, ActualSetTime)
     VALUES
     ()
GO

INSERT INTO dbo.WarehousedCommodity
           (TraderID, CommodityID, PortID, FuturesContractID, Quantity, PurchasePrice, PurchaseTime, SaleTime, SalePrice, SalePortID)
     VALUES
     ()
GO

INSERT INTO dbo.CommodityTransport
           (ShippingCompanyID, WarehousedCommodityID, DepartureTime, ArrivalTime)
     VALUES
     ()
GO
