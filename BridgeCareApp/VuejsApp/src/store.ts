import Vue from 'vue'
import Vuex from 'vuex'

import busy from '@/store-modules/busy.module';
import security from '@/store-modules/security.module';
import network from '@/store-modules/network.module';
import simulation from '@/store-modules/simulation.module';
import scenario from '@/store-modules/scenario.module';
import detailedReport from '@/store-modules/detailed-report.module';
import criteriaEditor from '@/store-modules/criteria-editor.module';
import inventory from '@/store-modules/inventory.module';

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        busy,
        security,
        network,
        simulation,
        detailedReport,
        scenario,
        criteriaEditor,
        inventory
    }
});
