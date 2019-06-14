function treatmentLibraryController(TreatmentLibrary) {
    /**
     * POST Nodejs API endpoint for treatment libraries; creates & returns a treatment library
     * @param req Http request
     * @param res Http response
     */
    function post(req, res) {
        const treatmentLibrary = new TreatmentLibrary(req.body);

        if (!req.body.name) {
            return res.status(400).send('name is required');
        }

        treatmentLibrary.save(function (err, library) {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(library);
        });
    }

    /**
     * GET Nodejs API endpoint for treatment libraries; returns treatment libraries if found
     * @param req Http request
     * @param res Http response
     */
    function get(req, res) {
        TreatmentLibrary.find((err, treatments) => {
            if (err) {
                return res.send(err);
            }

            return res.json(treatments);
        });
    }

    /**
     * PUT Nodejs API endpoint for treatment libraries; returns updates & returns a treatment library
     * @param req
     * @param res
     */
    function put(req, res) {
        TreatmentLibrary.findOneAndUpdate({_id: req.body._id}, req.body, {new: true}, (err, doc) => {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(doc);
        });
    }

    /**
     * DELETE Nodejs API endpoint for treatment libraries; returns 204 status code on success
     * @param req
     * @param res
     */
    function deleteLibrary(req, res) {
        TreatmentLibrary.findById(req.params.treatmentLibraryId, (err, library) => {
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

    return { post, get, put, deleteLibrary };
}

module.exports = treatmentLibraryController;