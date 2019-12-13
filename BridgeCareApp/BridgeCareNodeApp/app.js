const express = require("express");
const debug = require('debug')('app');
const app = express();
require('./config/express')(app);

const passport = require("passport");

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
    const CashFlowLibrary = require('./models/cashFlowLibraryModel');
    const cashFlowLibraryRoutes = require('./routes/cashFlowLibraryRoutes')(CashFlowLibrary);

    const DeficientLibrary = require('./models/deficientLibraryModel');
    const deficientLibraryRoutes = require('./routes/deficientLibraryRoutes')(DeficientLibrary);

    const InvestmentLibrary = require('./models/investmentLibraryModel');
    const investmentLibraryRoutes = require('./routes/investmentLibraryRoutes')(InvestmentLibrary);

    const Network = require('./models/NetworkModel');
    const networkRoutes = require('./routes/networkRoutes')(Network);

    const PerformanceLibrary = require('./models/performanceLibraryModel');
    const performanceLibraryRoutes = require('./routes/performanceLibraryRoutes')(PerformanceLibrary);

    const PriorityLibrary = require('./models/priorityLibraryModel');
    const priorityLibraryRoutes = require('./routes/priorityLibraryRoutes')(PriorityLibrary);

    const RemainingLifeLimitLibrary = require('./models/remainingLifeLimitLibraryModel');
    const remainingLifeLimitLibraryRoutes = require('./routes/remainingLifeLimitLibraryRoutes')(RemainingLifeLimitLibrary);

    const Scenario = require('./models/scenarioModel');
    const scenarioRoutes = require('./routes/scenarioRoutes')(Scenario);

    const TargetLibrary = require('./models/targetLibraryModel');
    const targetLibraryRoutes = require('./routes/targetLibraryRoutes')(TargetLibrary);

    const TreatmentLibrary = require('./models/treatmentLibraryModel');
    const treatmentLibraryRoutes = require('./routes/treatmentLibraryRoutes')(TreatmentLibrary);

    const options = { fullDocument: 'updateLookup' };

    CashFlowLibrary.watch([], options).on('change', data => {
        io.emit('cashFlowLibrary', data);
    });

    DeficientLibrary.watch([], options).on('change', data => {
        io.emit('deficientLibrary', data);
    });

    InvestmentLibrary.watch([], options).on('change', data => {
        io.emit('investmentLibrary', data);
    });

    Network.watch([], options).on('change', data => {
        io.emit('rollupStatus', data);
    });

    PerformanceLibrary.watch([], options).on('change', data => {
        io.emit('performanceLibrary', data);
    });

    PriorityLibrary.watch([], options).on('change', data => {
        io.emit('priorityLibrary', data);
    });

    RemainingLifeLimitLibrary.watch([], options).on('change', data => {
        io.emit('remainingLifeLimitLibrary', data);
    });

    Scenario.watch([], options).on('change', data => {
        io.emit('scenarioStatus', data);
    });

    TargetLibrary.watch([], options).on('change', data => {
        io.emit('targetLibrary', data);
    });

    TreatmentLibrary.watch([], options).on('change', data => {
        io.emit('treatmentLibrary', data);
    });

  app.use("/api", [
      cashFlowLibraryRoutes,
      deficientLibraryRoutes,
      investmentLibraryRoutes,
      networkRoutes,
      performanceLibraryRoutes,
      priorityLibraryRoutes,
      remainingLifeLimitLibraryRoutes,
      scenarioRoutes,
      targetLibraryRoutes,
      treatmentLibraryRoutes
  ]);
}
