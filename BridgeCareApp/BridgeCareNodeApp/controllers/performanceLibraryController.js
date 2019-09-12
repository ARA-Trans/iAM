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

    /**
     * PUT Nodejs API endpoint for performance libraries; returns updates & returns a performance library
     * @param req
     * @param res
     */
    function put(req, res) {
        PerformanceLibrary.findOneAndUpdate({_id: req.body._id}, req.body, {new: true}, (err, doc) => {
           if (err) {
               return res.status(400).json(err);
           }

           return res.status(200).json(doc);
        });
    }

    /**
     * DELETE Nodejs API endpoint for performance libraries; returns 204 status code on success
     * @param req
     * @param res
     */
    function deleteLibrary(req, res) {
        PerformanceLibrary.findById(req.params.performanceLibraryId, (err, library) => {
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

module.exports = performanceLibraryController;