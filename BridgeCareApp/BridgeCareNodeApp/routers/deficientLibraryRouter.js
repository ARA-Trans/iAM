const express = require('express');
const deficientLibraryController = require('../controllers/deficientLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function deficientLibraryRouter(DeficientLibrary) {
    const router = express.Router();
    const controller = deficientLibraryController(DeficientLibrary);

    router.route('/GetDeficientLibraries')
        .get(authorizationFilter(), controller.get);
    router.route('/CreateDeficientLibrary')
        .post(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.post);
    router.route('/UpdateDeficientLibrary')
        .put(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.put);

    return router;
}

module.exports = deficientLibraryRouter;
