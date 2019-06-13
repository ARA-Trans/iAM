const compression = require('compression');
const express = require("express");
const debug = require('debug')('app');
const cors = require('cors');
const morgan = require('morgan');
const app = express();
app.use(morgan('tiny'));
app.use(compression());

const bodyParser = require('body-parser');
const env = process.env.NODE_ENV = process.env.NODE_ENV || 'development';

const config = require('./config/config')[env];
require('./config/mongoose')(config);

const server = app.listen(config.port, () => {
  debug(`Running on port ${config.port}`);
});

const io = require('./config/socketIO')(server);

run().catch(error => console.error(error));

async function run() {

  const InvestmentLibrary = require('./models/investmentLibraryModel');
  const investmentLibraryrouter = require('./routes/investmentLibraryRouters')(InvestmentLibrary);

  const Scenario = require('./models/scenarioModel');
  const scenarioRouter = require('./routes/scenarioRouters')(Scenario);

  app.use(cors());
  app.use(bodyParser.urlencoded({ extended: true }));
  app.use(bodyParser.json());

  const pipeline = [{ $match: { 'ns.db': 'BridgeCare', 'ns.coll': 'investmentLibraries' } }];
  const options = { fullDocument: 'updateLookup' };
  InvestmentLibrary.watch(pipeline, options).on('change', data => {
    io.emit('investmentLibrary', data);
  });

  app.use("/api", investmentLibraryrouter);
  app.use("/api", scenarioRouter);
}







