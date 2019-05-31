/* eslint-disable no-console */
const express = require("express");
var cors = require('cors');
const mongoose = require("mongoose");
const app = express();

const bodyParser = require('body-parser');
const port = process.env.PORT || 5000;

const server = app.listen(port, () => {
  console.log(`Running on port ${port}`);
});

const io = require('socket.io')(server);

run().catch(error => console.error(error));

async function run() {
  await mongoose.connect("mongodb://localhost:3000/BridgeCare?replicaSet=r1", { useNewUrlParser: true });

  io.on('connect', () => {console.log('a user is connected')});
  io.on('disconnect', () => {console.log('a user is disconnected')});

  const InvestmentLibrary = require("./models/investmentLibraryModel");
  const investmentLibraryrouter = require('./routes/investmentLibraryRouters')(InvestmentLibrary);

  app.use(cors());
  io.origins('*:*');
  app.use(bodyParser.urlencoded({ extended: true }));
  app.use(bodyParser.json());

  app.use("/api", investmentLibraryrouter);

  const pipeline = [{$match: {'ns.db': 'BridgeCare', 'ns.coll': 'investmentLibraries'}}];
  const options = {fullDocument: 'updateLookup'};
  InvestmentLibrary.watch(pipeline, options).on('change', data => {
    io.emit('investmentLibrary', data);
  });
}







