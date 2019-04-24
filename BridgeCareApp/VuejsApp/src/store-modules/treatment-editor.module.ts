import {emptyTreatmentStrategy, TreatmentStrategy} from '@/shared/models/iAM/treatment';
import {any, propEq, findIndex} from 'ramda';
import TreatmentEditorService from '@/services/treatment-editor.service';
import {hasValue} from '@/shared/utils/has-value';

const state = {
    treatmentStrategies: [] as TreatmentStrategy[],
    selectedTreatmentStrategy: {...emptyTreatmentStrategy} as TreatmentStrategy
};

const mutations = {
    treatmentStrategiesMutator(state: any, treatmentStrategies: TreatmentStrategy[]) {
        state.treatmentStrategies = [...treatmentStrategies];
    },
    selectedTreatmentStrategyMutator(state: any, treatmentStrategyId: number) {
        if (any(propEq('id', treatmentStrategyId), state.treatmentStrategies)) {
            // find the existing treatment strategy in state.treatmentStrategies where the id matches treatmentStrategyId,
            // copy it, then update state.selectedTreatmentStrategy with the copy
            state.selectedTreatmentStrategy = {...state.treatmentStrategies
                .find((treatmentStrategy: TreatmentStrategy) =>
                    treatmentStrategy.id === treatmentStrategyId
                ) as TreatmentStrategy};
        } else if (treatmentStrategyId === null) {
            // update state.selectedTreatmentStrategy with an empty treatment strategy object
            state.selectedTreatmentStrategy = {...emptyTreatmentStrategy};
        }
    },
    updatedSelectedTreatmentStrategyMutator(state: any, updatedSelectedTreatmentStrategy: TreatmentStrategy) {
        // update state.selectedTreatmentStrategy with the updated selected treatment strategy
        state.selectedTreatmentStrategy = {...updatedSelectedTreatmentStrategy};
    },
    createdTreatmentStrategyMutator(state: any, createdTreatmentStrategy: TreatmentStrategy) {
        // append the created treatment strategy to a copy of state.treatmentStrategies, then update state.treatmentStrategies
        // with the copy
        state.treatmentStrategies = [...state.treatmentStrategies, createdTreatmentStrategy];
    },
    updatedTreatmentStrategyMutator(state: any, updatedTreatmentStrategy: TreatmentStrategy) {
        if (any(propEq('id', updatedTreatmentStrategy.id), state.treatmentStrategies)) {
            // make a copy of state.treatmentStrategies
            const treatmentStrategies: TreatmentStrategy[] = [...state.treatmentStrategies];
            // find the index of the existing treatment strategy in the copy that has a matching id with the updated
            // treatment strategy
            const index: number = findIndex(propEq('id', updatedTreatmentStrategy.id), treatmentStrategies);
            // update the copy at the specified index with the updated treatment strategy
            treatmentStrategies[index] = updatedTreatmentStrategy;
            // update state.treatmentStrategies with the copy
            state.treatmentStrategies = treatmentStrategies;
        }
    }
};

const actions = {
    async getTreatmentStrategies({commit}: any) {
        await new TreatmentEditorService().getTreatmentStrategies()
            .then((treatmentStrategies: TreatmentStrategy[]) =>
                commit('treatmentStrategiesMutator', treatmentStrategies)
            )
            .catch((error: any) => console.log(error));
    },
    selectTreatmentStrategy({commit}: any, payload: any) {
        commit('selectedTreatmentStrategyMutator', payload.treatmentStrategyId);
    },
    updateSelectedTreatmentStrategy({commit}: any, payload: any) {
        commit('updatedSelectedTreatmentStrategyMutator', payload.updatedSelectedTreatmentStrategy);
    },
    async createTreatmentStrategy({commit}: any, payload: any) {
        await new TreatmentEditorService().createTreatmentStrategy(payload.createdTreatmentStrategy)
            .then((createdTreatmentStrategy: TreatmentStrategy) => {
                commit('createdTreatmentStrategyMutator', createdTreatmentStrategy);
                commit('selectedTreatmentStrategyMutator', createdTreatmentStrategy.id);
            })
            .catch((error: any) => console.log(error));
    },
    async updateTreatmentStrategy({commit}: any, payload: any) {
        await new TreatmentEditorService().updateTreatmentStrategy(payload.updatedTreatmentStrategy)
            .then((updatedTreatmentStrategy: TreatmentStrategy) => {
                commit('updatedTreatmentStrategyMutator', updatedTreatmentStrategy);
                commit('selectedTreatmentStrategyMutator', updatedTreatmentStrategy.id);
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
