const express = require('express');
const treatmentLibraryController = require('../controllers/treatmentLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');
const roles = require('../authorization/roleConfig');

function treatmentLibraryRouter(TreatmentLibrary, connectionTest) {
    const router = express.Router();
    const controller = treatmentLibraryController(TreatmentLibrary);

    router.route('/GetTreatmentLibraries')
        .get(authorizationFilter(), controller.get);
    router.route('/CreateTreatmentLibrary')
        .post(authorizationFilter([roles.administrator, roles.engineer]), controller.post);
    router.route('/UpdateTreatmentLibrary')
        .put(authorizationFilter([roles.administrator, roles.engineer]), controller.put);
    router.route('/DeleteTreatmentLibrary/:libraryId')
        .delete(authorizationFilter([roles.administrator, roles.engineer]), controller.deleteLibrary);
    router.route('/')
        .get((request, response) => {
            return response.send(connectionTest);
        });

    return router;
}

module.exports = treatmentLibraryRouter;
