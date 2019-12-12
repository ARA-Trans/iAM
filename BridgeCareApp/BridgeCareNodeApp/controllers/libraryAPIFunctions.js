module.exports = {
    /**
     * Creates a deletion endpoint for a library. Returns a 204 status code on success.
     */
    getDeletionFunction: (model) => {
        return (request, response) => {
            ownerRestriction = request.user.role === 'PD-BAMS-ADMINISTRATOR' ? {} : {owner: [request.user.username]};
            model.findOneAndDelete({_id: request.params.libraryId, ...ownerRestriction}, (error, deleted) => {
                if (error)
                    return response.status(500).json(error);
                if (deleted)
                    return response.status(204).json(deleted);
                return response.status(500).json({error: `User ${request.user.username} has no library with id ${request.params.libraryId}`});
            });
        }
    },

    /**
     * Creates a PUT NodeJS API endpoint for libraries; Returns the modified library.
     */
    getUpdateFunction: (model) => {
        return (request, response) => {
            ownerRestriction = request.user.role === 'PD-BAMS-ADMINISTRATOR' ? {} : {owner: [request.user.username]};
            model.findOneAndUpdate({_id: request.body._id, ...ownerRestriction}, request.body, {new: true}, (error, updated) => {
                if (error)
                    return response.status(500).json(error);
                if (updated)
                    return response.status(200).json(updated);
                return response.status(500).json({error: `User ${request.user.username} has no library with id ${request.params.libraryId}`})
            });
        }
    }
}