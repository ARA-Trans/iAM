const debug = require('debug')('app:rollupController');

function rollupController(Rollup) {
    function post(req, res) {
        const rollup = new Rollup(req.body);

        rollup.save(function (err, rollupStatus) {
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
        Rollup.findOneAndUpdate({_id: req.params.networkId}, req.body, {new: true}, (err, doc) => {
            if (err) {
                return res.status(400).json(err);
            }
            return res.status(200).json(doc);
        });
    }

    function get(req, res) {
        Rollup.find((err, rollupStatus) => {
            if (err) {
                return res.send(err);
            }
            return res.json(rollupStatus);
        });
    }

    return { post, get, put };
}

module.exports = rollupController;