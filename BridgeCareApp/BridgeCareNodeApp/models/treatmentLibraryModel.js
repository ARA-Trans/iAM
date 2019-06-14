const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const feasibilitySchema = new Schema({
    _id: {type: String},
    criteria: {type: String},
    yearsBeforeAny: {type: Number},
    yearsBeforeSame: {type: Number}
});

const costSchema = new Schema({
    _id: {type: String},
    equation: {type: String},
    isFunction: {type: Boolean},
    criteria: {type: String}
});

const consequenceSchema = new Schema({
    _id: {type: String},
    attribute: {type: String},
    change: {type: String},
    equation: {type: String},
    isFunction: {type: Boolean},
    criteria: {type: String}
});

const treatmentSchema = new Schema({
    _id: {type: String},
    name: {type: String},
    feasibility: {type: feasibilitySchema},
    costs: [costSchema],
    consequences: [consequenceSchema],
    budgets: [{type: String}]
});

const treatmentLibrarySchema = new Schema({
    _id: {type: String},
    name: {type: String},
    description: {type: String},
    treatments: [treatmentSchema]
});

module.exports = mongoose.model('TreatmentLibrary', treatmentLibrarySchema, 'treatmentLibraries');