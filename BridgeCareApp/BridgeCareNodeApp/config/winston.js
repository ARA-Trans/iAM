var appRoot = require('app-root-path');
var winston = require('winston');

var options = {
    file: {
        level: 'info',
        filename: `${appRoot}/logs/nodeErrorLogs.log`,
        handleException: true,
        json: true,
        maxsize: 5242880, // 5MB
        maxFiles: 5,
        colorize: false
    }
};

var logger = new winston.createLogger({
    level: 'info',
    format: winston.format.json(),
    handleException: true,
    maxsize: 5242880, // 5MB
    maxFiles: 5,
    transports: [
        new winston.transports.File({filename: `${appRoot}/logs/nodeErrorLogs.log`, level: 'error'}),
        new winston.transports.File({filename: `${appRoot}/logs/nodeCombinedLogs.log`}),
    ],
    exitOnError: false // do not exit on handles exceptions
});

if (process.env.NODE_ENV !== 'production') {
    logger.add(new winston.transports.Console({
      format: winston.format.simple()
    }));
  }

logger.stream = {
    write: function(message, encoding){
        logger.info(message);
    }
};

module.exports = logger;