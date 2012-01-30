#buttons

def makeButton(name, onClick):
    b = Button(name)
    b.Click += onClick
    return b

#GBM sheets
updateCommodityButton = makeButton("Update", updateCommodityPrices)
commoditySheet.H7 = updateCommodityButton

commodForecastButton = makeButton("Random Walk Forecast", commodForecast)
commoditySheet.H8 = commodForecastButton

commodGraphButton = makeButton("Draw graph", commodGraph)
commoditySheet.H9 = commodGraphButton

updateFXButton = makeButton("Update", updateFX)
fxSheet.H6 = updateFXButton

fxForecastButton = makeButton("Random Walk Forecast", fxForecast)
fxSheet.H7 = fxForecastButton

fxGraphButton = makeButton("Draw graph", fxGraph)
fxSheet.H8 = fxGraphButton

#Summary sheets
summaryButtons = []
simpleSummaryNames = ('futures', 'purchases', 'shipping')
complexSummaryNames = ('traderSum', 'shippingSum', 'countrySum')

for name in simpleSummaryNames + complexSummaryNames:
    button = makeButton("Update", globals()['%sUpdate' % name])
    
    if name in simpleSummaryNames:
        globals()['%sSheet' % name].A1 = button
    else:
        globals()['%sSheet' % name].A2 = button
    summaryButtons.append(button)
    
#TDP sheet
#xxx MS Solver not installed right now
#solveTFPButton = makeButton("Solve", solveTFP)
#TFPSheet.B9 = solveTFPButton

#Algs sheet
runSortButton = makeButton("Run iterations", runSort)
sortingSheet.F8 = runSortButton

