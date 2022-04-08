CREATE FUNCTION [dbo].[ReplaceAllFunction]
(
    @input          VARCHAR(MAX),
    @match_pattern  VARCHAR(MAX),
    @match_length   INT,
    @replace_value  VARCHAR(MAX)
)
RETURNS NVARCHAR(250)  WITH SCHEMABINDING
BEGIN
    DECLARE @output     VARCHAR(MAX) = '',
            @input_copy VARCHAR(MAX) = @input,
            @match_ix   INT;

    SET @match_ix = PATINDEX(@match_pattern, @input_copy);
    
    WHILE @match_ix > 0
    BEGIN

        SET @output = @output + SUBSTRING(@input_copy, 1, @match_ix - 1) + @replace_value;
        SET @input_copy = SUBSTRING(@input_copy, @match_ix + @match_length, LEN(@input_copy));

        SET @match_ix = PATINDEX(@match_pattern, @input_copy);
    END

    SET @output = @output + @input_copy;

    RETURN @output;
END
