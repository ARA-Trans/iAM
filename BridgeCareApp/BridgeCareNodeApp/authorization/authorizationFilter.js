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
        if (!Array.isArray(permittedRoles) || permittedRoles.length === 0) {
            return done(null, jwtPayload);
        }
        console.log('verifying...');
        console.log(jwtPayload.roles);
        role = jwtPayload.roles.split(',')[0].split('=')[1];
        if (permittedRoles.some(permittedRole => permittedRole === role)){
            return done(null, jwtPayload);
        }
        return done(null, false);
    };

    const strategy = new jwtStrategy(jwtStrategyOptions, verify);
    passport.use(strategy);
    return passport.authenticate('jwt', { session: false });
}

module.exports = authorizationFilter;