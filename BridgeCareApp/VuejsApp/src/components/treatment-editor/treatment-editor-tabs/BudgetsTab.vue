<template>
    <v-container fluid grid-list-xl>
        <v-layout class="budgets-tab-content">
            <v-flex xs12>
                <v-layout justify-center>
                    <v-flex xs6>
                        <v-layout v-if="budgets.length === 0" column fill-height>
                            <h3>Investment Library Not Found</h3>
                            <div>
                                No investment library data was found for the selected scenario.
                            </div>
                            <div>
                                To add investment library data, go to the scenario's investment editor.
                            </div>
                        </v-layout>
                        <v-layout v-else fill-height>
                            <v-data-table :items="budgets" :headers="budgetHeaders" v-model="selectedBudgets" select-all
                                          item-key="budget" hide-actions
                                          class="elevation-1 fixed-header v-table__overflow budgets-data-table">
                                <template slot="items" slot-scope="props">
                                    <td>
                                        <v-checkbox v-model="props.selected" primary hide-details v-on:change="submitChanges">
                                        </v-checkbox>
                                    </td>
                                    <td>
                                        {{props.item.budget}}
                                    </td>
                                </template>
                            </v-data-table>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {clone, isEmpty, findIndex} from 'ramda';
    import {TabData} from '@/shared/models/child-components/treatment-editor/tab-data';
    import {
        BudgetGridRow,
        emptyTreatment,
        emptyTreatmentLibrary,
        Treatment,
        TreatmentLibrary
    } from "@/shared/models/iAM/treatment";
    import {InvestmentStrategy} from '@/shared/models/iAM/investment';
    import {sorter} from '@/shared/utils/sorter';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {DataTableHeader} from "@/shared/models/vue/data-table-header";

    @Component
    export default class BudgetsTab extends Vue {
        @Prop() budgetsTabData: TabData;

        @State(state => state.investmentEditor.investmentForScenario) scenarioInvestmentLibrary: InvestmentStrategy[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getInvestmentForScenario') getScenarioInvestmentLibraryAction: any;

        budgetsTabSelectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
        budgetsTabSelectedTreatment: Treatment = clone(emptyTreatment);
        budgetHeaders: DataTableHeader[] = [
            {text: 'Budget', value: 'budget', align: 'left', sortable: true, class: '', width: '300px'}
        ];
        budgets: BudgetGridRow[] = [];
        selectedBudgets: BudgetGridRow[] = [];

        @Watch('budgetsTabData')
        onBudgetsTabDataChanged() {
            this.budgetsTabSelectedTreatmentLibrary = this.budgetsTabData.tabSelectedTreatmentLibrary;
            this.budgetsTabSelectedTreatment = this.budgetsTabData.tabSelectedTreatment;
            this.selectedBudgets = this.budgetsTabSelectedTreatment.budgets.map((name: string) => ({budget: name}));
        }

        @Watch('scenarioInvestmentLibrary')
        onScenarioInvestmentLibraryChanged() {
            if (!isEmpty(this.scenarioInvestmentLibrary[0].budgetOrder)) {
                this.budgets = this.scenarioInvestmentLibrary[0].budgetOrder.map((name: string) => ({budget: name}));
            } else if (!isEmpty(this.scenarioInvestmentLibrary[0].budgetYears)) {
                this.budgets = (sorter(
                    getPropertyValues('budgetName', this.scenarioInvestmentLibrary[0].budgetYears)
                ) as string[]).map((name: string) => ({budget: name}));
            } else {
                this.budgets = [];
            }
        }

        /**
         * Modifies the selected treatment & selected treatment library with budget selection changes and emits the
         * modified objects to the parent component
         */
        submitChanges() {
            this.budgetsTabSelectedTreatment.budgets = getPropertyValues('budget', this.selectedBudgets) as string[];

            const index = findIndex((treatment: Treatment) =>
                treatment.id === this.budgetsTabSelectedTreatment.id,
                this.budgetsTabSelectedTreatmentLibrary.treatments
            );
            this.budgetsTabSelectedTreatmentLibrary.treatments[index] = this.budgetsTabSelectedTreatment;

            this.$emit('submit', this.budgetsTabSelectedTreatmentLibrary);
        }
    }
</script>

<style>
    .budgets-data-table {
        height: 245px !important;
        overflow-y: auto;
    }
</style>