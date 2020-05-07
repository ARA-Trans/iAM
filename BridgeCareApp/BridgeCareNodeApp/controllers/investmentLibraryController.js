const debug = require('debug')('investmentController');

function investmentLibraryController(InvestmentLibrary) {
    /**
     * POST Nodejs API endpoint for investment libraries; creates & returns an investment library
     * @param req Http request
     * @param res Http response
     */
    function post(req, res) {
        const investmentLibrary = new InvestmentLibrary(req.body);

        if (!req.body.name) {
            return res.status(400).send('name is required');
        }

        investmentLibrary.save(function (err, library) {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(library);
        });
    }

    /**
     * GET Nodejs API endpoint for investment libraries; returns investment libraries if found
     * @param req Http request
     * @param res Http response
     */
    function get(req, res) {
        InvestmentLibrary.find((err, investments) => {
            if (err) {
                return res.send(err);
            }

            return res.json(investments);
        });
    }

    /**
     *
     * @param req Http request
     * @param res Http response
     */
    function getById(req, res) {
        InvestmentLibrary.findById(req.params.investmentLibraryId, (err, library) => {
            if (err) {
                return res.send(err);
            }

            return res.json(library);
        });
    }

    const {getUpdateFunction, getDeletionFunction} = require('./libraryAPIFunctions');
    
    put = getUpdateFunction(InvestmentLibrary);
    deleteLibrary = getDeletionFunction(InvestmentLibrary);

    return { post, get, getById, put, deleteLibrary };
}

module.exports = investmentLibraryController;
