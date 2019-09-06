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

    /**
     * PUT Nodejs API endpoint for investment libraries; updates & returns an investment library
     * @param req
     * @param res
     */
    function put(req, res) {
        InvestmentLibrary.findOneAndUpdate({_id: req.body._id}, req.body, {new: true}, (err, doc) => {
debug(req.body);
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(doc);
        });
    }

    /**
     * DELETE Nodejs API endpoint for investment libraries; returns 204 status code on success
     * @param req
     * @param res
     */
    function deleteLibrary(req, res) {
        InvestmentLibrary.findById(req.params.investmentLibraryId, (err, library) => {
            if (err) {
                return res.json(err);
            }

            library.remove((err) => {
                if (err) {
                    return res.status(400).json(err);
                }
                return res.status(204);
            });
        });
    }
    return { post, get, getById, put, deleteLibrary };
}

module.exports = investmentLibraryController;
