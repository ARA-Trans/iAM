const debug = require('debug')('app:criteriaDrivenBudget');

function criteriaDrivenBudgetsController(CriteriaDrivenBudgets) {
    /**
     * POST Nodejs API endpoint for criteria driven budgets;
     * @param req Http request
     * @param res Http response
     */
    function post(req, res) {

            CriteriaDrivenBudgets.deleteMany({scenarioId: {$in: [req.params.scenarioId]}}, (err) => {
                if (err) {
                    return res.status(400).json(err);
                }
            }).then(_ => {
                CriteriaDrivenBudgets.insertMany(req.body,(err, docs) => {
                  if(err){
                      res.status(400);
                      return res.json(err);
                  }
                  return res.status(200).json(docs);
                });
            });
    }

    /**
     *
     * @param req Http request
     * @param res Http response
     */
    function getById(req, res) {
        CriteriaDrivenBudgets.findById({scenarioId: req.params.scenarioId}, (err, criteria) => {
            if (err) {
                return res.send(err);
            }

            return res.json(criteria);
        });
    }

    /**
     * PUT Nodejs API endpoint for criteria driven budgets; returns updates & returns a criteria driven budget
     * @param req
     * @param res
     */
    function put(req, res) {
        CriteriaDrivenBudgets.findByIdAndUpdate({scenarioId: req.body.scenarioId}, req.body, {new: true}, (err, criteria) => {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(criteria);
        });
    }

    return { post, getById, put};
}

module.exports = criteriaDrivenBudgetsController;