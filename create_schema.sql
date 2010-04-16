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

-- create everything
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

-- foreign keys for inheritance concept are added on table creation
CREATE TABLE dbo.Company
	(
	ID int IDENTITY (1, 1) PRIMARY KEY,
	Name nvarchar(50) NOT NULL,
	CompanyTypeID int references CompanyType(ID) not null,
	Balance Money NOT NULL DEFAULT 0,
    CountryID int NOT NULL
    constraint alternate_pk unique (ID,CompanyTypeID)
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.Trader
	(
	CompanyID int PRIMARY KEY REFERENCES Company(ID),
    CompanyTypeID as 1 persisted,    
    foreign key (CompanyID, CompanyTypeID) references Company(ID, CompanyTypeID)
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.ShippingCompany
	(
	CompanyID int PRIMARY KEY REFERENCES Company(ID),
    CompanyTypeID as 2 persisted,
    NStocks int NOT NULL DEFAULT 1000000,
    USDStockPrice Money NOT NULL DEFAULT 100,
    foreign key (CompanyID, CompanyTypeID) references Company(ID, CompanyTypeID)
	)  ON [PRIMARY]
GO

-- physical objects

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
    Price Money NOT NULL DEFAULT 0,
    ShortageProb int NOT NULL CHECK (ShortageProb >= 0 AND ShortageProb <= 100) DEFAULT 0,
    SurplusProb int NOT NULL CHECK (SurplusProb >= 0 AND SurplusProb <= 100) DEFAULT 0,
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
    ActualSetTime datetime default null,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.WarehousedCommodity
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    TraderID int NOT NULL,
    CommodityID int NOT NULL,
    PortID int NOT NULL,
    FuturesContractID int default null,
    Quantity int NOT NULL,
    PurchasePrice Money NOT NULL,    
    PurchaseTime datetime NOT NULL,
    SaleTime datetime default null,
    SalePrice datetime default null,
    SalePortID int default null,
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.CommodityTransport
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    ShippingCompanyID int NOT NULL,
    WarehousedCommodityID int NOT NULL,
    DepartureTime datetime NOT NULL,
    ArrivalTime datetime default null,
    )  ON [PRIMARY]
GO

-- historical data

CREATE TABLE dbo.HistoricalStockPrice
	(
	ID int NOT NULL IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    ShippingCompanyID int NOT NULL,
    PriceDate datetime NOT NULL,
    USDStockPrice Money NOT NULL,
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

-- foreign keys (except those used for table inheritance concept, already declared)
ALTER TABLE dbo.HistoricalStockPrice ADD
	CONSTRAINT fkHistStockPrice FOREIGN KEY
	(ShippingCompanyID) REFERENCES dbo.ShippingCompany (CompanyID)
GO

ALTER TABLE dbo.HistoricalBalance ADD
	CONSTRAINT fkHistBalance FOREIGN KEY
	(CompanyID) REFERENCES dbo.Company (ID)
GO

ALTER TABLE dbo.HistoricalCurrencyPrice ADD
	CONSTRAINT fkHistCurrencyPrice FOREIGN KEY
	(CurrencyID) REFERENCES dbo.Currency (ID)
GO

ALTER TABLE dbo.HistoricalPortCommodityPrice ADD
	CONSTRAINT fkHistPortCommodityPort FOREIGN KEY
	(PortID) REFERENCES dbo.Port (ID),
    CONSTRAINT fkHistPortCommodityCommod FOREIGN KEY
	(CommodityID) REFERENCES dbo.Commodity (ID);
GO

ALTER TABLE dbo.Company ADD
	CONSTRAINT fkCompanyCountry FOREIGN KEY
	(CountryID) REFERENCES dbo.Country (ID)
GO

ALTER TABLE dbo.Country ADD
	CONSTRAINT fkCountryCurr FOREIGN KEY
	(CurrencyID) REFERENCES dbo.Currency (ID)
GO

ALTER TABLE dbo.Port ADD
	CONSTRAINT fkPortCountry FOREIGN KEY
	(CountryID) REFERENCES dbo.Country (ID)
GO

ALTER TABLE dbo.FuturesContract ADD
	CONSTRAINT fkFuturesTrader FOREIGN KEY
	(TraderID) REFERENCES dbo.Trader (CompanyID),
    CONSTRAINT fkFuturesCommod FOREIGN KEY
	(CommodityID) REFERENCES dbo.Commodity (ID);
GO

-- TODO: warehousedcommod, commodtrans, portcommodprice
    
-- triggers
CREATE TRIGGER dbo.trgStockUpdate
ON dbo.ShippingCompany
AFTER update
AS
INSERT INTO dbo.HistoricalStockPrice
    (ShippingCompanyID, PriceDate, USDStockPrice)
SELECT updated.ID, GETDATE(), updated.USDStockPrice FROM updated
GO

CREATE TRIGGER dbo.trgBalanceUpdate
ON dbo.Company
AFTER update
AS
INSERT INTO dbo.HistoricalBalance
    (CompanyID, BalanceDate, Balance)
SELECT updated.ID, GETDATE(), updated.Balance FROM updated
GO

CREATE TRIGGER dbo.trgCurrencyUpdate
ON dbo.Currency
AFTER update
AS
INSERT INTO dbo.HistoricalCurrencyPrice
    (CurrencyID, ValueDate, USDValue)
SELECT updated.ID, GETDATE(), updated.USDValue FROM updated
GO

CREATE TRIGGER dbo.trgPortComodUpdate
ON dbo.PortCommodityPrice
AFTER update
AS
INSERT INTO dbo.HistoricalPortCommodityPrice
    (PortID, CommodityID, ValueDate, Price)
SELECT updated.PortID, updated.CommodityID, GETDATE(), updated.Price FROM updated
GO
    
COMMIT
