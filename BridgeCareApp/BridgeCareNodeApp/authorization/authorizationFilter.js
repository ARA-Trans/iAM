const passport = require('passport');
const jwkToPem = require('jwk-to-pem');
const passportJwt = require('passport-jwt');
const jwtStrategy = passportJwt.Strategy;
const extractJwt = passportJwt.ExtractJwt;
const authorizationConfig = require('./authorizationConfig');
const logger = require('../config/winston');

function authorizationFilter(permittedRoles) {
    const jwtStrategyOptions = {
        jwtFromRequest: extractJwt.fromAuthHeaderAsBearerToken(),
        secretOrKey: jwkToPem(authorizationConfig.esecPublicKey),
        issuer: authorizationConfig.issuer,
        audience: authorizationConfig.clientId
    };

    function verify(jwtPayload, done) {
        role = jwtPayload.roles.split(',')[0].split('=')[1];
        username = jwtPayload.sub.split(',')[0].split('=')[1];
        if (!Array.isArray(permittedRoles) || permittedRoles.length === 0) {
            return done(null, { username, role });
        }
        if (permittedRoles.some(permittedRole => permittedRole === role)){
            return done(null, { username, role });
        }
        logger.error('User unauthorized');
        return done(null, false, {message: 'You are not authorized to perform that action'});
    }

    const strategy = new jwtStrategy(jwtStrategyOptions, verify);

    const strategyName = permittedRoles === undefined ? 'all' : permittedRoles.join(',');
    
    passport.use(strategyName, strategy);

    const authenticationHandler = (request, response, next) => {
        passport.authenticate(strategyName, {session: false}, 
            (error, user, info) => {
                if (error) {
                    return next(error);
                }
                if (!user) {
                    return response.status(401).json({message: info.message || 'Authentication failed'});
                }
                request.user = user;
                return next(null, request, response);
            })(request, response, next);
    };

    return authenticationHandler;
}

module.exports = authorizationFilter;