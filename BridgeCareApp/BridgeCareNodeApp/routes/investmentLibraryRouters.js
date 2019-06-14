const express = require('express');
const investmentLibraryController = require('../controllers/investmentLibraryController');

function investmentLibraryRoutes(InvestmentLibrary, connectionTest){
    const investmentLibraryRouter = express.Router();
    const controller = investmentLibraryController(InvestmentLibrary);

    investmentLibraryRouter.route("/GetInvestmentLibraries")
        .get(controller.get);

    investmentLibraryRouter.route("/CreateInvestmentLibrary")
        .post(controller.post);

    investmentLibraryRouter.route("/UpdateInvestmentLibrary")
        .put(controller.put);

    investmentLibraryRouter.route("/GetInvestmentLibrary/:investmentLibraryId")
        .get(controller.getById);

    investmentLibraryRouter.route("/DeleteInvestmentLibrary/:investmentLibraryId")
        .delete(controller.deleteLibrary);

    investmentLibraryRouter.route("/")
        .get((req,res) => {
            return res.send(connectionTest);
        });

    return investmentLibraryRouter;
}

module.exports = investmentLibraryRoutes;