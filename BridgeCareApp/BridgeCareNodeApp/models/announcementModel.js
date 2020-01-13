const mongoose = require('mongoose');
mongoose.set('useFindAndModify', false);

const {Schema} = mongoose;

const announcementSchema = new Schema({
    title: {type: String},
    content: {type: String},
    creationDate: {type: Number}
});

module.exports = mongoose.model('announcement', announcementSchema, 'announcements');
