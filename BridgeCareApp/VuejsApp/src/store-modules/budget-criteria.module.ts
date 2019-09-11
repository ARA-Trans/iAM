import { AxiosResponse } from 'axios';
import { CriteriaDrivenBudgets } from '../shared/models/iAM/criteria-driven-budgets';
import { clone } from 'ramda';
import BudgetCriteriaService from '../services/budget-criteria.service';

const state = {
    budgetCriteria: [] as CriteriaDrivenBudgets[],
    intermittentBudgetCriteria: [] as CriteriaDrivenBudgets[]
};

const mutations = {
    budgetCriteriaMutator(state: any, budgetCriteria: CriteriaDrivenBudgets[]) {
        state.budgetCriteria = clone(budgetCriteria);
    },
    intermittentBudgetCriteriaMutator(state: any, budgetCriteria: CriteriaDrivenBudgets[]) {
        state.intermittentBudgetCriteria = clone(budgetCriteria);
    }
};

const actions = {
    async getBudgetCriteria({ commit }: any, payload: any) {

        await BudgetCriteriaService.getBudgetCriteria(payload.selectedScenarioId)
            .then((response: AxiosResponse<CriteriaDrivenBudgets[]>) => {
                commit('budgetCriteriaMutator', response.data);
                commit('intermittentBudgetCriteriaMutator', response.data);
            });
    },
    async saveIntermittentCriteriaDrivenBudget({ commit }: any, payload: any) {
        commit('intermittentBudgetCriteriaMutator', payload.updateIntermittentCriteriaDrivenBudget);
    },
    async saveBudgetCriteria({ dispatch, commit }: any, payload: any) {
        await BudgetCriteriaService.saveBudgetCriteria(payload.selectedScenarioId, payload.budgetCriteriaData)
            .then((response: AxiosResponse<CriteriaDrivenBudgets[]>) => {
                commit('budgetCriteriaMutator', response.data);
                dispatch('setSuccessMessage', { message: 'BudgetCriteria data successfully saved' });
            });
    },
    async saveIntermittentStateToBudgetCriteria({ commit }: any, payload: any) {
        commit('budgetCriteriaMutator', payload.intermittentState);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};