// provides the matching for location town/city or postcode
(function ($) {
    console.log(`running location autocomplete...`);

    $.fn.locationMatch = function (options) {
        console.log(`in locationMatch`);
        var self = $(this);
        var settings = $.extend({ delay: 200, minLength: 3, maxListSize: 25, url: '', longitude: null, latitude: null, latlonhash: null }, options);
        var tags = [];
        var locations;
        var locationVal = $('#Location').val();

        console.log(`locationMatch has value ${locationVal}`);

        var location = function (name) {
            this.Name = name;
            this.Longitude = null;
            this.Latitude = null;
        };

        var clearLatLonFields = function () {
            if (settings.longitude != null) {
                $(settings.longitude).val(null);
            }
            if (settings.latitude != null) {
                $(settings.latitude).val(null);
            }
        };

        var isNavigationKeyCode = function (keyCode) {
            var navigationKeyCodes = [
                $.ui.keyCode.UP,
                $.ui.keyCode.DOWN,
                $.ui.keyCode.ESCAPE,
                $.ui.keyCode.ENTER,
                $.ui.keyCode.TAB,
                $.ui.keyCode.PAGE_UP,
                $.ui.keyCode.PAGE_DOWN,
                $.ui.keyCode.HOME,
                $.ui.keyCode.END
            ];

            return $.inArray(keyCode, navigationKeyCodes) !== -1;
        };

        self.keyup(function (event) {
            locationVal = $('#Location').val();

            if (!isNavigationKeyCode(event.keyCode)) {
                // Consider non-navigation keyCodes to be user input.
                clearLatLonFields();
            }
        });

        self.autocomplete({
            delay: settings.delay,
            minLength: settings.minLength,
            source: function (request, response) {
                var digits = /[0-9]+/;

                if (request.term.match(digits)) {
                    // Search term looks like a postcode.
                    this.close();
                    return;
                }

                getLocationResults(response, request.term);
            },
            messages: {
                noResults: function () {
                    return "No places found with " + locationVal + " in their name";
                },
                results: function (amount) {
                    return "We've found " + amount + (amount > 1 ? " places" : " place") +
                        " with " + locationVal + " in the name. Use up and down arrow keys to navigate";
                }
            },
            select: function (event, ui) {

                $("#locationSuggestions").addClass("hidden");
                $("#locationSuggestions").removeAttr("open");
                $("#locationSuggestions").removeClass("open");

                this.value = ui.item.value;
                var longLat = getLonLatFromName(ui.item.value);

                if (settings.latlonhash != null) {
                    $(settings.latlonhash).val(0);
                }

                if (settings.longitude != null) {
                    $(settings.longitude).val(longLat.Longitude);
                }
                if (settings.latitude != null) {
                    $(settings.latitude).val(longLat.Latitude);
                }
            },
            change: function (event, ui) {
                if (ui.item == null) {
                    clearLatLonFields();
                }
            },
        });

        function getLocationResults(callback, term) {
            jQuery.support.cors = true;
            $.ajax({
                url: settings.url,
                type: 'GET',
                data: { term: term },
                success: function (response) {
                    tags = [];
                    locations = response;
                    if (response != null) {
                        $.each(response, function (i, item) {
                            if (i < settings.maxListSize) {
                                tags.push(item.Name);
                            }
                        });
                    }
                    callback(tags);
                },
                error: function (error) {
                    //Ignore, could be proxy issues so will work as 
                    //non-JS version.
                }
            });
            return true;
        };

        function getLonLatFromName(name) {
            if (locations != null) {
                var index = $.inArray(name, tags);
                if (index != -1) {
                    var search = locations[index];
                    if (search != null) {
                        return search;
                    }
                }
            }
            return location(name);
        };
    };
    if ($('#locationSuggestions').length) {
        console.log(`in locationSuggestions`);
        var numSuggestions = $('#location-suggestions li').length;
        $('#locSuggestionsAria').text('There are ' + numSuggestions + ' locations with similar names, tab down to the suggestions, or collapse this panel');
    }

    $(document).on("suggestedLocationsUpdated", function (event, data) {
        console.log(`in suggestedLocationsUpdated`);
        $("#locationSuggestions").removeClass("hidden");
        $("#locationSuggestions").attr("open", true);
        $("#locationSuggestions").addClass("open");
        $("#location-suggestions").empty();
        $("#Location").val(data.location);
        _.each(data.locations, function (location) {
            $("#location-suggestions").append(
                $("<li>").append(
                    $("<a>").attr("href", location.Href).append(location.Text)));
        });
    });

})(jQuery);