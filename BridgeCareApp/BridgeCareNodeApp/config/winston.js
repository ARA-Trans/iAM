var appRoot = require('app-root-path');
var winston = require('winston');

var logger = new winston.createLogger({
    level: 'info',
    format: winston.format.combine(
        winston.format.timestamp(),
        winston.format.json()
    ),
    handleException: true,
    maxsize: 5242880, // 5MB
    maxFiles: 5,
    timestamp: true,
    transports: [
        new winston.transports.File({
            filename: `${appRoot}/logs/nodeErrorLogs.log`, level: 'error', 
            handleExceptions: true,
            datePattern: 'MM-DD-YYYY'
        }),
        new winston.transports.File({filename: `${appRoot}/logs/nodeCombinedLogs.log`,
            level: 'info',
            datePattern: 'MM-DD-YYYY'
        }),
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
        logger.log('http', message);
    }
};

module.exports = logger;