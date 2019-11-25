import Vue from 'vue';
import VueRouter from 'vue-router';
import './register-hooks';
import Inventory from '@/components/Inventory.vue';
import EditAnalysis from '@/components/scenarios/EditAnalysis.vue';
import UnderConstruction from '@/components/UnderConstruction.vue';
import RemainingLifeLimitEditor from '@/components/remaining-life-limit-editor/RemainingLifeLimitEditor.vue';

import store from '@/store/root-store';

const Scenario = () => import(/* webpackChunkName: "scenario" */ '@/components/scenarios/Scenarios.vue');
const EditScenario = () => import(/* webpackChunkName: "editScenario" */ '@/components/scenarios/EditScenario.vue');
const InvestmentEditor = () => import(/* webpackChunkName: "investmentEditor" */ '@/components/investment-editor/InvestmentEditor.vue');
const PerformanceEditor = () => import(/* webpackChunkName: "performanceEditor" */ '@/components/performance-editor/PerformanceEditor.vue');
const TreatmentEditor = () => import(/* webpackChunkName: "treatmentEditor" */ '@/components/treatment-editor/TreatmentEditor.vue');
const PriorityEditor = () => import (/* webpackChunkName: "priorityEditor" */ '@/components/priority-editor/PriorityEditor.vue');
const TargetEditor = () => import (/* webpackChunkName: "targetEditor" */ '@/components/target-editor/TargetEditor.vue');
const DeficientEditor = () => import (/* webpackChunkName: "deficientEditor" */ '@/components/deficient-editor/DeficientEditor.vue');
const Authentication = () => import (/* webpackChunkName: "Authentication" */ '@/components/Authentication.vue');
const AuthenticationStart = () => import (/* webpackChunkName: "authenticationStart" */ '@/components/AuthenticationStart.vue');
const AuthenticationFailure = () => import (/* webpackChunkName: "authenticationFailure" */ '@/components/AuthenticationFailure.vue');

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
            path: '/Scenarios/',
            name: 'Scenarios',
            component: Scenario,
        },
        {
            path: '/EditScenario/',
            name: 'EditScenario',
            component: EditScenario,
            children: [
                {
                    path: '/EditAnalysis/',
                    name: 'EditAnalysis',
                    component: EditAnalysis,
                },
                {
                    path: '/InvestmentEditor/Scenario/',
                    component: InvestmentEditor,
                    props: true
                },
                {
                    path: '/PerformanceEditor/Scenario/',
                    component: PerformanceEditor,
                    props: true
                },
                {
                    path: '/TreatmentEditor/Scenario/',
                    component: TreatmentEditor,
                    props: true
                },
                {
                    path: '/PriorityEditor/Scenario/',
                    component: PriorityEditor,
                    props: true
                },
                {
                    path: '/TargetEditor/Scenario/',
                    component: TargetEditor,
                    props: true
                },
                {
                    path: '/DeficientEditor/Scenario/',
                    component: DeficientEditor,
                    props: true
                },
                {
                    path: '/RemainingLifeLimitEditor/Scenario/',
                    component: RemainingLifeLimitEditor,
                    props: true
                }
            ]
        },
        {
            path: '/InvestmentEditor/Library/',
            name: 'InvestmentEditor',
            component: InvestmentEditor,
            props: true
        },
        {
            path: '/PerformanceEditor/Library/',
            name: 'PerformanceEditor',
            component: PerformanceEditor,
            props: true
        },
        {
            path: '/TreatmentEditor/Library/',
            name: 'TreatmentEditor',
            component: TreatmentEditor,
            props: true
        },
        {
            path: '/PriorityEditor/Library/',
            name: 'PriorityEditor',
            component: PriorityEditor,
            props: true
        },
        {
            path: '/TargetEditor/Library/',
            name: 'TargetEditor',
            component: TargetEditor,
            props: true
        },
        {
            path: '/DeficientEditor/Library/',
            name: 'DeficientEditor',
            component: DeficientEditor,
            props: true
        },
        {
            path: '/RemainingLifeLimitEditor/Library/',
            name: 'RemainingLifeLimitEditor',
            component: RemainingLifeLimitEditor,
            props: true
        },
        {
            path: '/Authentication/',
            name: 'Authentication',
            component: Authentication
        },
        {
            path: '/AuthenticationStart/',
            name: 'AuthenticationStart',
            component: AuthenticationStart
        },
        {
            path: '/AuthenticationFailure/',
            name: 'AuthenticationFailure',
            component: AuthenticationFailure,
            meta: {
                isPublic: true
            }
        },
        {
            path: '/UnderConstruction/',
            name: 'UnderConstruction',
            component: UnderConstruction
        },
        {
            path: '*',
            redirect: '/AuthenticationStart/'
        }
    ]
});


export default router;
