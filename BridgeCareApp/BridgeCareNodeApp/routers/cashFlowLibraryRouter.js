const express = require('express');
const cashFlowLibraryController = require('../controllers/cashFlowLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function cashFlowLibraryRouter(CashFlowLibrary) {
    const router = express.Router();
    const controller = cashFlowLibraryController(CashFlowLibrary);

    router.route('/GetCashFlowLibraries')
        .get(authorizationFilter(), controller.get);
    router.route('/CreateCashFlowLibrary')
        .post(authorizationFilter(["PD-BAMS-Administrator"]), controller.post);
    router.route('/UpdateCashFlowLibrary')
        .put(authorizationFilter(["PD-BAMS-Administrator"]), controller.put);

    return router;
}

module.exports = cashFlowLibraryRouter;
