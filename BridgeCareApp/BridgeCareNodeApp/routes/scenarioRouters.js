const express = require('express');
const scenarioController = require('../controllers/scenarioController');

function routes(Scenario){
    const scenarioRouter = express.Router();
    const controller = scenarioController(Scenario);
      scenarioRouter.route("/scenarios")
        .post(controller.post)
        .get(controller.get);

        scenarioRouter.route("/scenarios/:scenarioId")
        .delete(controller.deleteScenario);

        scenarioRouter.route("/updateScenarios/:scenarioId")
        .put(controller.put);

        return scenarioRouter;
}

module.exports = routes;