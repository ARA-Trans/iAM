function announcementController(Announcement) {
    /**
     * GET NodeJS API endpoint for announcements; gets & returns all announcements
     * @param request
     * @param response
     */
    function get(request, response) {
        Announcement.find((error, announcements) => {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(announcements);
        });
    }

    /**
     * POST NodeJS API endpoint for announcements; creates & returns an announcement
     * @param request Http request
     * @param response Http response
     */
    function post(request, response) {
        const announcement = new Announcement(request.body);

        announcement.save(function(error, announcement) {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(announcement);
        });
    }

    /**
     * PUT NodeJS API endpoint for announcements; updates & returns an announcement
     * @param request Http request
     * @param response Http response
     */
    function put(request, response) {
        Announcement.findOneAndUpdate({_id: request.body._id}, request.body, {new: true}, (error, Announcement) => {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(Announcement);
        });
    }

    /**
     * DELETE NodeJS API endpoint for announcements; deletes an announcement
     * @param request Http request
     * @param response Http response
     */
    function deleteAnnouncement(request, response) {
        Announcement.deleteOne({_id: request.params.announcementId}, (error, Announcement) => {
            if (error)
                return response.status(500).json(error);

            return response.status(200).json(Announcement);
        });
    }

    return {get, post, put, deleteAnnouncement};
}

module.exports = announcementController;
