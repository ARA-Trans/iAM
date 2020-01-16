function pollingController() {
    var eventList = [];

    /**
     * GET Nodejs API endpoint for polling.
     * Fetches any new events from the past ten seconds
     * @param req Http request
     * @param res Http response
     */
    function get(request, response) {
        cullOldEvents();
        const id = request.params.browserId;
        // Get events that have not been sent to this poller yet
        var unsentList = eventList.filter(event => { 
            const notSent = !event.sentTo.includes(id);
            if (notSent) {
                event.sentTo.push(id);
            }
            return notSent;
        });
        var output = unsentList.map(event => {return {eventName: event.eventName, payload: event.payload}});
        return response.json(output);
    }

    /**
     * Removes any events older than ten seconds.
     */
    function cullOldEvents() {
        eventList = eventList.filter(event => event.time > Date.now() - 10000);
    }

    /**
     * Sends out an event to any polling clients
     */
    function emit(eventName, eventData) {
        eventList.push({eventName, payload: eventData, time: Date.now(), sentTo: []})
    }

    return { get, emit };
}

module.exports = pollingController;