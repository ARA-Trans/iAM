import Vue from 'vue';
import VueRouter from 'vue-router';
import Component from 'vue-class-component';
import './register-hooks';

import DetailedReport from '@/components/DetailedReport.vue';
import Inventory from '@/components/Inventory.vue';
import Scenario from '@/components/Scenarios.vue';
import EditScenario from '@/components/scenario/EditScenario.vue';
import EditAnalysis from '@/components/scenario/EditAnalysis.vue';
import Criteria from '@/components/Criteria.vue';
import InvestmentEditor from '@/components/investment-editor/InvestmentEditor.vue';
import PerformanceEditor from '@/components/performance-editor/PerformanceEditor.vue';

Vue.use(VueRouter);

const router = new VueRouter({
    mode: 'history',
    routes: [
        {
            path: '/Inventory',
            name: 'Inventory',
            component: Inventory
        },
        {
            path: '/DetailedReport',
            name: 'DetailedReport',
            component: DetailedReport,
            beforeEnter: (to, from, next) => {
                if (from.name == null) {
                    next('/Inventory');
                }
                else {
                    next();
                }
            }
        },
        {
            path: '/Scenarios',
            name: 'Scenarios',
            component: Scenario
        },
        {
            path: '/InvestmentEditor',
            name: 'InvestmentEditor',
            component: InvestmentEditor
        },
        {
            path: '/PerformanceEditor',
            name: 'PerformanceEditor',
            component: PerformanceEditor
        },
        {
            path: '/EditScenario',
            name: 'EditScenario',
            component: EditScenario
        },
        {
            path: '/EditAnalysis',
            name: 'EditAnalysis',
            component: EditAnalysis
        },
        {
            path: '/Criteria',
            name: 'Criteria',
            component: Criteria
        },
        { path: '*', redirect: '/Inventory' }
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
