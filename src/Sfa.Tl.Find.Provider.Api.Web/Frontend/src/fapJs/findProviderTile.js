/* Find Provider Tile */

$(document).ready(function () {
    const findProviderRedirectUrl =
        $('script[data-findProviderRedirectUrl][data-findProviderRedirectUrl!=null]').attr('data-findProviderRedirectUrl');
    let findProvidersApiUrl = 
        $('script[data-findProviderApiUri][data-findProviderApiUri!=null]').attr('data-findProviderApiUri');
    
    if (typeof findProviderRedirectUrl === "undefined" ||
        typeof findProvidersApiUrl === "undefined") {
        console.log('findProviderTile script requires data-findProviderApiUri and data-findProviderRedirectUrl to be passed via the script tag');
        return;
    }

    //TODO: Remove - only for local testing
    if ((findProvidersApiUrl === null || findProvidersApiUrl.startsWith("{{")) && $('#find_provider_api_uri')) findProvidersApiUrl = $('#find_provider_api_uri').val();
    if ((findProviderRedirectUrl === null || findProviderRedirectUrl.includes("{{")) && $('#find_provider_api_uri')) findProvidersApiUrl = $('#find_provider_redirect_uri').val();
    //

    if (findProvidersApiUrl.substr(-1) !== '/') findProvidersApiUrl += '/';

    console.log("findProviderRedirectUrl = " + findProviderRedirectUrl);
    console.log("findProvidersApiUrl = " + findProvidersApiUrl);

        $('#tl-search-term').val("");

        $('#tl-search-term').keypress(function(e) {
            if (e.which === 13) {
                $("#tl-search-providers").click();
                return false;
            }
        });

        $("#tl-search-providers").click(function() {
            const searchTerm = $("#tl-search-term").val().trim();

            if (searchTerm === "") {
                event.stopPropagation();
                showSearchTermError("Enter a postcode or town");
                return false;
            }

            $(location).attr('href', findProviderRedirectUrl + '?searchTerm=' + encodeURIComponent(searchTerm));

            return true;
        });

        $(".tl-fap-search-providers-form").submit(function() {
            event.preventDefault();
        });

        function showSearchTermError(message) {
            //if (!$("#tl-search-term-error")) return;
            $("#tl-search-term-error").text(message);
            $('#tl-search-term-error').removeClass("tl-hidden");
        }
    });
