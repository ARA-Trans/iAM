function remainingLifeLimitLibraryController(RemainingLifeLimitLibrary) {
    /**
     * GET NodeJS API endpoint for remaining life limit libraries; returns remaining life limit libraries if found
     * @param req Http request
     * @param res Http response
     */
    function get(req, res) {
        RemainingLifeLimitLibrary.find((err, remainingLifeLimitLibraries) => {
            if (err) {
                return res.send(err);
            }

            return res.json(remainingLifeLimitLibraries);
        });
    }

    /**
     * POST NodeJS API endpoint for remaining life limit libraries; creates & returns a remaining life limit library
     * @param req Http request
     * @param res Http response
     */
    function post(req, res) {
        const remainingLifeLimitLibrary = new RemainingLifeLimitLibrary(req.body);

        if (!req.body.name) {
            return res.status(400).send('name is required');
        }

        remainingLifeLimitLibrary.save(function(err, library) {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(library);
        });
    }

    /**
     * PUT NodeJS API endpoint for remaining life limit libraries; updates & returns a remaining life limit library
     * @param req
     * @param res
     */
    function put(req, res) {
        RemainingLifeLimitLibrary.findOneAndUpdate({_id: req.body._id}, req.body, {new: true}, (err, doc) => {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(doc);
        });
    }

    /**
     * DELETE NodeJS API endpoint for remaining life limit libraries; returns deleted library
     * @param req
     * @param res
     */
    function deleteLibrary(req, res) {
        RemainingLifeLimitLibrary.findOneAndDelete({_id: req.params.libraryId}, (err, doc) => {
            if (err) {
                return res.status(400).json(err);
            }
            return res.status(204).json(doc);
        });
    }

    

    return {get, post, put, deleteLibrary};
}

module.exports = remainingLifeLimitLibraryController;
