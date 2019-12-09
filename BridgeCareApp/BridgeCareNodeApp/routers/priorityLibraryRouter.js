const express = require('express');
const priorityLibraryController = require('../controllers/priorityLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function priorityLibraryRouter(PriorityLibrary) {
    const router = express.Router();
    const controller = priorityLibraryController(PriorityLibrary);

    router.route('/GetPriorityLibraries').get(authorizationFilter(), controller.get);
    router.route('/CreatePriorityLibrary').post(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.post);
    router.route('/UpdatePriorityLibrary').put(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.put);

    return router;
}

module.exports = priorityLibraryRouter;
