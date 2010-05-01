BEGIN TRANSACTION
GO

exec procStockUpdate 6, '2010-04-15T20:10:00', 100;
exec procStockUpdate 6, '2010-04-15T20:10:05', 101;
exec procStockUpdate 6, '2010-04-15T20:10:10', 98;
exec procStockUpdate 6, '2010-04-15T20:10:15', 54;

exec procBalanceUpdate 1, '2010-04-15T20:10:00', 10000;
exec procBalanceUpdate 1, '2010-04-15T20:10:05', 10100;
exec procBalanceUpdate 1, '2010-04-15T20:10:10', 98000;
exec procBalanceUpdate 1, '2010-04-15T20:10:15', 54000;

exec procPortComodUpdate 1, 1, '2010-04-15T20:10:00', 100;
exec procPortComodUpdate 1, 1, '2010-04-15T20:10:05', 101;
exec procPortComodUpdate 1, 1, '2010-04-15T20:10:10', 98;
exec procPortComodUpdate 1, 1, '2010-04-15T20:10:15', 54;
    
COMMIT
GO



