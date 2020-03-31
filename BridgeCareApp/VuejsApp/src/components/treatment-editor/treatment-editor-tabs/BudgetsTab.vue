<template>
    <v-container fluid grid-list-xl>
        <v-layout class="budgets-tab-content">
            <v-flex xs12>
                <v-layout justify-center>
                    <v-flex xs6>
                        <v-layout column v-if="budgets.length === 0">
                            <h3>Investment Library Not Found</h3>
                            <div>
                                No investment library data was found for the selected scenario.
                            </div>
                            <div>
                                To add investment library data, go to the scenario's investment editor.
                            </div>
                        </v-layout>
                        <v-layout v-else>
                            <v-data-table :headers="budgetHeaders" :items="budgets" class="elevation-1 fixed-header v-table__overflow budgets-data-table" hide-actions
                                          item-key="budget" select-all
                                          v-model="selectedBudgets">
                                <template slot="items" slot-scope="props">
                                    <td>
                                        <v-checkbox hide-details primary v-model="props.selected">
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
    import {clone, findIndex, equals, propEq, update} from 'ramda';
    import {TabData} from '@/shared/models/child-components/tab-data';
    import {
        BudgetGridRow,
        emptyTreatment,
        emptyTreatmentLibrary,
        Treatment,
        TreatmentLibrary
    } from '@/shared/models/iAM/treatment';
    import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';
    import {sorter} from '@/shared/utils/sorter-utils';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {hasValue} from '@/shared/utils/has-value-util';

    @Component
    export default class BudgetsTab extends Vue {
        @Prop() budgetsTabData: TabData;

        budgetsTabSelectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
        budgetsTabSelectedTreatment: Treatment = clone(emptyTreatment);
        budgetsTabScenarioInvestmentLibrary: InvestmentLibrary = clone(emptyInvestmentLibrary);
        budgetHeaders: DataTableHeader[] = [
            {text: 'Budget', value: 'budget', align: 'left', sortable: true, class: '', width: '300px'}
        ];
        budgets: BudgetGridRow[] = [];
        selectedBudgets: BudgetGridRow[] = [];
        selectedBudgetsTimeout: any | null = null;

        @Watch('budgetsTabData')
        onBudgetsTabDataChanged() {
            this.budgetsTabScenarioInvestmentLibrary = this.budgetsTabData.tabScenarioInvestmentLibrary;
            this.budgetsTabSelectedTreatmentLibrary = this.budgetsTabData.tabSelectedTreatmentLibrary;
            this.budgetsTabSelectedTreatment = this.budgetsTabData.tabSelectedTreatment;
        }

        @Watch('budgetsTabScenarioInvestmentLibrary')
        onScenarioInvestmentLibraryChanged() {
            if (hasValue(this.budgetsTabScenarioInvestmentLibrary.budgetOrder)) {
                this.budgets = this.budgetsTabScenarioInvestmentLibrary.budgetOrder
                    .map((name: string) => ({budget: name}));
            } else if (hasValue(this.budgetsTabScenarioInvestmentLibrary.budgetYears)) {
                this.budgets = (sorter(
                    getPropertyValues('budgetName', this.budgetsTabScenarioInvestmentLibrary.budgetYears)
                ) as string[]).map((name: string) => ({budget: name}));
            } else {
                this.budgets = [];
            }
        }

        @Watch('budgetsTabSelectedTreatment')
        onBudgetsTabSelectedTreatmentChanged() {
            this.selectedBudgets = hasValue(this.budgetsTabSelectedTreatment.budgets)
                ? this.budgetsTabSelectedTreatment.budgets.map((name: string) => ({budget: name}))
                : [];
        }

        @Watch('selectedBudgets')
        onSelectedBudgetsChanged() {
            const selectedBudgets: string[] = this.selectedBudgets.map((budgetGridRow: BudgetGridRow) => budgetGridRow.budget);

            if (!equals(this.budgetsTabSelectedTreatment.budgets, selectedBudgets)) {
                this.budgetsTabSelectedTreatmentLibrary = {
                    ...this.budgetsTabSelectedTreatmentLibrary,
                    treatments: update(
                        findIndex(propEq('id', this.budgetsTabSelectedTreatment), this.budgetsTabSelectedTreatmentLibrary.treatments),
                        {
                            ...this.budgetsTabSelectedTreatment,
                            budgets: getPropertyValues('budget', this.selectedBudgets)
                        },
                        this.budgetsTabSelectedTreatmentLibrary.treatments
                    )
                };

                this.$emit('submit', this.budgetsTabSelectedTreatmentLibrary);
            }
        }
    }
</script>

<style>
    .budgets-data-table {
        height: 245px !important;
        overflow-y: auto;
    }
</style>
