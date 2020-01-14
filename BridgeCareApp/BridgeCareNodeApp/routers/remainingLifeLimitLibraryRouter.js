const express = require('express');
const remainingLifeLimitLibraryController = require('../controllers/remainingLifeLimitLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function remainingLifeLimitLibraryRouter(RemainingLifeLimitLibrary) {
    const router = express.Router();
    const controller = remainingLifeLimitLibraryController(RemainingLifeLimitLibrary);

    router.route('/GetRemainingLifeLimitLibraries')
        .get(authorizationFilter(), controller.get);
    router.route('/CreateRemainingLifeLimitLibrary')
        .post(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.post);
    router.route('/UpdateRemainingLifeLimitLibrary')
        .put(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.put);

    return router;
}

module.exports = remainingLifeLimitLibraryRouter;
