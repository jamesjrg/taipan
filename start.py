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

nTraders = 2
nShipping = 2

startTraderId = 1
startShippingId = 5

def startInNewConsole(process, dir):
    print 'start ' + dir+process
    os.system('start /D ' + dir + " " + dir+process)

startInNewConsole('Bank.exe %d %d' % (nTraders, nShipping), bankDir)
startInNewConsole('FateAndGuesswork.exe', fateDir)
startInNewConsole('FXServer.exe', fxDir)

for i in range(startTraderId, startTraderId + nTraders):
    startInNewConsole('Trader.exe %d %d' % (i, nShipping), traderDir)
    
for i in range(startShippingId, startShippingId + nShipping):
    startInNewConsole('ShippingCompany.exe %d %d' % (i, nTraders), shippingDir)

print 'all processes started'

