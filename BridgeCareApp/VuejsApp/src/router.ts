import Vue from 'vue'
import Router from 'vue-router'

import HelloWorld from './components/HelloWorld.vue';
import DetailedReport from '@/components/DetailedReport.vue';
import EditScenario from '@/components/scenario/EditScenario.vue';
import EditAnalysis from '@/components/scenario/EditAnalysis.vue';

Vue.use(Router)

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
            path: '/EditScenario',
            name: 'EditScenario',
            component: EditScenario
        },
        {
            path: '/EditAnalysis',
            name: 'EditAnalysis',
            component: EditAnalysis
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