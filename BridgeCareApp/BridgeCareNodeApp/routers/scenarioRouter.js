const express = require('express');
const scenarioController = require('../controllers/scenarioController');
const authorizationFilter = require('../authorization/authorizationFilter');
const roles = require('../authorization/roleConfig');

function scenarioRouter(Scenario){
    const router = express.Router();
    const controller = scenarioController(Scenario);

    router.route("/GetMongoScenarios")
        .post(authorizationFilter([roles.administrator, roles.engineer]), controller.post)
        .get(authorizationFilter(), controller.get);
    router.route("/DeleteMongoScenario/:scenarioId")
        .delete(authorizationFilter(), controller.deleteScenario);
    router.route("/UpdateMongoScenario/:scenarioId")
        .put(authorizationFilter(), controller.put);
    router.route("/AddMultipleScenarios")
        .post(authorizationFilter(), controller.postMultipleScenarios);

    return router;
}

module.exports = scenarioRouter;
