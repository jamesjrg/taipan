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
        ('Algiers', 1),
        
        ('Bahía Blanca', 2),
        ('Buenos Aires', 2),
        
        ('Sydney', 3),
        ('Port Hedland', 3),
        
        ('Antwerp', 4),
        ('Rotterdam', 5),
        
        ('Felixstowe', 6),
        ('London', 6),
        ('Grimsby and Immingham', 6),
        
        ('Le Havre', 7),
        ('Marseille', 7),
        
        ('Bremen', 8),
        
       ('Hong Kong', 1)
GO

INSERT INTO dbo.Country
           (Name, CurrencyID)
     VALUES
           ('Algeria', 1),
           ('Argentina', 1),
           ('Australia', 1),
           
           ('Belgium', 1),
           ('Netherlands', 1),
           ('United Kingdom', 1),
           ('France', 1),
           ('Germany', 1),
           ('Italy', 1),
           ('Spain', 1),
           
           ('Bangladesh', 1),
           ('Belize', 1),
           ('Brazil', 1),
           ('United States', 1),
           ('Canada', 1),
           ('Chile', 1),
           ('China', 1),
           ('Colombia', 1),
           ('Denmark', 1),
           ('Hong Kong SAR', 1),
           ('Iceland', 1),
           ('India', 1)           
GO

INSERT INTO dbo.Commodity
           (Name)
     VALUES
           ('Citrus fruit'),
           ('Iron ore'),
           ('Cork'),
           ('Cereals'),
           ('Petroleum products'),
           ('Consumer goods'),
           ('Telecommunications equipment'),
           ('Gold'),
           ('Silver'),
           ('Industrial supplies'),
           ('Machinery'),
           ('Coal'),
           ('Tin ore'),
           ('Wool'),
           ('Meat'),
           ('Gemstones'),
           ('Cotton'),
           ('Rice')        
GO

INSERT INTO dbo.Currency
           (Name, ShortName)
     VALUES
           ('Algerian Dinar', 'DZD'),
           ('Argentine peso', 'ARS'),
           ('Australian dollar', 'AUD'),
           ('British pound', 'GBP'),
           ('Euro', 'EUR'),
           ('Bangladeshi taka', 'BDT'),
           ('Belize dollar', 'BZD'),
           ('Brazilian real', 'BRL'),
           ('United States dollar', 'USD'),
           ('Canadian dollar', 'CAD'),
           ('Chilean peso', 'CLP'),
           ('Chinese yuan', 'CNY'),
           ('Colombian peso', 'COP'),
           ('Danish krone', 'DKK'),
           ('Hong Kong Dollar', 'HKD'),
           ('Icelandic króna', 'ISK'),   
           ('Indian rupee', 'INR'),
           ('Iranian rial', 'IRR'),
           ('Israeli new sheqel', 'ILS'),
           ('Japanese yen', 'JPY'),
           ('Malagasy ariary', 'MGA'),
           ('Mexican peso', 'MXN'),
           ('Namibian dollar', 'NAD'),
           ('New Zealand dollar', 'NZD'),
           ('Norwegian krone', 'NOK'),
           ('Pakistani rupee', 'PKR'),
           ('Peruvian nuevo sol', 'PEN'),
           ('Ruble', 'RUB'),
           ('Saudi riyal', 'SAR'),
           ('Singapore dollar', 'SGD'),
           ('South African rand', 'ZAR'),
           ('South Korean won ', 'KRW'),
           ('Swedish krona', 'SEK'),
           ('New Taiwan dollar', 'TWD'),
           ('Thai baht', 'THB'),
           ('Turkish lira', 'TRY'),
           ('United Arab Emirates dirham', 'AED'),
           ('Venezuelan bolívar', 'VEF')
GO

INSERT INTO dbo.PortCommodityPrice
           (PortID, CommodityID)
     VALUES
           (1, 1)
GO
    
COMMIT


