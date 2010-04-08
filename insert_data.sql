BEGIN TRANSACTION
GO

-- categories

INSERT INTO dbo.CompanyType
           (Name)
     VALUES
           ('Trader'), ('DomesticCompany'), ('ShippingCompany')
GO

INSERT INTO dbo.DomesticCompanyType
           (Name, ShortageProb, SurplusProb)
     VALUES
           ('Primary Sector', 20, 80), ('Manufacturing', 50, 50), ('Service Sector', 80, 20)
GO

-- economic players

INSERT INTO dbo.Company
           (Name, CompanyTypeID)
     VALUES
           ('Devlin MacGregor', 1),
           ('Jackson Steinem', 1),
           ('Ecumena', 1),
           ('Pierce & Pierce', 1),
           ('Khumalo', 1),
           
           ('Slate Rock and Gravel Company', 2),
           ('Arctic & European Fish Oil Company', 2),
           
           ('Dunder Mifflin', 2),
           ('Cyberdyne Systems', 2),
           ('Tamaribuchi Heavy Manufacturing Concern', 2),
           ('Medical Mechanica', 2),
           ('Prescott Pharmaceuticals', 2),
           
           ('Goatberger Publishers', 2),
           ('Garak''s', 2),
           ('Omni Consumer Products', 2),           
           
           ('CHOAM', 3),
           ('M & M Enterprises', 3),
           ('Delos', 3),
           ('Bluecorp', 3),
           ('Planet Express', 3)
GO

INSERT INTO dbo.PublicCompany
           (CompanyID, NStocks, StockPrice, CountryID)
     VALUES
           (6, 1000, 100, 1)
GO

INSERT INTO dbo.Trader
           (CompanyID, CountryID)
     VALUES
           (1, 1)
GO

INSERT INTO dbo.DomesticCompany
           (PublicCompanyID, PortID, DomesticCompanyTypeID)
     VALUES
           (6, 1, 1)
GO

INSERT INTO dbo.Freighter
           (CompanyID)
     VALUES
           (1)
GO

INSERT INTO dbo.Port
           (Name, CountryID)
     VALUES
           ("Hong Kong", 1)
GO

INSERT INTO dbo.Country
           (Name, CurrencyID)
     VALUES
           ("Hong Kong SAR", 1)
GO

INSERT INTO dbo.Commodity
           (Name)
     VALUES
           ("Gold")
GO

INSERT INTO dbo.Currency
           (Name, ShortName, USDValue)
     VALUES
           ("Hong Kong Dollar", 100)
GO

INSERT INTO dbo.PortCommodityPrice
           (PortID, CommodityID, Price)
     VALUES
           (1, 1, 100)
GO
    
COMMIT


