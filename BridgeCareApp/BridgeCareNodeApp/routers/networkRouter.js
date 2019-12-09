const express = require('express');
const networkController = require('../controllers/networkController');
const authorizationFilter = require('../authorization/authorizationFilter');

function networkRouter(Network){
    const router = express.Router();
    const controller = networkController(Network);

    router.route("/GetMongoRollups")
        .post(authorizationFilter(), controller.post)
        .get(authorizationFilter(), controller.get);
    router.route("/UpdateMongoRollup/:networkId").put(authorizationFilter(), controller.put);
    router.route("/AddLegacyNetworks").post(authorizationFilter(), controller.postLegacyNetworks);

    return router;
}

module.exports = networkRouter;
