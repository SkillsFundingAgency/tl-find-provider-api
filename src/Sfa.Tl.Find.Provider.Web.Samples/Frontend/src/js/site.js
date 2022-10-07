//js goes here

function addHmacAuthHeader(xhr, uri, appId, apiKey, method, data) {
    method = (method === undefined ? "GET" : method);

    const ts = Math.round((new Date()).getTime() / 1000);

    // ReSharper disable StringLiteralTypo
    const uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g,
        // ReSharper restore StringLiteralTypo
        function (c) {
            const r = Math.random() * 16 | 0;
            const v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    const nonce = CryptoJS.enc.Hex.parse(uuid); // 12 Bytes

    var requestContentBase64String = "";

    if (data !== undefined && data) {
        var md5 = CryptoJS.MD5(data);
        requestContentBase64String = CryptoJS.enc.Base64.stringify(md5);
    }

    const dataToHash = appId + method + uri.toLowerCase() + ts + nonce + requestContentBase64String;

    const hash = CryptoJS.HmacSHA256(dataToHash, apiKey);
    const hashInBase64 = CryptoJS.enc.Base64.stringify(hash);

    xhr.setRequestHeader("Authorization", "amx " + appId + ":" + hashInBase64 + ":" + nonce + ":" + ts);
}

function urlDecode(str) {
    return decodeURIComponent((str + '').replace(/\+/g, '%20'));
}

function getUrlParameter(key, url) {
    if (!url) {
        url = window.location.search.substring(1);
    }

    const urlVars = url.split('&');
    for (let i = 0; i < urlVars.length; i++) {
        const parameter = urlVars[i].split('=');
        if (parameter[0] === key) {
            return parameter[1];
        }
    }

    return null;
}

//Select all checkboxes
$('.tl-selectall').click(function () {
    $('.tl-checkbox').prop('checked', this.checked);
});

// ReSharper disable StringLiteralTypo
$(".tl-checkbox").change(function () {
    if ($('.tl-checkbox:checked').length === $('.tl-checkbox').length) {
        $('.tl-selectall').prop('checked', true);
    }
    else {
        $('.tl-selectall').prop('checked', false);
    }
});
// ReSharper restore StringLiteralTypo

//Cookie functions from custom.js in zd site

function writeCookie(key, value, days) {
    var date = new Date();
    days = days || 365;// Default at 365 days
    date.setTime(+ date + (days * 86400000)); //24 * 60 * 60 * 1000
    window.document.cookie = key + "=" + value + "; expires=" + date.toGMTString() + "; path=/";
    return value;
}
function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}