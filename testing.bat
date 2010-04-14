echo starting testing processes

SET EXEDIR=\bin\Debug
SET BANK=Bank%EXEDIR%
SET DOMESTIC=DomesticCompany%EXEDIR%
SET FATE=FateAndGuesswork%EXEDIR%
SET FX=FXServer%EXEDIR%
SET SHIPPING=ShippingCompany%EXEDIR%
SET TRADER=Trader%EXEDIR%

:: start /D %FATE% %FATE%\FateAndGuesswork.exe
start /D %FX% %FX%\FXServer.exe
start /D %BANK% %BANK%\Bank.exe
:: for %%A in (1 2) do call :StartDomesticCompany %%A

GOTO End

:StartDomesticCompany
start /D %DOMESTIC% %DOMESTIC%\DomesticCompany.exe %1
goto:eof

:End
echo all processes started


