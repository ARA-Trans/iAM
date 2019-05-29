const express = require("express");
const mongoose = require("mongoose");
const app = express();
const db = mongoose.connect(
  "mongodb://localhost:3000/BridgeCare?replicaSet=r1",
  { useNewUrlParser: true }
);
const bodyParser = require('body-parser');
const port = process.env.PORT || 5000;
const InvestmentLibrary = require("./models/investmentLibraryModel");
const investmentLibraryrouter = require('./routes/investmentLibraryRouters')(InvestmentLibrary);

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());


app.use("/api", investmentLibraryrouter);

app.get("/", (req, res) => {
  res.send("Welcome to the API!");
});

app.listen(port, () => {
  console.log(`Running on port ${port}`);
});
