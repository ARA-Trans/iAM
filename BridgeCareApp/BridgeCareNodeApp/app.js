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

// const io = require('./config/socketIO')(server, {
//     pingTimeout: 60000
// });

run().catch(error => debug(error));

async function run() {
    const DeficientLibrary = require('./models/deficientLibraryModel');
    const deficientLibraryRouter = require('./routers/deficientLibraryRouter')(DeficientLibrary);

    const InvestmentLibrary = require('./models/investmentLibraryModel');
    const investmentLibraryRouter = require('./routers/investmentLibraryRouter')(InvestmentLibrary);

    const Network = require('./models/NetworkModel');
    const networkRouter = require('./routers/networkRouter')(Network);

    const PerformanceLibrary = require('./models/performanceLibraryModel');
    const performanceLibraryRouter = require('./routers/performanceLibraryRouter')(PerformanceLibrary);

    const PriorityLibrary = require('./models/priorityLibraryModel');
    const priorityLibraryRouter = require('./routers/priorityLibraryRouter')(PriorityLibrary);

    const RemainingLifeLimitLibrary = require('./models/remainingLifeLimitLibraryModel');
    const remainingLifeLimitLibraryRouter = require('./routers/remainingLifeLimitLibraryRouter')(RemainingLifeLimitLibrary);

    const Scenario = require('./models/scenarioModel');
    const scenarioRouter = require('./routers/scenarioRouter')(Scenario);

    const TargetLibrary = require('./models/targetLibraryModel');
    const targetLibraryRouter = require('./routers/targetLibraryRouter')(TargetLibrary);

    const TreatmentLibrary = require('./models/treatmentLibraryModel');
    const treatmentLibraryRouter = require('./routers/treatmentLibraryRouter')(TreatmentLibrary);

    // const options = { fullDocument: 'updateLookup' };

    // DeficientLibrary.watch([], options).on('change', data => {
    //     io.emit('deficientLibrary', data);
    // });

    // InvestmentLibrary.watch([], options).on('change', data => {
    //     io.emit('investmentLibrary', data);
    // });

    // Network.watch([], options).on('change', data => {
    //     io.emit('rollupStatus', data);
    // });

    // PerformanceLibrary.watch([], options).on('change', data => {
    //     io.emit('performanceLibrary', data);
    // });

    // PriorityLibrary.watch([], options).on('change', data => {
    //     io.emit('priorityLibrary', data);
    // });

    // RemainingLifeLimitLibrary.watch([], options).on('change', data => {
    //     io.emit('remainingLifeLimitLibrary', data);
    // });

    // Scenario.watch([], options).on('change', data => {
    //     io.emit('scenarioStatus', data);
    // });

    // TargetLibrary.watch([], options).on('change', data => {
    //     io.emit('targetLibrary', data);
    // });

    // TreatmentLibrary.watch([], options).on('change', data => {
    //     io.emit('treatmentLibrary', data);
    // });

    app.use("/api", [
        deficientLibraryRouter,
        investmentLibraryRouter,
        networkRouter,
        performanceLibraryRouter,
        priorityLibraryRouter,
        remainingLifeLimitLibraryRouter,
        scenarioRouter,
        targetLibraryRouter,
        treatmentLibraryRouter,
    ]);
}
