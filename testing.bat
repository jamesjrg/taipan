echo starting testing processes

SET EXEDIR=\bin\Debug
SET BANK=Bank%EXEDIR%
SET FATE=FateAndGuesswork%EXEDIR%
SET FX=FXServer%EXEDIR%
SET SHIPPING=ShippingCompany%EXEDIR%
SET TRADER=Trader%EXEDIR%

:: start /D %FATE% %FATE%\FateAndGuesswork.exe
start /D %FX% %FX%\FXServer.exe
start /D %BANK% %BANK%\Bank.exe

GOTO End

:End
echo all processes started


