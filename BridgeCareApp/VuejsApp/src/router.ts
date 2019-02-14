import Vue from 'vue'
import Router from 'vue-router'

import HelloWorld from './components/HelloWorld.vue';
import DetailedReport from '@/components/DetailedReport.vue';
import Scenario from '@/components/Scenario.vue';

Vue.use(Router);

export default new Router({
    mode: 'history',
    routes: [
        {
            path: '/',
            name: 'Home',
            component: HelloWorld
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
        { path: '*', redirect: '/' }
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