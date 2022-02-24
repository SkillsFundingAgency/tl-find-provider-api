
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

