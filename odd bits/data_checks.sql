﻿select USDValue from Currency inner join Country on Country.CurrencyID = Currency.ID inner join Port on Port.CountryID = Country.ID where port.id = 2

select Country.Name, Port.Name, Currency.Name, USDValue from Currency inner join Country on Country.CurrencyID = Currency.ID inner join Port on Port.CountryID = Country.ID
