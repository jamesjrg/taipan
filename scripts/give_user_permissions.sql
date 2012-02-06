-- create users for logins, and add database roles
CREATE USER [taipan-r] FOR LOGIN [taipan-r]
GO
EXEC sp_addrolemember N'db_datareader', N'taipan-r'
GO

CREATE USER [taipan-r] FOR LOGIN [taipan-r]
GO
EXEC sp_addrolemember N'db_datareader', N'taipan-r'
GO

CREATE USER [taipan-rw] FOR LOGIN [taipan-rw]
GO
EXEC sp_addrolemember N'db_datareader', N'taipan-rw'
GO
EXEC sp_addrolemember N'db_datawriter', N'taipan-rw'
GO
EXEC sp_addrolemember N'db_ddladmin', N'taipan-rw'
GO

-- stored procedure execution role
CREATE ROLE db_executor
GRANT EXECUTE TO db_executor	

EXEC sp_addrolemember 'db_executor', 'taipan-r'
EXEC sp_addrolemember 'db_executor', 'taipan-rw'