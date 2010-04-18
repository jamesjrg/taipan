#

updateCommodityButton = Button("Update")
updateCommodityButton.Click += updateCommodityPrices
commoditySheet.H5 = updateCommodityButton
   
updateFXButton = Button("Update")
updateFXButton.Click += updateFXRates
fxSheet.H4 = updateFXButton

solveTFPButton = Button("Solve")
solveTFPButton.Click += solveTFP
TFPSheet.H4 = solveTFPButton

runSortButton = Button("Run iterations")
runSortButton.Click += runSort
sortingSheet.H4 = runSortButton

