# R Scripts

## Setup

Open the project `provider-data.Rproj` in R Studio.

Before running these scripts for the first time, run the following command to install libraries:
```
renv::init()
```

(You might need to install `renv` first by running `install.packages("renv")`)


## Data import

The script `Provider Data 2023 Import.R` can be used to extract T Level providers and qualifications from the master spreadsheet.

Download the spreadsheet and save to the `temp` folder.

Make sure the file name is correct in script variable `providers_xlsx_file`. 

Run the script.

If all is well, the output will appear in the `output` folder.

- `ProviderData.json` is a full extract for use in the Find Provider API.
- `ProviderDataNoDetails.json` is a simplified extract for use in the Marketing and Communications Campaign website.
