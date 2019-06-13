const debug = require('debug')('app:socketIO');

module.exports = function(server){
    const io = require('socket.io')(server);

    io.on('connect', (socket) => { debug('a user is connected'); });
    io.on('disconnect', () => { debug('a user is disconnected'); });
    io.origins('*:*');

    return io;
}