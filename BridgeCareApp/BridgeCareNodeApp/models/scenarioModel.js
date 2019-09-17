const mongoose = require("mongoose");
mongoose.set('useFindAndModify', false)

const { Schema } = mongoose;

const scenarioModel = new Schema({
    createdDate: { type: Date, default: Date.now },
    lastModifiedDate: {type: Date, default: Date.now },
    networkId: { type: Number },
    simulationId: { type: Number },
    networkName: { type: String },
    simulationName: { type: String },
    status: { type: String },
    rollupStatus: {type: String},
    shared: {type: Boolean }
});

module.exports = mongoose.model('Scenario', scenarioModel, 'scenarios');