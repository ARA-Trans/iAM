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

    const {getUpdateFunction, getDeletionFunction} = require('./libraryAPIFunctions');

    put = getUpdateFunction(TreatmentLibrary);
    deleteLibrary = getDeletionFunction(TreatmentLibrary);

    return { post, get, put, deleteLibrary };
}

module.exports = treatmentLibraryController;