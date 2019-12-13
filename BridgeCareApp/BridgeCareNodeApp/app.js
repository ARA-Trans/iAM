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

const io = require('./config/socketIO')(server, {
  pingTimeout: 60000
});

run().catch(error => debug(error));

async function run() {
  const InvestmentLibrary = require('./models/investmentLibraryModel');
  const investmentLibraryRoutes = require('./routes/investmentLibraryRoutes')(InvestmentLibrary);

  const Scenario = require('./models/scenarioModel');
  const scenarioRoutes = require('./routes/scenarioRoutes')(Scenario);

  const PerformanceLibrary = require('./models/performanceLibraryModel');
  const performanceLibraryRoutes = require('./routes/performanceLibraryRoutes')(PerformanceLibrary);

  const TreatmentLibrary = require('./models/treatmentLibraryModel');
  const treatmentLibraryRoutes = require('./routes/treatmentLibraryRoutes')(TreatmentLibrary);

  const PriorityLibrary = require('./models/priorityLibraryModel');
  const priorityLibraryRoutes = require('./routes/priorityLibraryRoutes')(PriorityLibrary);

  const RemainingLifeLimitLibrary = require('./models/remainingLifeLimitLibraryModel');
  const remainingLifeLimitLibraryRoutes = require('./routes/remainingLifeLimitLibraryRoutes')(RemainingLifeLimitLibrary);

  const Network = require('./models/NetworkModel');
  const networkRoutes = require('./routes/networkRoutes')(Network);

  const CashFlowLibrary = require('./models/cashFlowLibraryModel');
  const cashFlowLibraryRoutes = require('./routes/cashFlowLibraryRoutes')(CashFlowLibrary);

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

  Scenario.watch([], options).on('change', data => {
    io.emit('scenarioStatus', data);
  });

  PriorityLibrary.watch([], options).on('change', data => {
    io.emit('priorityLibrary', data);
  });

  RemainingLifeLimitLibrary.watch([], options).on('change', data => {
    io.emit('remainingLifeLimitLibrary', data);
  });

  Network.watch([], options).on('change', data => {
    io.emit('rollupStatus', data);
  });

  CashFlowLibrary.watch([], options).on('change', data => {
      io.emit('cashFlowLibrary', data);
  });

  app.use("/api", [
      investmentLibraryRoutes,
      scenarioRoutes,
      performanceLibraryRoutes,
      treatmentLibraryRoutes,
      priorityLibraryRoutes,
      remainingLifeLimitLibraryRoutes,
      networkRoutes,
      cashFlowLibraryRoutes
  ]);
}
