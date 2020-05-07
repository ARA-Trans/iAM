const mongoose = require("mongoose");
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const investmentLibraryBudgetYearSchema = new Schema({
    budgetAmount: { type: Number },
    budgetName: { type: String },
    year: { type: Number },
    criteriaDrivenBudgetId: { type: String }
});

const criteriaDrivenBudgetSchema = new Schema({
    budgetName: { type: String },
    criteria: { type: String }
});

const investmentLibrarySchema = new Schema({
    description: { type: String },
    discountRate: { type: Number },
    inflationRate: { type: Number },
    name: { type: String },
    owner: { type: String },
    shared: {type: Boolean},
    budgetOrder: [{ type: String }],
    budgetYears: [investmentLibraryBudgetYearSchema],
    criteriaDrivenBudgets: [criteriaDrivenBudgetSchema]
});

module.exports = mongoose.model('InvestmentLibrary', investmentLibrarySchema, 'investmentLibraries');