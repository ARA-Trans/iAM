const authorizationConfig = {
    esecPublicKey: {
        "kty": "RSA",
        "e": "AQAB",
        "use": "sig",
        "kid": "6b00ffec-d51c-435b-867f-7c870ee6945c",
        "n": "sVBdiGSCsAXigGHn1KZej4aqnjZrgimUIRtL7VWx6VzEJ9fu8JsDiwx_cGkRuettINdYM9U4_akuJqITwSsTIv_VhJRYprVZJChlwObKVPLmPEs4oa4Wuk6Iy81P4jg2-7GV37ScOyD4rxxhff4F-Kh5MflYZpPUYjjJkaJcJVs"
    },
    issuer: 'https://oidcservices.penndot.gov/affwebservices/CASSO/oidc/BAMS',
    clientId: '37db8371-3f80-45d4-a4bd-48c080b2865a'
};

module.exports = authorizationConfig;