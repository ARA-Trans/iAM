module.exports = {
    increment: function(collection, counterId) {
        collection.setNext(counterId, function(err, collection) {
            if (err) {
                console.log(`Cannot increment ${counterId} because ${err}`);
            }
        });
    }
};
