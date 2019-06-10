import Vue from 'vue';
import VueRouter from 'vue-router';
import './register-hooks';
import Inventory from '@/components/Inventory.vue';
import Scenario from '@/components/scenarios/Scenarios.vue';
import EditScenario from '@/components/scenarios/EditScenario.vue';
import EditAnalysis from '@/components/scenarios/EditAnalysis.vue';
import InvestmentEditor from '@/components/investment-editor/InvestmentEditor.vue';
import UnderConstruction from '@/components/UnderConstruction.vue';
import PerformanceEditor from '@/components/performance-editor/PerformanceEditor.vue';
import TreatmentEditor from '@/components/treatment-editor/TreatmentEditor.vue';
import PrioritiesTargetsDeficients from '@/components/priorities-targets-deficients/PrioritiesTargetsDeficients.vue';

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
            props: true,
            alias: '/InvestmentEditor/Library/',
        },
        {
            path: '/PerformanceEditor/',
            name: 'PerformanceEditor',
            component: PerformanceEditor,
            props: true,
            alias: '/PerformanceEditor/Library/'
        },
        {
            path: '/TreatmentEditor/',
            name: 'TreatmentEditor',
            component: TreatmentEditor,
            props: true,
            alias: '/TreatmentEditor/Library/',
        },
        {
            path: '/InvestmentEditor/',
            component: InvestmentEditor,
            props: true,
            alias: '/InvestmentEditor/FromScenario/'
        },
        {
            path: '/PerformanceEditor/',
            component: PerformanceEditor,
            props: true,
            alias: '/PerformanceEditor/FromScenario/'
        },
        {
            path: '/TreatmentEditor/',
            component: TreatmentEditor,
            props: true,
            alias: '/TreatmentEditor/FromScenario/'
        },
        {
            path: '/PrioritiesTargetsDeficients/',
            component: PrioritiesTargetsDeficients,
            props: true
        },
        {
            path: '/UnderConstruction/',
            name: 'UnderConstruction',
            component: UnderConstruction
        },
        {
            path: '*',
            redirect: '/Inventory/'
        }
    ]
});

export default router;
