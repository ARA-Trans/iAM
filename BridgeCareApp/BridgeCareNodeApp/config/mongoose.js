const mongoose = require('mongoose');
const debug = require('debug')('app:mongoose');

module.exports = function(config) {

    mongoose.connect(config.db, { useNewUrlParser: true })
    .then(() => {
      debug('connected to mongo db on the local');
    })
    .catch((err) => {
      debug('error has occured in connection');
    });
};
