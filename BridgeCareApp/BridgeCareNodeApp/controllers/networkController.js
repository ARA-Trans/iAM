const debug = require('debug')('app:networkController');

function networkController(Network) {
    function post(req, res) {
        const network = new Network(req.body);

        network.save(function (err, rollupStatus) {
            if (err) {
                res.status(400);
                return res.json(err);
            }
            res.status(200);
            return res.json(rollupStatus);
        });
    }

/**
     * PUT Nodejs API endpoint for scenario; returns updates & returns a scenario
     * @param req
     * @param res
     */
    function put(req, res) {
        Network.findOneAndUpdate({_id: req.params.networkId}, req.body, {new: true}, (err, doc) => {
            if (err) {
                return res.status(400).json(err);
            }
            return res.status(200).json(doc);
        });
    }

    function get(req, res) {
        Network.find((err, rollupStatus) => {
            if (err) {
                return res.send(err);
            }
            return res.json(rollupStatus);
        });
    }

    function postLegacyNetworks(req, res) {
        const multipleNetworks = [];
        multipleNetworks.push(...req.body);
        const resultant = [];

        multipleNetworks.forEach(item => {
            const network = new Network(item);
            network.save(function (err) {
                if (err) {
                    res.status(400);
                    return res.json(err);
                }
                res.status(200);                
            });
        });

        Network.find((err, networks) => {
            if (err) {
                return res.send(err);
            }
            return res.json(networks);
        });
    }

    return { post, get, put, postLegacyNetworks };
}

module.exports = networkController;