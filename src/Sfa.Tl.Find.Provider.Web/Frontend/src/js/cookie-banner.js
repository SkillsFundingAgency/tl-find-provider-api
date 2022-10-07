(function () {
    "use strict";
    var root = this;
    if (typeof root.GOVUK === 'undefined') { root.GOVUK = {}; }

    /*
      Cookie methods
      ==============
  
      Usage:
  
        Setting a cookie:
        GOVUK.cookie('hobnob', 'tasty', { days: 30 });
  
        Reading a cookie:
        GOVUK.cookie('hobnob');
  
        Deleting a cookie:
        GOVUK.cookie('hobnob', null);
    */
    GOVUK.cookie = function (name, value, options) {
        if (typeof value !== 'undefined') {
            if (value === false || value === null) {
                return GOVUK.setCookie(name, '', { days: -1 });
            } else {
                return GOVUK.setCookie(name, value, options);
            }
        } else {
            return GOVUK.getCookie(name);
        }
    };
    GOVUK.setCookie = function (name, value, options) {
        if (typeof options === 'undefined' || options === null) {
            options = {};
        }
        var cookieString = name + "=" + value + "; path=/";
        if (options.days) {
            var date = new Date();
            date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000));
            cookieString = cookieString + "; expires=" + date.toGMTString();
        }
        if (document.location.protocol === 'https:') {
            cookieString = cookieString + "; Secure";
        }
        document.cookie = cookieString;
    };
    GOVUK.getCookie = function (name) {
        var nameEq = name + "=";
        var cookies = document.cookie.split(';');
        for (var i = 0, len = cookies.length; i < len; i++) {
            var cookie = cookies[i];
            while (cookie.charAt(0) === ' ') {
                cookie = cookie.substring(1, cookie.length);
            }
            if (cookie.indexOf(nameEq) === 0) {
                return decodeURIComponent(cookie.substring(nameEq.length));
            }
        }
        return null;
    };
}).call(this);


/* Set cookie on accept */

$("button[value='accept']").click(function () {
    console.log("Cookies set analytics accepted");
    GOVUK.cookie('cookies.analytics', 'true', { days: 365 });
    GOVUK.cookie('cookies.essential', 'true', { days: 365 });
    $("#govuk-cookie-banner__message").attr("hidden", "true");
    $("#govuk-cookie-banner__accepted").removeAttr("hidden");
});


/* Set cookie on reject */

$("button[value='reject']").click(function () {
    console.log("Cookies set analytics rejected");
    GOVUK.cookie('cookies.analytics', 'false', { days: 365 });
    GOVUK.cookie('cookies.essential', 'true', { days: 365 });
    $("#govuk-cookie-banner__message").attr("hidden", "true");
    $("#govuk-cookie-banner__rejected").removeAttr("hidden");

});

/* Show Hide Banner */

var essentialCookies = GOVUK.cookie('cookies.essential');

$(document).ready(function () {
    if (essentialCookies === "true") {
        $(".govuk-cookie-banner").attr("hidden", "true");
    }
});

$("button[value='hide']").click(function () {
    $(".govuk-cookie-banner").attr("hidden", "true");
});