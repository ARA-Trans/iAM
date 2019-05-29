const mongoose = require("mongoose");

const { Schema } = mongoose;

const budgetYearSchema = new Schema({
    budgetAmount: { type: Number },
    budgetName: { type: String },
    year: { type: Number },
    investmentLibraryId: { type: Number }
});

const investmentLibraryModel = new Schema({
    description: { type: String },
    discountRate: { type: Number },
    inflationRate: { type: Number },
    name: { type: String },
    budgetOrder: [{ type: String }],
    budgetYears: [budgetYearSchema]
});
investmentLibraryModel.set('toJSON', {
    virtuals: true
});
budgetYearSchema.set('toJSON', {
    virtuals: true
});
module.exports = mongoose.model('InvestmentLibrary', investmentLibraryModel, 'investmentLibraries');