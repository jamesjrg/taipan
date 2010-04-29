BEGIN TRANSACTION
GO

INSERT INTO dbo.Currency
           (Name, ShortName, USDValue)
     VALUES
           --('Algerian Dinar', 'DZD'),
           ('Argentine peso', 'ARS', 0.25562),
           ('Australian dollar', 'AUD', 0.9277),
           ('British pound', 'GBP', 1.5321),
           ('Euro', 'EUR', 1.3234),
           --('Bangladeshi taka', 'BDT'),
           ('Brazilian real', 'BRL', 0.575248),
           ('United States dollar', 'USD', 1),
           ('Canadian dollar', 'CAD', 0.9947),
           ('Chilean peso', 'CLP',  0.00191864),
           ('Chinese yuan', 'CNY', 0.146501),
           --('Colombian peso', 'COP'),
           --('Danish krone', 'DKK', 0.178122),
           ('Egyptian pound', 'EGP', 1),
           ('Hong Kong Dollar', 'HKD', 0.128779),
           --('Icelandic króna', 'ISK'),   
           ('Indian rupee', 'INR', 1),
           ('Iranian rial', 'IRR', 1),
           --('Israeli new sheqel', 'ILS'),
           ('Japanese yen', 'JPY', 0.0106),
           --('Malagasy ariary', 'MGA'),
           --('Mexican peso', 'MXN'),
           --('Namibian dollar', 'NAD'),
           --('New Zealand dollar', 'NZD'),
           --('Norwegian krone', 'NOK'),
           ('Pakistani rupee', 'PKR', 1),
           --('Peruvian nuevo sol', 'PEN'),
           ('Ruble', 'RUB', 1),
           ('Saudi riyal', 'SAR', 1),
           ('Singapore dollar', 'SGD', 1),
           --('South African rand', 'ZAR'),
           ('South Korean won', 'KRW', 1),
           --('Swedish krona', 'SEK'),
           ('New Taiwan dollar', 'TWD', 1),
           ('Thai baht', 'THB', 1),
           --('Turkish lira', 'TRY'),
           ('United Arab Emirates dirham', 'AED', 1)
           --('Venezuelan bolívar', 'VEF')
GO

INSERT INTO dbo.Country
           (Name, CurrencyID)
     VALUES
           --('Algeria', 1),
           ('Argentina', 1),
           ('Australia', 1),
           
           ('Belgium', 1),
           ('Netherlands', 1),
           ('United Kingdom', 1),
           ('France', 1),
           ('Germany', 1),
           --('Italy', 1),
           ('Spain', 1),
           
           --('Bangladesh', 1),
           ('Brazil', 1),
           ('United States', 1),
           ('Canada', 1),
           ('Chile', 1),
           ('China', 1),
           --('Colombia', 1),
           --('Denmark', 1),
           ('Egypt', 1),
           ('Hong Kong SAR', 1),
           --('Iceland', 1),
           ('India', 1),
           ('Iran', 1),
           --('Israel', 1),
           ('Japan', 1),
           --('Madagascar', 1),
           --('Mexico', 1),
           --('Namibia', 1),
           --('New Zealand', 1),
           --('Norway', 1),
           ('Pakistan', 1),
           --('Peru', 1),
           ('Russia', 1),
           ('Saudi Arabia', 1),
           ('Singapore', 1),
           --('South Africa', 1),
           ('South Korea', 1),
           --('Sweden', 1),
           ('Taiwan', 1),
           ('Thailand', 1),
           --('Turkey', 1),
           ('United Arab Emirates', 1)
           --('Venezuela', 1)
GO

