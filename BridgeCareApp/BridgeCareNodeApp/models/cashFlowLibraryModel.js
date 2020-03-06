const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const {Schema} = mongoose;

const splitTreatmentLimitSchema = new Schema({
    rank: {type: Number},
    amount: {type: Number},
    percentage: {type: String}
});

const splitTreatmentSchema = new Schema({
    description: {type: String},
    criteria: {type: String},
    splitTreatmentLimits: [splitTreatmentLimitSchema]
});

const cashFlowLibrarySchema = new Schema({
    name: {type: String},
    owner: {type: String},
    description: {type: String},
    splitTreatments: [splitTreatmentSchema]
});

module.exports = mongoose.model('CashFlowLibrary', cashFlowLibrarySchema, 'cashFlowLibraries');
