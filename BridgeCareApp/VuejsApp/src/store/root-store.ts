import Vue from 'vue';
import Vuex from 'vuex';

import busy from '@/store-modules/busy.module';
import authentication from '@/store-modules/authentication.module';
import network from '../store-modules/network.module';
import scenario from '../store-modules/scenario.module';
import inventory from '@/store-modules/inventory.module';
import investmentEditor from '@/store-modules/investment-editor.module';
import performanceEditor from '@/store-modules/performance-editor.module';
import reports from '@/store-modules/reports.module';
import breadcrumb from '@/store-modules/breadcrumb.module';
import treatmentEditor from '@/store-modules/treatment-editor.module';
import attribute from '@/store-modules/attribute.module';
import toastr from '@/store-modules/toastr.module';

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        busy,
        authentication,
        network,
        reports,
        scenario,
        inventory,
        investmentEditor,
        performanceEditor,
        attribute,
        breadcrumb,
        treatmentEditor,
        toastr
    }
});
