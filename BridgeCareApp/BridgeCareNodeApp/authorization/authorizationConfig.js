const authorizationConfig = {
    esecPublicKey: {
        "kty": "RSA",
        "e": "AQAB",
        "use": "sig",
        "kid": "6b0dd418-def7-48cf-96d2-673bb0b0244f",
        "n": "47FA3w2IWeIyh7IPr0NS7OeTpC94BtMLF8JMnqUWMLmWCud26lCb6-6L45BRoALPpksNwLfeRtf_jFE8kaoiiixJPP8jhL8oAVK9vE7X4KetUNT_HKtjzkp49Rvp0tpz-UKiv0F_u5XC54PdisfgQrstRMOHKUMeQEFjpF-pg9U"
    },
    issuer: 'https://oidcservicessyst.penndot.gov/affwebservices/CASSO/oidc/BAMS',
    clientId: 'a56bb9d5-190d-4873-afea-0767d82dd91c'
};

module.exports = authorizationConfig;