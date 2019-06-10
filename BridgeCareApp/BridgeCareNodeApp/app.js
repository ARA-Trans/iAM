const compression = require('compression');
const express = require("express");
const debug = require('debug')('app');
const cors = require('cors');
const mongoose = require("mongoose");
const morgan = require('morgan');
const app = express();
app.use(morgan('tiny'));
app.use(compression());

const bodyParser = require('body-parser');
const port = process.env.PORT || 5000;

const server = app.listen(port, () => {
  debug(`Running on port ${port}`);
});

const io = require('socket.io')(server);

run().catch(error => console.error(error));

async function run() {
  let connectionTest = process.env.NODE_ENV;
  if (process.env.NODE_ENV == 'development') {
    await mongoose.connect("mongodb://sbhardwaj:BridgecareARA123@localhost:27017/BridgeCare?replicaSet=rs0", { useNewUrlParser: true })
      .then(() => {
        connectionTest = 'connected to mongo db on the local';
        debug('connected to mongo db on the local');
      })
      .catch((err) => {
        connectionTest = 'error has occured in connection';
        debug('error has occured in connection');
      });
  }
  if (process.env.NODE_ENV == 'production') {
    await mongoose.connect("mongodb://admin:BridgecareARA123@localhost:27017/BridgeCare?replicaSet=r1", { useNewUrlParser: true })
      .then(() => debug('connected to mongo db on the server'))
      .then(() => {
        connectionTest = 'connected to mongo db on the server';
        debug('connected to mongo db on the server');
      })
      .catch((err) => {
        connectionTest = 'server: error has occured in connection';
        debug('server: error has occured in connection');
      });

  }

  io.on('connect', (socket) => {
    debug('a user is connected');
    const pipeline = [{ $match: { 'ns.db': 'BridgeCare', 'ns.coll': 'investmentLibraries' } }];
    const options = { fullDocument: 'updateLookup' };
    InvestmentLibrary.watch(pipeline, options).on('change', data => {
      socket.broadcast.emit('investmentLibrary', data);
    });
  });
  io.on('disconnect', () => { debug('a user is disconnected'); });

  const InvestmentLibrary = require("./models/investmentLibraryModel");
  const investmentLibraryrouter = require('./routes/investmentLibraryRouters')(InvestmentLibrary, connectionTest);

  app.use(cors());
  io.origins('*:*');
  app.use(bodyParser.urlencoded({ extended: true }));
  app.use(bodyParser.json());

  app.use("/api", investmentLibraryrouter);
}







