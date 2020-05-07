function criteriaLibraryController(CriteriaLibrary) {
    /**
     * GET Nodejs API endpoint for criteria libraries; returns all criteria libraries if found
     * @param req
     * @param res
     */
    function get(req, res) {
        CriteriaLibrary.find((err, criteriaLibraries) => {
            if (err) {
                return res.send(err);
            }

            return res.json(criteriaLibraries);
        })
    }

    /**
     * POST Nodejs API endpoint for criteria libraries; creates & returns a criteria library
     * @param req Http request
     * @param res Http response
     */
    function post(req, res) {
        const criteriaLibrary = new CriteriaLibrary(req.body);

        if (!req.body.name) {
            return res.status(400).send('name is required');
        }

        criteriaLibrary.save(function (err, library) {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(library);
        });
    }

    const {getUpdateFunction, getDeletionFunction} = require('./libraryAPIFunctions');

    put = getUpdateFunction(CriteriaLibrary);
    deleteLibrary = getDeletionFunction(CriteriaLibrary);

    return {get, post, put, deleteLibrary};
}

module.exports = criteriaLibraryController;
