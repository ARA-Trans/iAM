function libraryController(InvestmentLibrary) {
    function post(req, res) {
        const investmentLibrary = new InvestmentLibrary(req.body);

        if (!req.body.name) {
            res.status(400);
            return res.send('name is required');
        }

        investmentLibrary.save(function (err, library) {
            if (err) {
                res.status(400);
                return res.json(err);
            }
            res.status(200);
            return res.json(library);
        });
    }

    function get(req, res) {
        InvestmentLibrary.find((err, investments) => {
            if (err) {
                return res.send(err);
            }
            return res.json(investments);
        });
    }

    function getById(req, res) {
        InvestmentLibrary.findById(req.params.libraryId, (err, library) => {
            if (err) {
                return res.send(err);
            }
            return res.json(library);
        });
    }

    function put(req, res) {
        InvestmentLibrary.findById(req.params.libraryId, (err, library) => {
            if (err) {
                return res.send(err);
            }
            library.Investment = req.body.Investment;
            library.save(function (err, library) {
                if (err) {
                    return res.status(400).json(err);
                }
                return res.status(200).json(library);

            });
        });
    }

    function deleteLibrary(req, res) {
        InvestmentLibrary.findById(req.params.libraryId, (err, library) => {
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
    return { post, get, getById, put, deleteLibrary};
}

module.exports = libraryController;