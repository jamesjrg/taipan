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
           
           ('Slate Rock and Gravel Company', 2)
           ('Arctic & European Fish Oil Company', 2)
           
           ('Dunder Mifflin', 2)
           ('Cyberdyne Systems', 2)
           ('Tamaribuchi Heavy Manufacturing Concern', 2)
           ('Medical Mechanica', 2)
           ('Prescott Pharmaceuticals', 2)
           
           ('Goatberger Publishers', 2)
           ("Garak's", 2)
           ('Omni Consumer Products', 2)           
           
           ('CHOAM', 3),
           ('M & M Enterprises', 3),
           ('Delos', 3),
           ('Bluecorp', 3),
           ('Planet Express', 3)
GO

INSERT INTO dbo.PublicCompany
           (ID, NStocks, StockPrice, CompanyTypeID, CountryID)
     VALUES
           ()
GO

INSERT INTO dbo.Trader
           (ID, CountryID)
     VALUES
           ()
GO

INSERT INTO dbo.DomesticCompany
           (ID, PortID, DomesticCompanyTypeID)
     VALUES
           ()
GO
    
COMMIT


