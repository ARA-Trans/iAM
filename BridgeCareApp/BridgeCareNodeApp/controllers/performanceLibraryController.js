function performanceLibraryController(PerformanceLibrary) {
    /**
     * POST Nodejs API endpoint for performance libraries; creates & returns a performance library
     * @param req Http request
     * @param res Http response
     */
    function post(req, res) {
        const performanceLibrary = new PerformanceLibrary(req.body);

        if (!req.body.name) {
            return res.status(400).send('name is required');
        }

        performanceLibrary.save(function (err, library) {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(library);
        });
    }

    /**
     * GET Nodejs API endpoint for performance libraries; returns performance libraries if found
     * @param req Http request
     * @param res Http response
     */
    function get(req, res) {
        PerformanceLibrary.find((err, performances) => {
           if (err) {
               return res.send(err);
           }

           return res.json(performances);
        });
    }

    const {getUpdateFunction, getDeletionFunction} = require('./libraryAPIFunctions');

    put = getUpdateFunction(PerformanceLibrary);
    deleteLibrary = getDeletionFunction(PerformanceLibrary);

    return { post, get, put, deleteLibrary };
}

module.exports = performanceLibraryController;