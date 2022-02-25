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
- added search from current location. This does a call to get closest postcode
- 

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
