const express = require('express');
const priorityLibraryController = require('../controllers/priorityLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function priorityLibraryRouter(PriorityLibrary) {
    const router = express.Router();
    const controller = priorityLibraryController(PriorityLibrary);

    router.route('/GetPriorityLibraries').get(authorizationFilter(), controller.get);
    router.route('/CreatePriorityLibrary').post(authorizationFilter(), controller.post);
    router.route('/UpdatePriorityLibrary').put(authorizationFilter(), controller.put);

    return router;
}

module.exports = priorityLibraryRouter;
