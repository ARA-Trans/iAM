const express = require('express');
const priorityLibraryController = require('../controllers/priorityLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');
const roles = require('../authorization/roleConfig');

function priorityLibraryRouter(PriorityLibrary) {
    const router = express.Router();
    const controller = priorityLibraryController(PriorityLibrary);

    router.route('/GetPriorityLibraries')
        .get(authorizationFilter(), controller.get);
    router.route('/CreatePriorityLibrary')
        .post(authorizationFilter([roles.administrator, roles.engineer]), controller.post);
    router.route('/UpdatePriorityLibrary')
        .put(authorizationFilter([roles.administrator, roles.engineer]), controller.put);
    router.route('/DeletePriorityLibrary/:libraryId')
        .delete(authorizationFilter([roles.administrator, roles.engineer]), controller.deleteLibrary);

    return router;
}

module.exports = priorityLibraryRouter;