-- coordinates are in form longitude, latitude (i.e. x, y)
INSERT INTO dbo.Port
           (Name, CountryID, Location)
     VALUES
        --Algeria
        --('Algiers', 1, geography::STPointFromText('POINT(3.05 36.7833)', 4326)),
        --Argentina
        ('Bahía Blanca', 2, geography::STPointFromText('POINT(-62.28 -38.72)', 4326)),
        --('Buenos Aires', 2),        
        --Australia
        ('Sydney', 3, geography::STPointFromText('POINT(151.211111 -33.859972)', 4326)),
        ('Port Hedland', 3, geography::STPointFromText('POINT(118.601 -20.31)', 4326)),        
        --('Melbourne', 3, geography::STPointFromText('POINT(0 0)', 4326)),        
        --Belgium
        ('Antwerp', 4, geography::STPointFromText('POINT(4.42 51.22)', 4326)),
        --Netherlands
        ('Rotterdam', 5, geography::STPointFromText('POINT(4.48 51.93)', 4326)),        
        --UK
        ('Felixstowe', 6, geography::STPointFromText('POINT(1.305399 51.960637)', 4326)),
        --('Grimsby and Immingham', 6, geography::STPointFromText('POINT(0 0)', 4326)),        
        --France
        ('Le Havre', 7, geography::STPointFromText('POINT(0.12 49.5)', 4326)),
        --('Marseille', 7, geography::STPointFromText('POINT(0 0)', 4326)),        
        --Germany
        ('Bremen', 8, geography::STPointFromText('POINT(8.81 53.08)', 4326)),
        --('Hamburg', 8, geography::STPointFromText('POINT(0 0)', 4326)),        
       --Italy
       --('Gioia Tauro', 9, geography::STPointFromText('POINT(0 0)', 4326)),
       --Spain
       ('Valencia', 1, geography::STPointFromText('POINT(-0.39 39.48)', 4326)),
       --('Algeciras', 1, geography::STPointFromText('POINT(0 0)', 4326)),    
       --Bangladesh
       --('Chittagong', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --Brazil
       ('Santos', 1, geography::STPointFromText('POINT(-46.33 -23.95)', 4326)),       
       --United States
       --('Los Angeles', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       ('Long Beach', 1, geography::STPointFromText('POINT(-118.16 33.79)', 4326)),
       --('New York', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       ('Savannah', 1, geography::STPointFromText('POINT(-81.13 32.02)', 4326)),
       --Canada
       ('Vancouver', 1, geography::STPointFromText('POINT(-123.13 49.28)', 4326)),       
       --Chile
       ('San Antonio', 1, geography::STPointFromText('POINT(-71.61 -33.6)', 4326)),       
       --China
       ('Shanghai', 1, geography::STPointFromText('POINT(121.47 31.23)', 4326)),
       ('Shenzhen', 1, geography::STPointFromText('POINT(114.1 22.55)', 4326)),
       --('Ningbo', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --('Guangzhou', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --('Qingdao', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --('Tianjin', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --Colombia
       --('Buenaventura', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --Denmark
       --('Fredericia', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --Egypt
       ('Port Said', 1, geography::STPointFromText('POINT(32.2845 31.2593)', 4326)),
       --Hong Kong SAR
       ('Hong Kong', 1, geography::STPointFromText('POINT(114.116667 22.333333)', 4326)),
       --Iceland
       --('Reykjavík', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --India
       ('Nhava Sheva', 1, geography::STPointFromText('POINT(72.95 18.94)', 4326)),
       --Iran
       ('Bandar Abbas', 1, geography::STPointFromText('POINT(56.266667 27.183333)', 4326)),
       --Israel
       --('Ashdod', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --Japan
       --('Nagoya', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       ('Tokyo', 1, geography::STPointFromText('POINT(139.741 35.6705)', 4326)),
       ('Yokohama', 1, geography::STPointFromText('POINT(139.595 35.4527)', 4326)),       
       --Madagascar
       --('Tamatave', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --Mexico
       --('Manzanillo', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --('Veracruz', 1, geography::STPointFromText('POINT(-96.1293 19.1381)', 4326)),
       --Namibia
       --('Walvis Bay', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --New Zealand
       --('Auckland', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --Norway
       --('Bergen', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --Pakistan
       ('Karachi', 1, geography::STPointFromText('POINT(67.0285 24.8898)', 4326)),
       --Peru
       --('Callao', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --Russia
       ('Saint Petersburg', 1, geography::STPointFromText('POINT(30.3067 59.9327)', 4326)),
       --Saudi Arabia
       ('Jeddah', 1, geography::STPointFromText('POINT(39.172778 21.543333)', 4326)),
       --Singapore
       ('Singapore', 1, geography::STPointFromText('POINT(103.828 1.36558)', 4326)),
       --South Africa
       --('Durban', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --South Korea
        ('Busan', 1, geography::STPointFromText('POINT(129.033333 35.1)', 4326)),       
       --Sweden
       --('Gothenburg', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --Taiwan
       ('Kaohsiung', 1, geography::STPointFromText('POINT(120.266667 22.633333)', 4326)),
       --Thailand
       ('Laem Chabang', 1, geography::STPointFromText('POINT(100.883333 13.083333)', 4326)),
       --Turkey
       --('Istanbul', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --United Arab Emirates
       ('Dubai', 1, geography::STPointFromText('POINT(55.3085 25.2694)', 4326))
       --Venezuela
       --('Caracas', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --('Maracaibo', 1)
GO

INSERT INTO dbo.Commodity
           (Name)
     VALUES
           ('Citrus fruit'),
           ('Iron ore'),
           --('Cork'),
           ('Timber'),
           ('Coal'),
           ('Cereal'),
           --('Petroleum products'),
           ('Vehicles'),
           ('Consumer goods'),
           --('Telecommunications equipment'),
           ('Gold'),
           --('Silver'),
           --('Industrial supplies'),
           ('Machinery'),
           --('Tin ore'),
           --('Wool'),
           --('Meat'),
           ('Gemstones'),
           ('Cotton'),
           ('Rice'),
           ('Steel'),
           ('Coffee')
           --('Tea'),
           --('Paper'),
           --('Oil')
GO

-- categories

INSERT INTO dbo.CompanyType
           (Name)
     VALUES
           ('Trader'), ('ShippingCompany')
GO

-- economic players

INSERT INTO dbo.Company
           (Name, CompanyTypeID, CountryID, Balance)
     VALUES
           ('Devlin MacGregor', 1, 1, 102300),
           ('Jackson Steinem', 1, 1, 123231),
           ('Ecumena', 1, 1, 133781),
           ('Pierce & Pierce', 1, 1, 123891),
           ('Khumalo', 1, 1, 234237),
                     
           ('CHOAM', 2, 1, 763231),
           ('M & M Enterprises', 2, 1, 935231),
           ('Delos', 2, 1, 138571),
           ('Bluecorp', 2, 1, 204831),
           ('Planet Express', 2, 1, 395231)
GO

INSERT INTO dbo.ShippingCompany
           (CompanyID, NStocks, USDStockPrice)
     VALUES
           (6, 1000, 283),
           (7, 1000, 312),
           (8, 1000, 112),
           (9, 1000, 78),
           (10, 1000, 233)
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

-- PortCommodityPrice: fun with cursors
DECLARE Port_Cursor CURSOR LOCAL FOR
SELECT ID FROM Port ORDER BY ID ASC

DECLARE Commodity_Cursor CURSOR LOCAL SCROLL FOR
SELECT ID FROM Commodity ORDER BY ID ASC

DECLARE @port_id int, @commodity_id int

OPEN Port_Cursor;
OPEN Commodity_Cursor;

FETCH NEXT FROM Port_Cursor INTO @port_id;
WHILE @@FETCH_STATUS = 0
    BEGIN
        FETCH FIRST FROM Commodity_Cursor INTO @commodity_id;
            WHILE @@FETCH_STATUS = 0
                BEGIN
                    INSERT INTO PortCommodityPrice (PortID, CommodityID) VALUES (@port_id, @commodity_id)
                    FETCH NEXT FROM Commodity_Cursor INTO @commodity_id;
                END;
        FETCH NEXT FROM Port_Cursor INTO @port_id;
   END;
GO
    
COMMIT
GO

