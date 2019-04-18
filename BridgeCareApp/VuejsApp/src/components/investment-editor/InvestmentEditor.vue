<template>
    <v-container fluid grid-list-xl>
        <div class="investment-editor-container">
            <v-layout column>
                <v-flex xs12>
                    <v-layout justify-center fill-height>
                        <v-flex xs3>
                            <v-btn color="info" v-on:click="onNewLibrary">
                                New Library
                            </v-btn>
                            <v-select v-if="!hasSelectedInvestmentStrategy"
                                      :items="investmentStrategiesSelectListItems"
                                      label="Select an Investment Strategy" outline v-model="selectItemValue">
                            </v-select>
                            <v-text-field v-if="hasSelectedInvestmentStrategy" label="Strategy Name" append-icon="clear"
                                          v-model="selectedInvestmentStrategy.name"
                                          @click:append="onClearInvestmentStrategySelection">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                    <v-layout v-if="hasSelectedInvestmentStrategy" justify-center fill-height>
                        <v-flex xs2>
                            <v-text-field label="Inflation Rate (%)" outline :mask="'##########'"
                                          v-model="selectedInvestmentStrategy.inflationRate"
                                          :disabled="!hasSelectedInvestmentStrategy">
                            </v-text-field>
                        </v-flex>
                        <v-flex xs2>
                            <v-text-field label="Discount Rate (%)" outline :mask="'##########'"
                                          v-model="selectedInvestmentStrategy.discountRate"
                                          :disabled="!hasSelectedInvestmentStrategy">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedInvestmentStrategy"></v-divider>
                <v-flex xs12 v-if="hasSelectedInvestmentStrategy">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-layout justify-space-between fill-height>
                                <v-btn color="info lighten-2" v-on:click="onEditBudgets">
                                    Edit Budgets
                                </v-btn>
                                <v-btn color="info" v-on:click="onAddBudgetYear"
                                       :disabled="selectedInvestmentStrategy.budgetOrder.length === 0">
                                    Add Year
                                </v-btn>
                                <v-btn color="info lighten-1" v-on:click="onAddBudgetYearsByRange"
                                       :disabled="selectedInvestmentStrategy.budgetOrder.length === 0">
                                    Add Years by Range
                                </v-btn>
                                <v-btn color="error" v-on:click="onDeleteBudgetYears"
                                       :disabled="selectedGridRows.length === 0">
                                    Delete Budget Year(s)
                                </v-btn>
                            </v-layout>
                        </v-flex>
                    </v-layout>
                    <v-layout justify-center fill-height>
                        <v-flex xs8>
                            <v-layout fill-height>
                                <div class="investment-editor-data-table">
                                    <v-data-table :headers="budgetYearsGridHeaders" :items="budgetYearsGridData"
                                                  v-model="selectedGridRows" select-all item-key="year"
                                                  class="elevation-1 fixed-header v-table__overflow" hide-actions>
                                        <template slot="items" slot-scope="props">
                                            <td>
                                                <v-checkbox v-model="props.selected" primary hide-details></v-checkbox>
                                            </td>
                                            <td v-for="header in budgetYearsGridHeaders">
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
                </v-flex>
                <v-divider v-if="hasSelectedInvestmentStrategy"></v-divider>
                <v-flex xs12 v-if="hasSelectedInvestmentStrategy">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-textarea no-resize outline full-width
                                        :label="selectedInvestmentStrategy.description === '' ? 'Description' : ''"
                                        v-model="selectedInvestmentStrategy.description">
                            </v-textarea>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn color="info lighten-2" v-on:click="onCreateAsNewLibrary" :disabled="!hasSelectedInvestmentStrategy">
                    Create as New Library
                </v-btn>
                <v-btn color="info lighten-1" v-on:click="onUpdateLibrary" :disabled="!hasSelectedInvestmentStrategy">
                    Update Library
                </v-btn>
                <v-tooltip top>
                    <template slot="activator">
                        <v-btn color="info" v-on:click="onApplyToScenario" :disabled="true">
                            Apply
                        </v-btn>
                    </template>
                    <span>Feature not ready</span>
                </v-tooltip>
            </v-layout>
        </v-footer>

        <CreateInvestmentStrategyDialog :dialogData="createInvestmentStrategyDialogData"
                                        @submit="onCreateInvestmentStrategy" />

        <SetRangeForAddingBudgetYearsDialog :showDialog="showSetRangeForAddingBudgetYearsDialog"
                                            @submit="onSubmitBudgetYearRange" />

        <EditBudgetsDialog :dialogData="editBudgetsDialogData" @submit="onSubmitEditedBudgets" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Watch } from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {Action, State} from 'vuex-class';
    import AppSpinner from '../../shared/dialogs/AppSpinner.vue';
    import CreateInvestmentStrategyDialog from './investment-editor-dialogs/CreateInvestmentStrategyDialog.vue';
    import SetRangeForAddingBudgetYearsDialog from './investment-editor-dialogs/SetRangeForAddingBudgetYearsDialog.vue';
    import EditBudgetsDialog from './investment-editor-dialogs/EditBudgetsDialog.vue';
    import {
        BudgetYearsGridData,
        EditedBudget,
        InvestmentStrategy,
        InvestmentStrategyBudgetYear
    } from '@/shared/models/iAM/investment';
    import {any, clone, contains, groupBy, isEmpty, isNil, keys, pluck, propEq, last, uniq, findIndex} from 'ramda';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {hasValue} from '@/shared/utils/has-value';
    import moment from 'moment';
    import {
        CreateInvestmentStrategyDialogData,
        emptyCreateInvestmentStrategyDialogData
    } from '@/shared/models/dialogs/investment-editor-dialogs/create-investment-strategy-dialog-data';
    import {
        EditBudgetsDialogData,
        emptyEditBudgetsDialogData
    } from '@/shared/models/dialogs/investment-editor-dialogs/edit-budgets-dialog-data';
    import {getLatestPropertyValue, getPropertyValues} from '@/shared/utils/getter-utils';
    import {sorter} from '@/shared/utils/sorter';

    @Component({
        components: {AppSpinner, CreateInvestmentStrategyDialog, SetRangeForAddingBudgetYearsDialog, EditBudgetsDialog}
    })
    export default class InvestmentEditor extends Vue {
        @State(state => state.investmentEditor.investmentStrategies) investmentStrategies: InvestmentStrategy[];
        @State(state => state.investmentEditor.selectedInvestmentStrategy) selectedInvestmentStrategy: InvestmentStrategy;
        @State(state => state.breadcrumb.navigation) navigation: any[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getInvestmentStrategies') getInvestmentStrategiesAction: any;
        @Action('selectInvestmentStrategy') selectInvestmentStrategyAction: any;
        @Action('createInvestmentStrategy') createInvestmentStrategyAction: any;
        @Action('updateInvestmentStrategy') updateInvestmentStrategyAction: any;
        @Action('updateSelectedInvestmentStrategy') updateSelectedInvestmentStrategyAction: any;
        @Action('setNavigation') setNavigationAction: any;

        investmentStrategiesSelectListItems: SelectItem[] = [];
        selectItemValue: string = '';
        hasSelectedInvestmentStrategy: boolean = false;
        budgetYearsGridHeaders: DataTableHeader[] = [
            {text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: ''}
        ];
        budgetYearsGridData: BudgetYearsGridData[] = [];
        selectedGridRows: BudgetYearsGridData[] = [];
        selectedBudgetYears: number[] = [];
        createInvestmentStrategyDialogData: CreateInvestmentStrategyDialogData = {...emptyCreateInvestmentStrategyDialogData};
        editBudgetsDialogData: EditBudgetsDialogData = {...emptyEditBudgetsDialogData};
        showSetRangeForAddingBudgetYearsDialog: boolean = false;

        beforeRouteEnter(to: any, from: any, next: any) {
            if (from.name === 'EditScenario') {
                next((vm: any) => {
                    vm.setNavigationAction([
                        {
                            text: 'Scenario dashboard',
                            to: '/Scenarios/'
                        },
                        {
                            text: 'Scenario editor',
                            to: '/EditScenario/'
                        },
                        {
                            text: 'Investment editor',
                            to: '/InvestmentEditor/FromScenario/'
                        }
                    ]);
                });
            }
            else {
                next((vm: any) => {
                    vm.setNavigationAction([]);
                });
            }
        }
        beforeRouteUpdate(to: any, from: any, next: any) {
            console.log('Router rerendered');
            next();
            // called when the route that renders this component has changed,
            // but this component is reused in the new route.
            // For example, for a route with dynamic params `/foo/:id`, when we
            // navigate between `/foo/1` and `/foo/2`, the same `Foo` component instance
            // will be reused, and this hook will be called when that happens.
            // has access to `this` component instance.
        }

        /**
         * Watcher: investmentStrategies
         */
        @Watch('investmentStrategies')
        onInvestmentStrategiesChanged(investmentStrategies: InvestmentStrategy[]) {
            // set the investmentStrategiesSelectListItems list using investmentStrategies list
            this.investmentStrategiesSelectListItems = investmentStrategies
                .map((investmentStrategy: InvestmentStrategy) => ({
                    text: investmentStrategy.name,
                    value: investmentStrategy.id.toString()
                }));
        }

        /**
         * Watcher: selectItemValue
         */
        @Watch('selectItemValue')
        onInvestmentStrategiesSelectItemChanged() {
            if (hasValue(this.selectItemValue) && this.selectedInvestmentStrategy.id === 0) {
                // parse selectItemValue as an integer
                const id: number = parseInt(this.selectItemValue);
                // dispatch selectInvestmentStrategyAction with id
                this.selectInvestmentStrategyAction({investmentStrategyId: id});
            } else if (!hasValue(this.selectItemValue) && this.selectedInvestmentStrategy.id !== 0) {
                // dispatch selectInvestmentStrategyAction with null
                this.selectInvestmentStrategyAction({investmentStrategyId: null});
            }
        }

        /**
         * Watcher: selectedInvestmentStrategy
         */
        @Watch('selectedInvestmentStrategy')
        onSelectedInvestmentStrategyChanged() {
            // reset the selected grid rows
            this.selectedGridRows = [];
            if (this.selectedInvestmentStrategy.id !== 0) {
                if (!hasValue(this.selectItemValue)) {
                    // set selectItemValue with the id of selectedInvestmentStrategy
                    this.selectItemValue = this.selectedInvestmentStrategy.id.toString();
                }
                // set hasSelectedInvestmentStrategy to true
                this.hasSelectedInvestmentStrategy = true;
                if (!hasValue(this.selectedInvestmentStrategy.budgetOrder) && hasValue(this.selectedInvestmentStrategy.budgetYears)) {
                    // set the budget order for the selected investment strategy using the it's list of budget years
                    this.setSelectedInvestmentStrategyBudgetOrder();
                } else {
                    // set the grid headers
                    this.setGridHeaders();
                    // set the grid data
                    this.setGridData();
                }

            } else {
                // reset selectItemValue
                this.selectItemValue = '';
                // reset hasSelectedInvestmentStrategy
                this.hasSelectedInvestmentStrategy = false;
                // reset grid data
                this.budgetYearsGridData = [];
            }
        }

        /**
         * Watcher: selectedGridRows
         */
        @Watch('selectedGridRows')
        onSelectedGridRowsChanged() {
            // set the selectedBudgetYears with the years of the selected investment strategy budget years in selectedGridRows
            this.selectedBudgetYears = getPropertyValues('year', this.selectedGridRows) as number[];
        }

        /**
         * Component has been mounted
         */
        mounted() {
            // set isBusy to true, then dispatch action to get all investment strategies
            this.setIsBusyAction({isBusy: true});
            this.getInvestmentStrategiesAction()
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }

        beforeDestroy() {
            // clear the selected investment strategy
            this.onClearInvestmentStrategySelection();
        }

        /**
         * Sets the selected investment strategy's budget order using it's budget years
         */
        setSelectedInvestmentStrategyBudgetOrder() {
            // set a new budget order for the selected investment strategy
           const newBudgetOrder = sorter(
               getPropertyValues('budgetName', this.selectedInvestmentStrategy.budgetYears)
           ) as string[];
            // dispatch action to update the selected investment strategy with the new budget order
            this.updateSelectedInvestmentStrategyAction({
                updatedInvestmentStrategy: {
                    ...this.selectedInvestmentStrategy,
                    budgetOrder: newBudgetOrder
                }
            });
        }

        /**
         * Sets the data table headers
         */
        setGridHeaders() {
            // set the grid headers for each budget in the selected investment strategy's budget order list
            const budgetHeaders: DataTableHeader[] = this.selectedInvestmentStrategy.budgetOrder
                .map((budgetName: string) => ({
                    text: budgetName,
                    value: budgetName,
                    sortable: true,
                    align: 'left',
                    class: '',
                    width: ''
                }) as DataTableHeader);
            // combine the current headers with the budget headers
            this.budgetYearsGridHeaders = [this.budgetYearsGridHeaders[0], ...budgetHeaders];
        }

        /**
         * Sets the data table rows
         */
        setGridData() {
            // reset the budget years grid data
            this.budgetYearsGridData = [];
            // group the selected investment strategy's budget years by year
            const groupBudgetYearsByYear = groupBy((budgetYear: InvestmentStrategyBudgetYear) => budgetYear.year.toString());
            const groupedBudgetYears = groupBudgetYearsByYear(this.selectedInvestmentStrategy.budgetYears);
            // uses the group keys of the grouped budget years to loop over each group and create a grid data row for each year
            keys(groupedBudgetYears).forEach((year: any) => {
                // create a grid data row and set the current year
                const gridDataRow: BudgetYearsGridData = {
                    year: parseInt(year)
                };
                // get the group of budget years at the given year key
                const budgetYears: InvestmentStrategyBudgetYear[] = groupedBudgetYears[year];
                // add the budget year budget amounts to the gridDataRow using the selected investment strategy's budget order
                const budgetOrder = this.selectedInvestmentStrategy.budgetOrder;
                for (let i = 0; i < budgetOrder.length; i++) {
                    const budgetYear: InvestmentStrategyBudgetYear = budgetYears
                        .find((by: InvestmentStrategyBudgetYear) =>
                            by.budgetName === budgetOrder[i]
                        ) as InvestmentStrategyBudgetYear;
                    gridDataRow[budgetOrder[i]] = hasValue(budgetYear) ? budgetYear.budgetAmount : 0;
                }
                // add the gridDataRow to the budgetYearsGridData list
                this.budgetYearsGridData.push(gridDataRow);
            });
        }

        /**
         * 'New Investment Strategy' button has been clicked
         */
        onNewLibrary() {
            // create new CreateInvestmentStrategyDialogData object
            this.createInvestmentStrategyDialogData = {
                ...emptyCreateInvestmentStrategyDialogData,
                showDialog: true
            };
        }

        /**
         * The 'Add Budget Year' button has been clicked
         */
        onAddBudgetYear() {
            // get the latest year from the selected investment strategy's budget years
            const latestYear: number = getLatestPropertyValue('year', this.selectedInvestmentStrategy.budgetYears);
            // set next year as latestYear + 1 (if present), otherwise set it as current year
            const nextYear = hasValue(latestYear) ? latestYear + 1 : moment().year();
            // get the latest id from the selected investment strategy's budget years & deleted budget year ids
            const latestId: number = last(sorter([
                ...getPropertyValues('id', this.selectedInvestmentStrategy.budgetYears),
                ...this.selectedInvestmentStrategy.deletedBudgetYearIds
            ]) as number[]) as number;
            // set nextId as latestId + 1 (if present), otherwise set it as 1
            let nextId = hasValue(latestId) ? latestId + 1 : 1;
            // create a budget year for each budget
            const budgets = this.selectedInvestmentStrategy.budgetOrder;
            const newBudgetYears: InvestmentStrategyBudgetYear[] = budgets.map((budget: string) => {
                const newBudgetYear: InvestmentStrategyBudgetYear = {
                    investmentStrategyId: this.selectedInvestmentStrategy.id,
                    id: nextId,
                    year: nextYear,
                    budgetName: budget,
                    budgetAmount: 0
                };
                nextId++;
                return newBudgetYear;
            });
            // add the new budget years to the selected investment strategy's list of budget
            const editedBudgetYears = [...this.selectedInvestmentStrategy.budgetYears, ...newBudgetYears];
            // dispatch action to update the selected investment strategy's budget years
            this.updateSelectedInvestmentStrategyAction({
                updatedInvestmentStrategy: {
                    ...this.selectedInvestmentStrategy,
                    budgetYears: editedBudgetYears
                }
            });
        }

        /**
         * 'Add Range' button has been clicked
         */
        onAddBudgetYearsByRange() {
            // show the SetRangeForAddingBudgetYearsDialog modal
            this.showSetRangeForAddingBudgetYearsDialog = true;
        }

        /**
         * User has submitted an 'Add Range' dialog result
         */
        onSubmitBudgetYearRange(range: number) {
            // hide the SetRangeForAddingBudgetYearsDialog modal
            this.showSetRangeForAddingBudgetYearsDialog = false;
            if (range > 0) {
                // get the latest year from the selected investment strategy's budget years
                const latestYear = getLatestPropertyValue('year', this.selectedInvestmentStrategy.budgetYears);
                // set start year as latestYear + 1 (if present), otherwise set it as current year
                const startYear: number = hasValue(latestYear) ? latestYear + 1 : moment().year();
                // set end year as startYear + range
                const endYear = moment().year(startYear).add(range, 'years').year();
                // get the latest id from the selected investment strategy's budget years & deleted budget year ids
                const latestId: number = last(sorter([
                    ...getPropertyValues('id', this.selectedInvestmentStrategy.budgetYears),
                    ...this.selectedInvestmentStrategy.deletedBudgetYearIds
                ]) as number[]) as number;
                // set next id as latestId + 1 (if present), otherwise set it as 1
                let nextId = hasValue(latestId) ? latestId + 1 : 1;
                // create a list for new budget years
                const newBudgetYears: InvestmentStrategyBudgetYear[] = [];
                for (let currentYear = startYear; currentYear < endYear; currentYear++) {
                    // for each year create budget years for each budget
                    this.selectedInvestmentStrategy.budgetOrder.forEach((budget: string) => {
                        newBudgetYears.push({
                            investmentStrategyId: this.selectedInvestmentStrategy.id,
                            id: nextId,
                            year: currentYear,
                            budgetName: budget,
                            budgetAmount: 0
                        });
                        nextId++;
                    });
                }
                // add the new budget years to the selected investment strategy's list of budget
                const editedBudgetYears = [...this.selectedInvestmentStrategy.budgetYears, ...newBudgetYears];
                // dispatch action to update the selected investment strategy's budget years
                this.updateSelectedInvestmentStrategyAction({
                    updatedInvestmentStrategy: {
                        ...this.selectedInvestmentStrategy,
                        budgetYears: editedBudgetYears
                    }
                });
            }
        }

        /**
         * 'Delete Budget Year(s)' button has been clicked
         */
        onDeleteBudgetYears() {
            // get the list of budget years to delete
            const budgetYearsToDelete: InvestmentStrategyBudgetYear[] = this.selectedInvestmentStrategy.budgetYears
                .filter((budgetYear: InvestmentStrategyBudgetYear) => contains(budgetYear.year, this.selectedBudgetYears));
            // get the ids of the budget years to delete
            const deletedBudgetYearIds: number[] = getPropertyValues('id', budgetYearsToDelete);
            // add the ids of the budget years to delete to the current list of ids of budget years to delete
            const editedDeletedBudgetYearIds = uniq([...this.selectedInvestmentStrategy.deletedBudgetYearIds, ...deletedBudgetYearIds]);
            // filter the copy's existing budget years, removing any budget years where they have been marked as deleted
            const editedBudgetYears = this.selectedInvestmentStrategy.budgetYears
                .filter((budgetYear: InvestmentStrategyBudgetYear) =>
                    !contains(budgetYear.id, editedDeletedBudgetYearIds)
                );
            // dispatch action to update the selected investment strategy's budget years & deleted budget year ids
            this.updateSelectedInvestmentStrategyAction({
                updatedInvestmentStrategy: {
                    ...this.selectedInvestmentStrategy,
                    budgetYears: editedBudgetYears,
                    deletedBudgetYearIds: editedDeletedBudgetYearIds
                }
            });
        }

        /**
         * 'Edit Budgets' button has been clicked
         */
        onEditBudgets() {
            // create new EditBudgetsDialogData object and set the budgets using selectedInvestmentStrategy.budgetOrder
            this.editBudgetsDialogData = {
                showDialog: true,
                budgets: this.selectedInvestmentStrategy.budgetOrder
            };
        }

        /**
         * User has submitted EditBudgetsDialog result
         * @param editedBudgets The edited budgets
         */
        onSubmitEditedBudgets(editedBudgets: EditedBudget[]) {
            // reset editBudgetsDialogData
            this.editBudgetsDialogData = {...emptyEditBudgetsDialogData};
            if (!isNil(editedBudgets)) {
                // get the previous budgets
                const previousBudgets = getPropertyValues('previousName', editedBudgets.filter((budget: EditedBudget) => !budget.isNew));
                // set the deleted budget year ids based on each budget year's budget that is not present in previousBudgets
                const editedDeletedBudgetYearIds = [
                    ...this.selectedInvestmentStrategy.deletedBudgetYearIds,
                    ...getPropertyValues('id', this.selectedInvestmentStrategy.budgetYears
                        .filter((budgetYear: InvestmentStrategyBudgetYear) =>
                            !contains(budgetYear.budgetName, previousBudgets)
                        )) as number[]
                ];
                // get the remaining budget years by filtering out budget years that were marked as deleted
                const remainingBudgetYears = this.selectedInvestmentStrategy.budgetYears
                    .filter((budgetYear: InvestmentStrategyBudgetYear) =>
                        !contains(budgetYear.id, editedDeletedBudgetYearIds)
                    );
                // create a list for updated budget years
                const editedBudgetYears: InvestmentStrategyBudgetYear[] = [];
                if (!isEmpty(editedBudgets)) {
                    // get all budget years' year values from the remaining budget years
                    const years = getPropertyValues('year', remainingBudgetYears);
                    // get the latest id from the selected investment strategy's budget years & deleted budget year ids
                    const latestId: number = last(sorter([
                        ...getPropertyValues('id', remainingBudgetYears),
                        ...editedDeletedBudgetYearIds
                    ]) as number[]) as number;
                    // set next id as latestId + 1 (if present), otherwise set it as 1
                    let nextId = hasValue(latestId) ? latestId + 1 : 1;
                    years.forEach((year: number) => {
                        // get all budget years for the current year
                        const currentYearBudgetYears = remainingBudgetYears
                            .filter((budgetYear: InvestmentStrategyBudgetYear) => budgetYear.year === year);
                        editedBudgets.forEach((editedBudget: EditedBudget) => {
                            if (editedBudget.isNew) {
                                // create a new budget year for the new budget
                                editedBudgetYears.push({
                                    investmentStrategyId: this.selectedInvestmentStrategy.id,
                                    id: nextId,
                                    year: year,
                                    budgetName: editedBudget.name,
                                    budgetAmount: 0,
                                });
                                nextId++;
                            } else {
                                // find the budget year that has a matching budgetName with the editedBudget.previousName
                                const updatedBudget = currentYearBudgetYears
                                    .find((budgetYear: InvestmentStrategyBudgetYear) =>
                                        budgetYear.budgetName === editedBudget.previousName
                                    ) as InvestmentStrategyBudgetYear;
                                // update the budget year's budgetName with the editedBudget.name
                                editedBudgetYears.push({
                                    ...updatedBudget,
                                    budgetName: editedBudget.name
                                });
                            }
                        });
                    });
                }
                // dispatch action to update the selected investment strategy's budget order, budget years, & deleted
                // budget year ids
                this.updateSelectedInvestmentStrategyAction({
                    updatedInvestmentStrategy: {
                        ...this.selectedInvestmentStrategy,
                        budgetOrder: getPropertyValues('name', editedBudgets),
                        budgetYears: editedBudgetYears,
                        deletedBudgetYearIds: editedDeletedBudgetYearIds
                    }
                });
            }
        }

        /**
         * User has submitted an inline edit for a budget year's budget's amount
         */
        onEditBudgetYearAmount(year: number, budgetName: string, amount: number) {

            if (any(propEq('year', year), this.selectedInvestmentStrategy.budgetYears) &&
                any(propEq('budgetName', budgetName), this.selectedInvestmentStrategy.budgetYears)) {
                // get the selected investment strategy's list of budget years
                const updatedBudgetYears = this.selectedInvestmentStrategy.budgetYears;
                // find the index of the updated budget year in the list of budget years
                const index: number = findIndex(
                    (budgetYear: InvestmentStrategyBudgetYear) =>
                        budgetYear.year === year && budgetYear.budgetName === budgetName,
                    updatedBudgetYears
                );
                // update the budget year's budget amount with the specified amount at the given index of the list of budget years
                updatedBudgetYears[index].budgetAmount = amount;
                // dispatch action to update the selected investment strategy's budget years
                this.updateSelectedInvestmentStrategyAction({
                    updatedInvestmentStrategy: {
                        ...this.selectedInvestmentStrategy,
                        budgetYears: updatedBudgetYears
                    }
                });
            }
        }

        /**
         * 'Create as New Library' button has been clicked
         */
        onCreateAsNewLibrary() {
            // create a new CreateInvestmentStrategyDialogData object, setting the inflation/discount rates, description,
            // budgetOrder, and budgetYears using the selected investment strategy's data
            this.createInvestmentStrategyDialogData = {
                showDialog: true,
                inflationRate: this.selectedInvestmentStrategy.inflationRate,
                discountRate: this.selectedInvestmentStrategy.discountRate,
                description: this.selectedInvestmentStrategy.description,
                budgetOrder: this.selectedInvestmentStrategy.budgetOrder,
                budgetYears: this.selectedInvestmentStrategy.budgetYears
            };
        }

        /**
         * User has submitted CreateInvestmentStrategyDialog result
         * @param createdInvestmentStrategy The created investment strategy to save
         */
        onCreateInvestmentStrategy(createdInvestmentStrategy: InvestmentStrategy) {
            // reset createInvestmentStrategyDialogData
            this.createInvestmentStrategyDialogData = {...emptyCreateInvestmentStrategyDialogData};
            if (!isNil(createdInvestmentStrategy)) {
                // get the latest id from the list of investment strategies
                const latestId: number = getLatestPropertyValue('id', this.investmentStrategies);
                // set the created investment strategy's id by adding 1 to latestId (if present), otherwise use 1
                createdInvestmentStrategy.id = hasValue(latestId) ? latestId + 1 : 1;
                // set isBusy to true, then dispatch action to create investment strategy
                this.setIsBusyAction({isBusy: true});
                this.createInvestmentStrategyAction({createdInvestmentStrategy: createdInvestmentStrategy})
                    .then(() => this.setIsBusyAction({isBusy: false}))
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
        }

        /**
         * 'Update Library' button has been clicked
         */
        onUpdateLibrary() {
            // dispatch action to update selected investment strategy on the server
            this.updateInvestmentStrategyAction({updatedInvestmentStrategy: this.selectedInvestmentStrategy});
        }

        /**
         * 'Apply' button has been clicked
         */
        onApplyToScenario() {
            // TODO: add scenario pathway after defined
        }

        onClearInvestmentStrategySelection() {
            // dispatch an action to unselect the selected investment strategy
            this.selectInvestmentStrategyAction({investmentStrategyId: null});
        }
    }
</script>

<style>
    .investment-editor-container {
        height: 785px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .investment-editor-data-table {
        height: 280px;
        overflow-y: auto;
    }
</style>
