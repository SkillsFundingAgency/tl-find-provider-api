--This index requires a unique key - Search field has duplicates unless id is appended
CREATE UNIQUE CLUSTERED INDEX [IX_TownSearchView_Search]
ON TownSearchView(Search);
