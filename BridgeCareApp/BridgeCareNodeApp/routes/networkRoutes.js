const express = require('express');
const networkController = require('../controllers/networkController');

function routes(Network){
    const networkRouter = express.Router();
    const controller = networkController(Network);
    networkRouter.route("/GetMongoRollups")
        .post(controller.post)
        .get(controller.get);

        networkRouter.route("/UpdateMongoRollup/:networkId")
        .put(controller.put);

        networkRouter.route("/AddLegacyNetworks")
        .post(controller.postLegacyNetworks);

        return networkRouter;
}

module.exports = routes;