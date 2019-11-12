const express = require('express');
const treatmentLibraryController = require('../controllers/treatmentLibraryController');

function treatmentLibraryRouter(TreatmentLibrary, connectionTest) {
    const router = express.Router();
    const controller = treatmentLibraryController(TreatmentLibrary);

    router.route('/GetTreatmentLibraries').get(controller.get);
    router.route('/CreateTreatmentLibrary').post(controller.post);
    router.route('/UpdateTreatmentLibrary').put(controller.put);
    router.route('/DeleteTreatmentLibrary/:treatmentLibraryId').delete(controller.deleteLibrary);
    router.route('/').get((req, res) => {
        return res.send(connectionTest);
    });

    return router;
}

module.exports = treatmentLibraryRouter;
