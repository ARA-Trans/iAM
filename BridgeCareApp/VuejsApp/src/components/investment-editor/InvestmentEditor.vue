<template>
    <v-container fluid grid-list-xl>
        <v-layout justify-center>
            <v-spacer></v-spacer>
            <v-flex xsl-6>
                <v-select :items="investmentStrategiesSelect"
                          label="Select an Investment Strategy"
                          item-text="text"
                          item-value="value"
                          v-on:change="onSelectInvestmentStrategy">
                </v-select>
            </v-flex>
            <v-spacer></v-spacer>
        </v-layout>
        <v-divider></v-divider>
        <v-layout column justify-center>
            <v-flex>
                <v-layout row style="width: 50%; margin: 0 auto;">
                    <v-text-field label="Inflation Rate (%)" v-model="selectedInvestmentStrategy.inflationRate" outline :mask="'##########'"
                                  style="width: 200px">
                    </v-text-field>
                    <v-spacer></v-spacer>
                    <v-text-field label="Discount Rate (%)" v-model="selectedInvestmentStrategy.discountRate" outline :mask="'##########'"
                                  style="width: 200px">
                    </v-text-field>
                </v-layout>
            </v-flex>

            <v-flex>
                <v-layout column justify-center>
                    <v-spacer></v-spacer>
                    <v-layout style="width: 50%; margin: 0 auto;">
                        <v-flex>
                            <v-btn color="info" v-on:click="onAddBudgetYear"
                                   :disabled="!onCheckItemHasValue(selectedInvestmentStrategy)">
                                Add Budget Year
                            </v-btn>
                        </v-flex>
                        <v-flex>
                            <v-btn color="info" v-on:click="onAddBudgetYearRange"
                                   :disabled="!onCheckItemHasValue(selectedInvestmentStrategy)">
                                Add Range
                            </v-btn>
                        </v-flex>
                        <v-flex>
                            <v-btn color="info" v-on:click="onShowEditBudgetsDialog"
                                   :disabled="!onCheckItemHasValue(selectedInvestmentStrategy)">
                                Edit Budgets
                            </v-btn>
                        </v-flex>
                        <v-flex>
                            <v-btn color="error" v-on:click="onDeleteBudgetYears"
                                   :disabled="selectedGridRows.length === 0">
                                Delete Budget Year(s)
                            </v-btn>
                        </v-flex>
                    </v-layout>
                    <div v-if="onCheckItemHasValue(investmentStrategyGridData)"
                         style=" height: 500px; width: 50%; margin: 0 auto; overflow: auto;">
                        <v-data-table :headers="investmentStrategyGridHeaders"
                                      :items="investmentStrategyGridData"
                                      v-model="selectedGridRows"
                                      select-all
                                      item-key="year"
                                      class="elevation-1"
                                      hide-actions>
                            <template slot="items" slot-scope="props">
                                <td>
                                    <v-checkbox v-model="props.selected" primary hide-details></v-checkbox>
                                </td>
                                <td v-for="header in investmentStrategyGridHeaders">
                                    <div v-if="header.value !== 'year'">
                                        <v-edit-dialog :return-value.sync="props.item[header.value]"
                                                       large lazy persistent>
                                            {{props.item[header.value]}}
                                            <template slot="input">
                                                <v-text-field v-model="props.item[header.value]"
                                                              label="Edit" single-line>
                                                </v-text-field>
                                            </template>
                                        </v-edit-dialog>
                                    </div>
                                    <div v-if="header.value === 'year'">
                                        {{props.item.year}}
                                    </div>
                                </td>
                            </template>
                        </v-data-table>
                    </div>
                    <v-spacer></v-spacer>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider></v-divider>
        <v-layout justify-center>
            <v-flex xsl-6>

            </v-flex>
        </v-layout>
        <v-layout>
            <AppSpinner />
            <EditBudgetsDialog :showDialog="showEditBudgetsDialog" @result="onEditBudgets" />
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';

    import AppSpinner from '../../shared/AppSpinner.vue';
    import {
        InvestmentStrategy,
        emptyInvestmentStrategy,
        InvestmentStrategyBudgetYear,
        InvestmentStrategyGridData, InvestmentStrategyBudget, SavedInvestmentStrategy
    } from '@/shared/models/iAM/investment';
    import {emptyNetwork} from '@/shared/models/iAM/network';
    import * as R from 'ramda';
    import {VueSelect, vueSelectDefault} from '@/shared/models/vue/vue-select';
    import {VueDataTableHeader} from '@/shared/models/vue/vue-data-table-header';
    import {hasValue} from '@/shared/utils/has-value';
    import moment from 'moment';
    import EditBudgetsDialog from './investment-editor-dialogs/EditBudgetsDialog.vue';
    import {EditBudgetsDialogResult} from '@/shared/models/dialogs/edit-budgets-dialog-result';

    @Component({
        components: {AppSpinner, EditBudgetsDialog}
    })
    export default class InvestmentEditor extends Vue {
        @State(state => state.investmentEditor.investmentStrategies) investmentStrategies: InvestmentStrategy[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getInvestmentStrategies') getInvestmentStrategiesAction: any;
        @Action('saveInvestmentStrategy') saveInvestmentStrategyAction: any;
        @Action('setBudgets') setBudgetsAction: any;

        investmentStrategyGridHeaders: VueDataTableHeader[] = [
            {text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: ''}
        ];
        investmentStrategiesSelect: VueSelect[] = [vueSelectDefault];
        selectedInvestmentStrategy: InvestmentStrategy = emptyInvestmentStrategy;
        investmentStrategyGridData: InvestmentStrategyGridData[] = [];
        selectedGridRows: InvestmentStrategyGridData[] = [];
        savedInvestmentStrategy: SavedInvestmentStrategy = {
            ...emptyInvestmentStrategy,
            deletedBudgetYears: [],
            deletedBudgets: []
        };
        showCreateInvestmentStrategyDialog: boolean = false;
        showEditBudgetsDialog: boolean = false;

        /**
         * Watcher for investmentStrategies property
         */
        @Watch('investmentStrategies')
        onInvestmentStrategiesChanged(investmentStrategies: InvestmentStrategy[]) {
            // set the investmentStrategiesSelect by mapping the investmentStrategies as a VueSelect list
            this.investmentStrategiesSelect = investmentStrategies.map((investmentStrategy: InvestmentStrategy) => (
                {
                    text: investmentStrategy.name,
                    value: investmentStrategy.simulationId.toString()
                }
            ));
        }

        /**
         * Component has been mounted
         */
        mounted() {
            // set isBusy = true
            this.setIsBusyAction({isBusy: true});
            // get the investment strategies for the currently selected network
            this.getInvestmentStrategiesAction({network: emptyNetwork})
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                })
        }

        onShowCreateInvestmentStrategyDialog() {
            // show the CreateInvestmentStrategyDialog
            this.showCreateInvestmentStrategyDialog = true;
        }

        onCreateInvestmentStrategy() {
            // hide the CreateInvestmentStrategyDialog
            this.showCreateInvestmentStrategyDialog = false;
        }

        /**
         * An investment strategy has been selected
         * @param value The simulationId of the selected investment strategy
         */
        onSelectInvestmentStrategy(value: string) {
            const simulationId = parseInt(value);
            // find the selected investment strategy in the investmentStrategies list and set it to the
            // selectedInvestmentStrategy property
            this.selectedInvestmentStrategy = this.investmentStrategies
                .find((investmentStrategy: InvestmentStrategy) => investmentStrategy.simulationId === simulationId) as InvestmentStrategy;
            // sort the selectedInvestmentStrategy budgetYears
            this.selectedInvestmentStrategy.budgetYears = R.sortBy(
                R.prop('year'), this.selectedInvestmentStrategy.budgetYears
            );
            // update savedInvestmentStrategy with selectedInvestmentStrategy data
            this.savedInvestmentStrategy = {
                ...this.selectedInvestmentStrategy,
                deletedBudgetYears: [],
                deletedBudgets: []
            };
            // set the grid data
            this.setGridData();
        }

        /**
         * The 'Add Budget Year' button has been clicked
         */
        onAddBudgetYear() {
            // get all selectedInvestmentStrategy budgetYears years into a list
            const years = this.selectedInvestmentStrategy.budgetYears
                .map((budgetYear: InvestmentStrategyBudgetYear) => budgetYear.year);
            // sort the list of years and get the last year in the list
            const latestYear = hasValue(years)
                ? R.last(R.sort((year1: number, year2: number) => year1 - year2, [...years])) as number
                : moment().year();
            // add 1 to latestYear to get the next year
            const nextYear = hasValue(years)
                ? latestYear + 1
                : latestYear;
            // create a new budgetYear
            const newBudgetYear: InvestmentStrategyBudgetYear = {
                year: nextYear,
                budgets: this.selectedInvestmentStrategy.budgetOrder.map((budget: string) => ({
                    name: budget,
                    amount: 0
                }))
            };
            // add the new budgetYear to the selectedInvestmentStrategy's budgetYears list
            this.selectedInvestmentStrategy.budgetYears.push(newBudgetYear);
            // reset the grid data
            this.setGridData();
        }

        onAddBudgetYearRange() {

        }

        /**
         * 'Delete Budget Year(s)' button has been clicked
         */
        onDeleteBudgetYears() {
            // add the deleted years to the savedInvestmentStrategy.deletedBudgetYears list
            this.savedInvestmentStrategy.deletedBudgetYears = R.uniq([
                ...this.savedInvestmentStrategy.deletedBudgetYears,
                ...this.selectedGridRows.map((gridRow: InvestmentStrategyGridData) => gridRow.year)
            ]);
            // filter the selectedInvestmentStrategy.budgetYears list so that only non-deleted years remain
            this.selectedInvestmentStrategy.budgetYears = this.selectedInvestmentStrategy.budgetYears
                .filter((budgetYear: InvestmentStrategyBudgetYear) =>
                    this.savedInvestmentStrategy.deletedBudgetYears.indexOf(budgetYear.year) === -1
                );
            // clear the selectedGridRows list
            this.selectedGridRows = [];
            // reset the grid data
            this.setGridData();
        }

        /**
         * 'Edit Budgets' button has been clicked
         */
        onShowEditBudgetsDialog() {
            // add the selected investment strategy's budgets to state
            this.setBudgetsAction({budgets: this.selectedInvestmentStrategy.budgetOrder});
            // show EditBudgetsDialog
            this.showEditBudgetsDialog = true;
        }

        /**
         * EditBudgetsDialog 'Save' button has been clicked
         * @param budgets The EditBudgetsDialog budgets list return value
         */
        onEditBudgets(result: EditBudgetsDialogResult) {
            // hide EditBudgetsDialog
            this.showEditBudgetsDialog = false;
            if (!result.canceled) {
                // set the selectedInvestmentStrategy.budgetOrder list with the incoming modal budgets list
                this.selectedInvestmentStrategy.budgetOrder = result.budgets;
                // reset the grid data
                this.setGridData();
            }
        }

        /**
         * Sets the grid data & grid headers using the selectedInvestmentStrategy data
         */
        setGridData() {
            this.investmentStrategyGridData = this.selectedInvestmentStrategy.budgetYears
                .map((budgetYear: InvestmentStrategyBudgetYear) => {
                    const gridData: InvestmentStrategyGridData = {
                        year: budgetYear.year
                    };
                    const budgets: string[] = this.selectedInvestmentStrategy.budgetOrder;
                    for (let i = 0; i < budgets.length; i++) {
                        // add the current budget to the gridData
                        gridData[budgets[i]] = 0;
                        // check if the current budgetYear.budgets has a match for the current budget
                        if (R.any(R.propEq('name', budgets[i]), budgetYear.budgets)) {
                            // set the grid data value at the current budget with the found budgetYear.budgets amount
                            // @ts-ignore
                            gridData[budgets[i]] = budgetYear.budgets
                                .find((budget: InvestmentStrategyBudget) => budget.name === budgets[i])
                                .amount as number;
                        }
                    }
                    return gridData;
                });
            // create headers with the budget order string list
            const budgetHeaders: VueDataTableHeader[] = this.selectedInvestmentStrategy.budgetOrder
                .map((budgetName: string) => ({
                    text: budgetName,
                    value: budgetName,
                    sortable: true,
                    align: 'left',
                    class: '',
                    width: ''
                }) as VueDataTableHeader);
            // add the new headers to the investmentStrategyGridHeaders list
            this.investmentStrategyGridHeaders = [this.investmentStrategyGridHeaders[0], ...budgetHeaders];
        }

        /**
         * 'Saved to Library' button has been clicked
         */
        onSaveToLibrary() {
            // TODO: add library pathway after defined
            // update the savedInvestmentStrategy with the latest changes made to selectedInvestmentStrategy
            this.savedInvestmentStrategy = {
                ...this.selectedInvestmentStrategy,
                deletedBudgetYears: this.savedInvestmentStrategy.deletedBudgetYears,
                deletedBudgets: this.savedInvestmentStrategy.deletedBudgets
            };
            // call server to saved selectedInvestmentStrategy data in the database
            this.setIsBusyAction({isBusy: true});
            this.saveInvestmentStrategyAction({savedInvestmentStrategy: this.savedInvestmentStrategy})
                .then(() => {
                    this.setIsBusyAction({isBusy: false});
                    this.savedInvestmentStrategy = {
                        ...emptyInvestmentStrategy,
                        deletedBudgetYears: [],
                        deletedBudgets: []
                    };
                })
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }

        /**
         * 'Apply' button has been clicked
         */
        onApplyToScenario() {
            // TODO: add scenario pathway after defined
            // update the savedInvestmentStrategy with the latest changes made to selectedInvestmentStrategy
            this.savedInvestmentStrategy = {
                ...this.selectedInvestmentStrategy,
                deletedBudgetYears: this.savedInvestmentStrategy.deletedBudgetYears,
                deletedBudgets: this.savedInvestmentStrategy.deletedBudgets
            };
            // call server to saved selectedInvestmentStrategy data in the database
            this.setIsBusyAction({isBusy: true});
            this.saveInvestmentStrategyAction({savedInvestmentStrategy: this.savedInvestmentStrategy})
                .then(() => {
                    this.setIsBusyAction({isBusy: false});
                    this.savedInvestmentStrategy = {
                        ...emptyInvestmentStrategy,
                        deletedBudgetYears: [],
                        deletedBudgets: []
                    };
                })
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }

        /**
         * Whether or not the specified item has a value (not null|undefined, not empty)
         * @param item
         */
        onCheckItemHasValue(item: any) {
            return hasValue(item);
        }
    }
</script>