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
                    path: '/Prioritization/',
                    component: PrioritiesTargetsDeficients,
                    props: true
                },
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
