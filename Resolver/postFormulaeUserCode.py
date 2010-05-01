#buttons

def makeButton(name, onClick):
    b = Button(name)
    b.Click += onClick
    return b

updateCommodityButton = makeButton("Update", updateCommodityPrices)
commoditySheet.H7 = updateCommodityButton

commodForecastButton = makeButton("Random Walk Forecast", commodForecast)
commoditySheet.H8 = commodForecastButton

commodGraphButton = makeButton("Draw graph", commodGraph)
commoditySheet.H9 = commodGraphButton

updateFXButton = Button("Update")
updateFXButton.Click += updateFXRates
fxSheet.H6 = updateFXButton

fxForecastButton = Button("Random Walk Forecast")
fxForecastButton.Click += fxForecast
fxSheet.H7 = fxForecastButton

fxGraphButton = makeButton("Draw graph", fxGraph)
fxSheet.H8 = fxGraphButton

solveTFPButton = Button("Solve")
solveTFPButton.Click += solveTFP
TFPSheet.H4 = solveTFPButton

runSortButton = Button("Run iterations")
runSortButton.Click += runSort
sortingSheet.F8 = runSortButton

