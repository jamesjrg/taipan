-- settings
BEGIN TRANSACTION
GO

SET QUOTED_IDENTIFIER ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
GO

COMMIT

-- drop everything
BEGIN TRANSACTION
GO

-- drop all FK constraints
DECLARE @sql nvarchar(255)
WHILE EXISTS(select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'FOREIGN KEY')
BEGIN
    select    @sql = 'ALTER TABLE ' + TABLE_NAME + ' DROP CONSTRAINT ' + CONSTRAINT_NAME 
    from    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    where CONSTRAINT_TYPE = 'FOREIGN KEY'
    exec    sp_executesql @sql
END
GO

-- drop all tables
EXEC sp_MSforeachtable @command1 = "DROP TABLE ?"
GO

COMMIT

-- create tables + constraints
BEGIN TRANSACTION
GO

-- categories

CREATE TABLE dbo.CompanyType
	(
	ID int IDENTITY (1, 1) PRIMARY KEY,
	Name nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO

-- economic players

CREATE TABLE dbo.Company
	(
	ID int IDENTITY (1, 1) PRIMARY KEY,
	Name nvarchar(50) NOT NULL,
	CompanyTypeID int NOT NULL,
	Balance Money NOT NULL DEFAULT 0
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.PublicCompany
	(
	CompanyID int PRIMARY KEY REFERENCES Company(ID),
    NStocks int NOT NULL DEFAULT 1000000,
    StockPrice Money NOT NULL DEFAULT 100,
    CountryID int NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.Trader
	(
	CompanyID int PRIMARY KEY REFERENCES Company(ID),
    CountryID int NOT NULL,
	)  ON [PRIMARY]
GO

-- physical objects

CREATE TABLE dbo.Freighter
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
	CompanyID int NOT NULL
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.Port
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    Name nvarchar(50) NOT NULL,
    CountryID int NOT NULL,
    Location geography DEFAULT NULL,
    LocationString AS Location.STAsText()
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.Country
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    Name nvarchar(50) NOT NULL,
    CurrencyID int NOT NULL,
	)  ON [PRIMARY]
GO

-- financial objects inserted with initial data

CREATE TABLE dbo.Commodity
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    Name nvarchar(50) NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.Currency
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    Name nvarchar(50) NOT NULL,
    ShortName nchar(3) NOT NULL,
    USDValue Money NOT NULL DEFAULT 100,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.PortCommodityPrice
	(
	PortID int NOT NULL,
    CommodityID int NOT NULL,
    Price Money NOT NULL DEFAULT 100,
    CONSTRAINT PK_PCP PRIMARY KEY (PortID, CommodityID)
	)  ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_PCP ON dbo.[PortCommodityPrice] 
(
	CommodityID,
	PortID
) ON [PRIMARY]
GO

-- financial objects inserted during runtime

CREATE TABLE dbo.FuturesContract
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    TraderID int NOT NULL,
    CommodityID int NOT NULL,
    PortID int NOT NULL,
    Price Money  NOT NULL,
    Quantity int NOT NULL,
    PurchaseTime datetime NOT NULL,
    SettlementTime datetime NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.WarehousedCommodity
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    TraderID int NOT NULL,
    CommodityID int NOT NULL,
    PortID int NOT NULL,
    Price Money  NOT NULL,
    Quantity int NOT NULL,
    PurchaseTime datetime NOT NULL,
    PlannedSaleTime datetime NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.CommodityTransport
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    ShippingCompanyID int NOT NULL,
    TraderID int NOT NULL,
    CommodityID int NOT NULL,
    DepartureTime datetime NOT NULL,
    ArrivalTime datetime NOT NULL,
    DeparturePort int NOT NULL,
    ArrivalPort int NOT NULL,
    Quantity int NOT NULL,
    FreighterID int NOT NULL,
	)  ON [PRIMARY]
GO

-- historical data

CREATE TABLE dbo.HistoricalStockPrice
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    CompanyID int NOT NULL,
    PriceDate datetime NOT NULL,
    Price Money NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.HistoricalBalance
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    CompanyID int NOT NULL,
    BalanceDate datetime NOT NULL,
    Balance Money NOT NULL,
    )  ON [PRIMARY]
GO

CREATE TABLE dbo.HistoricalCurrencyPrice
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    CurrencyID int NOT NULL,
    ValueDate datetime NOT NULL,
    USDValue Money NOT NULL,    
    )  ON [PRIMARY]
GO

CREATE TABLE dbo.HistoricalPortCommodityPrice
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    PortID int NOT NULL,
    CommodityID int NOT NULL,
    ValueDate datetime NOT NULL,
    Price Money NOT NULL,    
    )  ON [PRIMARY]
GO

-- foreign keys (except those used for base table/inheritance concept, already declared)
ALTER TABLE dbo.Company ADD CONSTRAINT
	FK_Company_CompanyType FOREIGN KEY
	(
	CompanyTypeID
	) REFERENCES dbo.CompanyType
	(
	ID
	)
GO

    
COMMIT


