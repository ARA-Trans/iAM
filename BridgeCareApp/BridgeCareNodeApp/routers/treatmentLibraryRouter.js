const express = require('express');
const treatmentLibraryController = require('../controllers/treatmentLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function treatmentLibraryRouter(TreatmentLibrary, connectionTest) {
    const router = express.Router();
    const controller = treatmentLibraryController(TreatmentLibrary);

    router.route('/GetTreatmentLibraries').get(authorizationFilter(), controller.get);
    router.route('/CreateTreatmentLibrary').post(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.post);
    router.route('/UpdateTreatmentLibrary').put(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.put);
    router.route('/DeleteTreatmentLibrary/:treatmentLibraryId').delete(authorizationFilter(["PD-BAMS-Administrator", "PD-BAMS-DBEngineer"]), controller.deleteLibrary);
    router.route('/').get((req, res) => {
        return res.send(connectionTest);
    });

    return router;
}

module.exports = treatmentLibraryRouter;
