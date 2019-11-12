const express = require('express');
const scenarioController = require('../controllers/scenarioController');

function scenarioRoutes(Scenario){
    const router = express.Router();
    const controller = scenarioController(Scenario);

    router.route("/GetMongoScenarios")
        .post(controller.post)
        .get(controller.get);
    router.route("/DeleteMongoScenario/:scenarioId").delete(controller.deleteScenario);
    router.route("/UpdateMongoScenario/:scenarioId").put(controller.put);
    router.route("/AddMultipleScenarios").post(controller.postMultipleScenarios);

    return router;
}

module.exports = scenarioRoutes;
