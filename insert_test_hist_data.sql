BEGIN TRANSACTION
GO

INSERT INTO dbo.HistoricalStockPrice
           (ShippingCompanyID, PriceDate, USDStockPrice)
     VALUES
           (6, '2010-04-15T20:10:00', 100),
           (6, '2010-04-15T20:10:01', 101),
           (6, '2010-04-15T20:10:02', 98),
           (6, '2010-04-15T20:10:03', 54)
GO

INSERT INTO dbo.HistoricalBalance
           (CompanyID, BalanceDate, Balance)
     VALUES
           (1, '2010-04-15T20:10:00', 10000),
           (1, '2010-04-15T20:10:01', 10100),
           (1, '2010-04-15T20:10:02', 98000),
           (1, '2010-04-15T20:10:03', 54000)
GO

INSERT INTO dbo.HistoricalCurrencyPrice
           (CurrencyID, ValueDate, USDValue)
     VALUES
           (1, '2010-04-15T20:10:00', 1.01),
           (1, '2010-04-15T20:10:01', 0.98),
           (1, '2010-04-15T20:10:02', 1),
           (1, '2010-04-15T20:10:03', 1.03)
GO

INSERT INTO dbo.HistoricalPortCommodityPrice
           (PortID, CommodityID, ValueDate, LocalPrice)
     VALUES
           (1, 1, '2010-04-15T20:10:00', 100),
           (1, 1,  '2010-04-15T20:10:01', 101),
           (1, 1,  '2010-04-15T20:10:02', 98),
           (1, 1,  '2010-04-15T20:10:03', 54)
GO
    
COMMIT


