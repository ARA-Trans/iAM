const express = require('express');
const targetLibraryController = require('../controllers/targetLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function targetLibraryRouter(TargetLibrary) {
    const router = express.Router();
    const controller = targetLibraryController(TargetLibrary);

    router.route('/GetTargetLibraries').get(authorizationFilter(), controller.get);
    router.route('/CreateTargetLibrary').post(authorizationFilter(), controller.post);
    router.route('/UpdateTargetLibrary').put(authorizationFilter(), controller.put);
    router.route('/DeleteTargetLibrary/:libraryId').delete(authorizationFilter(), controller.deleteLibrary);

    return router;
}

module.exports = targetLibraryRouter;