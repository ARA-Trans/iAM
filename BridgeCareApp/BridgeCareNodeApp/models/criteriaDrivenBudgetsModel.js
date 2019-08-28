const mongoose = require("mongoose");
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const criteriaDrivenBudgetsSchema = new Schema({
    scenarioId: { type: Number },
    budgetName: { type: String },
    criteria: { type: String }
});

module.exports = mongoose.model('CriteriaDrivenBudgets', criteriaDrivenBudgetsSchema, 'criteriaDrivenBudgets');