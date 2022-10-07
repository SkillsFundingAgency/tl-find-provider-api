# 2023 Provider import from spreadsheet
# Assumes Provider Data 2023 Download.R has been run to download the spreadsheet

library(jsonlite)
library(glue)
library(tidyverse)
library(readxl)
library(magrittr)

source("./src/Postcode Functions.R")

#rm(list=ls())

############################
# Constants and file paths #
############################

output_dir <- "./output"
temp_dir <- "./temp"

#providers_xlsx_file <- fs::path(temp_dir, "MASTER PROVIDER LIST BY T LEVEL - DO NOT DELETE.xlsx")
providers_xlsx_file <- fs::path(temp_dir, "MASTER Provider List by T level October 2022.xlsx")

provider_json_file = fs::path(output_dir, "ProviderData.json")
provider_json_simplified_file = fs::path(output_dir, "ProviderDataNoDetails.json")

####################
# Useful functions #
####################
 
if(!exists("%not_in%")) {
  `%not_in%` = function(x,y) !(x %in% y) #Then you can do x %notin% y instead of !(x %in% y)
}

#############################
# Get qualification mapping #
#############################

# Create qualifications lookup table

#need to remove a few small words (surrounded by space \\s) and punctuation characters, as well as any digits
lookup_key_regex = "(?:,|\\sand\\s|\\sfor\\s|[:space:]|\\(|\\)|\\d)*"

qualifications <- tribble(
  ~frameworkCode, ~name,
  36, "Design, Surveying and Planning for Construction",
  37, "Digital Production, Design and Development",
  38, "Education and Childcare",
  39, "Digital Business Services",
  40, "Digital Support Services",
  41, "Health",
  42, "Healthcare Science",
  43, "Science",
  44, "Onsite Construction",
  45, "Building Services Engineering for Construction",
  46, "Finance",
  47, "Accounting",
  #TODO: The next 2 pairs are the wrong way round and will be swapped later - 48<->49, 50<->51
  48, "Design and Development for Engineering and Manufacturing",
  49, "Maintenance, Installation and Repair for Engineering and Manufacturing",
  49, "Maintenance, Installation and Repair",   #alternative title in spreadsheet
  50, "Engineering, Manufacturing, Processing and Control",
  50, "Manufacturing, Processing and Control",  #alternative title in spreadsheet
  51, "Management and Administration",
  #New 2023/24 courses
  52, "Legal",
  52, "Legal Services",
  53, "Hair, Beauty and Aesthetics",
  53, "Hairdressing, Barbering and Beauty Therapy",
  54, "Craft and Design",
  55, "Media, Broadcast and Production",
  56, "Catering",
  57, "Agriculture, Land Management and Production",
  58, "Animal Care and Management"
  ) %>% 
  mutate(across(frameworkCode, as.integer),
         lookup_key = str_remove_all(name, lookup_key_regex))

#############
# Load data #
#############

#Skip first two rows, nothing useful in them
if(exists("provider_data_download")) {
  providers_xlsx_file <- provider_data_download$local_path
}

provider_spreadsheet = read_xlsx(providers_xlsx_file, 
                             col_names = FALSE, 
                             sheet = 1,
                             skip = 2)

# Get the years
extract_academic_years <- function(row, year_col_name = "year") {
  year_col_name = enquo(year_col_name)
  row |> 
    pivot_longer(cols = everything(), 
                 names_to = "col_index", 
                 values_to = "name") |>
    mutate(name = ifelse(str_starts(name, "202"), name, NA)) |> 
    separate(name, c("year"), extra = "drop") |> 
    mutate_at(c("year"), as.integer) |> 
    fill(year) |> 
    select(!!quo_name(year_col_name) := year)
}

academic_years <- extract_academic_years(provider_spreadsheet[1,], "deliveryYear")
#View(academic_years)  

# Get the unique qualification names from the row headers
(unique_qualifications <- provider_spreadsheet[3,] |> 
    pivot_longer(cols = everything(), 
                 names_to = "col_index", 
                 values_to = "name") |> 
    filter(name %not_in% c("UKPRN",
                           "Approved",
                           "Year Approved", # Old name for the Approved column
                           "Name",
                           "Already completed course directory",
                           "Town",
                           "Postcode",
                           "Website",
                           "Email address",
                           "Telephone number",
                           "Deferral of all T Levels",
                           "Provider Type",
                           "Region")) |>
    filter(!str_ends(name, "route[2|3]?")) |>
    filter(!str_starts(name, "Total T Levels")) |>
    #filter(name %in% c("Healthcare Science  (    2)", "Healthcare Science")) |>
    #mutate(name = str_remove(name, " \\([0-9]\\)")) # |> View()
    #mutate(name = str_remove(name, " \\(\\w\\*[0-9]\\)")) # |> View()
    
    mutate(name = str_remove(name, "[:space:]*\\([:space:]*[0-9]\\)$"), 
           name = str_remove(name, "T Level")) |>
    group_by(name) |> 
    summarise() |>
    mutate(lookup_key = str_remove_all(name, lookup_key_regex))
) # %T>% View()

