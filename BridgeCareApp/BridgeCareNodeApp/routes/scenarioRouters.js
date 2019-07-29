const express = require('express');
const scenarioController = require('../controllers/scenarioController');

function routes(Scenario){
    const scenarioRouter = express.Router();
    const controller = scenarioController(Scenario);
      scenarioRouter.route("/GetMongoScenarios")
        .post(controller.post)
        .get(controller.get);

        scenarioRouter.route("/DeleteMongoScenario/:scenarioId")
        .delete(controller.deleteScenario);

        scenarioRouter.route("/UpdateMongoScenario/:scenarioId")
        .put(controller.put);

        scenarioRouter.route("/AddMultipleScenarios")
        .post(controller.postMultipleScenarios);

        return scenarioRouter;
}

module.exports = routes;