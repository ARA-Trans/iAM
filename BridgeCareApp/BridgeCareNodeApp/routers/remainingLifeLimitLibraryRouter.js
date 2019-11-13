const express = require('express');
const remainingLifeLimitLibraryController = require('../controllers/remainingLifeLimitLibraryController');

function remainingLifeLimitLibraryRouter(RemainingLifeLimitLibrary) {
    const router = express.Router();
    const controller = remainingLifeLimitLibraryController(RemainingLifeLimitLibrary);

    router.route('/GetRemainingLifeLimitLibraries').get(controller.get);
    router.route('/CreateRemainingLifeLimitLibrary').post(controller.post);
    router.route('/UpdateRemainingLifeLimitLibrary').put(controller.put);

    return router;
}

module.exports = remainingLifeLimitLibraryRouter;
