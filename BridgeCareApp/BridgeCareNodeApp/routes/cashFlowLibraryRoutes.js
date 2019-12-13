const express = require('express');
const cashFlowLibraryController = require('../controllers/cashFlowLibraryController');

function cashFlowLibraryRoutes(CashFlowLibrary) {
    const router = express.Router();
    const controller = cashFlowLibraryController(CashFlowLibrary);

    router.route('/GetCashFlowLibraries').get(controller.get);

    router.route('/CreateCashFlowLibrary').post(controller.post);

    router.route('/UpdateCashFlowLibrary').put(controller.put);

    return router;
}

module.exports = cashFlowLibraryRoutes;