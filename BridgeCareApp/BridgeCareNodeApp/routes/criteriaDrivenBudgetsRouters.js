const express = require('express');
const criteriaDrivenBudgetsController = require('../controllers/criteriaDrivenBudgetsController');

function criteriaDrivenBudgetsyRoutes(CriteriaDrivenBudgets){
    const criteriaDrivenBudgetsRouter = express.Router();
    const controller = criteriaDrivenBudgetsController(CriteriaDrivenBudgets);

    criteriaDrivenBudgetsRouter.route("/CreateCriteriaDrivenBudgets/")
        .post(controller.post);

        criteriaDrivenBudgetsRouter.route("/UpdateCriteriaDrivenBudgets/:id")
        .put(controller.put);

        criteriaDrivenBudgetsRouter.route("/GetCriteriaDrivenBudgets/:id")
        .get(controller.getById);

    return criteriaDrivenBudgetsRouter;
}

module.exports = criteriaDrivenBudgetsyRoutes;