const express = require('express');
const targetLibraryController = require('../controllers/targetLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function targetLibraryRoutes(TargetLibrary) {
    const router = express.Router();
    const controller = targetLibraryController(TargetLibrary);

    router.route('/GetTargetLibraries').get(authorizationFilter(), controller.get);

    router.route('/CreateTargetLibrary')
        .post(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.post);

    router.route('/UpdateTargetLibrary')
        .put(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.put);

    return router;
}

module.exports = targetLibraryRoutes;