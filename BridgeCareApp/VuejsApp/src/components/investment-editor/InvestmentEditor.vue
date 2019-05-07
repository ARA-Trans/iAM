<template>
    <v-container fluid grid-list-xl>
        <div class="investment-editor-container">
            <v-layout column>
                <v-flex xs12>
                    <v-chip label v-show="fromScenario" color="indigo" text-color="white">
                        <v-icon left v-model="scenarioName">label</v-icon>Scenario name:
                        {{scenarioName}}
                    </v-chip>
                    <v-layout justify-center fill-height>
                        <v-flex xs3>
                            <v-btn v-if="!fromScenario" color="info" v-on:click="onNewLibrary">
                                New Library
                            </v-btn>
                            <v-select v-if="!hasSelectedInvestmentLibrary || !showLibrary"
                                      :items="investmentLibrariesSelectListItems"
                                      label="Select an Investment library" outline v-model="selectItemValue">
                            </v-select>
                            <v-text-field v-if="hasSelectedInvestmentLibrary && showLibrary" label="Library Name" append-icon="clear"
                                          v-model="selectedInvestmentLibrary.name"
                                          @click:append="onClearInvestmentLibrarySelection">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                    <v-layout v-if="hasSelectedInvestmentLibrary" justify-center fill-height>
                        <v-flex xs2>
                            <v-text-field label="Inflation Rate (%)" outline :mask="'##########'"
                                          v-model="selectedInvestmentLibrary.inflationRate"
                                          :disabled="!hasSelectedInvestmentLibrary">
                            </v-text-field>
                        </v-flex>
                        <v-flex xs2>
                            <v-text-field label="Discount Rate (%)" outline :mask="'##########'"
                                          v-model="selectedInvestmentLibrary.discountRate"
                                          :disabled="!hasSelectedInvestmentLibrary">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedInvestmentLibrary"></v-divider>
                <v-flex xs12 v-if="hasSelectedInvestmentLibrary">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-layout justify-space-between fill-height>
                                <v-btn color="info lighten-2" v-on:click="onEditBudgets">
                                    Edit Budgets
                                </v-btn>
                                <v-btn color="info" v-on:click="onAddBudgetYear"
                                       :disabled="selectedInvestmentLibrary.budgetOrder.length === 0">
                                    Add Year
                                </v-btn>
                                <v-btn color="info lighten-1" v-on:click="onAddBudgetYearsByRange"
                                       :disabled="selectedInvestmentLibrary.budgetOrder.length === 0">
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
                <v-divider v-if="hasSelectedInvestmentLibrary"></v-divider>
                <v-flex xs12 v-if="hasSelectedInvestmentLibrary">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-textarea no-resize outline full-width
                                        :label="selectedInvestmentLibrary.description === '' ? 'Description' : ''"
                                        v-model="selectedInvestmentLibrary.description">
                            </v-textarea>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn v-show="fromScenario" color="error lighten-1" v-on:click="onDiscardChanges" :disabled="!hasSelectedInvestmentLibrary">
                    Discard changes
                </v-btn>
                <v-btn color="info lighten-2" v-on:click="onCreateAsNewLibrary" :disabled="!hasSelectedInvestmentLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="fromScenario" color="info" v-on:click="onApplyToScenario" :disabled="!hasSelectedInvestmentLibrary">
                    Apply
                </v-btn>
                <v-btn v-show="!fromScenario" color="info lighten-1" v-on:click="onUpdateLibrary" :disabled="!hasSelectedInvestmentLibrary">
                    Update Library
                </v-btn>
            </v-layout>
        </v-footer>

        <CreateInvestmentLibraryDialog :dialogData="createInvestmentLibraryDialogData"
                                        @submit="onCreateInvestmentLibrary" />

        <SetRangeForAddingBudgetYearsDialog :showDialog="showSetRangeForAddingBudgetYearsDialog"
                                            @submit="onSubmitBudgetYearRange" />

        <EditBudgetsDialog :dialogData="editBudgetsDialogData" @submit="onSubmitEditedBudgets" />
        <v-flex xs12>
            <AppModalPopup :modalData="warning" @decision="onWarningModalDecision" />
        </v-flex>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Watch } from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {Action, State} from 'vuex-class';
    import CreateInvestmentLibraryDialog from './investment-editor-dialogs/CreateInvestmentLibraryDialog.vue';
    import SetRangeForAddingBudgetYearsDialog from './investment-editor-dialogs/SetRangeForAddingBudgetYearsDialog.vue';
    import EditBudgetsDialog from './investment-editor-dialogs/EditBudgetsDialog.vue';
    import {
        BudgetYearsGridData,
        EditedBudget,
        InvestmentLibrary,
        InvestmentLibraryBudgetYear
    } from '@/shared/models/iAM/investment';
    import {any, clone, contains, groupBy, isEmpty, isNil, keys, pluck, propEq, last, uniq, findIndex} from 'ramda';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {hasValue} from '@/shared/utils/has-value';
    import moment from 'moment';
    import {
        CreateInvestmentLibraryDialogData,
        emptyCreateInvestmentLibraryDialogData
    } from '@/shared/models/dialogs/investment-editor-dialogs/create-investment-library-dialog-data';
    import {
        EditBudgetsDialogData,
        emptyEditBudgetsDialogData
    } from '@/shared/models/dialogs/investment-editor-dialogs/edit-budgets-dialog-data';
    import {getLatestPropertyValue, getPropertyValues} from '@/shared/utils/getter-utils';
    import { sorter } from '@/shared/utils/sorter';
    import { Alert } from '@/shared/models/iAM/alert';
    import AppModalPopup from '@/shared/dialogs/AppModalPopup.vue';

    @Component({
        components: { CreateInvestmentLibraryDialog, SetRangeForAddingBudgetYearsDialog, EditBudgetsDialog, AppModalPopup}
    })
    export default class InvestmentEditor extends Vue {
        @State(state => state.investmentEditor.investmentLibraries) investmentLibraries: InvestmentLibrary[];
        @State(state => state.investmentEditor.selectedInvestmentLibrary) selectedInvestmentLibrary: InvestmentLibrary;
        @State(state => state.investmentEditor.loadedInvestmentLibrary) loadedInvestmentLibrary: InvestmentLibrary;
        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.investmentEditor.scenarioInvestmentLibrary) scenarioInvestmentLibrary: InvestmentLibrary[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getInvestmentLibraries') getInvestmentLibrariesAction: any;
        @Action('selectInvestmentLibrary') selectInvestmentLibraryAction: any;
        @Action('selectInvestmentForScenario') selectInvestmentForScenarioAction: any;
        @Action('createInvestmentLibrary') createInvestmentLibraryAction: any;
        @Action('updateInvestmentLibrary') updateInvestmentLibraryAction: any;
        @Action('updateScenarioInvestmentLibrary') updateInvestmentScenarioAction: any;
        @Action('updateSelectedInvestmentLibrary') updateSelectedInvestmentLibraryAction: any;
        @Action('setNavigation') setNavigationAction: any;
        @Action('getScenarioInvestmentLibrary') getInvestmentForScenarioAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        investmentLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string = '';
        hasSelectedInvestmentLibrary: boolean = false;
        budgetYearsGridHeaders: DataTableHeader[] = [
            {text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: ''}
        ];
        budgetYearsGridData: BudgetYearsGridData[] = [];
        selectedGridRows: BudgetYearsGridData[] = [];
        selectedBudgetYears: number[] = [];
        createInvestmentLibraryDialogData: CreateInvestmentLibraryDialogData = {...emptyCreateInvestmentLibraryDialogData};
        editBudgetsDialogData: EditBudgetsDialogData = {...emptyEditBudgetsDialogData};
        showSetRangeForAddingBudgetYearsDialog: boolean = false;
        selectedScenarioId: number =  0;

        warning: Alert = { showModal: false, heading: '', message: '', choice: false };
        fromScenario: boolean = false;
        showLibrary: boolean = false;
        scenarioName: string = '';

        beforeRouteEnter(to: any, from: any, next: any) {
            if (to.path === '/InvestmentEditor/FromScenario/') {
                next((vm: any) => {
                    vm.selectedScenarioId = to.query.simulationId;
                    vm.fromScenario = true;
                    vm.setNavigationAction([
                        {
                            text: 'Scenario dashboard',
                            to: {
                                path: '/Scenarios/', query: {}
                            }
                        },
                        {
                            text: 'Scenario editor',
                            to: {
                                path: '/EditScenario/', query: {
                                    simulationId: to.query.simulationId
                                }
                            }
                        },
                        {
                            text: 'Investment editor',
                            to: {
                                path: '/InvestmentEditor/FromScenario/', query: {
                                    simulationId: to.query.simulationId
                                }
                            }
                        }
                    ]);
                    vm.getInvestmentLibraries();
                    vm.getScenarioInvestmentLibrary();
                });
            }
            else {
                next((vm: any) => {
                    vm.fromScenario = false;
                    vm.showLibrary = true;
                    vm.setNavigationAction([]);
                    vm.onClearInvestmentLibrarySelection();
                    vm.getInvestmentLibraries();
                });
            }
        }
        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === '/InvestmentEditor/Library/') {
                this.fromScenario = false;
                this.showLibrary = true;
                this.hasSelectedInvestmentLibrary = false;
                this.onClearInvestmentLibrarySelection();
                this.getInvestmentLibraries();
            }
            next();
        }

        /**
         * Watcher: investmentLibraries
         */
        @Watch('investmentLibraries')
        onInvestmentLibrariesChanged(investmentLibraries: InvestmentLibrary[]) {
            // set the investmentLibrariesSelectListItems list using investmentLibraries list
            this.investmentLibrariesSelectListItems = investmentLibraries
                .map((investmentLibrary: InvestmentLibrary) => ({
                    text: investmentLibrary.name,
                    value: investmentLibrary.id.toString()
                }));
        }

        /**
         * Watcher: selectItemValue
         */
        @Watch('selectItemValue')
        onInvestmentLibrariesSelectItemChanged() {
            if (this.fromScenario === true) {
                if (hasValue(this.selectItemValue)) {
                    // parse selectItemValue as an integer
                    const id: number = parseInt(this.selectItemValue);
                    // dispatch selectInvestmentLibraryAction with id
                    this.selectInvestmentLibraryAction({ investmentLibraryId: id });
                }
            }
            else {
                if (hasValue(this.selectItemValue) && this.selectedInvestmentLibrary.id === 0) {
                    // parse selectItemValue as an integer
                    const id: number = parseInt(this.selectItemValue);
                    // dispatch selectInvestmentLibraryAction with id
                    this.selectInvestmentLibraryAction({ investmentLibraryId: id });
                } else if (!hasValue(this.selectItemValue) && this.selectedInvestmentLibrary.id !== 0) {
                    // dispatch selectInvestmentLibraryAction with null
                    this.selectInvestmentLibraryAction({ investmentLibraryId: null });
                }
            }
        }

        /**
         * Watcher: selectedInvestmentLibrary
         */
        @Watch('selectedInvestmentLibrary')
        onSelectedInvestmentLibraryChanged() {
            // reset the selected grid rows
            this.selectedGridRows = [];
            if (this.fromScenario === false) {
                if (this.selectedInvestmentLibrary.id !== 0) {
                    if (!hasValue(this.selectItemValue)) {
                        // set selectItemValue with the id of selectedInvestmentLibrary
                        this.selectItemValue = this.selectedInvestmentLibrary.id.toString();
                    }
                    // set hasSelectedInvestmentLibrary to true
                    this.hasSelectedInvestmentLibrary = true;
                    this.showLibrary = true;
                    if (!hasValue(this.selectedInvestmentLibrary.budgetOrder) && hasValue(this.selectedInvestmentLibrary.budgetYears)) {
                        // set the budget order for the selected investment library using the it's list of budget years
                        this.setSelectedInvestmentLibraryBudgetOrder();
                    } else {
                        // set the grid headers
                        this.setGridHeaders();
                        // set the grid data
                        this.setGridData();
                    }

                } else {
                    // reset selectItemValue
                    this.selectItemValue = '';
                    // reset hasSelectedInvestmentLibrary
                    this.hasSelectedInvestmentLibrary = false;
                    this.showLibrary = false;
                    // reset grid data
                    this.budgetYearsGridData = [];
                }
            }
            else {
                this.hasSelectedInvestmentLibrary = true;
                this.showLibrary = false;
                this.setGridHeaders();
                // set the grid data
                this.setGridData();
            }
        }

        /**
         * Watcher: selectedGridRows
         */
        @Watch('selectedGridRows')
        onSelectedGridRowsChanged() {
            // set the selectedBudgetYears with the years of the selected investment library budget years in selectedGridRows
            this.selectedBudgetYears = getPropertyValues('year', this.selectedGridRows) as number[];
        }


        getInvestmentLibraries() {
            // set isBusy to true, then dispatch action to get all investment libraries
            this.getInvestmentLibrariesAction()
                .catch((error: any) => {
                    this.warning.showModal = true;
                    this.warning.heading = 'Error';
                    this.warning.choice = false;
                    this.warning.message = 'Failed to load the libraries';
                });
        }

        beforeDestroy() {
            // clear the selected investment library
            this.onClearInvestmentLibrarySelection();
        }

        /**
         * Sets the selected investment library's budget order using it's budget years
         */
        setSelectedInvestmentLibraryBudgetOrder() {
            // set a new budget order for the selected investment library
           const newBudgetOrder = sorter(
               getPropertyValues('budgetName', this.selectedInvestmentLibrary.budgetYears)
           ) as string[];
            // dispatch action to update the selected investment library with the new budget order
            this.updateSelectedInvestmentLibraryAction({
                updatedInvestmentLibrary: {
                    ...this.selectedInvestmentLibrary,
                    budgetOrder: newBudgetOrder
                }
            });
        }

        /**
         * Sets the data table headers
         */
        setGridHeaders() {
            // set the grid headers for each budget in the selected investment library's budget order list
            const budgetHeaders: DataTableHeader[] = this.selectedInvestmentLibrary.budgetOrder
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
            // group the selected investment library's budget years by year
            const groupBudgetYearsByYear = groupBy((budgetYear: InvestmentLibraryBudgetYear) => budgetYear.year.toString());
            const groupedBudgetYears = groupBudgetYearsByYear(this.selectedInvestmentLibrary.budgetYears);
            // uses the group keys of the grouped budget years to loop over each group and create a grid data row for each year
            keys(groupedBudgetYears).forEach((year: any) => {
                // create a grid data row and set the current year
                const gridDataRow: BudgetYearsGridData = {
                    year: parseInt(year)
                };
                // get the group of budget years at the given year key
                const budgetYears: InvestmentLibraryBudgetYear[] = groupedBudgetYears[year];
                // add the budget year budget amounts to the gridDataRow using the selected investment library's budget order
                const budgetOrder = this.selectedInvestmentLibrary.budgetOrder;
                for (let i = 0; i < budgetOrder.length; i++) {
                    const budgetYear: InvestmentLibraryBudgetYear = budgetYears
                        .find((by: InvestmentLibraryBudgetYear) =>
                            by.budgetName === budgetOrder[i]
                        ) as InvestmentLibraryBudgetYear;
                    gridDataRow[budgetOrder[i]] = hasValue(budgetYear) ? budgetYear.budgetAmount : 0;
                }
                // add the gridDataRow to the budgetYearsGridData list
                this.budgetYearsGridData.push(gridDataRow);
            });
        }

        /**
         * 'New Investment Library' button has been clicked
         */
        onNewLibrary() {
            // create new CreateInvestmentLibraryDialogData object
            this.createInvestmentLibraryDialogData = {
                ...emptyCreateInvestmentLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * The 'Add Budget Year' button has been clicked
         */
        onAddBudgetYear() {
            // get the latest year from the selected investment library's budget years
            const latestYear: number = getLatestPropertyValue('year', this.selectedInvestmentLibrary.budgetYears);
            // set next year as latestYear + 1 (if present), otherwise set it as current year
            const nextYear = hasValue(latestYear) ? latestYear + 1 : moment().year();
            // get the latest id from the selected investment library's budget years & deleted budget year ids
            const latestId: number = last(sorter([
                ...getPropertyValues('id', this.selectedInvestmentLibrary.budgetYears),
                ...this.selectedInvestmentLibrary.deletedBudgetYearIds
            ]) as number[]) as number;
            // set nextId as latestId + 1 (if present), otherwise set it as 1
            let nextId = hasValue(latestId) ? latestId + 1 : 1;
            // create a budget year for each budget
            const budgets = this.selectedInvestmentLibrary.budgetOrder;
            const newBudgetYears: InvestmentLibraryBudgetYear[] = budgets.map((budget: string) => {
                const newBudgetYear: InvestmentLibraryBudgetYear = {
                    investmentLibraryId: this.selectedInvestmentLibrary.id,
                    id: nextId,
                    year: nextYear,
                    budgetName: budget,
                    budgetAmount: 0
                };
                nextId++;
                return newBudgetYear;
            });
            // add the new budget years to the selected investment library's list of budget
            const editedBudgetYears = [...this.selectedInvestmentLibrary.budgetYears, ...newBudgetYears];
            // dispatch action to update the selected investment library's budget years
            this.updateSelectedInvestmentLibraryAction({
                updatedInvestmentLibrary: {
                    ...this.selectedInvestmentLibrary,
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
                // get the latest year from the selected investment library's budget years
                const latestYear = getLatestPropertyValue('year', this.selectedInvestmentLibrary.budgetYears);
                // set start year as latestYear + 1 (if present), otherwise set it as current year
                const startYear: number = hasValue(latestYear) ? latestYear + 1 : moment().year();
                // set end year as startYear + range
                const endYear = moment().year(startYear).add(range, 'years').year();
                // get the latest id from the selected investment library's budget years & deleted budget year ids
                const latestId: number = last(sorter([
                    ...getPropertyValues('id', this.selectedInvestmentLibrary.budgetYears),
                    ...this.selectedInvestmentLibrary.deletedBudgetYearIds
                ]) as number[]) as number;
                // set next id as latestId + 1 (if present), otherwise set it as 1
                let nextId = hasValue(latestId) ? latestId + 1 : 1;
                // create a list for new budget years
                const newBudgetYears: InvestmentLibraryBudgetYear[] = [];
                for (let currentYear = startYear; currentYear < endYear; currentYear++) {
                    // for each year create budget years for each budget
                    this.selectedInvestmentLibrary.budgetOrder.forEach((budget: string) => {
                        newBudgetYears.push({
                            investmentLibraryId: this.selectedInvestmentLibrary.id,
                            id: nextId,
                            year: currentYear,
                            budgetName: budget,
                            budgetAmount: 0
                        });
                        nextId++;
                    });
                }
                // add the new budget years to the selected investment library's list of budget
                const editedBudgetYears = [...this.selectedInvestmentLibrary.budgetYears, ...newBudgetYears];
                // dispatch action to update the selected investment library's budget years
                this.updateSelectedInvestmentLibraryAction({
                    updatedInvestmentLibrary: {
                        ...this.selectedInvestmentLibrary,
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
            const budgetYearsToDelete: InvestmentLibraryBudgetYear[] = this.selectedInvestmentLibrary.budgetYears
                .filter((budgetYear: InvestmentLibraryBudgetYear) => contains(budgetYear.year, this.selectedBudgetYears));
            // get the ids of the budget years to delete
            const deletedBudgetYearIds: number[] = getPropertyValues('id', budgetYearsToDelete);
            // add the ids of the budget years to delete to the current list of ids of budget years to delete
            const editedDeletedBudgetYearIds = uniq([...this.selectedInvestmentLibrary.deletedBudgetYearIds, ...deletedBudgetYearIds]);
            // filter the copy's existing budget years, removing any budget years where they have been marked as deleted
            const editedBudgetYears = this.selectedInvestmentLibrary.budgetYears
                .filter((budgetYear: InvestmentLibraryBudgetYear) =>
                    !contains(budgetYear.id, editedDeletedBudgetYearIds)
                );
            // dispatch action to update the selected investment library's budget years & deleted budget year ids
            this.updateSelectedInvestmentLibraryAction({
                updatedInvestmentLibrary: {
                    ...this.selectedInvestmentLibrary,
                    budgetYears: editedBudgetYears,
                    deletedBudgetYearIds: editedDeletedBudgetYearIds
                }
            });
        }

        /**
         * 'Edit Budgets' button has been clicked
         */
        onEditBudgets() {
            // create new EditBudgetsDialogData object and set the budgets using selectedInvestmentLibrary.budgetOrder
            this.editBudgetsDialogData = {
                showDialog: true,
                budgets: this.selectedInvestmentLibrary.budgetOrder
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
                    ...this.selectedInvestmentLibrary.deletedBudgetYearIds,
                    ...getPropertyValues('id', this.selectedInvestmentLibrary.budgetYears
                        .filter((budgetYear: InvestmentLibraryBudgetYear) =>
                            !contains(budgetYear.budgetName, previousBudgets)
                        )) as number[]
                ];
                // get the remaining budget years by filtering out budget years that were marked as deleted
                const remainingBudgetYears = this.selectedInvestmentLibrary.budgetYears
                    .filter((budgetYear: InvestmentLibraryBudgetYear) =>
                        !contains(budgetYear.id, editedDeletedBudgetYearIds)
                    );
                // create a list for updated budget years
                const editedBudgetYears: InvestmentLibraryBudgetYear[] = [];
                if (!isEmpty(editedBudgets)) {
                    // get all budget years' year values from the remaining budget years
                    const years = getPropertyValues('year', remainingBudgetYears);
                    // get the latest id from the selected investment library's budget years & deleted budget year ids
                    const latestId: number = last(sorter([
                        ...getPropertyValues('id', remainingBudgetYears),
                        ...editedDeletedBudgetYearIds
                    ]) as number[]) as number;
                    // set next id as latestId + 1 (if present), otherwise set it as 1
                    let nextId = hasValue(latestId) ? latestId + 1 : 1;
                    years.forEach((year: number) => {
                        // get all budget years for the current year
                        const currentYearBudgetYears = remainingBudgetYears
                            .filter((budgetYear: InvestmentLibraryBudgetYear) => budgetYear.year === year);
                        editedBudgets.forEach((editedBudget: EditedBudget) => {
                            if (editedBudget.isNew) {
                                // create a new budget year for the new budget
                                editedBudgetYears.push({
                                    investmentLibraryId: this.selectedInvestmentLibrary.id,
                                    id: nextId,
                                    year: year,
                                    budgetName: editedBudget.name,
                                    budgetAmount: 0,
                                });
                                nextId++;
                            } else {
                                // find the budget year that has a matching budgetName with the editedBudget.previousName
                                const updatedBudget = currentYearBudgetYears
                                    .find((budgetYear: InvestmentLibraryBudgetYear) =>
                                        budgetYear.budgetName === editedBudget.previousName
                                    ) as InvestmentLibraryBudgetYear;
                                // update the budget year's budgetName with the editedBudget.name
                                editedBudgetYears.push({
                                    ...updatedBudget,
                                    budgetName: editedBudget.name
                                });
                            }
                        });
                    });
                }
                // dispatch action to update the selected investment library's budget order, budget years, & deleted
                // budget year ids
                this.updateSelectedInvestmentLibraryAction({
                    updatedInvestmentLibrary: {
                        ...this.selectedInvestmentLibrary,
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

            if (any(propEq('year', year), this.selectedInvestmentLibrary.budgetYears) &&
                any(propEq('budgetName', budgetName), this.selectedInvestmentLibrary.budgetYears)) {
                // get the selected investment library's list of budget years
                const updatedBudgetYears = this.selectedInvestmentLibrary.budgetYears;
                // find the index of the updated budget year in the list of budget years
                const index: number = findIndex(
                    (budgetYear: InvestmentLibraryBudgetYear) =>
                        budgetYear.year === year && budgetYear.budgetName === budgetName,
                    updatedBudgetYears
                );
                // update the budget year's budget amount with the specified amount at the given index of the list of budget years
                updatedBudgetYears[index].budgetAmount = amount;
                // dispatch action to update the selected investment library's budget years
                this.updateSelectedInvestmentLibraryAction({
                    updatedInvestmentLibrary: {
                        ...this.selectedInvestmentLibrary,
                        budgetYears: updatedBudgetYears
                    }
                });
            }
        }

        /**
         * 'Create as New Library' button has been clicked
         */
        onCreateAsNewLibrary() {
            // create a new CreateInvestmentLibraryDialogData object, setting the inflation/discount rates, description,
            // budgetOrder, and budgetYears using the selected investment library's data
            this.createInvestmentLibraryDialogData = {
                showDialog: true,
                inflationRate: this.selectedInvestmentLibrary.inflationRate,
                discountRate: this.selectedInvestmentLibrary.discountRate,
                description: this.selectedInvestmentLibrary.description,
                budgetOrder: this.selectedInvestmentLibrary.budgetOrder,
                budgetYears: this.selectedInvestmentLibrary.budgetYears
            };
        }

        /**
         * User has submitted CreateInvestmentLibraryDialog result
         * @param createdInvestmentLibrary The created investment library to save
         */
        onCreateInvestmentLibrary(createdInvestmentLibrary: InvestmentLibrary) {
            // reset createInvestmentLibraryDialogData
            this.createInvestmentLibraryDialogData = {...emptyCreateInvestmentLibraryDialogData};
            if (!isNil(createdInvestmentLibrary)) {
                // get the latest id from the list of investment libraries
                const latestId: number = getLatestPropertyValue('id', this.investmentLibraries);
                // set the created investment library's id by adding 1 to latestId (if present), otherwise use 1
                createdInvestmentLibrary.id = hasValue(latestId) ? latestId + 1 : 1;
                // set isBusy to true, then dispatch action to create investment library
                this.setIsBusyAction({ isBusy: true });
                this.createInvestmentLibraryAction({ createdInvestmentLibrary: createdInvestmentLibrary })
                    .then(() => {
                        this.setIsBusyAction({ isBusy: false });
                        this.setSuccessMessageAction({ message: 'New library created successfully' });
                    })
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        this.setErrorMessageAction({ message: 'Could not create the new library' });
                    });
            }
        }

        /**
         * 'Update Library' button has been clicked
         */
        onUpdateLibrary() {
            this.setIsBusyAction({ isBusy: true });
            // dispatch action to update selected investment library on the server
            this.updateInvestmentLibraryAction({ updatedInvestmentLibrary: this.selectedInvestmentLibrary })
                .then(() => {
                    this.setIsBusyAction({ isBusy: false });
                    this.setSuccessMessageAction({ message: 'Library updated successfully' });
                })
                .catch(() => {
                    this.setIsBusyAction({ isBusy: false });
                    this.setErrorMessageAction({ message: 'Library failed to update' });
                });
        }

        /**
         * 'Apply' button has been clicked
         */
        onApplyToScenario() {
            this.setIsBusyAction({ isBusy: true });
            // dispatch action to update selected investment library on the server
            this.updateInvestmentScenarioAction({ updatedInvestmentScenario: this.selectedInvestmentLibrary })
                .then((response: any) => {
                    this.setIsBusyAction({ isBusy: false });
                    this.warning.showModal = true;
                    this.warning.heading = 'Success';
                    this.warning.choice = false;
                    if (response == 200) {
                        this.warning.message = 'Investment scenario updated successfully';
                    }
                })
                .catch(() => {
                    // This catch will be called if something goes wrong in mutation. Even though the service call is successfull
                    this.setIsBusyAction({ isBusy: false });
                    this.setErrorMessageAction({ message: 'Failed to update investment scenario' });
                });
        }

        onClearInvestmentLibrarySelection() {
            // dispatch an action to unselect the selected investment library
            this.selectInvestmentLibraryAction({investmentLibraryId: null});
        }

        onWarningModalDecision(value: boolean) {
            this.warning.showModal = false;
        }

        onDiscardChanges() {
            this.selectItemValue = '';
            this.getInvestmentForScenario();
        }

        getScenarioInvestmentLibrary() {
            this.setIsBusyAction({ isBusy: true });
            this.getInvestmentForScenarioAction({ selectedScenario: this.selectedScenarioId })
                .then(() => {

                    this.selectInvestmentForScenarioAction({ investmentLibraryId: this.investmentForScenario[0].id })
                        .then(() => {
                            this.setIsBusyAction({ isBusy: false });
                        });
                    this.scenarioName = this.investmentForScenario[0].name;
                });
        }
    }
</script>

<style>
    .investment-editor-container {
        height: 775px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .investment-editor-data-table {
        height: 300px;
        overflow-y: auto;
    }
</style>
