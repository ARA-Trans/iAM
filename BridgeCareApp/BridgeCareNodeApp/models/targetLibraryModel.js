const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const {Schema} = mongoose;

const targetSchema = new Schema({
    attribute: {type: String},
    name: {type: String},
    year: {type: Number},
    targetMean: {type: Number},
    criteria: {type: String}
});

const targetLibrarySchema = new Schema({
    name: {type: String},
    owner: {type: String},
    shared: {type: Boolean},
    description: {type: String},
    targets: [targetSchema]
});

module.exports = mongoose.model('TargetLibrary', targetLibrarySchema, 'targetLibraries');
