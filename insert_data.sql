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
           --('Algeria', 0),
           ('Argentina', 1),
           ('Australia', 2),
           
           ('Belgium', 4),
           ('Netherlands', 4),
           ('United Kingdom', 3),
           ('France', 4),
           ('Germany', 4),
           --('Italy', 4),
           ('Spain', 4),
           
           --('Bangladesh', 0),
           ('Brazil', 5),
           ('United States', 6),
           ('Canada', 7),
           ('Chile', 8),
           ('China', 9),
           --('Colombia', ),
           --('Denmark', 0),
           ('Egypt', 10),
           ('Hong Kong SAR', 11),
           --('Iceland', 0),
           ('India', 12),
           ('Iran', 13),
           --('Israel', 0),
           ('Japan', 14),
           --('Madagascar', 1),
           --('Mexico', 1),
           --('Namibia', 1),
           --('New Zealand', 1),
           --('Norway', 1),
           ('Pakistan', 15),
           --('Peru', 1),
           ('Russia', 16),
           ('Saudi Arabia', 17),
           ('Singapore', 18),
           --('South Africa', 0),
           ('South Korea', 19),
           --('Sweden', 0),
           ('Taiwan', 20),
           ('Thailand', 21),
           --('Turkey', 0),
           ('United Arab Emirates', 22)
           --('Venezuela', 0)
GO

-- coordinates are in form longitude, latitude (i.e. x, y)
INSERT INTO dbo.Port
           (Name, CountryID, Location)
     VALUES
        --Algeria
        --('Algiers', 1, geography::STPointFromText('POINT(3.05 36.7833)', 4326)),
        --Argentina
        ('Bahía Blanca', 1, geography::STPointFromText('POINT(-62.28 -38.72)', 4326)),
        --('Buenos Aires', 2),        
        --Australia
        ('Sydney', 2, geography::STPointFromText('POINT(151.211111 -33.859972)', 4326)),
        ('Port Hedland', 2, geography::STPointFromText('POINT(118.601 -20.31)', 4326)),        
        --('Melbourne', 3, geography::STPointFromText('POINT(0 0)', 4326)),        
        --Belgium
        ('Antwerp', 3, geography::STPointFromText('POINT(4.42 51.22)', 4326)),
        --Netherlands
        ('Rotterdam', 4, geography::STPointFromText('POINT(4.48 51.93)', 4326)),        
        --UK
        ('Felixstowe', 5, geography::STPointFromText('POINT(1.305399 51.960637)', 4326)),
        --('Grimsby and Immingham', 6, geography::STPointFromText('POINT(0 0)', 4326)),        
        --France
        ('Le Havre', 6, geography::STPointFromText('POINT(0.12 49.5)', 4326)),
        --('Marseille', 7, geography::STPointFromText('POINT(0 0)', 4326)),        
        --Germany
        ('Bremen', 7, geography::STPointFromText('POINT(8.81 53.08)', 4326)),
        --('Hamburg', 8, geography::STPointFromText('POINT(0 0)', 4326)),        
       --Italy
       --('Gioia Tauro', 9, geography::STPointFromText('POINT(0 0)', 4326)),
       --Spain
       ('Valencia', 8, geography::STPointFromText('POINT(-0.39 39.48)', 4326)),
       --('Algeciras', 1, geography::STPointFromText('POINT(0 0)', 4326)),    
       --Bangladesh
       --('Chittagong', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --Brazil
       ('Santos', 9, geography::STPointFromText('POINT(-46.33 -23.95)', 4326)),       
       --United States
       --('Los Angeles', 10, geography::STPointFromText('POINT(0 0)', 4326)),
       ('Long Beach', 10, geography::STPointFromText('POINT(-118.16 33.79)', 4326)),
       --('New York', 10, geography::STPointFromText('POINT(0 0)', 4326)),
       ('Savannah', 10, geography::STPointFromText('POINT(-81.13 32.02)', 4326)),
       --Canada
       ('Vancouver', 11, geography::STPointFromText('POINT(-123.13 49.28)', 4326)),       
       --Chile
       ('San Antonio', 12, geography::STPointFromText('POINT(-71.61 -33.6)', 4326)),       
       --China
       ('Shanghai', 13, geography::STPointFromText('POINT(121.47 31.23)', 4326)),
       ('Shenzhen', 13, geography::STPointFromText('POINT(114.1 22.55)', 4326)),
       --('Ningbo', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --('Guangzhou', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --('Qingdao', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --('Tianjin', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --Colombia
       --('Buenaventura', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --Denmark
       --('Fredericia', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --Egypt
       ('Port Said', 14, geography::STPointFromText('POINT(32.2845 31.2593)', 4326)),
       --Hong Kong SAR
       ('Hong Kong', 15, geography::STPointFromText('POINT(114.116667 22.333333)', 4326)),
       --Iceland
       --('Reykjavík', 1, geography::STPointFromText('POINT(0 0)', 4326)),       
       --India
       ('Nhava Sheva', 16, geography::STPointFromText('POINT(72.95 18.94)', 4326)),
       --Iran
       ('Bandar Abbas', 17, geography::STPointFromText('POINT(56.266667 27.183333)', 4326)),
       --Israel
       --('Ashdod', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --Japan
       --('Nagoya', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       ('Tokyo', 18, geography::STPointFromText('POINT(139.741 35.6705)', 4326)),
       ('Yokohama', 18, geography::STPointFromText('POINT(139.595 35.4527)', 4326)),       
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
       ('Karachi', 19, geography::STPointFromText('POINT(67.0285 24.8898)', 4326)),
       --Peru
       --('Callao', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --Russia
       ('Saint Petersburg', 20, geography::STPointFromText('POINT(30.3067 59.9327)', 4326)),
       --Saudi Arabia
       ('Jeddah', 21, geography::STPointFromText('POINT(39.172778 21.543333)', 4326)),
       --Singapore
       ('Singapore', 22, geography::STPointFromText('POINT(103.828 1.36558)', 4326)),
       --South Africa
       --('Durban', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --South Korea
        ('Busan', 23, geography::STPointFromText('POINT(129.033333 35.1)', 4326)),       
       --Sweden
       --('Gothenburg', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --Taiwan
       ('Kaohsiung', 24, geography::STPointFromText('POINT(120.266667 22.633333)', 4326)),
       --Thailand
       ('Laem Chabang', 25, geography::STPointFromText('POINT(100.883333 13.083333)', 4326)),
       --Turkey
       --('Istanbul', 1, geography::STPointFromText('POINT(0 0)', 4326)),
       --United Arab Emirates
       ('Dubai', 26, geography::STPointFromText('POINT(55.3085 25.2694)', 4326))
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
           --('Rice'),
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
                    DECLARE @made_up_price int
                    SET @made_up_price = 80 + (RAND() * 40)
                    SET @made_up_price = (1 / dbo.funcGetUSDValue(1, @port_id)) * @made_up_price
                    INSERT INTO PortCommodityPrice (PortID, CommodityID, LocalPrice) VALUES (@port_id, @commodity_id, @made_up_price)
                    FETCH NEXT FROM Commodity_Cursor INTO @commodity_id;
                END;
        FETCH NEXT FROM Port_Cursor INTO @port_id;
   END;
GO
    
COMMIT
GO

