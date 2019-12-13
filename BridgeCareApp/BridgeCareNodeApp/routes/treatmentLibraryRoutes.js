const express = require('express');
const treatmentLibraryController = require('../controllers/treatmentLibraryController');

function treatmentLibraryRoutes(TreatmentLibrary, connectionTest) {
    const treatmentLibraryRouter = express.Router();
    const controller = treatmentLibraryController(TreatmentLibrary);

    treatmentLibraryRouter.route('/GetTreatmentLibraries')
        .get(controller.get);

    treatmentLibraryRouter.route('/CreateTreatmentLibrary')
        .post(controller.post);

    treatmentLibraryRouter.route('/UpdateTreatmentLibrary')
        .put(controller.put);

    treatmentLibraryRouter.route('/DeleteTreatmentLibrary/:treatmentLibraryId')
        .delete(controller.deleteLibrary);

    treatmentLibraryRouter.route('/')
        .get((req, res) => {
            return res.send(connectionTest);
        });

    return treatmentLibraryRouter;
}

module.exports = treatmentLibraryRoutes;