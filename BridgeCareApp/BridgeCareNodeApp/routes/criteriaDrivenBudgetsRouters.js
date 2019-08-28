const express = require('express');
const criteriaDrivenBudgetsController = require('../controllers/criteriaDrivenBudgetsController');

function criteriaDrivenBudgetsyRoutes(CriteriaDrivenBudgets){
    const criteriaDrivenBudgetsRouter = express.Router();
    const controller = criteriaDrivenBudgetsController(CriteriaDrivenBudgets);

    criteriaDrivenBudgetsRouter.route("/CreateCriteriaDrivenBudgets/:scenarioId")
        .post(controller.post);

        criteriaDrivenBudgetsRouter.route("/UpdateCriteriaDrivenBudgets")
        .put(controller.put);

        criteriaDrivenBudgetsRouter.route("/GetCriteriaDrivenBudgets/:scenarioId")
        .get(controller.getById);

    return criteriaDrivenBudgetsRouter;
}

module.exports = criteriaDrivenBudgetsyRoutes;