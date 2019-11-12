const express = require('express');
const networkController = require('../controllers/networkController');

function networkRouter(Network){
    const router = express.Router();
    const controller = networkController(Network);

    router.route("/GetMongoRollups")
        .post(controller.post)
        .get(controller.get);
    router.route("/UpdateMongoRollup/:networkId").put(controller.put);
    router.route("/AddLegacyNetworks").post(controller.postLegacyNetworks);

    return router;
}

module.exports = networkRouter;
