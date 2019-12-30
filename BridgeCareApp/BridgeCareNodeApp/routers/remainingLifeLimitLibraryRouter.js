const express = require('express');
const remainingLifeLimitLibraryController = require('../controllers/remainingLifeLimitLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');
const roles = require('../authorization/roleConfig');

function remainingLifeLimitLibraryRouter(RemainingLifeLimitLibrary) {
    const router = express.Router();
    const controller = remainingLifeLimitLibraryController(RemainingLifeLimitLibrary);

    router.route('/GetRemainingLifeLimitLibraries').get(authorizationFilter([roles.administrator]), controller.get);
    router.route('/CreateRemainingLifeLimitLibrary').post(authorizationFilter([roles.administrator]), controller.post);
    router.route('/UpdateRemainingLifeLimitLibrary').put(authorizationFilter([roles.administrator]), controller.put);
    router.route('/DeleteRemainingLifeLimitLibrary/:libraryId').delete(authorizationFilter([roles.administrator]), controller.deleteLibrary);

    return router;
}

module.exports = remainingLifeLimitLibraryRouter;
