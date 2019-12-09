const express = require('express');
const scenarioController = require('../controllers/scenarioController');
const authorizationFilter = require('../authorization/authorizationFilter');

function scenarioRoutes(Scenario){
    const router = express.Router();
    const controller = scenarioController(Scenario);

    router.route("/GetMongoScenarios")
        .post(authorizationFilter(), controller.post)
        .get(authorizationFilter(), controller.get);
    router.route("/DeleteMongoScenario/:scenarioId").delete(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.deleteScenario);
    router.route("/UpdateMongoScenario/:scenarioId").put(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.put);
    router.route("/AddMultipleScenarios").post(authorizationFilter(), controller.postMultipleScenarios);

    return router;
}

module.exports = scenarioRoutes;
