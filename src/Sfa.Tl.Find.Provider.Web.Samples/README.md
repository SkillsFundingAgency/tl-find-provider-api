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
- added location name
  - try EX4 3RX or NG7 2SZ
  - Can Course Directory supply group name?
    - but look at group name vs provider name vs location name
- added search from current location. This does a call to get closest postcode
- added links to Ofsted and UKRLP - data is imported from ULRLP and URN used to create link
- added checkbox list with routes ("skill areas") and use that in search
- TODO: Autocomplete for towns/postcodes
  - what does "search by town" mean? Find a centroid lat/long and search from there? 
  - we only have a few towns in our data, so we'd need a service/API endpoint listing towns and lat/long
- locations - Ordnance Survey API, loooks like it's free for public sector
  - https://developer.ordnancesurvey.co.uk/os-places-api
  - https://osdatahub.os.uk/
  - ArcGIS 
    - https://developers.arcgis.com/javascript/3/
    - https://gis.stackexchange.com/questions/141660/is-arcgis-javascript-api-and-sdk-free
  - ONS - https://www.api.gov.uk/ons/open-geography-portal/#open-geography-portal
  - - open geography - https://geoportal.statistics.gov.uk/datasets/ons::built-up-area-to-built-up-area-sub-division-march-2011-lookup-in-england-and-wales/api
    - 

## TODO

- typo in message in LoadAdditionalProviderData - check in marcoms as well
```
     _logger.LogInformation("Saved {providerCount} providers from ", providers.Count);

```

