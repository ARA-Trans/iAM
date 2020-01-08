const debug = require('debug')('app:socketIO');

module.exports = function(server){
    const io = require('socket.io')(server);
    io.set('transports', ['websocket']);

    io.on('connect', () => { debug('a user is connected'); });
    io.on('disconnect', () => { debug('a user is disconnected'); });
    io.origins('*:*');

    return io;
};
