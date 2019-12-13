const express = require('express');
const remainingLifeLimitLibraryController = require('../controllers/remainingLifeLimitLibraryController');

function remainingLifeLimitLibraryRoutes(RemainingLifeLimitLibrary) {
    const remainingLifeLimitLibraryRouter = express.Router();
    const controller = remainingLifeLimitLibraryController(RemainingLifeLimitLibrary);

    remainingLifeLimitLibraryRouter.route('/GetRemainingLifeLimitLibraries')
        .get(controller.get);

    remainingLifeLimitLibraryRouter.route('/CreateRemainingLifeLimitLibrary')
        .post(controller.post);

    remainingLifeLimitLibraryRouter.route('/UpdateRemainingLifeLimitLibrary')
        .put(controller.put);

    return remainingLifeLimitLibraryRouter;
}

module.exports = remainingLifeLimitLibraryRoutes;
