#!/usr/bin/env python

import os
import sys

print 'starting all processes'

exeDir = '\\bin\\Debug\\'
bankDir = 'Bank' + exeDir
fateDir = 'FateAndGuesswork' + exeDir
fxDir = 'FXServer' + exeDir
traderDir = 'Trader' + exeDir
shippingDir = 'ShippingCompany' + exeDir

nBank = 1
nFate = 1

nTraders = 2
nShipping = 2

startTraderId = 1
startShippingId = 5

if len(sys.argv) > 1:
    nBank = int(sys.argv[1])
    nFate = int(sys.argv[2])
    nTraders = int(sys.argv[3])
    nShipping = int(sys.argv[4])
    
def startInNewConsole(process, dir):
    print 'start ' + dir+process
    os.system('start /D ' + dir + " " + dir+process)

if nBank:
    startInNewConsole('Bank.exe %d %d' % (nTraders, nShipping), bankDir)
if nFate:    
    startInNewConsole('FateAndGuesswork.exe', fateDir)
startInNewConsole('FXServer.exe', fxDir)

for i in range(startTraderId, startTraderId + nTraders):
    startInNewConsole('Trader.exe %d %d' % (i, nShipping), traderDir)
    
for i in range(startShippingId, startShippingId + nShipping):
    startInNewConsole('ShippingCompany.exe %d %d' % (i, nTraders), shippingDir)

print 'all processes started'

