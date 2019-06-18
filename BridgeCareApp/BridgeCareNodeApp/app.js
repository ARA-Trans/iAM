﻿const express = require("express");
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
  const investmentLibraryRouter = require('./routes/investmentLibraryRouters')(InvestmentLibrary);

  const Scenario = require('./models/scenarioModel');
  const scenarioRouter = require('./routes/scenarioRouters')(Scenario);

  const PerformanceLibrary = require('./models/performanceLibraryModel');
  const performanceLibraryRouter = require('./routes/performanceLibraryRouters')(PerformanceLibrary);

  const TreatmentLibrary = require('./models/treatmentLibraryModel');
  const treatmentLibraryRouter = require('./routes/treatmentLibraryRouters')(TreatmentLibrary);

  const options = { fullDocument: 'updateLookup' };

  InvestmentLibrary.watch([], options).on('change', data => {
    io.emit('investmentLibrary', data);
  });

  PerformanceLibrary.watch([], options).on('change', data => {
    io.emit('performanceLibrary', data);
  });

  TreatmentLibrary.watch([], options).on('change', data => {
    io.emit('treatmentLibrary', data);
  });

  Scenario.watch([]).on('change', data => {
    debug(data);
    io.emit('scenarioStatus', data);
  });

  app.use("/api", [investmentLibraryRouter, scenarioRouter, performanceLibraryRouter, treatmentLibraryRouter]);
}







