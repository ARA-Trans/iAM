const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const feasibilitySchema = new Schema({
    criteria: {type: String},
    yearsBeforeAny: {type: Number},
    yearsBeforeSame: {type: Number}
});

const costSchema = new Schema({
    equation: {type: String},
    isFunction: {type: Boolean},
    criteria: {type: String}
});

const consequenceSchema = new Schema({
    attribute: {type: String},
    change: {type: String},
    equation: {type: String},
    isFunction: {type: Boolean},
    criteria: {type: String}
});

const treatmentSchema = new Schema({
    name: {type: String},
    feasibility: {type: feasibilitySchema},
    costs: [costSchema],
    consequences: [consequenceSchema],
    budgets: [{type: String}]
});

const treatmentLibrarySchema = new Schema({
    name: {type: String},
    owner: {type: String},
    shared: {type: Boolean},
    description: {type: String},
    treatments: [treatmentSchema]
});

module.exports = mongoose.model('TreatmentLibrary', treatmentLibrarySchema, 'treatmentLibraries');