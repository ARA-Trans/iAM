function scenarioController(Scenario) {
    function post(req, res) {
        const scenario = new Scenario(req.body);

        scenario.save(function (err, scenariostatus) {
            if (err) {
                res.status(400);
                return res.json(err);
            }
            res.status(200);
            return res.json(scenariostatus);
        });
    }

    function get(req, res) {
        Scenario.find((err, scenariostatus) => {
            if (err) {
                return res.send(err);
            }
            return res.json(scenariostatus);
        });
    }

    function deleteScenario(req, res) {
        Scenario.findOneAndDelete({simulationId: req.params.scenarioId})
        .then(scenario => {
            if(!scenario) {
                return res.status(404).send({
                    message: `Scenario not found with id ${req.params.scenarioId}`
                });
            }
            res.send({message: 'Scenario deleted successfully'});
        })
        .catch(err => {
            if(err.name === 'NotFound'){
                return res.status(404).send({
                    message: `Scenario not found with Id ${req.params.scenarioId}`
                });
            }
            return res.status(500)
        });
    }
    return { post, get, deleteScenario};
}

module.exports = scenarioController;