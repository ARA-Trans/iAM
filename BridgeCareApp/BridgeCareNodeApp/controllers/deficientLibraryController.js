function deficientLibraryController(DeficientLibrary) {
    /**
     * GET NodeJS API endpoint for deficient libraries; gets & returns all deficient libraries
     * @param request
     * @param response
     */
    function get(request, response) {
        DeficientLibrary.find((error, deficientLibraries) => {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(deficientLibraries);
        });
    }

    /**
     * POST NodeJS API endpoint for deficient libraries; creates & returns a deficient library
     * @param request Http request
     * @param response Http response
     */
    function post(request, response) {
        const deficientLibrary = new DeficientLibrary(request.body);

        if (!request.body.name) {
            return response.status(400).send('Library name is required');
        }

        deficientLibrary.save(function(error, library) {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(library);
        });
    }

    /**
     * PUT NodeJS API endpoint for deficient libraries; updates & returns a deficient library
     * @param request Http request
     * @param response Http response
     */
    function put(request, response) {
        DeficientLibrary.findOneAndUpdate({_id: request.body._id}, request.body, {new: true}, (error, DeficientLibrary) => {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(DeficientLibrary);
        });
    }

    return {get, post, put};
}

module.exports = deficientLibraryController;
