const express = require('express');
const deficientLibraryController = require('../controllers/deficientLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function deficientLibraryRouter(DeficientLibrary) {
    const router = express.Router();
    const controller = deficientLibraryController(DeficientLibrary);

    router.route('/GetDeficientLibraries')
        .get(authorizationFilter(), controller.get);
    router.route('/CreateDeficientLibrary')
        .post(authorizationFilter(), controller.post);
    router.route('/UpdateDeficientLibrary')
        .put(authorizationFilter(), controller.put);
    router.route('/DeleteDeficientLibrary/:libraryId')
        .delete(authorizationFilter(), controller.deleteLibrary);

    return router;
}

module.exports = deficientLibraryRouter;
