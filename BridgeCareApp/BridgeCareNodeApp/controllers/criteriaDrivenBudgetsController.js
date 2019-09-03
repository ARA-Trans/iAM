const debug = require('debug')('app:criteriaDrivenBudget');

function criteriaDrivenBudgetsController(CriteriaDrivenBudgets) {
    /**
     * POST Nodejs API endpoint for criteria driven budgets;
     * @param req Http request
     * @param res Http response
     */
    function post(req, res) {


        const criteriaDrivenBudgets = new CriteriaDrivenBudgets(req.body);

        criteriaDrivenBudgets.save(function (err, criteria) {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(criteria);
        });
    }

    /**
     *
     * @param req Http request
     * @param res Http response
     */
    function getById(req, res) {
        CriteriaDrivenBudgets.findById({_id: req.params.id}, (err, criteria) => {
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
        CriteriaDrivenBudgets.findByIdAndUpdate({_id: req.params.id}, req.body, {upsert: true, new: true}, (err, criteria) => {
            if (err) {
                return res.status(400).json(err);
            }

            return res.status(200).json(criteria);
        });
    }

    return { post, getById, put};
}

module.exports = criteriaDrivenBudgetsController;