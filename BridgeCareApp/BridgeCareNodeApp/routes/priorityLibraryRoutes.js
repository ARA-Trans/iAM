const express = require('express');
const priorityLibraryController = require('../controllers/priorityLibraryController');

function priorityLibraryRoutes(PriorityLibrary) {
    const priorityLibraryRouter = express.Router();
    const controller = priorityLibraryController(PriorityLibrary);

    priorityLibraryRouter.route('/GetPriorityLibraries')
        .get(controller.get);

    priorityLibraryRouter.route('/CreatePriorityLibrary')
        .post(controller.post);

    priorityLibraryRouter.route('/UpdatePriorityLibrary')
        .put(controller.put);

    priorityLibraryRouter.route('/GetPriorityLibrary/:priorityLibraryId')
        .get(controller.getById);

    priorityLibraryRouter.route('/DeletePriorityLibrary/:priorityLibraryId')
        .delete(controller.deleteLibrary);

    return priorityLibraryRouter;
}

module.exports = priorityLibraryRoutes;
