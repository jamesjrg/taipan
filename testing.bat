echo starting testing processes

SET EXEDIR=\bin\Debug
SET BANK=Bank%EXEDIR%
SET DOMESTIC=DomesticCompany%EXEDIR%

start /D %BANK% %BANK%\Bank.exe

for %%A in (1) do call :StartDomesticCompany %%A

GOTO End

:StartDomesticCompany
start /D %DOMESTIC% %DOMESTIC%\DomesticCompany.exe %1
goto:eof

:End
echo all processes started


