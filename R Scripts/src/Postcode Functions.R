library(httr)
library(jsonlite)
library(tidyr)
library(dplyr)
library(stringr)
library(utils)
library(purrr)

# Lookup:
# api.postcodes.io/postcodes/OX2+9GX

postcode_regex <-   "^(([A-Z][0-9]{1,2})|(([A-Z][A-HJ-Y][0-9]{1,2})|(([A-Z][0-9][A-Z])|([A-Z][A-HJ-Y][0-9]?[A-Z]))))( *[0-9][A-Z]{2})?$"

get_postcode_location <- function(postcode) {
  base_uri <- "http://api.postcodes.io/"

  if(str_length(postcode) <= 4) {
    message("Searching for outcode ", postcode)
    outcode_result <- GET(paste0(base_uri, "outcodes/", URLencode(postcode)))
    
    if(http_error(outcode_result))
    {
      message("No result found for ", postcode)
      return (NULL)
    }
    
    with(fromJSON(content(outcode_result, as = "text")), 
         tibble(postcode = result$outcode, 
                latitude = ifelse(!is.null(result$latitude), result$latitude, NA_real_),
                longitude = ifelse(!is.null(result$longitude), result$longitude, NA_real_),
                region = NA_character_,
                terminated = FALSE,
                terminated_month = NA_integer_,
                terminated_year = NA_integer_)
         #wrdz = result$year_terminated,
         #terminated_year = if_else(isTerminated, "result$year_terminated", "NA"))
    )
  } else {
    message("Searching for postcode ", postcode)
    postcode_result <- GET(paste0(base_uri, "postcodes/", URLencode(postcode)))
  
    isTerminated <- FALSE
    
    if(http_error(postcode_result))
    {
      message("Searching for terminated postcode ", postcode)
      postcode_result <- GET(paste0(base_uri, "terminated_postcodes/", URLencode(postcode)))
      isTerminated <- !http_error(postcode_result)
    }
    
    if(http_error(postcode_result))
    {
      message("No result found for ", postcode)
      return (NULL)
    }
    
    with(fromJSON(content(postcode_result, as = "text")), 
         tibble(postcode = result$postcode, 
                latitude = ifelse(!is.null(result$latitude), result$latitude, NA_real_),
                longitude = ifelse(!is.null(result$longitude), result$longitude, NA_real_),
                region = ifelse(!is.null(result$region), result$region, NA_character_),
                terminated = isTerminated,
                terminated_month = ifelse(isTerminated, result$month_terminated, NA_integer_),
                terminated_year = ifelse(isTerminated, result$year_terminated, NA_integer_)
                )
         )
    }
}

get_postcode_locations <- function(postcodes) {
  #loop over all postcodes, call the single postcode function, and bind into a tibble
  bind_rows(map(postcodes, get_postcode_location))
}

get_all_postcodes <- function(path = "D:/ESFA/ONSPD_NOV_2021_UK.csv") {
  postcodes <-  read_csv(path, 
                         col_types = cols(.default = col_character())) #read all as chars to avoid parsing errors
  postcodes %>%  
    select(pcds, lep1) %>% 
    mutate(searchable_postcode = str_replace_all(pcds, fixed(" "), ""))
}

normalizePostcode <- function(postcode) {
  #Normalises postcode to all upper case with no spaces
  str_replace_all(str_to_upper(postcode), " ", "")
}

groupNormalizePostcodes <- function(postcodes) {
  #Normalises postcodes, then groups them
  postcodes %>%
    #mutate(postcode = postcode)
    mutate(postcode = normalizePostcode(postcode)) %>% 
    group_by(postcode) %>% 
    summarize()
}

# Requires data with a single postcode column
# The data doesn't need to be distict as this function will habdle that
load_and_update_postcodes_file <- function(data, 
                                           postcode_data_file_name = "postcode_locations.RDS",
                                           postcode_data_dir = "./temp") {
    postcode_data_file_path <-  fs::path(postcode_data_dir, postcode_data_file_name)
    if(fs::file_exists(postcode_data_file_path)) {
      cat("loading file ", postcode_data_file_path)
      known_postcode_locations <- read_rds(postcode_data_file_path)
  } else {
    cat("postcodes file not found, ", postcode_data_file_path)
  }
    
    groupNormalizePostcodes(data)
    
  if(exists("known_postcode_locations")) {
    print("known_postcode_locations found")
    unknown_postcode_locations <- groupNormalizePostcodes(data) %>% 
      anti_join(groupNormalizePostcodes(known_postcode_locations))
  } else {
    print("known_postcode_locations not found")
    unknown_postcode_locations <- data
  }

  new_postcode_locations <- unknown_postcode_locations %>% 
    distinct(postcode, .keep_all = FALSE) %>% 
    filter(str_detect(postcode, postcode_regex)) %>% #Filter out invalid postcodes
    pull() %>% 
    get_postcode_locations()

  # Merge back to known_postcode_locations
  if(exists("known_postcode_locations")) {
    print("joining back")
    known_postcode_locations <- known_postcode_locations %>% 
    bind_rows(new_postcode_locations)
  } else {
    known_postcode_locations <- new_postcode_locations
  }
  
  # Write known postcodes back
  # Save back to file
  print("Saving data back to file")
  if(!fs::dir_exists(postcode_data_dir)) { fs::dir_create(postcode_data_dir) };
  write_rds(known_postcode_locations, 
            file = postcode_data_file_path)

  known_postcode_locations  
}
  

#Tests

# Set significant figure display
#options(pillar.sigfig=8)

#get_postcode_location("OX2 9GX")
#get_postcode_location("CV1 2WT")
#get_postcode_location("BB5 2AW") #Terminated
#get_postcode_location("S70 2YW") #Terminated
#get_postcode_location("ABC 123") #Not a postcode

#get_postcode_locations(c("OX2 9GX", "BB5 2AW", "S70 2YW", "X"))

# Load list of postcodes from csv
# p <- read_csv("./data/raw_postcodes.csv")
# p
# x <- get_postcode_locations(p$Postcode)
# x
# x %>% write_csv(path = "./data/pnew.csv")
# write_csv(x, path = "./data/pnew.csv")
# 
