function priorityLibraryController(PriorityLibrary) {
    /**
     * GET NodeJS API endpoint for priority libraries; returns priority libraries if found
     * @param request Http request
     * @param response Http response
     */
    function get(request, response) {
        PriorityLibrary.find((error, priorityLibraries) => {
            if (error)
                return response.status(500).send(error);

            return response.status(200).json(priorityLibraries);
        });
    }
    
    /**
     * POST NodeJS API endpoint for priority libraries; creates & returns a priority library
     * @param request Http request
     * @param response Http response
     */
    function post(request, response) {
        const priorityLibrary = new PriorityLibrary(request.body);

        if (!request.body.name)
            return response.status(400).send('Library name is required');

        priorityLibrary.save(function(error, library) {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(library);
        });
    }

    const {getUpdateFunction, getDeletionFunction} = require('./libraryAPIFunctions');

    put = getUpdateFunction(PriorityLibrary);
    deleteLibrary = getDeletionFunction(PriorityLibrary);

    return {get, post, put, deleteLibrary};
}

module.exports = priorityLibraryController;
