#!/usr/bin/env python

import os
print 'starting all processes'

exeDir = '\\bin\\Debug\\'
bankDir = 'Bank' + exeDir
print bankDir
fateDir = 'FateAndGuesswork' + exeDir
fxDir = 'FXServer' + exeDir
traderDir = 'Trader' + exeDir
shippingDir = 'ShippingCompany' + exeDir

def startInNewConsole(process, dir):
    print 'start ' + dir+process
    os.system('start /D ' + dir + " " + dir+process)

startInNewConsole('Bank.exe', bankDir)
startInNewConsole('FateAndGuesswork.exe', fateDir)
startInNewConsole('FXServer.exe', fxDir)

for i in range(1, 3):
    startInNewConsole('Trader.exe %d' % i, traderDir)
    
for i in range(5, 7):
    startInNewConsole('ShippingCompany.exe %d' % i, shippingDir)

print 'all processes started'

