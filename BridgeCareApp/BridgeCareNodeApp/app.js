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
