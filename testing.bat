echo starting testing processes

SET EXEDIR=\bin\Debug
SET BANK=Bank%EXEDIR%
SET DOMESTIC=DomesticCompany%EXEDIR%
SET FATE=FateAndGuesswork%EXEDIR%
SET FX=FXServer%EXEDIR%
SET SHIPPING=ShippingCompany%EXEDIR%
SET TRADER=Trader%EXEDIR%

start /D %BANK% %BANK%\Bank.exe
start /D %FX% %FX%\FXServer.exe

echo all processes started


