<template>
    <v-container fluid grid-list-xl>
        <div class="investment-editor-container">
            <v-layout row justify-center align-end>
                <v-flex xs3>
                    <v-btn color="info" v-on:click="onShowCreateInvestmentStrategyDialog">
                        New Investment Strategy
                    </v-btn>
                    <v-select :items="investmentStrategiesSelectList"
                              label="Select an Investment Strategy"
                              outline
                              v-on:change="onSelectInvestmentStrategy"
                              v-model="investmentStrategiesSelectItem">
                    </v-select>
                </v-flex>
                <v-flex xs2>
                    <v-text-field label="Inflation Rate (%)"
                                  v-model="selectedInvestmentStrategy.inflationRate"
                                  outline
                                  :mask="'##########'"
                                  :disabled="hasNoSelectedInvestmentStrategy">
                    </v-text-field>
                </v-flex>
                <v-flex xs2>
                    <v-text-field label="Discount Rate (%)"
                                  v-model="selectedInvestmentStrategy.discountRate"
                                  outline
                                  :mask="'##########'"
                                  :disabled="hasNoSelectedInvestmentStrategy">
                    </v-text-field>
                </v-flex>
            </v-layout>

            <v-divider></v-divider>

            <v-layout column justify-center>
                <v-flex>
                    <v-layout justify-center>
                        <div>
                            <v-layout justify-space-between>
                                <v-btn color="info" v-on:click="onAddBudgetYear"
                                       :disabled="hasNoSelectedInvestmentStrategy">
                                    Add Budget Year
                                </v-btn>
                                <v-btn color="info" v-on:click="onAddBudgetYearRange"
                                       :disabled="hasNoSelectedInvestmentStrategy">
                                    Add Range

                                </v-btn>
                                <v-btn color="info" v-on:click="onShowEditBudgetsDialog"
                                       :disabled="hasNoSelectedInvestmentStrategy">
                                    Edit Budgets
                                </v-btn>
                                <v-btn color="error" v-on:click="onDeleteBudgetYears"
                                       :disabled="selectedGridRows.length === 0">
                                    Delete Budget Year(s)
                                </v-btn>
                            </v-layout>
                        </div>
                    </v-layout>
                </v-flex>

                <v-flex>
                    <v-layout justify-center class="investment-strategy-grid-layout">
                        <div v-if="onCheckItemHasValue(investmentStrategyGridData)"
                             class="investment-strategy-grid-container">
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
                                                           large lazy persistent
                                                           @save="onEditBudgetYearAmount(props.item.year, header.value, props.item[header.value])">
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
                    </v-layout>
                </v-flex>
            </v-layout>
            <v-divider></v-divider>
            <v-layout justify-center>
                <v-flex xsl-6>

                </v-flex>
            </v-layout>
            <v-layout>
                <v-dialog v-model="showBudgetYearRangeEditDialog" persistent max-width="200px">
                    <v-card>
                        <v-card-title>Add Range</v-card-title>
                        <v-card-text class="budget-year-range-edit-dialog-card-text">
                            <v-text-field v-model="budgetYearRange" label="Edit" single-line :mask="'####'"></v-text-field>
                        </v-card-text>
                        <v-card-actions>
                            <v-btn color="info" v-on:click="onSubmitBudgetYearRange(true)">Save</v-btn>
                            <v-btn v-on:click="onSubmitBudgetYearRange(false)">Cancel</v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
                <AppSpinner />
                <CreateInvestmentStrategyDialog :showDialog="showCreateInvestmentStrategyDialog"
                                                @result="onSubmitCreateInvestmentStrategyDialogResult" />
                <EditBudgetsDialog :showDialog="showEditBudgetsDialog" @result="onSubmitEditBudgetsDialogResult" />
            </v-layout>
        </div>

        <v-footer height="auto">
            <v-layout row justify-end>
                <v-btn v-on:click="resetComponentProperties" :disabled="hasNoSelectedInvestmentStrategy">Cancel</v-btn>
                <v-btn color="info lighten-2" v-on:click="onSaveToLibrary" :disabled="hasNoSelectedInvestmentStrategy">
                    Create as New Library
                </v-btn>
                <v-btn color="info" v-on:click="onApplyToScenario" :disabled="hasNoSelectedInvestmentStrategy">
                    Apply
                </v-btn>
            </v-layout>
        </v-footer>
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
    import {SelectItem, defaultSelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {hasValue} from '@/shared/utils/has-value';
    import moment from 'moment';
    import EditBudgetsDialog from './investment-editor-dialogs/EditBudgetsDialog.vue';
    import {BudgetNames, EditBudgetsDialogResult} from '@/shared/models/dialogs/edit-budgets-dialog-result';
    import CreateInvestmentStrategyDialog from '@/components/investment-editor/investment-editor-dialogs/CreateInvestmentStrategyDialog.vue';
    import {CreateInvestmentStrategyDialogResult} from '@/shared/models/dialogs/create-investment-strategy-dialog-result';

    @Component({
        components: {CreateInvestmentStrategyDialog, AppSpinner, EditBudgetsDialog}
    })
    export default class InvestmentEditor extends Vue {
        @State(state => state.investmentEditor.investmentStrategies) investmentStrategies: InvestmentStrategy[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getInvestmentStrategies') getInvestmentStrategiesAction: any;
        @Action('saveInvestmentStrategyToLibrary') saveInvestmentStrategyAction: any;
        @Action('setBudgets') setBudgetsAction: any;

        investmentStrategyGridHeaders: DataTableHeader[] = [
            {text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: ''}
        ];
        investmentStrategiesSelectList: SelectItem[] = [{...defaultSelectItem}];
        investmentStrategiesSelectItem: SelectItem = {...defaultSelectItem};
        selectedInvestmentStrategy: InvestmentStrategy = {...emptyInvestmentStrategy};
        investmentStrategyGridData: InvestmentStrategyGridData[] = [];
        selectedGridRows: InvestmentStrategyGridData[] = [];
        savedInvestmentStrategy: SavedInvestmentStrategy = {
            ...emptyInvestmentStrategy,
            deletedBudgetYears: [],
            deletedBudgets: []
        };
        hasNoSelectedInvestmentStrategy: boolean = true;
        showCreateInvestmentStrategyDialog: boolean = false;
        showEditBudgetsDialog: boolean = false;
        showBudgetYearRangeEditDialog: boolean = false;
        budgetYearRange: number = 0;


        /**
         * Watcher for investmentStrategies property
         */
        @Watch('investmentStrategies')
        onInvestmentStrategiesChanged(investmentStrategies: InvestmentStrategy[]) {
            // set the investmentStrategiesSelectList by mapping the investmentStrategies as a SelectItem list
            this.investmentStrategiesSelectList = investmentStrategies.map((investmentStrategy: InvestmentStrategy) => (
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
                });
        }

        /**
         * 'New Investment Strategy' button has been clicked
         */
        onShowCreateInvestmentStrategyDialog() {
            // show the CreateInvestmentStrategyDialog
            this.showCreateInvestmentStrategyDialog = true;
        }

        /**
         * User has submitted CreateInvestmentStrategyDialog result
         * @param result CreateInvestmentStrategyDialogResult object
         */
        onSubmitCreateInvestmentStrategyDialogResult(result: CreateInvestmentStrategyDialogResult) {
            // hide the CreateInvestmentStrategyDialog
            this.showCreateInvestmentStrategyDialog = false;
            if (!result.canceled) {
                // add new select options for new investment strategy
                const newInvestmentStrategiesSelectItem: SelectItem = {
                    text: result.newInvestmentStrategy.name,
                    value: result.newInvestmentStrategy.simulationId.toString()
                };
                // push the new select item to the select list
                this.investmentStrategiesSelectList.push({
                    text: result.newInvestmentStrategy.name,
                    value: result.newInvestmentStrategy.simulationId.toString()
                });
                // set the investmentStrategySelectItem
                this.investmentStrategiesSelectItem = newInvestmentStrategiesSelectItem;
                // set the selectedInvestmentStrategy.budgetOrder list with the incoming modal budgets list
                this.selectedInvestmentStrategy = result.newInvestmentStrategy;
                // add the new investment strategy to the investmentStrategies list
                this.investmentStrategies.push(result.newInvestmentStrategy);
                // set hasSelectedInvestmentStrategy to false
                this.hasNoSelectedInvestmentStrategy = false;
                // reset the grid data
                this.setGridData();
            }
        }

        /**
         * An investment strategy has been selected
         * @param value The simulationId of the selected investment strategy
         */
        onSelectInvestmentStrategy(value: string) {
            // set the investmentStrategiesSelectItem
            this.investmentStrategiesSelectItem = this.investmentStrategiesSelectList
                .find((selectItem: SelectItem) => selectItem.value === value) as SelectItem;
            // parse the value as an integer
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
            // set hasSelectedInvestmentStrategy to false
            this.hasNoSelectedInvestmentStrategy = false;
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

        /**
         * 'Add Range' button has been clicked
         */
        onAddBudgetYearRange() {
            // show the edit dialog for adding a budget year range to the selected investment strategy
            this.showBudgetYearRangeEditDialog = true;
        }

        /**
         * User has submitted an 'Add Range' dialog result
         */
        onSubmitBudgetYearRange(notCanceled: boolean) {
            // hide the budget year range edit dialog
            this.showBudgetYearRangeEditDialog = false;
            if (this.budgetYearRange > 0 && notCanceled) {
                // get the budget years' year values
                const budgetYears = this.selectedInvestmentStrategy.budgetYears
                    .map((budgetYear: InvestmentStrategyBudgetYear) => budgetYear.year) as number[];
                // get the last budget year in the list of budget years
                const lastBudgetYear = R.last(budgetYears);
                // set the current year as 1 more than the last budget year if present, otherwise use
                // actual current year as start year
                let currentYear = hasValue(lastBudgetYear)
                    // @ts-ignore
                    ? moment().year(lastBudgetYear).clone().add(1, 'year').year()
                    : moment().year();
                // add a budget year starting with the start year until loop hits beyond the budget year range
                for (let i = 0; i < this.budgetYearRange; i++) {
                    this.selectedInvestmentStrategy.budgetYears.push({
                        year: currentYear,
                        budgets: this.selectedInvestmentStrategy.budgetOrder.map((budgetName: string) => ({
                            name: budgetName,
                            amount: 0
                        }))
                    });
                    currentYear++;
                }
                // reset the budgetYearRange property
                this.budgetYearRange = 0;
                // reset the grid data
                this.setGridData();
            }
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
        onSubmitEditBudgetsDialogResult(result: EditBudgetsDialogResult) {
            // hide EditBudgetsDialog
            this.showEditBudgetsDialog = false;
            if (!result.canceled) {
                // update the selectedInvestmentStrategy.budgetYears.budgets list with the name values from result.budgets list
                // using the previousName values to find existing budgets and replacing those previous names with the new names,
                // otherwise if no previousName value exists in the list it is considered a new budget
                this.selectedInvestmentStrategy.budgetYears = this.selectedInvestmentStrategy.budgetYears
                    .map((budgetYear: InvestmentStrategyBudgetYear) => {
                        return {
                            year: budgetYear.year,
                            budgets: result.budgets.map((budgetNames: BudgetNames) => {
                                if (R.any(R.propEq('name', budgetNames.previousName), budgetYear.budgets)) {
                                    // find the existing budget with the previousName value
                                    const budget: InvestmentStrategyBudget = budgetYear.budgets
                                        .find((budget: InvestmentStrategyBudget) =>
                                            budget.name === budgetNames.previousName
                                        ) as InvestmentStrategyBudget;
                                    // return the budget's new name with its current amount
                                    return {name: budgetNames.name, amount: budget.amount};
                                }
                                // if no budget was found with the budgetNames.previousName, then return as a new budget
                                return {name: budgetNames.name, amount: 0};
                            })
                        };
                    });
                // update the selectedInvestmentStrategy.budgetOrder list using the name values from the result.budgets list
                this.selectedInvestmentStrategy.budgetOrder = result.budgets
                    .map((budgetNames: BudgetNames) => budgetNames.name);
                // reset the grid data
                this.setGridData();
            }
        }

        /**
         * User has submitted an inline edit for a budget year's budget's amount
         */
        onEditBudgetYearAmount(year: number, budgetName: string, amount: number) {
            if (R.any(R.propEq('year', year), this.selectedInvestmentStrategy.budgetYears)) {
                const budgetYearIndex = R.findIndex(
                    R.propEq('year', year),
                    this.selectedInvestmentStrategy.budgetYears
                ) as number;
                /*const budgetYear = this.selectedInvestmentStrategy.budgetYears
                    .find((budgetYear: InvestmentStrategyBudgetYear) =>
                        budgetYear.year === year
                    ) as InvestmentStrategyBudgetYear;*/
                if (R.any(R.propEq('name', budgetName), this.selectedInvestmentStrategy.budgetYears[budgetYearIndex].budgets)) {
                    const budgetIndex = R.findIndex(
                        R.propEq('name', budgetName),
                        this.selectedInvestmentStrategy.budgetYears[budgetYearIndex].budgets
                    ) as number;
                    this.selectedInvestmentStrategy.budgetYears[budgetYearIndex].budgets[budgetIndex].amount = amount;
                    console.log('done setting budget amount');
                }
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
            const budgetHeaders: DataTableHeader[] = this.selectedInvestmentStrategy.budgetOrder
                .map((budgetName: string) => ({
                    text: budgetName,
                    value: budgetName,
                    sortable: true,
                    align: 'left',
                    class: '',
                    width: ''
                }) as DataTableHeader);
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
                budgetYears: this.investmentStrategyGridData.map((gridData: InvestmentStrategyGridData) => {
                    const budgets: InvestmentStrategyBudget[] = [];
                    for (let prop in gridData) {
                        if (prop !== 'year') {
                            budgets.push({
                                name: prop,
                                amount: gridData[prop]
                            });
                        }
                    }
                    return {
                        year: gridData.year,
                        budgets: budgets
                    };
                }),
                deletedBudgetYears: this.savedInvestmentStrategy.deletedBudgetYears,
                deletedBudgets: this.savedInvestmentStrategy.deletedBudgets
            };
            // call server to saved selectedInvestmentStrategy data in the database
            this.setIsBusyAction({isBusy: true});
            this.saveInvestmentStrategyAction({savedInvestmentStrategy: this.savedInvestmentStrategy})
                .then(() => {
                    this.setIsBusyAction({isBusy: false});
                    this.resetComponentProperties();
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
                    this.resetComponentProperties();
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

        /**
         * Resets the InvestmentEditor component properties
         */
        resetComponentProperties() {
            this.investmentStrategiesSelectItem = {...defaultSelectItem};
            this.selectedInvestmentStrategy = {...emptyInvestmentStrategy};
            this.investmentStrategyGridData = [];
            this.selectedGridRows = [];
            this.savedInvestmentStrategy = {
                ...emptyInvestmentStrategy,
                deletedBudgetYears: [],
                deletedBudgets: []
            };
            this.hasNoSelectedInvestmentStrategy = true;
        }
    }
</script>

<style>
    .investment-editor-container {
        height: 850px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .investment-strategy-grid-layout {
        margin: 10px;
    }

    .investment-strategy-grid-container {
        height: 500px;
        overflow: auto;
    }

    .budget-year-range-edit-dialog-card-text {
        height: 100px;
    }
</style>
