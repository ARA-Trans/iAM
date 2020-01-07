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
        if (!Array.isArray(permittedRoles) || permittedRoles.length === 0) {
            return done(null, jwtPayload);
        }
        role = jwtPayload.roles.split(',')[0].split('=')[1];
        if (permittedRoles.some(permittedRole => permittedRole === role)){
            return done(null, jwtPayload);
        }
        logger.error('User unauthorized');
        return done(null, false);
    }

    const strategy = new jwtStrategy(jwtStrategyOptions, verify);

    const strategyName = permittedRoles === undefined ? 'all' : permittedRoles.join(',');
    
    passport.use(strategyName, strategy);
    return passport.authenticate(strategyName, { session: false });
}

module.exports = authorizationFilter;