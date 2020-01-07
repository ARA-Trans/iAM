const compression = require('compression');
const cors = require('cors');
const morgan = require('morgan');
const bodyParser = require('body-parser');

module.exports = function (app, winston) {
    app.use(morgan('combined', {stream: winston.stream}));
    app.use(compression());

    app.use(cors());
    app.use(bodyParser.urlencoded({ limit: '10mb', extended: true }));
    app.use(bodyParser.json({limit: '10mb', extended: true}));
};
