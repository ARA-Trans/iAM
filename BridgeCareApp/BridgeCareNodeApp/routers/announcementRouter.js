const express = require('express');
const announcementController = require('../controllers/announcementController');
const authorizationFilter = require('../authorization/authorizationFilter');

function announcementRouter(Announcement) {
    const router = express.Router();
    const controller = announcementController(Announcement);

    router.route('/GetAnnouncements').get(authorizationFilter(), controller.get);
    router.route('/CreateAnnouncement').post(authorizationFilter(["PD-BAMS-Administrator"]), controller.post);
    router.route('/UpdateAnnouncement').put(authorizationFilter(["PD-BAMS-Administrator"]), controller.put);
    router.route('/DeleteAnnouncement/:announcementId').delete(authorizationFilter(["PD-BAMS-Administrator"]), controller.deleteAnnouncement);

    return router;
}

module.exports = announcementRouter;