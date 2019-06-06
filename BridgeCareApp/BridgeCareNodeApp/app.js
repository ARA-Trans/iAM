/* eslint-disable no-console */
var compression = require('compression');
const express = require("express");
var cors = require('cors');
const mongoose = require("mongoose");
const app = express();
app.use(compression());

const bodyParser = require('body-parser');
const port = process.env.PORT || 5000;

const server = app.listen(port, () => {
  console.log(`Running on port ${port}`);
});

const io = require('socket.io')(server);

run().catch(error => console.error(error));

async function run() {
  let connectionTest = process.env.NODE_ENV;
  if (process.env.NODE_ENV == 'development') {
    await mongoose.connect("mongodb://sbhardwaj:BridgecareARA123@localhost:27017/BridgeCare?replicaSet=rs0", { useNewUrlParser: true })
      .then(() => {
        connectionTest = 'connected to mongo db on the local';
        console.log('connected to mongo db on the local');
      })
      .catch((err) => {
        connectionTest = 'error has occured in connection';
        console.log('error has occured in connection');
      });
  }
  if (process.env.NODE_ENV == 'production') {
    await mongoose.connect("mongodb://admin:BridgecareARA123@localhost:27017/BridgeCare?replicaSet=r1", { useNewUrlParser: true })
      .then(() => console.log('connected to mongo db on the server'))
      .then(() => {
        connectionTest = 'connected to mongo db on the server';
        console.log('connected to mongo db on the server');
      })
      .catch((err) => {
        connectionTest = 'server: error has occured in connection';
        console.log('server: error has occured in connection');
      });

  }

  io.on('connect', () => { console.log('a user is connected') });
  io.on('disconnect', () => { console.log('a user is disconnected') });

  const InvestmentLibrary = require("./models/investmentLibraryModel");
  const investmentLibraryrouter = require('./routes/investmentLibraryRouters')(InvestmentLibrary, connectionTest);

  app.use(cors());
  io.origins('*:*');
  app.use(bodyParser.urlencoded({ extended: true }));
  app.use(bodyParser.json());

  app.use("/api", investmentLibraryrouter);

  const pipeline = [{ $match: { 'ns.db': 'BridgeCare', 'ns.coll': 'investmentLibraries' } }];
  const options = { fullDocument: 'updateLookup' };
  InvestmentLibrary.watch(pipeline, options).on('change', data => {
    io.emit('investmentLibrary', data);
  });
}







