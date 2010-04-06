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

CREATE TABLE dbo.DomesticCompanyType
	(
	ID int IDENTITY (1, 1) PRIMARY KEY,
    Name nvarchar(50) NOT NULL,
    ShortageProb int NOT NULL CHECK (ShortageProb > 0 AND ShortageProb <= 100) DEFAULT 0,
    SurplusProb int NOT NULL CHECK (SurplusProb > 0 AND SurplusProb <= 100) DEFAULT 0
	)  ON [PRIMARY]
GO

-- economic players

CREATE TABLE dbo.Company
	(
	ID int IDENTITY (1, 1) PRIMARY KEY,
	Name nvarchar(50) NOT NULL,
	CompanyTypeID int NOT NULL,
	Balance int NOT NULL DEFAULT 0
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.PublicCompany
	(
	ID int PRIMARY KEY REFERENCES Company(ID),
    NStocks int NOT NULL,
    StockPrice int NOT NULL,
    CountryID int NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.Trader
	(
	ID int PRIMARY KEY REFERENCES Company(ID),
    CountryID int NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.DomesticCompany
	(
	ID int PRIMARY KEY REFERENCES PublicCompany(ID),
    PortID int NOT NULL,
    DomesticCompanyTypeID int NOT NULL,
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
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.Country
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    Name nvarchar(50) NOT NULL,
    CurrencyID int NOT NULL,
	)  ON [PRIMARY]
GO

-- financial objects

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
    USDValue int NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.FuturesContract
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    TraderID int NOT NULL,
    CommodityID int NOT NULL,
    DomesticCompanyID int NOT NULL,
    Price int NOT NULL,
    Quantity int NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.WarehousedCommodity
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    TraderID int NOT NULL,
    CommodityID int NOT NULL,
    DomesticCompanyID int NOT NULL,
    Quantity int NOT NULL,
    PurchaseTime datetime NOT NULL,
    SaleTime datetime NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.DomesticCompanyCommodity
	(
	CompanyID int NOT NULL,
    CommodityID int NOT NULL,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.CommodityTransport
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.PortCommodityPrice
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
	)  ON [PRIMARY]
GO

-- historical data

CREATE TABLE dbo.HistoricalStockPrice
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.HistoricalBalance
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.HistoricalCurrencyPrice
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.HistoricalPortCommodityPrice
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
	)  ON [PRIMARY]
GO

-- foreign keys
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


