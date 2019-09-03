const mongoose = require("mongoose");
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const budgetNameCriteriaSchema = new Schema({
    budgetName: { type: String },
    criteria: { type: String }
});

const criteriaDrivenBudgetsSchema = new Schema({
    budgetCriteria: [budgetNameCriteriaSchema]
});

module.exports = mongoose.model('CriteriaDrivenBudgets', criteriaDrivenBudgetsSchema, 'criteriaDrivenBudgets');