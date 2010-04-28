#buttons

def makeButton(name, onClick):
    b = Button(name)
    b.Click += onClick
    return b

updateCommodityButton = makeButton("Update", updateCommodityPrices)
commoditySheet.H6 = updateCommodityButton

commodForecastButton = makeButton("Random Walk Forecast", commodForecast)
commoditySheet.H7 = commodForecastButton

commodGraphButton = makeButton("Draw graph", commodGraph)
commoditySheet.H8 = commodGraphButton

updateFXButton = Button("Update")
updateFXButton.Click += updateFXRates
fxSheet.H5 = updateFXButton

fxForecastButton = Button("Random Walk Forecast")
fxForecastButton.Click += fxForecast
fxSheet.H6 = fxForecastButton

fxGraphButton = makeButton("Draw graph", fxGraph)
fxSheet.H7 = fxGraphButton

solveTFPButton = Button("Solve")
solveTFPButton.Click += solveTFP
TFPSheet.H4 = solveTFPButton

runSortButton = Button("Run iterations")
runSortButton.Click += runSort
sortingSheet.F8 = runSortButton

