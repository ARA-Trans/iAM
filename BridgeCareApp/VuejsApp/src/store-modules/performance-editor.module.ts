import {
    DeletedPerformanceStrategyEquations,
    PerformanceStrategy,
    PerformanceStrategyEquation
} from '@/shared/models/iAM/performance';
import {clone, merge, append, sortBy, prop, contains, last} from 'ramda';
import PerformanceEditorService from '@/services/performance-editor.service';
import {hasValue} from '@/shared/utils/has-value';

const state = {
    performanceStrategies: [] as PerformanceStrategy[]
};

const mutations = {
    performanceStrategiesMutator(state: any, performanceStrategies: PerformanceStrategy[]) {
        state.performanceStrategies = clone(performanceStrategies);
    },
    createdPerformanceStrategyMutator(state: any, createdPerformanceStrategy: PerformanceStrategy) {
        // TODO: remove code to add a mock id to a created performance strategy when services become available
        const sortById = sortBy(prop('id'));
        const sortedPerformanceStrategies: PerformanceStrategy[] = sortById(state.performanceStrategies);
        const lastPerformanceStrategyInList: PerformanceStrategy = last(sortedPerformanceStrategies) as PerformanceStrategy;
        const newPerformanceStrategyId = hasValue(lastPerformanceStrategyInList) ? lastPerformanceStrategyInList.id + 1 : 1;
        createdPerformanceStrategy.id = newPerformanceStrategyId;
        // end of TODO
        state.performanceStrategies = append(createdPerformanceStrategy, sortedPerformanceStrategies);
    },
    updatedPerformanceStrategyMutator(state: any, updatedPerformanceStrategy: PerformanceStrategy) {
        state.performanceStrategies = state.performanceStrategies.map((performanceStrategy: PerformanceStrategy) => {
            if (performanceStrategy.id === updatedPerformanceStrategy.id) {
                return merge(performanceStrategy, updatedPerformanceStrategy);
            }
            return performanceStrategy;
        });
    },
    createdEquationMutator(state: any, createdEquation: PerformanceStrategyEquation) {
        state.performanceStrategies = state.performanceStrategies.map((performanceStrategy: PerformanceStrategy) => {
           if (performanceStrategy.id === createdEquation.performanceStrategyId) {
               // TODO: replace with a simple append to the performance strategy's equations list when web services available
               const sortById = sortBy(prop('performanceStrategyEquationId'));
               const sortedEquations: PerformanceStrategyEquation[] = sortById(performanceStrategy.performanceStrategyEquations);
               const lastEquationInList: PerformanceStrategyEquation = last(sortedEquations) as PerformanceStrategyEquation;
               const newEquationId = hasValue(lastEquationInList) ? lastEquationInList.performanceStrategyEquationId + 1 : 1;
               createdEquation.performanceStrategyEquationId = newEquationId;
               performanceStrategy.performanceStrategyEquations = append(createdEquation, sortedEquations);
           }
           return performanceStrategy;
        });
    },
    updatedEquationMutator(state: any, updatedEquation: PerformanceStrategyEquation) {
        state.performanceStrategies = state.performanceStrategies.map((performanceStrategy: PerformanceStrategy) => {
            if (performanceStrategy.id === updatedEquation.performanceStrategyId) {
                performanceStrategy.performanceStrategyEquations = performanceStrategy.performanceStrategyEquations
                    .map((equation: PerformanceStrategyEquation) => {
                        if (equation.performanceStrategyEquationId === updatedEquation.performanceStrategyEquationId) {
                            return merge(equation, updatedEquation);
                        }
                        return equation;
                    });
            }
            return performanceStrategy;
        });
    },
    deletedEquationsMutator(state: any, deletedEquations: DeletedPerformanceStrategyEquations) {
        state.performanceStrategies = state.performanceStrategies.map((performanceStrategy: PerformanceStrategy) => {
            if (performanceStrategy.id === deletedEquations.performanceStrategyId) {
                performanceStrategy.performanceStrategyEquations = performanceStrategy.performanceStrategyEquations
                    .filter((equation: PerformanceStrategyEquation) =>
                        !contains(equation.performanceStrategyEquationId, deletedEquations.deletedEquationIds)
                    );
            }
            return performanceStrategy;
        });
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
    async createPerformanceStrategy({commit}: any, payload: any) {
        await new PerformanceEditorService().createPerformanceStrategy(payload.createdPerformanceStrategy)
            .then((createdPerformanceStrategy: PerformanceStrategy) =>
                commit('createdPerformanceStrategyMutator', createdPerformanceStrategy)
            )
            .catch((error: any) => console.log(error));
    },
    async updatePerformanceStrategy({commit}: any, payload: any) {
        await new PerformanceEditorService().updatePerformanceStrategy(payload.updatedPerformanceStrategy)
            .then((updatedPerformanceStrategy: PerformanceStrategy) =>
                commit('updatedPerformanceStrategyMutator', updatedPerformanceStrategy)
            )
            .catch((error: any) => console.log(error));
    },
    async createEquation({commit}: any, payload: any) {
        await new PerformanceEditorService().createEquation(payload.createdEquation)
            .then((createdEquation: PerformanceStrategyEquation) =>
                commit('createdEquationMutator', createdEquation)
            )
            .catch((error: any) => console.log(error));
    },
    async updateEquation({commit}: any, payload: any) {
        await new PerformanceEditorService().updateEquation(payload.updatedEquation)
            .then((updatedEquation: PerformanceStrategyEquation) =>
                commit('updatedEquationMutator', updatedEquation)
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
