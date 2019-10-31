const express = require('express');
const cashFlowLibraryController = require('../controllers/cashFlowLibraryController');

function cashFlowLibraryRoutes(CashFlowLibrary) {
    const cashFlowLibraryRouter = express.Router();
    const controller = cashFlowLibraryController(CashFlowLibrary);

    cashFlowLibraryRouter.route('/GetCashFlowLibraries').get(controller.get);

    cashFlowLibraryRouter.route('/CreateCashFlowLibrary').post(controller.post);

    cashFlowLibraryRouter.route('/UpdateCashFlowLibrary').put(controller.put);

    return cashFlowLibraryRouter;
}

module.exports = cashFlowLibraryRoutes;