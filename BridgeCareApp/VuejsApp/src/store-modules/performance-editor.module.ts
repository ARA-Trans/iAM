import {emptyPerformanceStrategy, PerformanceStrategy} from '@/shared/models/iAM/performance';
import {clone, append, any, propEq, findIndex} from 'ramda';
import PerformanceEditorService from '@/services/performance-editor.service';

const state = {
    performanceStrategies: [] as PerformanceStrategy[],
    selectedPerformanceStrategy: {...emptyPerformanceStrategy} as PerformanceStrategy
};

const mutations = {
    performanceStrategiesMutator(state: any, performanceStrategies: PerformanceStrategy[]) {
        // update state.performanceStrategies with a clone of the incoming list of performance strategies
        state.performanceStrategies = clone(performanceStrategies);
    },
    selectedPerformanceStrategyMutator(state: any, performanceStrategyId: number) {
        if (any(propEq('id', performanceStrategyId), state.performanceStrategies)) {
            // find the existing performance strategy in state.performanceStrategies where the id matches performanceStrategyId,
            // clone it, then update state.selectedPerformanceStrategy with the cloned, existing performance strategy
            state.selectedPerformanceStrategy = clone(state.performanceStrategies
                .find((performanceStrategy: PerformanceStrategy) =>
                    performanceStrategy.id === performanceStrategyId
                ) as PerformanceStrategy);
        } else {
            // reset state.selectedPerformanceStrategy as an empty performance strategy
            state.selectedPerformanceStrategy = {...emptyPerformanceStrategy};
        }
    },
    updateSelectedPerformanceStrategyMutator(state: any, updatedSelectedPerformanceStrategy: PerformanceStrategy) {
        // update the state.selectedPerformanceStrategy with the updated selected performance strategy
        state.selectedPerformanceStrategy = updatedSelectedPerformanceStrategy;
    },
    createdPerformanceStrategyMutator(state: any, createdPerformanceStrategy: PerformanceStrategy) {
        // append the created performance strategy to a cloned list of state.performanceStrategies, then update
        // state.performanceStrategies with the cloned list
        state.performanceStrategies = append(createdPerformanceStrategy, state.performanceStrategies);
    },
    updatedPerformanceStrategyMutator(state: any, updatedPerformanceStrategy: PerformanceStrategy) {
        if (any(propEq('id', updatedPerformanceStrategy.id), state.performanceStrategies)) {
            // clone the list of performance strategies in state
            const performanceStrategies: PerformanceStrategy[] = clone(state.performanceStrategies);
            // find the index of the existing performance strategy in the cloned list of performance strategies that has
            // a matching id with the updated performance strategy
            const index: number = findIndex(propEq('id', updatedPerformanceStrategy.id), performanceStrategies);
            // set the updated performance strategy at the specified index
            performanceStrategies[index] = updatedPerformanceStrategy;
            // update state.performanceStrategies with the cloned list of performance strategies
            state.performanceStrategies = performanceStrategies;
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
    selectPerformanceStrategy({commit}: any, payload: any) {
        commit('selectedPerformanceStrategyMutator', payload.performanceStrategyId);
    },
    updateSelectedPerformanceStrategy({commit}: any, payload: any) {
        commit('updateSelectedPerformanceStrategyMutator', payload.updatedSelectedPerformanceStrategy);
    },
    async createPerformanceStrategy({commit}: any, payload: any) {
        await new PerformanceEditorService().createPerformanceStrategy(payload.createdPerformanceStrategy)
            .then((createdPerformanceStrategy: PerformanceStrategy) => {
                commit('createdPerformanceStrategyMutator', createdPerformanceStrategy);
                commit('selectedPerformanceStrategyMutator', createdPerformanceStrategy.id);
            })
            .catch((error: any) => console.log(error));
    },
    async updatePerformanceStrategy({commit}: any, payload: any) {
        await new PerformanceEditorService().updatePerformanceStrategy(payload.updatedPerformanceStrategy)
            .then((updatedPerformanceStrategy: PerformanceStrategy) => {
                commit('updatedPerformanceStrategyMutator', updatedPerformanceStrategy);
            })
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
