const mongoose = require("mongoose");
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const priorityFundSchema = new Schema({
    budget: { type: String },
    funding: { type: Number }
});

const prioritySchema = new Schema({
    priorityLevel: { type: Number },
    year: { type: Number },
    criteria: { type: String },
    priorityFunds: [priorityFundSchema]
});

const priorityLibrarySchema = new Schema({
    name: { type: String },
    description: { type: String },
    priorities: [prioritySchema]
});

module.exports = mongoose.model('PriorityLibrary', priorityLibrarySchema, 'priorityLibraries');
