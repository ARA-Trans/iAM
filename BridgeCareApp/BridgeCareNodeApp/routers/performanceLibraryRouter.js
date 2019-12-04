const express = require('express');
const performanceLibraryController = require('../controllers/performanceLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function performanceLibraryRouter(PerformanceLibrary, connectionTest) {
    const router = express.Router();
    const controller = performanceLibraryController(PerformanceLibrary);

    router.route('/GetPerformanceLibraries').get(authorizationFilter(), controller.get);
    router.route('/CreatePerformanceLibrary').post(authorizationFilter(), controller.post);
    router.route('/UpdatePerformanceLibrary').put(authorizationFilter(), controller.put);
    router.route('/DeletePerformanceLibrary/:performanceLibraryId').delete(authorizationFilter(), controller.deleteLibrary);
    router.route('/').get((req, res) => {
        return res.send(connectionTest);
    });

    return router;
}

module.exports = performanceLibraryRouter;