const express = require('express');
const libraryController = require('../controllers/libraryController');

function routes(InvestmentLibrary, connectionTest){
    const investmentLibraryRouter = express.Router();
    const controller = libraryController(InvestmentLibrary);
      investmentLibraryRouter.route("/investmentLibraries")
        .post(controller.post)
        .get(controller.get);

        investmentLibraryRouter.route("/investmentLibraries/:libraryId")
        .get(controller.getById)
        .put(controller.put)
        .delete(controller.deleteLibrary);

        investmentLibraryRouter.route("/")
        .get((req,res) => {
          return res.send(connectionTest);
        });
        return investmentLibraryRouter;
}

module.exports = routes;