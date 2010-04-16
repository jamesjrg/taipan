BEGIN TRANSACTION
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
           ('Brazilian real', 'BRL'),
           ('United States dollar', 'USD'),
           ('Canadian dollar', 'CAD'),
           ('Chilean peso', 'CLP'),
           ('Chinese yuan', 'CNY'),
           ('Colombian peso', 'COP'),
           ('Danish krone', 'DKK'),
           ('Egyptian pound', 'EGP'),
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
           ('South Korean won', 'KRW'),
           ('Swedish krona', 'SEK'),
           ('New Taiwan dollar', 'TWD'),
           ('Thai baht', 'THB'),
           ('Turkish lira', 'TRY'),
           ('United Arab Emirates dirham', 'AED'),
           ('Venezuelan bolívar', 'VEF')
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
           ('Brazil', 1),
           ('United States', 1),
           ('Canada', 1),
           ('Chile', 1),
           ('China', 1),
           ('Colombia', 1),
           ('Denmark', 1),
           ('Egypt', 1),
           ('Hong Kong SAR', 1),
           ('Iceland', 1),
           ('India', 1),
           ('Iran', 1),
           ('Israel', 1),
           ('Japan', 1),
           ('Madagascar', 1),
           ('Mexico', 1),
           ('Namibia', 1),
           ('New Zealand', 1),
           ('Norway', 1),
           ('Pakistan', 1),
           ('Peru', 1),
           ('Russia', 1),
           ('Saudi Arabia', 1),
           ('Singapore', 1),
           ('South Africa', 1),
           ('South Korea', 1),
           ('Sweden', 1),
           ('Taiwan', 1),
           ('Thailand', 1),
           ('Turkey', 1),
           ('United Arab Emirates', 1),
           ('Venezuela', 1)
GO

INSERT INTO dbo.Port
           (Name, CountryID)
     VALUES
        --Algeria
        ('Algiers', 1),        
        --Argentina
        ('Bahía Blanca', 2),
        ('Buenos Aires', 2),        
        --Australia
        ('Sydney', 3),
        ('Port Hedland', 3),        
        ('Melbourne', 3),        
        --Belgium
        ('Antwerp', 4),
        --Netherlands
        ('Rotterdam', 5),        
        --UK
        ('Felixstowe', 6),
        ('Grimsby and Immingham', 6),        
        --France
        ('Le Havre', 7),
        ('Marseille', 7),        
        --Germany
        ('Bremen', 8),
        ('Hamburg', 8),        
       --Italy
       ('Gioia Tauro', 9),
       --Spain
       ('Valencia', 1),
       ('Algeciras', 1),    
       --Bangladesh
       ('Chittagong', 1),       
       --Brazil
       ('Santos', 1),       
       --United States
       ('Los Angeles', 1),
       ('Long Beach', 1),
       ('New York', 1),
       ('Savannah', 1),
       --Canada
       ('Vancouver', 1),       
       --Chile
       ('San Antonio', 1),       
       --China
       ('Shanghai', 1),
       ('Shenzhen', 1),
       ('Ningbo', 1),
       ('Guangzhou', 1),
       ('Qingdao', 1),
       ('Tianjin', 1),       
       --Colombia
       ('Buenaventura', 1),       
       --Denmark
       ('Fredericia', 1),       
       --Egypt
       ('Port Said', 1),
       --Hong Kong SAR
       ('Hong Kong', 1),
       --Iceland
       ('Reykjavík', 1),       
       --India
       ('Jawaharlal Nehru', 1),
       --Iran
       ('Bandar Abbas', 1),
       --Israel
       ('Ashdod', 1),
       --Japan
       ('Nagoya', 1),
       ('Tokyo', 1),
       ('Yokohama', 1),       
       --Madagascar
       ('Tamatave', 1),
       --Mexico
       ('Manzanillo', 1),
       ('Veracruz', 1),
       --Namibia
       ('Walvis Bay', 1),
       --New Zealand
       ('Auckland', 1),
       --Norway
       ('Bergen', 1),
       --Pakistan
       ('Karachi', 1),
       --Peru
       ('Callao', 1),
       --Russia
       ('Saint Petersburg', 1),
       --Saudi Arabia
       ('Jeddah', 1),
       --Singapore
       ('Singapore', 1),
       --South Africa
       ('Durban', 1),
       --South Korea
        ('Busan', 1),       
       --Sweden
       ('Gothenburg', 1),
       --Taiwan
       ('Kaohsiung', 1),
       --Thailand
       ('Laem Chabang', 1),
       --Turkey
       ('Istanbul', 1),
       --United Arab Emirates
       ('Dubai', 1),
       --Venezuela
       ('Caracas', 1),
       ('Maracaibo', 1)
GO

INSERT INTO dbo.Commodity
           (Name)
     VALUES
           ('Citrus fruit'),
           ('Iron ore'),
           ('Cork'),
           ('Timber'),
           ('Coal'),
           ('Cereal'),
           ('Petroleum products'),
           ('Vehicles'),
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
           ('Rice'),
           ('Steel'),
           ('Coffee'),
           ('Tea'),
           ('Paper'),
           ('Oil')
GO

-- categories

INSERT INTO dbo.CompanyType
           (Name)
     VALUES
           ('Trader'), ('ShippingCompany')
GO

-- economic players

INSERT INTO dbo.Company
           (Name, CompanyTypeID, CountryID)
     VALUES
           ('Devlin MacGregor', 1, 1),
           ('Jackson Steinem', 1, 1),
           ('Ecumena', 1, 1),
           ('Pierce & Pierce', 1, 1),
           ('Khumalo', 1, 1),
                     
           ('CHOAM', 2, 1),
           ('M & M Enterprises', 2, 1),
           ('Delos', 2, 1),
           ('Bluecorp', 2, 1),
           ('Planet Express', 2, 1)
GO

INSERT INTO dbo.ShippingCompany
           (CompanyID, NStocks, USDStockPrice)
     VALUES
           (6, 1000, 100),
           (7, 1000, 100),
           (8, 1000, 100),
           (9, 1000, 100),
           (10, 1000, 100)
GO

INSERT INTO dbo.Trader
           (CompanyID)
     VALUES
           (1),
           (2),
           (3),
           (4),
           (5)
GO

INSERT INTO dbo.PortCommodityPrice
           (PortID, CommodityID)
     VALUES
           (1, 1)
GO
    
COMMIT


