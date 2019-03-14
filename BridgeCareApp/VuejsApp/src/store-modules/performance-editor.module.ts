import {emptyPerformanceEquation, PerformanceEquation, PerformanceStrategy} from '@/shared/models/iAM/performance';
import {clone} from 'ramda';
import PerformanceEditorService from '@/services/performance-editor.service';

const state = {
    performanceStrategies: [] as PerformanceStrategy[],
    performanceEquation: {...emptyPerformanceEquation} as PerformanceEquation
};

const mutations = {
    performanceStrategiesMutator(state: any, performanceStrategies: PerformanceStrategy[]) {
        state.performanceStrategies = clone(performanceStrategies);
    },
    performanceEquationMutator(state: any, performanceEquation: PerformanceEquation) {
        state.performanceEquation = performanceEquation;
    }
};

const actions = {
    async getPerformanceStrategies({commit}: any) {
        await new PerformanceEditorService().getPerformanceStrategies()
            .then((performanceStrategies: PerformanceStrategy[]) =>
                commit('performanceStrategiesMutator', performanceStrategies)
            )
            .catch((error: any) => console.log(error));
    },
    setPerformanceEquation({commit}: any, payload: any) {
        commit('performanceEquationMutator', payload.performanceEquation);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
