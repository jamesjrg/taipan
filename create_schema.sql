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

--drop all procedures
declare @procName varchar(500)
declare cur cursor
    for select [name] from sys.objects where type = 'p'
open cur
fetch next from cur into @procName
while @@fetch_status = 0
begin
    exec('drop procedure ' + @procName)
    fetch next from cur into @procName
end
close cur
deallocate cur

--drop all functions
declare @funcName varchar(500)
declare cur cursor
    for select [name] from sys.objects where type = 'fn'
open cur
fetch next from cur into @funcName
while @@fetch_status = 0
begin
    exec('drop function ' + @funcName)
    fetch next from cur into @funcName
end
close cur
deallocate cur

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
    ShortName nchar(3) NOT NULL CHECK (ShortName LIKE '[A-Z][A-Z][A-Z]') UNIQUE,
    USDValue Money NOT NULL DEFAULT 1,
    constraint currency_alternate_pk unique (ShortName)
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.PortCommodityPrice
	(
	PortID int NOT NULL,
    CommodityID int NOT NULL,
    LocalPrice Money NOT NULL DEFAULT 100,
    ShortageProb int NOT NULL CHECK (ShortageProb >= 0) DEFAULT 50,
    SurplusProb int NOT NULL CHECK (SurplusProb >= 0) DEFAULT 50,
    CONSTRAINT PK_PCP PRIMARY KEY (PortID, CommodityID),
    CONSTRAINT Prob_Total CHECK (ShortageProb + SurplusProb <= 100)
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
    LocalPrice Money  NOT NULL,
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
    CompanyID int NOT NULL,
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
    LocalPrice Money NOT NULL,    
    )  ON [PRIMARY]
GO

-- foreign keys (except those used for table inheritance concept, already declared)
ALTER TABLE dbo.HistoricalStockPrice ADD
	CONSTRAINT fkHistStockPrice FOREIGN KEY
	(CompanyID) REFERENCES dbo.ShippingCompany (CompanyID)
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

ALTER TABLE dbo.WarehousedCommodity ADD
	CONSTRAINT fkWareCommodTrader FOREIGN KEY
	(TraderID) REFERENCES dbo.Trader (CompanyID),
    CONSTRAINT fkWareCommodCommod FOREIGN KEY
	(CommodityID) REFERENCES dbo.Commodity (ID),
    CONSTRAINT fkWareCommodPort FOREIGN KEY
	(PortID) REFERENCES dbo.Port (ID),
    CONSTRAINT fkWareCommodFutures FOREIGN KEY
	(FuturesContractID) REFERENCES dbo.FuturesContract (ID);
GO

ALTER TABLE dbo.PortCommodityPrice ADD
	CONSTRAINT fkPortCommodityPort FOREIGN KEY
	(PortID) REFERENCES dbo.Port (ID),
    CONSTRAINT fkPortCommodityCommod FOREIGN KEY
	(CommodityID) REFERENCES dbo.Commodity (ID);
GO

ALTER TABLE dbo.CommodityTransport ADD
	CONSTRAINT fkCommTransportShippingComp FOREIGN KEY
	(ShippingCompanyID) REFERENCES dbo.ShippingCompany (CompanyID),
    CONSTRAINT fkCommTransportWarehouse FOREIGN KEY
	(WarehousedCommodityID) REFERENCES dbo.WarehousedCommodity (ID);
GO
    
-- ARG if this were another database these could be triggers with FOR EACH ROW
CREATE PROCEDURE procStockUpdate 
   @CompanyID int, 
   @PriceDate datetime,
   @USDStockPrice Money
AS 
insert into HistoricalStockPrice (CompanyID, PriceDate, USDStockPrice) VALUES (@CompanyID, @PriceDate, @USDStockPrice);
update ShippingCompany set USDStockPrice = @USDStockPrice where CompanyID = @CompanyID;
GO

CREATE PROCEDURE procBalanceUpdate 
   @CompanyID int, 
   @BalanceDate datetime,
   @Balance Money
AS 
BEGIN
insert into HistoricalBalance (CompanyID, BalanceDate, Balance) VALUES (@CompanyID, @BalanceDate, @Balance);
update Company set Balance = @Balance where ID = @CompanyID;
END
GO

CREATE PROCEDURE procCurrencyUpdate 
   @CurrencyID int, 
   @ValueDate datetime,
   @USDValue Money
AS
BEGIN
insert into HistoricalCurrencyPrice (CurrencyID, ValueDate, USDValue) VALUES (@CurrencyID, @ValueDate, @USDValue);
update Currency set USDValue = @USDValue where ID = @CurrencyID;
END
GO

CREATE PROCEDURE procPortComodUpdate
   @PortID int, 
   @CommodityID int, 
   @ValueDate datetime,
   @LocalPrice Money
AS 
BEGIN
    insert into HistoricalPortCommodityPrice (PortID, CommodityID, ValueDate, LocalPrice) VALUES (@PortID, @CommodityID, @ValueDate, @LocalPrice);
    update PortCommodityPrice set LocalPrice = @LocalPrice where PortID = @PortID and CommodityID = @CommodityID;
END
GO

-- currency conversion
CREATE FUNCTION funcGetUSDValue (@LocalPrice Money, @PortID int)
RETURNS Money
AS
BEGIN
declare @USDValue Money, @ConvertedPrice Money
set @USDValue = (select USDValue from Currency join Country on Country.CurrencyID = Currency.ID join Port on Port.CountryID = Country.ID where port.id = @PortID)
set @ConvertedPrice = (select @LocalPrice * @USDValue)
return @ConvertedPrice
END
GO

CREATE FUNCTION funcGetUSDValueAtDate(@LocalPrice Money, @PortID int, @TheDate datetime)
RETURNS Money
AS
BEGIN
declare @USDValue Money, @ConvertedPrice Money
set @USDValue = (select TOP 1 USDValue from HistoricalCurrencyPrice join Country on Country.CurrencyID = HistoricalCurrencyPrice.ID  join Port on Port.CountryID = Country.ID where port.id = @PortID and HistoricalCurrencyPrice.ValueDate < @TheDate order by ValueDate DESC)
set @ConvertedPrice = (select @LocalPrice * @USDValue)
return @ConvertedPrice
END
GO


COMMIT
GO

