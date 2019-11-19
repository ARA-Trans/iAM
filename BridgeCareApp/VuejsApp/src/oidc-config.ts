module.exports = {
    authority: 'placeholder for ESEC url',
    clientId: 'placeholder for client id from ESEC',
    redirectUri: process.env.VUE_APP_URL + '/Authentication/',
    responseType: 'id_token token',
    scope: 'openid profile'
}
