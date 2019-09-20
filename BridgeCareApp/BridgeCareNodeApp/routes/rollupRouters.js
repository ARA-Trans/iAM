const express = require('express');
const rollupController = require('../controllers/rollupController');

function routes(Rollup){
    const rollupRouter = express.Router();
    const controller = rollupController(Rollup);
    rollupRouter.route("/GetMongoRollup")
        .post(controller.post)
        .get(controller.get);

        rollupRouter.route("/UpdateMongoRollup/:networkId")
        .put(controller.put);


        return rollupRouter;
}

module.exports = routes;