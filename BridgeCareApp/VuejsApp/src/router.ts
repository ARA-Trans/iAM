import Vue from 'vue'
import Router from 'vue-router'

import Todo from './components/todo.vue'
import HelloWorld from './components/HelloWorld.vue';
import ReportData from './components/ReportData.vue'
import DetailedReport from '@/components/DetailedReport.vue';

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
            path: '/todo',
            name: 'TODO',
            component: Todo
        },
        {
            path: '/ReportData',
            name: 'ReportData',
            component: ReportData
        },
        {
            path: '/DetailedReport',
            name: 'DetailedReport',
            component: DetailedReport
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