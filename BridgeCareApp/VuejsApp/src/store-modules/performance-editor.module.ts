import {
    DeletedPerformanceStrategyEquations, emptyPerformanceStrategy,
    PerformanceStrategy,
    PerformanceStrategyEquation
} from '@/shared/models/iAM/performance';
import {clone, merge, append, sortBy, prop, contains, last, any, propEq} from 'ramda';
import PerformanceEditorService from '@/services/performance-editor.service';
import {hasValue} from '@/shared/utils/has-value';

const state = {
    performanceStrategies: [] as PerformanceStrategy[],
    selectedPerformanceStrategy: {...emptyPerformanceStrategy} as PerformanceStrategy
};

const mutations = {
    performanceStrategiesMutator(state: any, performanceStrategies: PerformanceStrategy[]) {
        state.performanceStrategies = clone(performanceStrategies);
    },
    selectedPerformanceStrategyMutator(state: any, performanceStrategyId: number) {
        if (any(propEq('id', performanceStrategyId), state.performanceStrategies)) {
            state.selectedPerformanceStrategy = clone(state.performanceStrategies
                .find((performanceStrategy: PerformanceStrategy) =>
                    performanceStrategy.id === performanceStrategyId
                ) as PerformanceStrategy);
        } else {
            state.selectedPerformanceStrategy = {...emptyPerformanceStrategy};
        }
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
                return {
                    ...updatedPerformanceStrategy,
                    performanceStrategyEquations: performanceStrategy.performanceStrategyEquations
                } as PerformanceStrategy;
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
    updatedEquationsMutator(state: any, updatedEquations: PerformanceStrategyEquation[]) {
        state.performanceStrategies = state.performanceStrategies.map((performanceStrategy: PerformanceStrategy) => {
            const performanceStrategyId = updatedEquations[0].performanceStrategyId;
            if (performanceStrategy.id === performanceStrategyId) {
                performanceStrategy.performanceStrategyEquations = performanceStrategy.performanceStrategyEquations
                    .map((equation: PerformanceStrategyEquation) => {
                        if (any(propEq('performanceStrategyEquationId', equation.performanceStrategyEquationId), updatedEquations)) {
                            const updatedEquation = updatedEquations
                                .find((e: PerformanceStrategyEquation) =>
                                    e.performanceStrategyEquationId === equation.performanceStrategyEquationId
                                ) as PerformanceStrategyEquation;
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
    selectPerformanceStrategy({commit}: any, payload: any) {
        commit('selectedPerformanceStrategyMutator', payload.performanceStrategyId);
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
                commit('selectedPerformanceStrategyMutator', updatedPerformanceStrategy.id);
            })
            .catch((error: any) => console.log(error));
    },
    async createEquation({commit}: any, payload: any) {
        await new PerformanceEditorService().createEquation(payload.createdEquation)
            .then((createdEquation: PerformanceStrategyEquation) => {
                commit('createdEquationMutator', createdEquation);
                commit('selectedPerformanceStrategyMutator', createdEquation.performanceStrategyId);
            })
            .catch((error: any) => console.log(error));
    },
    async updateEquations({commit}: any, payload: any) {
        await new PerformanceEditorService().updateEquations(payload.updatedEquations)
            .then((updatedEquations: PerformanceStrategyEquation[]) => {
                commit('updatedEquationsMutator', updatedEquations);
                commit('selectedPerformanceStrategyMutator', updatedEquations[0].performanceStrategyId);
            })
            .catch((error: any) => console.log(error));
    },
    async deleteEquations({commit}: any, payload: any) {
        await new PerformanceEditorService().deleteEquations(payload.deletedEquations)
            .then((deletedEquations: DeletedPerformanceStrategyEquations) => {
                commit('deletedEquationsMutator', deletedEquations);
                commit('selectedPerformanceStrategyMutator', deletedEquations.performanceStrategyId);
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
