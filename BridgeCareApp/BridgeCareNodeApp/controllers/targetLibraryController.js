function targetLibraryController(TargetLibrary) {
    /**
     * GET NodeJS API endpoint for target libraries; gets & returns all target libraries
     * @param request
     * @param response
     */
    function get(request, response) {
        TargetLibrary.find((error, targetLibraries) => {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(targetLibraries);
        });
    }
    
    /**
     * POST NodeJS API endpoint for target libraries; creates & returns a target library
     * @param request Http request
     * @param response Http response
     */
    function post(request, response) {
        const targetLibrary = new TargetLibrary(request.body);
        
        if (!request.body.name)
            return response.status(400).send('Library name is required');
        
        targetLibrary.save(function(error, library) {
            if (error)
                return response.status(500).json(error);
            
            return response.status(200).json(library);
        });
    }

    /**
     * PUT NodeJS API endpoint for target libraries; updates & returns a target library
     * @param request Http request
     * @param response Http response
     */
    function put(request, response) {
        TargetLibrary.findOneAndUpdate({_id: request.body._id}, request.body, {new: true}, (error, targetLibrary) => {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(targetLibrary);
        });
    }

    return {get, post, put};
}

module.exports = targetLibraryController;
