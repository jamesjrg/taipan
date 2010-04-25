#buttons

def makeButton(name, onClick):
    b = Button(name)
    b.Click += onClick
    return b

commoditySheet.Cells.I1.DropdownItems = ['spam', 'spam']
commoditySheet.Cells.I2.DropdownItems = ['spam', 'spam']
commoditySheet.Cells.I3.DropdownItems = ['spam', 'spam']
commoditySheet.Cells.I4.DropdownItems = ['spam', 'spam']
  
updateCommodityButton = makeButton("Update", updateCommodityPrices)
commoditySheet.H6 = updateCommodityButton

commodForecastButton = makeButton("Random Walk Forecast", commodForecast)
commoditySheet.H7 = commodForecastButton

commodClearForecastButton = makeButton("Clear Forecast", commodClearForecast)
commoditySheet.H8 = commodClearForecastButton

commodGraphButton = makeButton("Draw graph", commodGraph)
commoditySheet.H10 = commodGraphButton

fxSheet.Cells.I1.DropdownItems = ['spam', 'spam']
fxSheet.Cells.I2.DropdownItems = ['spam', 'spam']
fxSheet.Cells.I3.DropdownItems = ['spam', 'spam']

updateFXButton = Button("Update")
updateFXButton.Click += updateFXRates
fxSheet.H5 = updateFXButton

fxForecastButton = Button("Random Walk Forecast")
fxForecastButton.Click += fxForecast
fxSheet.H6 = fxForecastButton

fxClearForecastButton = makeButton("Clear Forecast", fxClearForecast)
fxSheet.H7 = fxClearForecastButton

fxGraphButton = makeButton("Draw graph", fxGraph)
fxSheet.H9 = fxGraphButton

solveTFPButton = Button("Solve")
solveTFPButton.Click += solveTFP
TFPSheet.H4 = solveTFPButton

runSortButton = Button("Run iterations")
runSortButton.Click += runSort
sortingSheet.H4 = runSortButton

