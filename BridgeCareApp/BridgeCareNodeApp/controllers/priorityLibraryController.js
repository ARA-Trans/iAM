function priorityLibraryController(PriorityLibrary) {
    /**
     * POST NodeJS API endpoint for priority libraries; creates & returns a priority library
     * @param req Http request
     * @param res Http response
     */
    function post(req, res) {
        const priorityLibrary = new PriorityLibrary(req.body);

        if (!req.body.name) {
            return res.status(400).send('name is required');
        }

        priorityLibrary.save(function(err, library) {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(library);
        });
    }

    /**
     * GET NodeJS API endpoint for priority libraries; returns priority libraries if found
     * @param req Http request
     * @param res Http response
     */
    function get(req, res) {
        PriorityLibrary.find((err, priorities) => {
            if (err) {
                return res.send(err);
            }

            return res.json(priorities);
        });
    }

    /**
     * GET NodeJS API endpoint for priority libraries; gets & returns a priority library by id
     * @param req Http request
     * @param res Http response
     */
    function getById(req, res) {
        PriorityLibrary.findById(req.params.priorityLibraryId, (err, library) => {
            if (err) {
                return res.send(err);
            }

            return res.json(library);
        });
    }

    /**
     * PUT NodeJS API endpoint for priority libraries; updates & returns a priority library
     * @param req Http request
     * @param res Http response
     */
    function put(req, res) {
        PriorityLibrary.findOneAndUpdate({_id: req.body._id}, req.body, {new: true}, (err, doc) => {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(doc);
        });
    }

    /**
     * DELETE NodeJS API endpoint for priority libraries; returns 204 status code on success
     * @param req Http request
     * @param res Http response
     */
    function deleteLibrary(req, res) {
        PriorityLibrary.findById(req.params.priorityLibraryId, (err, library) => {
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

module.exports = priorityLibraryController;
