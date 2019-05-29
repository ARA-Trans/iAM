const express = require('express');
const libraryController = require('../controllers/libraryController');

function routes(InvestmentLibrary){
    const investmentLibraryRouter = express.Router();
    const controller = libraryController(InvestmentLibrary);
      investmentLibraryRouter.route("/investmentLibraries")
        .post(controller.post)
        .get(controller.get);

        investmentLibraryRouter.route("/investmentLibraries/:libraryId")
        .get(controller.getById)
        .put(controller.put)
        .delete(controller.deleteLibrary);
        return investmentLibraryRouter;
}

module.exports = routes;