function cashFlowLibraryController(CashFlowLibrary) {
    /**
     * GET NodeJS API endpoint for cash flow libraries; returns cash flow libraries if found
     * @param req Http request
     * @param res Http response
     */
    function get(req, res) {
        CashFlowLibrary.find((err, cashFlowLibraries) => {
            if (err) {
                return res.status(500).json(err);
            }

            return res.status(200).json(cashFlowLibraries);
        });
    }

    /**
     * POST NodeJS API endpoint for cash flow libraries; creates & returns a cash flow library
     * @param req Http request
     * @param res Http response
     */
    function post(req, res) {
        const cashFlowLibrary = new CashFlowLibrary(req.body);

        if (!req.body.name) {
            return res.status(400).send('Library name required');
        }

        cashFlowLibrary.save(function(err, library) {
            if (err) {
                return res.status(500).json(err);
            }

            return res.status(200).json(library);
        });
    }

    const {getUpdateFunction, getDeletionFunction} = require('./libraryAPIFunctions');

    put = getUpdateFunction(CashFlowLibrary);
    deleteLibrary = getDeletionFunction(CashFlowLibrary);

    return {get, post, put, deleteLibrary};
}

module.exports = cashFlowLibraryController;
