const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const {Schema} = mongoose;

const cashFlowParameterSchema = new Schema({
    parameter: {type: String},
    criteria: {type: String}
});

const cashFlowDurationSchema = new Schema({
    duration: {type: Number},
    maxTreatmentCost: {type: String}
});

const cashFlowLibrarySchema = new Schema({
    name: {type: String},
    description: {type: String},
    parameters: [cashFlowParameterSchema],
    durations: [cashFlowDurationSchema]
});

module.exports = mongoose.model('CashFlowLibrary', cashFlowLibrarySchema, 'cashFlowLibraries');
