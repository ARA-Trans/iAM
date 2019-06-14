const express = require("express");
const debug = require('debug')('app');
const app = express();
require('./config/express')(app);

const env = process.env.NODE_ENV = process.env.NODE_ENV || 'development';

const config = require('./config/config')[env];
require('./config/mongoose')(config);

const server = app.listen(config.port, () => {
  debug(`Running on port ${config.port}`);
});

const io = require('./config/socketIO')(server);

run().catch(error => debug(error));

async function run() {

  const InvestmentLibrary = require('./models/investmentLibraryModel');
  const investmentLibraryrouter = require('./routes/investmentLibraryRouters')(InvestmentLibrary);

  const Scenario = require('./models/scenarioModel');
  const scenarioRouter = require('./routes/scenarioRouters')(Scenario);

  const options = { fullDocument: 'updateLookup' };
  InvestmentLibrary.watch(options).on('change', data => {
    io.emit('investmentLibrary', data);
  });

  app.use("/api", investmentLibraryrouter);
  app.use("/api", scenarioRouter);
}







