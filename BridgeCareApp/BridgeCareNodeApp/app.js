/* eslint-disable no-console */
const express = require("express");
var cors = require('cors');
const mongoose = require("mongoose");
const app = express();

const bodyParser = require('body-parser');
const port = process.env.PORT || 5000;


run().catch(error => console.error(error));

async function run() {
  await mongoose.connect("mongodb://localhost:3000/BridgeCare?replicaSet=r1", { useNewUrlParser: true });

  const InvestmentLibrary = require("./models/investmentLibraryModel");
  const investmentLibraryrouter = require('./routes/investmentLibraryRouters')(InvestmentLibrary);

  app.use(cors());
  app.use(bodyParser.urlencoded({ extended: true }));
  app.use(bodyParser.json());

  app.use("/api", investmentLibraryrouter);

  const pipeline = [{$match: {'ns.db': 'BridgeCare', 'ns.coll': 'investmentLibraries'}}];
  InvestmentLibrary.watch(pipeline).on('change', data => {
    console.log(data);
  });

  app.listen(port, () => {
    console.log(`Running on port ${port}`);
  });
}







