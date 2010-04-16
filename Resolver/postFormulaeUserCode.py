#

updateCommodityButton = Button("Update")
updateCommodityButton.Click += updateCommodityPrices
commoditySheet.H5 = updateCommodityButton
   
updateFXButton = Button("Update")
updateFXButton.Click += updateFXRates
fxSheet.H4 = updateFXButton
