const express = require('express');
const performanceLibraryController = require('../controllers/performanceLibraryController');

function performanceLibraryRouter(PerformanceLibrary, connectionTest) {
    const router = express.Router();
    const controller = performanceLibraryController(PerformanceLibrary);

    router.route('/GetPerformanceLibraries').get(controller.get);
    router.route('/CreatePerformanceLibrary').post(controller.post);
    router.route('/UpdatePerformanceLibrary').put(controller.put);
    router.route('/DeletePerformanceLibrary/:performanceLibraryId').delete(controller.deleteLibrary);
    router.route('/').get((req, res) => {
        return res.send(connectionTest);
    });

    return router;
}

module.exports = performanceLibraryRouter;