echo starting all processes

SET EXEDIR=\bin\Debug
SET BANK=Bank%EXEDIR%
SET FATE=FateAndGuesswork%EXEDIR%
SET FX=FXServer%EXEDIR%
SET SHIPPING=ShippingCompany%EXEDIR%
SET TRADER=Trader%EXEDIR%

start /D %BANK% %BANK%\Bank.exe
start /D %FATE% %FATE%\FateAndGuesswork.exe
start /D %FX% %FX%\FXServer.exe

for %%A in (1 2) do call :StartShippingCompany %%A
for %%A in (1 2) do call :StartTrader %%A

GOTO End

:StartShippingCompany
start /D %SHIPPING% %SHIPPING%\ShippingCompany.exe %1
goto:eof

:StartTrader
start /D %TRADER% %TRADER%\Trader.exe %1
goto:eof

:End
echo all processes started


