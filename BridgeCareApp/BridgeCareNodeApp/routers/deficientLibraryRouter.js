const express = require('express');
const deficientLibraryController = require('../controllers/deficientLibraryController');

function deficientLibraryRouter(DeficientLibrary) {
    const router = express.Router();
    const controller = deficientLibraryController(DeficientLibrary);

    router.route('/GetDeficientLibraries').get(controller.get);
    router.route('/CreateDeficientLibrary').post(controller.post);
    router.route('/UpdateDeficientLibrary').put(controller.put);

    return router;
}

module.exports = deficientLibraryRouter;