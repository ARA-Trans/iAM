const express = require('express');
const performanceLibraryController = require('../controllers/performanceLibraryController');

function performanceLibraryRoutes(PerformanceLibrary, connectionTest) {
    const performanceLibraryRouter = express.Router();
    const controller = performanceLibraryController(PerformanceLibrary);

    performanceLibraryRouter.route('/GetPerformanceLibraries')
        .get(controller.get);

    performanceLibraryRouter.route('/CreatePerformanceLibrary')
        .post(controller.post);

    performanceLibraryRouter.route('/UpdatePerformanceLibrary')
        .put(controller.put);

    performanceLibraryRouter.route('/DeletePerformanceLibrary/:performanceLibraryId')
        .delete(controller.deleteLibrary);

    performanceLibraryRouter.route('/')
        .get((req, res) => {
            return res.send(connectionTest);
        });

    return performanceLibraryRouter;
}

module.exports = performanceLibraryRoutes;