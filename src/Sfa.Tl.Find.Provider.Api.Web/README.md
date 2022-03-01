# POC for possible design changes

## Setup

Added the following npm package for HMAC:
```
npm install crypto-js
```

Added the following dev dependenciews for gulp:
```
npm install gulp --save-dev
npm install gulp-concat --save-dev
npm install gulp-minify --save-dev
```

Had to add this site to the configuration as a CORS origin: 
in the Configuration table for `LOCAL` `Sfa.Tl.Find.Provider.Api_1.0`
include the site in `"AllowedCorsOrigins"` as below:

```
  "AllowedCorsOrigins": "<existing sites>;https://localhost:7026;",
```


## Changes to demo

- added address line 1/2 and county
- added journey link
- added location name
  - rty EX4 3RX or NG7 2SZ
- added search from current location. This does a call to get closest postcode
- added links to Ofsted and UKRLP - data is imported from ULRLP and URN used to create link
- added checkbox list with routes ("skill areas") and use that in search
- TODO: Autocomplete for towns/postcodes
  - what does "search by town" mean? Find a centroid lat/long and search from there? 
  - we only have a few towns in our data, so we'd need a service/API endpoint listing towns and lat/long


## TODO

- check the call below in zendesk - it shouldn't be passing the page and page size defaults
```
         $("#tl-search-providers").click(function() {
            return providerSearch($("#tl-postcode").val().trim(), 0, 5);
        });
```

- type in message in LoadAdditionalProviderData - check in marcoms as well
```
     _logger.LogInformation("Saved {providerCount} providers from ", providers.Count);

```

- search from current location uses closest postcode. Could also pass the lat-long to a new API method
