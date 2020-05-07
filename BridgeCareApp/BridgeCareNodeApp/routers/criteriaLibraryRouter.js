const express = require('express');
const criteriaLibraryController = require('../controllers/criteriaLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function criteriaLibraryRouter(CriteriaLibrary) {
    const router = express.Router();
    const controller = criteriaLibraryController(CriteriaLibrary);

    router.route('/GetCriteriaLibraries')
        .get(authorizationFilter(), controller.get);
    router.route('/CreateCriteriaLibrary')
        .post(authorizationFilter(), controller.post);
    router.route('/UpdateCriteriaLibrary')
        .put(authorizationFilter(), controller.put);
    router.route('/DeleteCriteriaLibrary/:libraryId')
        .delete(authorizationFilter(), controller.deleteLibrary);

    return router;
}

module.exports = criteriaLibraryRouter;
