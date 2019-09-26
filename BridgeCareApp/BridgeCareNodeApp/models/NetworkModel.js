const mongoose = require("mongoose");
mongoose.set('useFindAndModify', false)

const { Schema } = mongoose;

const networkModel = new Schema({
    createdDate: { type: Date, default: Date.now },
    lastModifiedDate: {type: Date, default: Date.now },
    networkId: { type: Number },
    networkName: { type: String },
    rollupStatus: {type: String}
});

module.exports = mongoose.model('Network', networkModel, 'networks');