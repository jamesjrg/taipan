USE TaiPan

-- stored procedure execution role
CREATE ROLE db_executor
GRANT EXECUTE TO db_executor	

EXEC sp_addrolemember 'db_executor', 'taipan-r'
EXEC sp_addrolemember 'db_executor', 'taipan-rw'