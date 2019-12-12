const express = require('express');
const investmentLibraryController = require('../controllers/investmentLibraryController');
const authorizationFilter = require('../authorization/authorizationFilter');

function investmentLibraryRouter(InvestmentLibrary){
    const router = express.Router();
    const controller = investmentLibraryController(InvestmentLibrary);

    router.route("/GetInvestmentLibraries").get(authorizationFilter(), controller.get);
    router.route("/CreateInvestmentLibrary").post(authorizationFilter(), controller.post);
    router.route("/UpdateInvestmentLibrary").put(authorizationFilter(), controller.put);
    router.route("/GetInvestmentLibrary/:investmentLibraryId").get(authorizationFilter(), controller.getById);
    router.route("/DeleteInvestmentLibrary/:libraryId").delete(authorizationFilter(), controller.deleteLibrary);

    return router;
}

module.exports = investmentLibraryRouter;
