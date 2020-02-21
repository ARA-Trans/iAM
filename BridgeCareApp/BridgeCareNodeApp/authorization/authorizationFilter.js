const passport = require('passport');
const jwkToPem = require('jwk-to-pem');
const passportJwt = require('passport-jwt');
const jwtStrategy = passportJwt.Strategy;
const extractJwt = passportJwt.ExtractJwt;
const authorizationConfig = require('./authorizationConfig');

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
        return done(null, false);
    }

    const strategy = new jwtStrategy(jwtStrategyOptions, verify);

    const strategyName = permittedRoles === undefined ? 'all' : permittedRoles.join(',');
    
    passport.use(strategyName, strategy);
    return passport.authenticate(strategyName, { session: false });
}

module.exports = authorizationFilter;