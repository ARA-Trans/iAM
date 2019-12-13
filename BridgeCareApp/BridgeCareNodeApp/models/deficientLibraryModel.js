const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const {Schema} = mongoose;

const deficientSchema = new Schema({
    attribute: {type: String},
    name: {type: String},
    deficient: {type: Number},
    percentDeficient: {type: Number},
    criteria: {type: String}
});

const deficientLibrarySchema = new Schema({
    name: {type: String},
    description: {type: String},
    deficients: [deficientSchema]
});

module.exports = mongoose.model('DeficientLibrary', deficientLibrarySchema, 'deficientLibraries');
