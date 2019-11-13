const express = require('express');
const targetLibraryController = require('../controllers/targetLibraryController');

function targetLibraryRouter(TargetLibrary) {
    const router = express.Router();
    const controller = targetLibraryController(TargetLibrary);

    router.route('/GetTargetLibraries').get(controller.get);
    router.route('/CreateTargetLibrary').post(controller.post);
    router.route('/UpdateTargetLibrary').put(controller.put);

    return router;
}

module.exports = targetLibraryRouter;