#Get all possible names
new_names <- provider_spreadsheet[3,] |> 
  pivot_longer(cols = everything(), 
               names_to = "col_index", 
               values_to = "name") |> 
  mutate(name = str_remove(name, "[:space:]*\\([:space:]*[0-9]\\)$"), 
         name = str_remove(name, "T Level")) |> 
  mutate(lookup_key = str_remove_all(name, lookup_key_regex)) %>% 
  # Add known names
  mutate(new_name = case_when(
    name ==  "UKPRN" ~ "ukprn",
    name == "Approved" ~ "approved_year",
    name == "Year Approved" ~ "approved_year",
    name == "Name" ~ "providerName",
    name == "Already completed course directory" ~ "is_in_course_directory",
    name == "Town" ~ "town",
    name == "Postcode" ~ "postcode",
    name == "Website" ~ "website",
    name == "Email address" ~ "email",
    name == "Telephone number" ~ "telephone"
  )) %>% 
  #add the academic years
  bind_cols(academic_years |> select(deliveryYear)) %>%
  #join to qualifications lookup to get framework code
  left_join((qualifications |> rename(qual_name = name)), by = "lookup_key")  %>% 
  mutate(new_name = if_else(!is.na(frameworkCode) & !is.na(deliveryYear), as.character(glue("{deliveryYear}_{frameworkCode}")), new_name)) %>%  #as.character(frameworkCode)
  #Copy any left-over names
  mutate(new_name = if_else(!is.na(new_name), new_name, unclass(glue("unused_{name}_{col_index}"))))
#View(new_names)

included_years <- c(2022, 2023, 2024)

provider_data <- provider_spreadsheet %>% 
  rename_all(~new_names %>% pull(new_name)) %>% 
  slice(4:n()) %>% 
  # rename(ukprn = lookupNameIndex("UKPRN"),
  #        approved_year =  lookupNameIndex("Year Approved"),
  #        providerName = lookupNameIndex("Name"),
  #        is_in_course_directory = lookupNameIndex("Already completed course directory"),
  #        town = lookupNameIndex("Town"),
  #        postcode = lookupNameIndex("Postcode"),
  #        website = lookupNameIndex("Website"),
  #        email = lookupNameIndex("Email address"),
  #        telephone = lookupNameIndex("Telephone number")
  #        ) %>% 
  # remove cols with name starting unused_
  select(!starts_with("unused")) %>% 
  select(!starts_with("Totals")) %>% 
  # remove some rows we won't need
  filter(ukprn != "Total") %>% 
  # Filter to the required approved year and exclude any that are already in course director
  #mutate(across(c(ukprn, approved_year), as.integer)) %>% 
  #mutate(across(c(is_in_course_directory), as.logical)) %>% 
  #mutate(is_in_course_directory = replace_na(is_in_course_directory, FALSE)) %>% 
  mutate(across(c(ukprn, approved_year), as.integer), 
         across(c(is_in_course_directory), as.logical),
         is_in_course_directory = replace_na(is_in_course_directory, FALSE)) %>% 
  # Filter to the required approved year and exclude any that are already in course director
  filter(approved_year %in% included_years, !is_in_course_directory) %>% 
 #filter(name %in% c("Healthcare Science  (    2)", "Healthcare Science")) |>
  arrange(providerName, postcode)

#view(provider_data)

provider_data %>% 
  select(ukprn, approved_year, providerName, is_in_course_directory,
         town, postcode, website, email, telephone) 

#################
# Add postcodes #
#################

#provider_data
known_postcode_locations <- load_and_update_postcodes_file(provider_data)

# Set display options so we can see lat/long 
options(pillar.sigfig = 8)

#Merge locations into data
# known_postcode_locations %>% 
#   filter(!is.na(latitude)) %>% 
#   select(postcode, latitude, longitude)

provider_data <- provider_data %>%  #85 x 79
  left_join(known_postcode_locations %>% 
              filter(!is.na(latitude)) %>% 
              select(postcode, latitude, longitude)) 

