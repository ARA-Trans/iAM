const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const {Schema} = mongoose;

const criteriaLibrarySchema = new Schema({
    name: {type: String},
    description: {type: String},
    criteria: {type: String},
    owner: {type: String},
    shared: {type: Boolean}
});

module.exports = mongoose.model('CriteriaLibrary', criteriaLibrarySchema, 'criteriaLibraries');
