const express = require('express');
const pollingController = require('../controllers/pollingController');

function pollingRouter() {
    const router = express.Router();
    const controller = pollingController();

    router.route('/Polling/:browserId').get(controller.get);

    return {router, emit: controller.emit};
}

module.exports = pollingRouter;