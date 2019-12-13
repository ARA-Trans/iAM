const express = require('express');
const remainingLifeLimitLibraryController = require('../controllers/remainingLifeLimitLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function remainingLifeLimitLibraryRouter(RemainingLifeLimitLibrary) {
    const router = express.Router();
    const controller = remainingLifeLimitLibraryController(RemainingLifeLimitLibrary);

    router.route('/GetRemainingLifeLimitLibraries').get(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-PlanningPartner"]), controller.get);
    router.route('/CreateRemainingLifeLimitLibrary').post(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-PlanningPartner"]), controller.post);
    router.route('/UpdateRemainingLifeLimitLibrary').put(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-PlanningPartner"]), controller.put);
    router.route('/DeleteRemainingLifeLimitLibrary').delete(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-PlanningPartner"]), controller.deleteLibrary);

    return router;
}

module.exports = remainingLifeLimitLibraryRouter;
