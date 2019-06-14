const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const performanceLibraryEquationSchema = new Schema({
   _id: {type: String},
   attribute: {type: String},
   equationName: {type: String},
   equation: {type: String},
   criteria: {type: String},
   shift: {type: Boolean},
   piecewise: {type: Boolean},
   isFunction: {type: Boolean}
});

const performanceLibrarySchema = new Schema({
   _id: {type: String},
   name: {type: String},
   description: {type: String},
   equations: [performanceLibraryEquationSchema]
});

module.exports = mongoose.model('PerformanceLibrary', performanceLibrarySchema, 'performanceLibraries');