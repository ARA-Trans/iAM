import Vue from 'vue';
import VueRouter from 'vue-router';
import './register-hooks';

import DetailedReport from '@/components/DetailedReport.vue';
import Inventory from '@/components/Inventory.vue';
import Scenario from '@/components/scenarios/Scenarios.vue';
import EditScenario from '@/components/scenarios/EditScenario.vue';
import EditAnalysis from '@/components/scenarios/EditAnalysis.vue';
import InvestmentEditor from '@/components/investment-editor/InvestmentEditor.vue';
import UnderConstruction from '@/components/UnderConstruction.vue';
import PerformanceEditor from '@/components/performance-editor/PerformanceEditor.vue';
import TreatmentEditor from '@/components/treatment-editor/TreatmentEditor.vue';

Vue.use(VueRouter);

const router = new VueRouter({
    mode: 'history',
    routes: [
        {
            path: '/Inventory/',
            name: 'Inventory',
            component: Inventory
        },
        {
            path: '/DetailedReport',
            name: 'DetailedReport',
            component: DetailedReport,
            beforeEnter: (to, from, next) => {
                if (from.name == null) {
                    next('/Inventory/');
                }
                else {
                    next();
                }
            }
        },
        {
            path: '/Scenarios/',
            name: 'Scenarios',
            component: Scenario,
        },
        {
            path: '/EditScenario/',
            name: 'EditScenario',
            component: EditScenario,
        },
        {
            path: '/EditAnalysis/',
            name: 'EditAnalysis',
            component: EditAnalysis,
        },
        {
            path: '/InvestmentEditor/',
            name: 'InvestmentEditor',
            component: InvestmentEditor,
            alias: '/InvestmentEditor/Library/',
        },
        {
            path: '/PerformanceEditor/',
            name: 'PerformanceEditor',
            component: PerformanceEditor,
            alias: '/PerformanceEditor/Library/'
        },
        {
            path: '/TreatmentEditor/',
            name: 'TreatmentEditor',
            component: TreatmentEditor,
            alias: '/TreatmentEditor/Library/'
        },
        {
            path: '/InvestmentEditor/',
            component: InvestmentEditor,
            alias: '/InvestmentEditor/FromScenario/'
        },
        {
            path: '/PerformanceEditor/',
            component: PerformanceEditor,
            alias: '/PerformanceEditor/FromScenario/'
        },
        {
            path: '/TreatmentEditor/',
            component: TreatmentEditor,
            alias: '/TreatmentEditor/FromScenario/'
        },
        {
            path: '/UnderConstruction/',
            name: 'UnderConstruction',
            component: UnderConstruction
        },
        { path: '*', redirect: '/Inventory/' }
        //{
        //    path: '/about',
        //    name: 'about',
        //    // route level code-splitting
        //    // this generates a separate chunk (about.[hash].js) for this route
        //    // which is lazy-loaded when the route is visited.
        //    component: () => import(/* webpackChunkName: "about" */ './views/About.vue')
        //}
    ]
});

export default router;