####################
# Fix known errors #
####################
# provider_data <- provider_data %>% 
#   mutate(providerName = case_when(
#     providerName ==  "NORTH WARWICKSHIRE AND SOUTH LEICESTERSHIRE COLLEG" 
#                       ~ "NORTH WARWICKSHIRE AND SOUTH LEICESTERSHIRE COLLEGE",
#     TRUE ~ providerName
#     ))


##########################################
# Tidy delivery years and qualifications #
##########################################

# Pivot out the delivery year and qualification framework 
# Only include mpm-na values, which represent the "x" indicating the qualificartion is offered 
# Drop the value column at the end since we won't use it

provider_data_with_qualifications <- provider_data %>% 
  pivot_longer(cols = c(matches('^202\\d_\\d{2}$')),
               names_to = c("deliveryYear", "frameworkCode"),
               names_sep = "_",
               values_to = "x",
               values_drop_na = TRUE) %>% 
  mutate(across(c(deliveryYear, frameworkCode), as.integer)) %>% 
  select(-x)

#View(provider_data_with_qualifications)

#Look for duplicates
provider_data %>% 
  group_by(ukprn, providerName) %>% 
  summarise(n = n()) %>% 
  filter(n > 1) %>% View()

########################
# Nest the hierarchies #
########################

filtered_provider_data <- provider_data_with_qualifications %>% 
  # Only select delivery years that we want to upload
  filter(deliveryYear %in% included_years) %>%
  ###filter((approved_year == 2022 & deliveryYear == 2022) |
  ###         (approved_year >= 2023)) %>% 
  # Only include existing locations 
  filter(!is.na(postcode)) %>% 
  rename(ukPrn = ukprn, 
         name = providerName) %>% 
  select(-approved_year, -is_in_course_directory) 
#filtered_provider_data %>% filter(ukPrn == 10007527)
#View(filtered_provider_data)

provider_data_nested_without_details <- filtered_provider_data %>% 
  #For campaign site, don't need email or phone
  select(-email, -telephone) %>% 
  # rename(ukPrn = ukprn, 
  #        name = providerName) %>% 
  # select(-approved_year, -is_in_course_directory) %>% 
  group_by(ukPrn, postcode, deliveryYear) %>% 
  mutate(qualifications = list(sort(unlist(list(frameworkCode))))) %>%
  distinct(ukPrn, postcode, deliveryYear, .keep_all = TRUE) %>% 
  ungroup() %>% 
  select(-frameworkCode) %>% 
  rename( year = deliveryYear) %>% 
  nest_by(ukPrn,
          name,
          postcode,
          town,
          latitude,
          longitude,
          website,
          .key = "deliveryYears") %>%
  nest_by(ukPrn, 
          name, 
          .key = "locations") %>% 
  arrange(name) 

provider_data_nested_with_details <- filtered_provider_data %>% 
  # rename(ukPrn = ukprn, 
  #        name = providerName) %>% 
  # select(-approved_year, -is_in_course_directory) %>% 
  group_by(ukPrn, postcode, deliveryYear) %>% 
  mutate(qualifications = list(sort(unlist(list(frameworkCode))))) %>%
  distinct(ukPrn, postcode, deliveryYear, .keep_all = TRUE) %>% 
  ungroup() %>% 
  select(-frameworkCode) %>% 
  rename( year = deliveryYear) %>% 
  mutate(
    providerPostcode = postcode,
    providerTown = town,
    providerWebsite = website,
    providerEmail = email,
    providerTelephone = telephone
  ) %>% 
  nest_by(ukPrn,
          name,
          providerPostcode, 
          providerTown, 
          providerWebsite, 
          providerEmail, 
          providerTelephone, 
          postcode,
          town,
          latitude,
          longitude,
          website,
          email,
          telephone,
          .key = "deliveryYears") %>%
  nest_by(ukPrn, 
          name, 
          providerPostcode, 
          providerTown, 
          providerWebsite, 
          providerEmail, 
          providerTelephone, 
          .key = "locations") %>% 
  rename(
    postcode = providerPostcode,
    town = providerTown,
    website = providerWebsite,
    email = providerEmail,
    telephone = providerTelephone
  ) %>% 
  arrange(name) 

  
######################
# Write to json file #
######################

if(!fs::dir_exists(output_dir)) { 
  fs::dir_create(output_dir) 
}

list(providers = provider_data_nested_with_details) %>% 
  #toJSON(pretty = TRUE)
  write_json(provider_json_file, 
             digits = 6,
             pretty = TRUE)

list(providers = provider_data_nested_without_details) %>% 
  #toJSON(pretty = TRUE)
  write_json(provider_json_simplified_file, 
             digits = 6,
             pretty = TRUE)

#unlink(provider_json_file)
#unlink(provider_json_simplified_file)
