import {PerformanceStrategy} from '@/shared/models/iAM/performance';
import {clone, any, propEq, merge, append} from 'ramda';
import PerformanceEditorService from '@/services/performance-editor.service';

const state = {
    performanceStrategies: [] as PerformanceStrategy[]
};

const mutations = {
    performanceStrategiesMutator(state: any, performanceStrategies: PerformanceStrategy[]) {
        state.performanceStrategies = clone(performanceStrategies);
    },
    savedPerformanceStrategyMutator(state: any, savedPerformanceStrategy: PerformanceStrategy) {
        if (any(propEq('simulationId', savedPerformanceStrategy.simulationId), state.performanceStrategies)) {
            state.performanceStrategies = state.performanceStrategies
                .map((existingPerformanceStrategy: PerformanceStrategy) => {
                    if (existingPerformanceStrategy.simulationId === savedPerformanceStrategy.simulationId) {
                        return merge(existingPerformanceStrategy, savedPerformanceStrategy);
                    }
                    return existingPerformanceStrategy;
                });
        } else {
            state.performanceStrategies = append(savedPerformanceStrategy, state.performanceStrategies);
        }
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
    async savePerformanceStrategyToLibrary({commit}: any, payload: any) {
        await new PerformanceEditorService().savePerformanceStrategyToLibrary(payload.savedPerformanceStrategy)
            .then((performanceStrategy: PerformanceStrategy) =>
                commit('savedPerformanceStrategyMutator', performanceStrategy)
            )
            .catch((error: any) => console.log(error));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
