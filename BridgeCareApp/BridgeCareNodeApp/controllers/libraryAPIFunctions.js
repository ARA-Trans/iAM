const roles = require('../authorization/roleConfig');

module.exports = {
    /**
     * Creates a deletion endpoint for a library. Returns a 204 status code on success.
     */
    getDeletionFunction: (model) => {
        return (request, response) => {
            ownerRestriction = request.user.roles.indexOf(roles.administrator) >= 0 ? {} : {$or:[{owner: [request.user.username]}, {shared: true}]};
            model.findOneAndDelete({_id: request.params.libraryId, ...ownerRestriction}, (error, deleted) => {
                if (error)
                    return response.status(400).json({message: error});
                if (deleted)
                    return response.status(204).json(deleted);
                return response.status(400).json({message: `You do not have permission to delete this library`});
            });
        }
    },

    /**
     * Creates a PUT NodeJS API endpoint for libraries; Returns the modified library.
     */
    getUpdateFunction: (model) => {
        return (request, response) => {
            ownerRestriction = request.user.roles.indexOf(roles.administrator) >= 0 ? 
                {} : {$or:[{owner: [request.user.username]}, {shared: true}]};
            model.findOneAndUpdate({_id: request.body._id, ...ownerRestriction}, request.body, {new: true}, (error, updated) => {
                if (error)
                    return response.status(400).json({message: error});
                if (updated)
                    return response.status(200).json(updated);
                return response.status(400).json({message: `You do not have permission to modify this library`})
            });
        }
    }
}