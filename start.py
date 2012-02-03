#!/usr/bin/env python

import os
import sys

exeDir = '\\bin\\Debug\\'
bankDir = 'Bank' + exeDir
fateDir = 'FateAndGuesswork' + exeDir
fxDir = 'FXServer' + exeDir
traderDir = 'Trader' + exeDir
shippingDir = 'ShippingCompany' + exeDir

nBank = 1
nFate = 1
nFX = 1

nTraders = 2
nShipping = 2

startTraderId = 1
startShippingId = 6

if len (sys.argv) == 1:
    pass
elif len(sys.argv) == 6:
    nBank = int(sys.argv[1])
    nFate = int(sys.argv[2])
    nFX = int(sys.argv[3])
    nTraders = int(sys.argv[4])
    nShipping = int(sys.argv[5])
else:
    print ('./start.py [nBank nFate nFX nTraders nShipping] (or no args for defaults)')
    sys.exit(2)
    
def startInNewConsole(process, dir):
    print 'start ' + dir+process
    os.system('start /D ' + dir + " " + dir+process)

print 'starting requested processes'
if nBank:
    startInNewConsole('Bank.exe', bankDir)
if nFate:    
    startInNewConsole('FateAndGuesswork.exe', fateDir)
if nFX:
    startInNewConsole('FXServer.exe', fxDir)

for i in range(startTraderId, startTraderId + nTraders):
    startInNewConsole('Trader.exe %d' % i, traderDir)
    
for i in range(startShippingId, startShippingId + nShipping):
    startInNewConsole('ShippingCompany.exe %d %d' % (i, nTraders), shippingDir)

print 'all processes started'

