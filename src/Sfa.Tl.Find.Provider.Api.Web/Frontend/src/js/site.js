//js goes here

function addHmacAuthHeader_NEW(xhr, uri, appId, apiKey) {
    //const appId = $('#find_provider_api_app_id').val();
    //const apiKey = $('#find_provider_api_key').val();
    const ts = Math.round((new Date()).getTime() / 1000);

    const uuid = //createUniqueId();
        'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g,
            function (c) {
                const r = Math.random() * 16 | 0;
                const v = c === 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
    const nonce = CryptoJS.enc.Hex.parse(uuid); // 12 Bytes

    const data = appId + "GET" + uri.toLowerCase() + ts + nonce;

    const hash = CryptoJS.HmacSHA256(data, apiKey);
    const hashInBase64 = CryptoJS.enc.Base64.stringify(hash);
    const hashInBase64_2 = atob(hash);

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


