<template>
    <v-container fluid grid-list-xl>
        <div class="investment-editor-container">
            <v-layout column>
                <v-flex xs12>
                    <v-layout justify-center fill-height>
                        <v-flex xs2 v-if="selectedScenarioId === 0">
                            <v-btn color="info" v-on:click="onNewLibrary">
                                New Library
                            </v-btn>
                        </v-flex>
                        <v-flex xs3>
                            <v-select v-if="!hasSelectedInvestmentLibrary || selectedScenarioId > 0"
                                      :items="investmentLibrariesSelectListItems"
                                      label="Select an Investment library" outline v-model="selectItemValue">
                            </v-select>
                            <v-text-field v-if="hasSelectedInvestmentLibrary && selectedScenarioId === 0"
                                          label="Library Name" append-icon="clear" v-model="selectedInvestmentLibrary.name"
                                          @click:append="onClearSelectedInvestmentLibrary">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                    <v-divider v-if="hasSelectedInvestmentLibrary"></v-divider>
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
                <v-flex xs12 v-if="hasSelectedInvestmentLibrary">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-layout justify-space-between fill-height>
                                <v-btn color="info" v-on:click="onEditBudgets">
                                    Edit Budgets
                                </v-btn>
                                <v-btn color="info lighten-1" v-on:click="onAddBudgetYear"
                                       :disabled="selectedInvestmentLibrary.budgetOrder.length === 0">
                                    Add Year
                                </v-btn>
                                <v-btn color="info lighten-1" v-on:click="onAddBudgetYearsByRange"
                                       :disabled="selectedInvestmentLibrary.budgetOrder.length === 0">
                                    Add Years by Range
                                </v-btn>
                                <v-btn color="error lighten-1" v-on:click="onDeleteBudgetYears"
                                       :disabled="selectedGridRows.length === 0">
                                    Delete Budget Year(s)
                                </v-btn>
                            </v-layout>
                        </v-flex>
                    </v-layout>
                    <v-layout justify-center fill-height>
                        <v-flex xs8>
                            <v-card>
                                <v-data-table :headers="budgetYearsGridHeaders" :items="budgetYearsGridData"
                                                v-model="selectedGridRows" select-all item-key="year"
                                                class="elevation-1 fixed-header v-table__overflow">
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
                            </v-card>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedInvestmentLibrary"></v-divider>
                <v-flex xs12 v-if="hasSelectedInvestmentLibrary && (scenarioInvestmentLibrary === null || selectedInvestmentLibrary.id !== scenarioInvestmentLibrary.id)">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-textarea rows="4" no-resize outline label="Description"
                                        v-model="selectedInvestmentLibrary.description">
                            </v-textarea>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn v-show="selectedScenarioId > 0" color="info" v-on:click="onApplyToScenario"
                       :disabled="!hasSelectedInvestmentLibrary">
                    Apply
                </v-btn>
                <v-btn v-show="selectedScenarioId === 0" color="info" v-on:click="onUpdateLibrary"
                       :disabled="!hasSelectedInvestmentLibrary">
                    Update Library
                </v-btn>
                <v-btn color="info lighten-1" v-on:click="onCreateAsNewLibrary" :disabled="!hasSelectedInvestmentLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId > 0" color="error lighten-1" v-on:click="onDiscardChanges"
                       :disabled="!hasSelectedInvestmentLibrary">
                    Discard changes
                </v-btn>
            </v-layout>
        </v-footer>

        <CreateInvestmentLibraryDialog :dialogData="createInvestmentLibraryDialogData"
                                       @submit="onCreateInvestmentLibrary" />

        <SetRangeForAddingBudgetYearsDialog :showDialog="showSetRangeForAddingBudgetYearsDialog"
                                            @submit="onSubmitBudgetYearRange" />

        <EditBudgetsDialog :dialogData="editBudgetsDialogData" @submit="onSubmitEditedBudgets" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Watch } from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import { Action, State } from 'vuex-class';
    import CreateInvestmentLibraryDialog from './investment-editor-dialogs/CreateInvestmentLibraryDialog.vue';
    import SetRangeForAddingBudgetYearsDialog from './investment-editor-dialogs/SetRangeForAddingBudgetYearsDialog.vue';
    import EditBudgetsDialog from '../../shared/modals/EditBudgetsDialog.vue';
    import {
        BudgetYearsGridData,
        emptyInvestmentLibrary,
        InvestmentLibrary,
        InvestmentLibraryBudgetYear
    } from '@/shared/models/iAM/investment';
    import { any, clone, groupBy, isEmpty, isNil, keys, propEq, contains } from 'ramda';
    import { SelectItem } from '@/shared/models/vue/select-item';
    import { DataTableHeader } from '@/shared/models/vue/data-table-header';
    import { hasValue } from '@/shared/utils/has-value-util';
    import moment from 'moment';
    import {
        CreateInvestmentLibraryDialogData,
        emptyCreateInvestmentLibraryDialogData
    } from '@/shared/models/modals/create-investment-library-dialog-data';
    import {
        EditBudgetsDialogData,
        emptyEditBudgetsDialogData
    } from '@/shared/models/modals/edit-budgets-dialog';
    import { getLatestPropertyValue, getPropertyValues } from '@/shared/utils/getter-utils';
    import { sortByProperty, sorter } from '@/shared/utils/sorter-utils';
    import {EditedBudget} from '@/shared/models/modals/edit-budgets-dialog';
    const ObjectID = require('bson-objectid');

    @Component({
        components: { CreateInvestmentLibraryDialog, SetRangeForAddingBudgetYearsDialog, EditBudgetsDialog }
    })
    export default class InvestmentEditor extends Vue {
        @State(state => state.investmentEditor.investmentLibraries) stateInvestmentLibraries: InvestmentLibrary[];
        @State(state => state.investmentEditor.selectedInvestmentLibrary) stateSelectedInvestmentLibrary: InvestmentLibrary;
        @State(state => state.investmentEditor.scenarioInvestmentLibrary) stateScenarioInvestmentLibrary: InvestmentLibrary;

        @Action('setNavigation') setNavigationAction: any;
        @Action('getInvestmentLibraries') getInvestmentLibrariesAction: any;
        @Action('getScenarioInvestmentLibrary') getScenarioInvestmentLibraryAction: any;
        @Action('selectInvestmentLibrary') selectInvestmentLibraryAction: any;
        @Action('updateSelectedInvestmentLibrary') updateSelectedInvestmentLibraryAction: any;
        @Action('createInvestmentLibrary') createInvestmentLibraryAction: any;
        @Action('updateInvestmentLibrary') updateInvestmentLibraryAction: any;
        @Action('saveScenarioInvestmentLibrary') saveScenarioInvestmentLibraryAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        investmentLibraries: InvestmentLibrary[] = [];
        selectedInvestmentLibrary: InvestmentLibrary = clone(emptyInvestmentLibrary);
        scenarioInvestmentLibrary: InvestmentLibrary = clone(emptyInvestmentLibrary);

        selectedScenarioId: number = 0;
        hasSelectedInvestmentLibrary: boolean = false;
        investmentLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string = '';
        budgetYearsGridHeaders: DataTableHeader[] = [
            { text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: '' }
        ];
        budgetYearsGridData: BudgetYearsGridData[] = [];
        selectedGridRows: BudgetYearsGridData[] = [];
        selectedBudgetYears: number[] = [];
        createInvestmentLibraryDialogData: CreateInvestmentLibraryDialogData = clone(emptyCreateInvestmentLibraryDialogData);
        editBudgetsDialogData: EditBudgetsDialogData = clone(emptyEditBudgetsDialogData);
        showSetRangeForAddingBudgetYearsDialog: boolean = false;

        /**
         * Sets component UI properties that triggers cascading UI updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/InvestmentEditor/FromScenario/') {
                    vm.selectedScenarioId = isNaN(parseInt(to.query.selectedScenarioId)) ? 0 : parseInt(to.query.selectedScenarioId);
                    if (vm.selectedScenarioId === 0) {
                        // set 'no selected scenario' error message, then redirect user to Scenarios UI
                        vm.setErrorMessageAction({ message: 'Found no selected scenario for edit' });
                        vm.$router.push('/Scenarios/');
                    }
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
                                path: '/EditScenario/', query: { selectedScenarioId: to.query.selectedScenarioId, simulationName: to.query.simulationName }
                            }
                        },
                        {
                            text: 'Investment editor',
                            to: {
                                path: '/InvestmentEditor/FromScenario/', query: { selectedScenarioId: to.query.selectedScenarioId, simulationName: to.query.simulationName }
                            }
                        }
                    ]);
                }
                vm.onClearSelectedInvestmentLibrary();
                setTimeout(() => {
                    vm.getInvestmentLibrariesAction()
                        .then(() => {
                            if (vm.selectedScenarioId > 0) {
                                vm.getScenarioInvestmentLibraryAction({ selectedScenarioId: vm.selectedScenarioId });
                            }
                        });
                });
            });


        }
        /**
         * Resets component UI properties that triggers cascading UI updates
         */
        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === '/InvestmentEditor/Library/') {
                this.selectedScenarioId = 0;
                this.onClearSelectedInvestmentLibrary();
                next();
            }
        }

        /**
         * Sets investmentLibraries object = copy of stateInvestmentLibraries array
         */
        @Watch('stateInvestmentLibraries')
        onStateInvestmentLibrariesChanged() {
            this.investmentLibraries = clone(this.stateInvestmentLibraries);
        }

        /**
         * Sets selectedInvestmentLibrary object = copy of stateSelectedInvestmentLibrary object
         */
        @Watch('stateSelectedInvestmentLibrary')
        onStateSelectedInvestmentLibraryChanged() {
            this.selectedInvestmentLibrary = clone(this.stateSelectedInvestmentLibrary);
        }

        /**
         * Sets scenarioInvestmentLibrary object = stateScenarioInvestmentLibrary object
         */
        @Watch('stateScenarioInvestmentLibrary')
        onStateScenarioInvestmentLibraryChanged() {
            this.scenarioInvestmentLibrary = clone(this.stateScenarioInvestmentLibrary);
        }

        /**
         * Sets the investmentLibrariesSelectListItems using the investmentLibraries from state
         */
        @Watch('investmentLibraries')
        onInvestmentLibrariesChanged() {
            this.investmentLibrariesSelectListItems = this.investmentLibraries
                .map((investmentLibrary: InvestmentLibrary) => ({
                    text: investmentLibrary.name,
                    value: investmentLibrary.id.toString()
                }));
        }

        /**
         * Sets the selected investment library based on selectItemValue
         */
        @Watch('selectItemValue')
        onInvestmentLibrariesSelectItemChanged() {
            this.selectInvestmentLibraryAction({ investmentLibraryId: this.selectItemValue });
        }

        /**
         * Sets/resets the component UI properties reliant on a selected investment library
         */
        @Watch('selectedInvestmentLibrary')
        onSelectedInvestmentLibraryChanged() {
            this.selectedGridRows = [];

            if (hasValue(this.selectedInvestmentLibrary) && this.selectedInvestmentLibrary.id !== 0) {
                this.hasSelectedInvestmentLibrary = true;

                if (!hasValue(this.selectedInvestmentLibrary.budgetOrder) &&
                    hasValue(this.selectedInvestmentLibrary.budgetYears)) {
                    this.setSelectedInvestmentLibraryBudgetOrder();
                } else {
                    this.setGridHeaders();
                    this.setGridData();
                }
            } else {
                this.hasSelectedInvestmentLibrary = false;
                this.budgetYearsGridData = [];
            }
        }

        /**
         * Sets the selected budget years using the year values from the selectedGridRows list
         */
        @Watch('selectedGridRows')
        onSelectedGridRowsChanged() {
            this.selectedBudgetYears = getPropertyValues('year', this.selectedGridRows) as number[];
        }

        /**
         * Sets the selected investment library's budget order using it's budget years
         */
        setSelectedInvestmentLibraryBudgetOrder() {
            this.selectedInvestmentLibrary.budgetOrder = sorter(
                getPropertyValues('budgetName', this.selectedInvestmentLibrary.budgetYears)
            ) as string[];

            this.updateSelectedInvestmentLibraryAction({
                updatedSelectedInvestmentLibrary: this.selectedInvestmentLibrary
            });
        }

        /**
         * Sets the data table headers using the selected investment library's budgetOrder
         */
        setGridHeaders() {
            const budgetHeaders: DataTableHeader[] = this.selectedInvestmentLibrary.budgetOrder
                .map((budgetName: string) => ({
                    text: budgetName,
                    value: budgetName,
                    sortable: true,
                    align: 'left',
                    class: '',
                    width: ''
                }) as DataTableHeader);
            this.budgetYearsGridHeaders = [this.budgetYearsGridHeaders[0], ...budgetHeaders];
        }

        /**
         * Sets the data table rows using the selected investment library's budgetYears
         */
        setGridData() {

            this.budgetYearsGridData = [];

            const groupBudgetYearsByYear = groupBy((budgetYear: InvestmentLibraryBudgetYear) => budgetYear.year.toString());
            const groupedBudgetYears = groupBudgetYearsByYear(this.selectedInvestmentLibrary.budgetYears);

            keys(groupedBudgetYears).forEach((year: any) => {
                const gridDataRow: BudgetYearsGridData = {
                    year: parseInt(year)
                };

                const budgetYears: InvestmentLibraryBudgetYear[] = groupedBudgetYears[year];

                const budgets: string[] = this.selectedInvestmentLibrary.budgetOrder;
                for (let i = 0; i < budgets.length; i++) {
                    const budgetYear: InvestmentLibraryBudgetYear = budgetYears
                        .find((bY: InvestmentLibraryBudgetYear) =>
                            bY.budgetName === budgets[i]
                        ) as InvestmentLibraryBudgetYear;

                    gridDataRow[budgets[i]] = hasValue(budgetYear) ? budgetYear.budgetAmount : 0;
                }
                this.budgetYearsGridData.push(gridDataRow);
            });
        }

        /**
         * Clears the selected investment library by setting selectItemValue to an empty value or 0
         */
        onClearSelectedInvestmentLibrary() {
            this.selectItemValue = hasValue(this.selectItemValue) ? '' : '0';
        }

        /**
         * Shows the CreateInvestmentLibraryDialog to allow a user to create a new investment library
         */
        onNewLibrary() {
            this.createInvestmentLibraryDialogData = {
                ...emptyCreateInvestmentLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * Adds a budget year for each of the selected investment library's budgets (budgetOrder) for the next year (or
         * current year if there are no existing budget year entries)
         */
        onAddBudgetYear() {
            const latestYear: number = getLatestPropertyValue('year', this.selectedInvestmentLibrary.budgetYears);
            const nextYear = hasValue(latestYear) ? latestYear + 1 : moment().year();

            const newBudgetYears: InvestmentLibraryBudgetYear[] = this.selectedInvestmentLibrary.budgetOrder
                .map((budget: string) => {
                    const newBudgetYear: InvestmentLibraryBudgetYear = {
                        id: ObjectID.generate(),
                        year: nextYear,
                        budgetName: budget,
                        budgetAmount: 0
                    };
                    return newBudgetYear;
                });

            this.selectedInvestmentLibrary.budgetYears = [...this.selectedInvestmentLibrary.budgetYears, ...newBudgetYears];

            this.updateSelectedInvestmentLibraryAction({ updatedSelectedInvestmentLibrary: this.selectedInvestmentLibrary });
        }

        /**
         * Shows the SetRangeForAddingBudgetYearsDialog to allow a user to add a range of years to create budget years
         * for
         */
        onAddBudgetYearsByRange() {
            this.showSetRangeForAddingBudgetYearsDialog = true;
        }

        /**
         * Adds budget years for the range of years submitted by a user via the SetRangeForAddingBudgetYearsDialog for
         * each of the selected investment library's budgets
         */
        onSubmitBudgetYearRange(range: number) {
            this.showSetRangeForAddingBudgetYearsDialog = false;

            if (range > 0) {
                const latestYear = getLatestPropertyValue('year', this.selectedInvestmentLibrary.budgetYears);
                const startYear: number = hasValue(latestYear) ? latestYear + 1 : moment().year();
                const endYear = moment().year(startYear).add(range, 'years').year();

                const newBudgetYears: InvestmentLibraryBudgetYear[] = [];
                for (let currentYear = startYear; currentYear < endYear; currentYear++) {
                    this.selectedInvestmentLibrary.budgetOrder.forEach((budget: string) => {
                        newBudgetYears.push({
                            id: ObjectID.generate(),
                            year: currentYear,
                            budgetName: budget,
                            budgetAmount: 0
                        });
                    });
                }

                this.selectedInvestmentLibrary.budgetYears = [
                    ...this.selectedInvestmentLibrary.budgetYears, ...newBudgetYears
                ];

                this.updateSelectedInvestmentLibraryAction({ updatedSelectedInvestmentLibrary: this.selectedInvestmentLibrary });
            }
        }

        /**
         * Removes budget years from the selected investment library's budgetYears list based on the selectedBudgetYears
         * list
         */
        onDeleteBudgetYears() {
            this.selectedInvestmentLibrary.budgetYears = this.selectedInvestmentLibrary.budgetYears
                .filter((budgetYear: InvestmentLibraryBudgetYear) =>
                    !contains(budgetYear.year, this.selectedBudgetYears)
                );

            this.updateSelectedInvestmentLibraryAction({ updatedSelectedInvestmentLibrary: this.selectedInvestmentLibrary });
        }

        /**
         * Shows the EditBudgetsDialog and passes in the selected investment library's budgets to allow a user to edit
         * the budget order, budget names, or to add/remove budgets
         */
        onEditBudgets() {
            this.editBudgetsDialogData = {
                showDialog: true,
                budgets: this.selectedInvestmentLibrary.budgetOrder,
                canOrderBudgets: true
            };
        }

        /**
         * Updates the selected investment library's budgetOrder and budgetYears based on a user's submitted
         * EditBudgetsDialog result
         * @param editedBudgets List of edited budgets
         */
        onSubmitEditedBudgets(editedBudgets: EditedBudget[]) {
            this.editBudgetsDialogData = clone(emptyEditBudgetsDialogData);

            if (!isNil(editedBudgets)) {
                const remainingBudgets: string[] = getPropertyValues('previousName', editedBudgets
                    .filter((budget: EditedBudget) => !budget.isNew));

                const deletedBudgetYearIds = [
                    ...getPropertyValues('id', this.selectedInvestmentLibrary.budgetYears
                        .filter((budgetYear: InvestmentLibraryBudgetYear) =>
                            !contains(budgetYear.budgetName, remainingBudgets)
                        )) as number[]
                ];

                const remainingBudgetYears = this.selectedInvestmentLibrary.budgetYears
                    .filter((budgetYear: InvestmentLibraryBudgetYear) =>
                        !contains(budgetYear.id, deletedBudgetYearIds)
                    );

                const editedBudgetYears: InvestmentLibraryBudgetYear[] = [];
                if (!isEmpty(editedBudgets)) {
                    const yearsForRemainingBudgetYears = getPropertyValues('year', remainingBudgetYears);

                    yearsForRemainingBudgetYears.forEach((year: number) => {
                        const currentYearBudgetYears = remainingBudgetYears
                            .filter((budgetYear: InvestmentLibraryBudgetYear) => budgetYear.year === year);

                        editedBudgets.forEach((editedBudget: EditedBudget) => {
                            if (editedBudget.isNew) {
                                editedBudgetYears.push({
                                    id: ObjectID.generate(),
                                    year: year,
                                    budgetName: editedBudget.name,
                                    budgetAmount: 0,
                                });
                            } else {
                                const editedBudgetYear = currentYearBudgetYears
                                    .find((budgetYear: InvestmentLibraryBudgetYear) =>
                                        budgetYear.budgetName === editedBudget.previousName
                                    ) as InvestmentLibraryBudgetYear;
                                editedBudgetYears.push({
                                    ...editedBudgetYear,
                                    budgetName: editedBudget.name
                                });
                            }
                        });
                    });
                }

                this.selectedInvestmentLibrary.budgetOrder = getPropertyValues('name', editedBudgets);
                this.selectedInvestmentLibrary.budgetYears = editedBudgetYears;

                this.updateSelectedInvestmentLibraryAction({ updatedSelectedInvestmentLibrary: this.selectedInvestmentLibrary });
            }
        }

        /**
         * Updates the selected investment library's specified budget year's budget's amount
         */
        onEditBudgetYearAmount(year: number, budgetName: string, amount: number) {
            if (any(propEq('year', year), this.selectedInvestmentLibrary.budgetYears) &&
                any(propEq('budgetName', budgetName), this.selectedInvestmentLibrary.budgetYears)) {

                this.selectedInvestmentLibrary.budgetYears = this.selectedInvestmentLibrary.budgetYears
                    .map((budgetYear: InvestmentLibraryBudgetYear) => {
                        if (budgetYear.year === year && budgetYear.budgetName === budgetName) {
                            return {
                                ...budgetYear,
                                budgetAmount: amount
                            };
                        }
                        return budgetYear;
                    });

                this.updateSelectedInvestmentLibraryAction({ updatedSelectedInvestmentLibrary: this.selectedInvestmentLibrary });
            }
        }

        /**
         * Shows the CreateInvestmentLibraryDialog and passes in the selected investment library's inflation rate,
         * discount rate, description, budget order, and budget years to allow a user to create a new investment library
         * with this data
         */
        onCreateAsNewLibrary() {
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
         * Dispatches an action with a user's submitted CreateInvestmentLibraryDialog result in order to create a new
         * investment library on the server
         * @param createdInvestmentLibrary InvestmentLibrary object data
         */
        onCreateInvestmentLibrary(createdInvestmentLibrary: InvestmentLibrary) {
            this.createInvestmentLibraryDialogData = clone(emptyCreateInvestmentLibraryDialogData);

            if (!isNil(createdInvestmentLibrary)) {
                createdInvestmentLibrary.id = ObjectID.generate();
                createdInvestmentLibrary = this.setIdsForNewInvestmentLibraryRelatedData(createdInvestmentLibrary);

                this.createInvestmentLibraryAction({ createdInvestmentLibrary: createdInvestmentLibrary })
                    .then(() => {
                        setTimeout(() => {
                            this.onClearSelectedInvestmentLibrary();
                            setTimeout(() => {
                                this.selectItemValue = createdInvestmentLibrary.id.toString();
                            });
                        });
                    });
            }
        }

        /**
         * Sets the ids for the createdInvestmentLibrary object's budgetYears
         */
        setIdsForNewInvestmentLibraryRelatedData(createdInvestmentLibrary: InvestmentLibrary) {
            if (hasValue(createdInvestmentLibrary.budgetYears)) {
                // let nextBudgetYearId: number = hasValue(this.latestBudgetYearId) ? this.latestBudgetYearId + 1 : 1;

                createdInvestmentLibrary.budgetYears = sortByProperty('id', createdInvestmentLibrary.budgetYears)
                    .map((budgetYear: InvestmentLibraryBudgetYear) => {
                        budgetYear.id = ObjectID.generate();
                        return budgetYear;
                    });
            }

            return createdInvestmentLibrary;
        }

        /**
         * Dispatches an action with the selected investment library data in order to update the selected investment
         * library on the server
         */
        onUpdateLibrary() {
            this.updateInvestmentLibraryAction({ updatedInvestmentLibrary: this.selectedInvestmentLibrary });
        }

        /**
         * Dispatches an action with the selected investment library data in order to update the selected scenario's
         * investment library data on the server
         */
        onApplyToScenario() {
            const appliedInvestmentLibrary: InvestmentLibrary = clone(this.selectedInvestmentLibrary);
            appliedInvestmentLibrary.id = this.selectedScenarioId;
            appliedInvestmentLibrary.name = this.scenarioInvestmentLibrary.name;

            this.saveScenarioInvestmentLibraryAction({ saveScenarioInvestmentLibraryData: appliedInvestmentLibrary })
                .then(() => {
                    setTimeout(() => {
                        this.onClearSelectedInvestmentLibrary();
                        setTimeout(() => {
                            this.updateSelectedInvestmentLibraryAction({
                                updatedSelectedInvestmentLibrary: this.scenarioInvestmentLibrary
                            });
                        });
                    });
                });
        }

        /**
         * Clears the selected investment library and dispatches an action to update the selected investment library
         * in state with the scenario investment library (if present), otherwise an action is dispatched to get the
         * scenario investment library from the server to update the selected investment library in state
         */
        onDiscardChanges() {
            this.onClearSelectedInvestmentLibrary();

            if (this.scenarioInvestmentLibrary.id > 0) {
                setTimeout(() => {
                    this.updateSelectedInvestmentLibraryAction({
                        updatedSelectedInvestmentLibrary: this.scenarioInvestmentLibrary
                    });
                });
            } else {
                setTimeout(() => {
                    this.getScenarioInvestmentLibraryAction({ selectedScenarioId: this.selectedScenarioId });
                });
            }
        }
    }
</script>

<style>
    .investment-editor-container {
        height: 730px;
        overflow-x: hidden;
        overflow-y: auto;
    }
</style>
