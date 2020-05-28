const authorizationConfig = require('./authorizationConfig');
const logger = require('../config/winston');
const axios = require('axios');
const https = require('https');

function authorizationFilter(permittedRoles) {
    return async function authenticationHandler(request, response, next) {
        accessToken = request.headers.authorization.split(' ')[1];

        requestPath = `${authorizationConfig.issuer}/userinfo?access_token=${accessToken}`;

        userInfoResponse = await axios.get(requestPath, { httpsAgent: new https.Agent({rejectUnauthorized: false})}).then(esecResponse => {
            return esecResponse.data;
        }).catch(error => {
            return {error: response.status(401).json({message: error.response.data.error_description || 'Authentication Failed'})};
        });

        if (userInfoResponse.error !== undefined) {
            return userInfoResponse.error;
        }
        
        roles = userInfoResponse.roles.split('^').map(segment => segment.split(',')[0].split('=')[1]);
        username = userInfoResponse.sub.split(',')[0].split('=')[1];
        if (!Array.isArray(permittedRoles) || permittedRoles.length === 0) {
            request.user = { username, roles };
            return next();
        }
        if (permittedRoles.some(permittedRole => roles.some(role => permittedRole === role))){
            request.user = { username, roles };
            return next();
        }
        
        return response.status(401).json({message: 'User is not authorized for this action.'});
    };
}

module.exports = authorizationFilter;