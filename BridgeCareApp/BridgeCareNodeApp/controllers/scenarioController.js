const debug = require('debug')('app:scenarioController');
const roles = require('../authorization/roleConfig');
const logger = require('../config/winston');

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

    function postMultipleScenarios(req, res) {
        const multipleScenarios = [];
        multipleScenarios.push(...req.body);

        multipleScenarios.forEach(item => {
            const scenario = new Scenario(item);
            scenario.save(function (err) {
                if (err) {
                    res.status(400);
                    return res.json(err);
                }
                res.status(200);                
            });
        });

        Scenario.find((err, scenariostatus) => {
            if (err) {
                return res.send(err);
            }
            return res.json(scenariostatus);
        });
    }

    /**
     * PUT Nodejs API endpoint for scenario; returns updates & returns a scenario
     * @param req
     * @param res
     */
    function put(req, res) {
        var newScenario = req.body;
        newScenario.lastModifiedDate = Date.now();
        Scenario.findOneAndUpdate({_id: req.params.scenarioId}, newScenario, {new: true}, (err, doc) => {
            if (err) {
                return res.status(400).json(err);
            }
            return res.status(200).json(doc);
        });
    }

    function updateScenarioStatus(request, response) {
        var newStatus = request.body.status;
        Scenario.findOneAndUpdate({simulationId: request.params.scenarioId}, {status: newStatus}, {new: true}, (error, document) => {
            if (error) {
                return response.status(400).json(error);
            }
            return response.status(200).json(document);
        });
    }

    function get(req, res) {
        if (req.user.roles.indexOf(roles.administrator) >= 0 || req.user.roles.indexOf(roles.cwopa) >= 0) {
            Scenario.find((err, scenariostatus) => {
                if (err) {
                    logger.error('Error in get request of scenarioController: ', err);
                    return res.send(err);
                }
                return res.json(scenariostatus);
            });
        } else {
            const query = {
                $or: [
                    {owner: [req.user.username, null, undefined]},
                    {users: { $elemMatch: {username: [req.user.username, null, undefined]}}}
                ]
            };
            Scenario.find(query, (err, scenariostatus) => {
                if (err) {
                    return res.send(err);
                }
                return res.json(scenariostatus);
            });
        }
    }

    function deleteScenario(req, res) {
        Scenario.deleteOne({_id: req.params.scenarioId}, (err, result) => {
            console.log("code entered");
            if (err) {
                return res.status(500).json(err);
            }

            if (result.deletedCount === 1) {
                return res.status(200).json(result);
            } else {
                return res.status(404).json({message: 'Scenario not found'});
            }
        });
    }

    return { post, get, deleteScenario, put, updateScenarioStatus, postMultipleScenarios};
}

module.exports = scenarioController;