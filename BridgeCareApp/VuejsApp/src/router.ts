import Vue from 'vue'
import Router from 'vue-router'

import DetailedReport from '@/components/DetailedReport.vue';
import Inventory from '@/components/Inventory.vue';
import Scenario from '@/components/Scenario.vue';

Vue.use(Router);

export default new Router({
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
            component: DetailedReport
        },
        {
            path: '/Scenarios',
            name: 'Scenarios',
            component: Scenario
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
})