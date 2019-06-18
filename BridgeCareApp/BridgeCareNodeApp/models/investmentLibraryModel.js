const mongoose = require("mongoose");
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const investmentLibraryBudgetYearSchema = new Schema({
    budgetAmount: { type: Number },
    budgetName: { type: String },
    year: { type: Number }
});

const investmentLibrarySchema = new Schema({
    description: { type: String },
    discountRate: { type: Number },
    inflationRate: { type: Number },
    name: { type: String },
    budgetOrder: [{ type: String }],
    budgetYears: [investmentLibraryBudgetYearSchema]
});

module.exports = mongoose.model('InvestmentLibrary', investmentLibrarySchema, 'investmentLibraries');