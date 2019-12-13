const express = require('express');
const priorityLibraryController = require('../controllers/priorityLibraryController');

function priorityLibraryRoutes(PriorityLibrary) {
    const router = express.Router();
    const controller = priorityLibraryController(PriorityLibrary);

    router.route('/GetPriorityLibraries').get(controller.get);

    router.route('/CreatePriorityLibrary').post(controller.post);

    router.route('/UpdatePriorityLibrary').put(controller.put);

    return router;
}

module.exports = priorityLibraryRoutes;
