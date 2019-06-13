const mongoose = require("mongoose");
const autoIncrement = require('mongoose-sequence')(mongoose);
mongoose.set('useFindAndModify', false)

const { Schema } = mongoose;

const budgetYearSchema = new Schema({
    id: {type: Number},
    budgetAmount: { type: Number },
    budgetName: { type: String },
    year: { type: Number },
    investmentLibraryId: { type: Number }
}, {_id: false});

const investmentLibraryModel = new Schema({
    description: { type: String },
    discountRate: { type: Number },
    inflationRate: { type: Number },
    name: { type: String },
    budgetOrder: [{ type: String }],
    budgetYears: [budgetYearSchema]
});
investmentLibraryModel.plugin(autoIncrement,{inc_field: 'id'});

module.exports = mongoose.model('InvestmentLibrary', investmentLibraryModel, 'investmentLibraries');