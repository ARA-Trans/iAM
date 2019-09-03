const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const {Schema} = mongoose;

const remainingLifeLimitSchema = new Schema({
    attribute: {type: String},
    limit: {type: Number},
    criteria: {type: String}
});

const remainingLifeLimitLibrarySchema = new Schema({
    name: {type: String},
    description: {type: String},
    remainingLifeLimits: [remainingLifeLimitSchema]
});

module.exports = mongoose.model('RemainingLifeLimitLibrary', remainingLifeLimitLibrarySchema, 'remainingLifeLimitLibraries');
