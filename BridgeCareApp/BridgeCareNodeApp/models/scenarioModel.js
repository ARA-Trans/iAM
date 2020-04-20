const mongoose = require("mongoose");
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const scenarioUserSchema = new Schema({
    id: {type: Number},
    username: {type: String},
    canModify: {type: Boolean}
});

const scenarioModel = new Schema({
    createdDate: { type: Date, default: Date.now },
    lastModifiedDate: {type: Date, default: Date.now },
    lastRun: {type: Date},
    networkId: { type: Number },
    simulationId: { type: Number },
    networkName: { type: String },
    simulationName: { type: String },
    status: { type: String },
    rollupStatus: {type: String},
    shared: {type: Boolean },
    owner: {type: String },
    creator: {type: String},
    users: [scenarioUserSchema]
});

module.exports = mongoose.model('Scenario', scenarioModel, 'scenarios');