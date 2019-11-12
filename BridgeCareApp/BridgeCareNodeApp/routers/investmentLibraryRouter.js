const express = require('express');
const investmentLibraryController = require('../controllers/investmentLibraryController');

function investmentLibraryRouter(InvestmentLibrary){
    const router = express.Router();
    const controller = investmentLibraryController(InvestmentLibrary);

    router.route("/GetInvestmentLibraries").get(controller.get);
    router.route("/CreateInvestmentLibrary").post(controller.post);
    router.route("/UpdateInvestmentLibrary").put(controller.put);
    router.route("/GetInvestmentLibrary/:investmentLibraryId").get(controller.getById);
    router.route("/DeleteInvestmentLibrary/:investmentLibraryId").delete(controller.deleteLibrary);

    return router;
}

module.exports = investmentLibraryRouter;
