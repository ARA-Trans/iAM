const mongoose = require("mongoose");

const { Schema } = mongoose;

const budgetYearSchema = {
    budgetAmount: { type: Number },
    budgetName: { type: String },
    year: { type: Number },
    investmentLibraryId: {type: Number}
}

const investmentLibraryModel = new Schema({
    Investment: {
        description: { type: String },
        discountRate: { type: Number },
        inflationRate: { type: Number },
        name: { type: String },
        budgetOrder: [{ type: String }],
        budgetYears: [budgetYearSchema]
    }
});

module.exports = mongoose.model('InvestmentLibrary', investmentLibraryModel, 'investmentLibraries');