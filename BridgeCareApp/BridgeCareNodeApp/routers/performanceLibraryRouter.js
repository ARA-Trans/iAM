const express = require('express');
const performanceLibraryController = require('../controllers/performanceLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');
const roles = require('../authorization/roleConfig');

function performanceLibraryRouter(PerformanceLibrary, connectionTest) {
    const router = express.Router();
    const controller = performanceLibraryController(PerformanceLibrary);

    router.route('/GetPerformanceLibraries')
        .get(authorizationFilter(), controller.get);
    router.route('/CreatePerformanceLibrary')
        .post(authorizationFilter([roles.administrator, roles.engineer]), controller.post);
    router.route('/UpdatePerformanceLibrary')
        .put(authorizationFilter([roles.administrator, roles.engineer]), controller.put);
    router.route('/DeletePerformanceLibrary/:libraryId')
        .delete(authorizationFilter([roles.administrator, roles.engineer]), controller.deleteLibrary);
    router.route('/')
        .get((request, response) => {
            return response.send(connectionTest);
        });

    return router;
}

module.exports = performanceLibraryRouter;
