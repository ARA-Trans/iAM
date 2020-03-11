import Vue from 'vue';
import Vuex from 'vuex';

import busy from '@/store-modules/busy.module';
import authentication from '@/store-modules/authentication.module';
import network from '../store-modules/network.module';
import scenario from '../store-modules/scenario.module';
import inventory from '@/store-modules/inventory.module';
import investmentEditor from '@/store-modules/investment-editor.module';
import performanceEditor from '@/store-modules/performance-editor.module';
import treatmentEditor from '@/store-modules/treatment-editor.module';
import attribute from '@/store-modules/attribute.module';
import toastr from '@/store-modules/toastr.module';
import deficientEditor from '@/store-modules/deficient.module';
import priorityEditor from '@/store-modules/priority.module';
import targetEditor from '@/store-modules/target.module';
import remainingLifeLimitEditor from '@/store-modules/remaining-life-limit.module';
import criteriaDrivenBudgets from '@/store-modules/budget-criteria.module';
import rollup from '../store-modules/rollup.module';
import polling from '@/store-modules/polling.module';
import announcement from '@/store-modules/announcement.module';
import cashFlowEditor from '@/store-modules/cash-flow.module';
import userCriteria from '@/store-modules/user-criteria.module';

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        busy,
        authentication,
        network,
        scenario,
        inventory,
        investmentEditor,
        performanceEditor,
        attribute,
        treatmentEditor,
        toastr,
        deficientEditor,
        priorityEditor,
        targetEditor,
        remainingLifeLimitEditor,
        criteriaDrivenBudgets,
        rollup,
        polling,
        announcement,
        cashFlowEditor,
        userCriteria
    }
});
