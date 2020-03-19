<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn v-show="selectedScenarioId === 0" class="ara-blue-bg white--text" @click="onNewLibrary">
                        New Library
                    </v-btn>
                    <v-select v-if="!hasSelectedInvestmentLibrary || selectedScenarioId > 0"
                              :items="investmentLibrariesSelectListItems"
                              label="Select an Investment library" outline v-model="selectItemValue">
                    </v-select>
                    <v-text-field v-if="hasSelectedInvestmentLibrary && selectedScenarioId === 0" label="Library Name"
                                  v-model="selectedInvestmentLibrary.name">
                        <template slot="append">
                            <v-btn class="ara-orange" icon @click="selectItemValue = null">
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedInvestmentLibrary && selectedScenarioId === 0">
                        Owner: {{selectedInvestmentLibrary.owner ? selectedInvestmentLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" v-if="hasSelectedInvestmentLibrary && selectedScenarioId === 0"
                        v-model="selectedInvestmentLibrary.shared" label="Shared"/>
                </v-flex>
            </v-layout>
            <v-layout v-show="hasSelectedInvestmentLibrary" justify-center>
                <v-flex xs12>
                    <v-layout justify-center>
                        <v-flex xs3>
                            <v-text-field label="Inflation Rate (%)" outline :mask="'##########'"
                                          v-model="selectedInvestmentLibrary.inflationRate"
                                          :disabled="!hasSelectedInvestmentLibrary">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedInvestmentLibrary"></v-divider>
        <v-flex xs12 v-show="hasSelectedInvestmentLibrary">
            <v-layout justify-center>
                <v-flex xs6>
                    <v-layout justify-space-between>
                        <v-btn class="ara-blue-bg white--text" @click="onEditBudgets">
                            Edit Budgets
                        </v-btn>
                        <v-btn class="ara-blue-bg white--text" @click="onAddBudgetYear"
                               :disabled="selectedInvestmentLibrary.budgetOrder.length === 0">
                            Add Year
                        </v-btn>
                        <v-btn class="ara-blue-bg white--text" @click="showSetRangeForAddingBudgetYearsDialog = true"
                               :disabled="selectedInvestmentLibrary.budgetOrder.length === 0">
                            Add Years by Range
                        </v-btn>
                        <v-btn class="ara-orange-bg white--text" @click="onDeleteBudgetYears"
                               :disabled="selectedGridRows.length === 0">
                            Delete Budget Year(s)
                        </v-btn>
                    </v-layout>
                </v-flex>
            </v-layout>
            <v-layout justify-center>
                <v-flex xs8>
                    <v-card>
                        <v-data-table :headers="budgetYearsGridHeaders" :items="budgetYearsGridData"
                                      v-model="selectedGridRows" select-all item-key="year"
                                      class="elevation-1 v-table__overflow">
                            <template slot="items" slot-scope="props">
                                <td>
                                    <v-checkbox v-model="props.selected" primary hide-details></v-checkbox>
                                </td>
                                <td v-for="header in budgetYearsGridHeaders">
                                    <div v-if="header.value !== 'year'">
                                        <v-edit-dialog :return-value.sync="props.item[header.value]"
                                                       large lazy persistent
                                                       @save="onEditBudgetYearAmount(props.item.year, header.value, props.item[header.value])">
                                            {{formatAsCurrency(props.item[header.value])}}
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
        <v-divider v-show="hasSelectedInvestmentLibrary"></v-divider>
        <v-flex xs12 v-show="hasSelectedInvestmentLibrary && (stateScenarioInvestmentLibrary === null || selectedInvestmentLibrary.id !== stateScenarioInvestmentLibrary.id)">
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea rows="4" no-resize outline label="Description"
                                v-model="selectedInvestmentLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout v-show="hasSelectedInvestmentLibrary" justify-end row>
                <v-btn v-show="selectedScenarioId > 0" class="ara-blue-bg white--text" @click="onApplyToScenario"
                       :disabled="!hasSelectedInvestmentLibrary">
                    Save
                </v-btn>
                <v-btn v-show="selectedScenarioId === 0" class="ara-blue-bg white--text" @click="onUpdateLibrary"
                       :disabled="!hasSelectedInvestmentLibrary">
                    Update Library
                </v-btn>
                <v-btn class="ara-blue-bg white--text" @click="onCreateAsNewLibrary" :disabled="!hasSelectedInvestmentLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId === 0" class="ara-orange-bg white--text" @click="onDeleteInvestmentLibrary">
                    Delete Library
                </v-btn>
                <v-btn v-show="selectedScenarioId > 0" class="ara-orange-bg white--text" @click="onDiscardChanges"
                       :disabled="!hasSelectedInvestmentLibrary">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse" />

        <CreateInvestmentLibraryDialog :dialogData="createInvestmentLibraryDialogData"
                                       @submit="onCreateLibrary" />

        <SetRangeForAddingBudgetYearsDialog :showDialog="showSetRangeForAddingBudgetYearsDialog"
                                            @submit="onSubmitBudgetYearRange" />

        <EditBudgetsDialog :dialogData="editBudgetsDialogData" @submit="onSubmitEditedBudgets" />
    </v-layout>
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
    import { any, groupBy, isEmpty, isNil, keys, propEq, contains, find, clone } from 'ramda';
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
    import { sorter } from '@/shared/utils/sorter-utils';
    import { EditedBudget } from '@/shared/models/modals/edit-budgets-dialog';
    import { CriteriaDrivenBudgets } from '@/shared/models/iAM/criteria-driven-budgets';
    import {formatAsCurrency} from '@/shared/utils/currency-formatter';
    import Alert from '@/shared/modals/Alert.vue';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    const ObjectID = require('bson-objectid');

    @Component({
        components: { CreateInvestmentLibraryDialog, SetRangeForAddingBudgetYearsDialog, EditBudgetsDialog, Alert }
    })
    export default class InvestmentEditor extends Vue {
        @State(state => state.investmentEditor.investmentLibraries) stateInvestmentLibraries: InvestmentLibrary[];
        @State(state => state.investmentEditor.selectedInvestmentLibrary) stateSelectedInvestmentLibrary: InvestmentLibrary;
        @State(state => state.investmentEditor.scenarioInvestmentLibrary) stateScenarioInvestmentLibrary: InvestmentLibrary;
        @State(state => state.criteriaDrivenBudgets.budgetCriteria) stateBudgetCriteria: CriteriaDrivenBudgets[];
        @State(state => state.criteriaDrivenBudgets.intermittentBudgetCriteria) stateIntermittentBudgetCriteria: CriteriaDrivenBudgets[];

        @Action('getInvestmentLibraries') getInvestmentLibrariesAction: any;
        @Action('getScenarioInvestmentLibrary') getScenarioInvestmentLibraryAction: any;
        @Action('selectInvestmentLibrary') selectInvestmentLibraryAction: any;
        @Action('createInvestmentLibrary') createInvestmentLibraryAction: any;
        @Action('updateInvestmentLibrary') updateInvestmentLibraryAction: any;
        @Action('deleteInvestmentLibrary') deleteInvestmentLibraryAction: any;
        @Action('saveScenarioInvestmentLibrary') saveScenarioInvestmentLibraryAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getBudgetCriteria') getBudgetCriteriaAction: any;
        @Action('saveBudgetCriteria') saveBudgetCriteriaAction: any;
        @Action('saveIntermittentCriteriaDrivenBudget') saveIntermittentCriteriaDrivenBudgetAction: any;
        @Action('saveIntermittentStateToBudgetCriteria') saveIntermittentStateToBudgetCriteriaAction: any;

        tabs: string[] = ['investments', 'budget criteria'];
        activeTab: number = 0;

        selectedInvestmentLibrary: InvestmentLibrary = clone(emptyInvestmentLibrary);
        selectedScenarioId: number = 0;
        hasSelectedInvestmentLibrary: boolean = false;
        investmentLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string | null = '';
        budgetYearsGridHeaders: DataTableHeader[] = [
            { text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: '' }
        ];
        budgetCriteriaHeaders: DataTableHeader[] = [
            { text: 'Multi select', value: 'multiselect', sortable: true, align: 'left', class: '', width: '' },
            { text: 'Edit', value: 'edit', sortable: true, align: 'left', class: '', width: '' }
        ];
        intermittentBudgetsCriteria: CriteriaDrivenBudgets[] = [];
        budgetYearsGridData: BudgetYearsGridData[] = [];
        selectedGridRows: BudgetYearsGridData[] = [];
        selectedBudgetYears: number[] = [];
        createInvestmentLibraryDialogData: CreateInvestmentLibraryDialogData = clone(emptyCreateInvestmentLibraryDialogData);
        editBudgetsDialogData: EditBudgetsDialogData = clone(emptyEditBudgetsDialogData);
        showSetRangeForAddingBudgetYearsDialog: boolean = false;
        alertBeforeDelete: AlertData = clone(emptyAlertData);
        objectIdMOngoDBForScenario: string = '';

        /**
         * Sets component UI properties that triggers cascading UI updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                vm.selectedScenarioId = 0;

                if (to.path === '/InvestmentEditor/Scenario/') {
                    vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;
                    vm.selectedScenarioId = isNaN(parseInt(to.query.selectedScenarioId)) ? 0 : parseInt(to.query.selectedScenarioId);
                    if (vm.selectedScenarioId === 0) {
                        vm.setErrorMessageAction({ message: 'Found no selected scenario for edit' });
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.selectItemValue = null;
                setTimeout(() => {
                    vm.getInvestmentLibrariesAction()
                        .then(() => {
                            if (vm.selectedScenarioId > 0) {
                                vm.getScenarioInvestmentLibraryAction({selectedScenarioId: vm.selectedScenarioId});
                                vm.getBudgetCriteriaAction({selectedScenarioId: vm.selectedScenarioId});
                            }
                        });
                });
            });
        }

        /**
         * Setter for the investmentLibrariesSelectListItems object
         */
        @Watch('stateInvestmentLibraries')
        onStateInvestmentLibrariesChanged() {
            this.investmentLibrariesSelectListItems = this.stateInvestmentLibraries
                .map((investmentLibrary: InvestmentLibrary) => ({
                    text: investmentLibrary.name,
                    value: investmentLibrary.id.toString()
                }));
        }

        /**
         * Dispatches an action to set the selected investment library using the selectItemValue property
         */
        @Watch('selectItemValue')
        onInvestmentLibrariesSelectItemChanged() {
            const selectedInvestmentLibrary: InvestmentLibrary = find(
                propEq('id', this.selectItemValue), this.stateInvestmentLibraries
            ) as InvestmentLibrary;

            this.selectInvestmentLibraryAction({
                selectedInvestmentLibrary: hasValue(selectedInvestmentLibrary) ? selectedInvestmentLibrary : emptyInvestmentLibrary
            });
        }

        /**
         * Sets component UI/functional properties on a change to the stateSelectedInvestmentLibrary object
         */
        @Watch('stateSelectedInvestmentLibrary')
        onStateSelectedInvestmentLibraryChanged() {
            this.selectedInvestmentLibrary = clone(this.stateSelectedInvestmentLibrary);
            this.selectedGridRows = [];
            this.hasSelectedInvestmentLibrary = this.selectedInvestmentLibrary.id !== '0';

            if (!hasValue(this.selectedInvestmentLibrary.budgetOrder) &&
                hasValue(this.selectedInvestmentLibrary.budgetYears)) {
                this.setSelectedInvestmentLibraryBudgetOrder();
            } else {
                this.setGridHeaders();
                this.setGridData();
            }

            this.saveIntermittentCriteriaDrivenBudgetAction({
                updateIntermittentCriteriaDrivenBudget: this.selectedInvestmentLibrary.budgetCriteria
            });
        }

        /**
         * Setter for the selectedBudgetYears object
         */
        @Watch('selectedGridRows')
        onSelectedGridRowsChanged() {
            this.selectedBudgetYears = getPropertyValues('year', this.selectedGridRows) as number[];
        }

        /**
         * Setter for the stateIntermittentBudgetCriteria object
         */
        @Watch('stateIntermittentBudgetCriteria')
        onStateIntermittentBudgetCriteriaChanged() {
            this.intermittentBudgetsCriteria = clone(this.stateIntermittentBudgetCriteria);
        }

        /**
         * Sorts the selected investment library's budgetOrder property
         */
        setSelectedInvestmentLibraryBudgetOrder() {
            this.selectInvestmentLibraryAction({selectedInvestmentLibrary: {
                    ...this.selectedInvestmentLibrary,
                    budgetOrder: sorter(
                        getPropertyValues('budgetName', this.selectedInvestmentLibrary.budgetYears)
                    ) as string[]
                }
            });
        }

        /**
         * Setter for the budgetYearsGridHeaders object
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
         * Setter for the budgetYearsGridData object
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
         * Triggers the create investment library modal
         */
        onNewLibrary() {
            this.createInvestmentLibraryDialogData = {...emptyCreateInvestmentLibraryDialogData, showDialog: true};
        }

        /**
         * Appends a new investment library budget year to the selected investment library's budgetYears property
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

            this.selectInvestmentLibraryAction({selectedInvestmentLibrary: {
                    ...this.selectedInvestmentLibrary,
                    budgetYears: [
                        ...this.selectedInvestmentLibrary.budgetYears,
                        ...newBudgetYears
                    ]
                }
            });
        }

        /**
         * Appends n number of new InvestmentLibraryBudgetYear objects to the selectedInvestmentLibrary budgetYears property
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

                this.selectInvestmentLibraryAction({selectedInvestmentLibrary: {
                        ...this.selectedInvestmentLibrary,
                        budgetYears: [
                            ...this.selectedInvestmentLibrary.budgetYears,
                            ...newBudgetYears
                        ]
                    }
                });
            }
        }

        /**
         * Removes n number of InvestmentLibraryBudgetYear objects from the selectedInvestmentLibrary budgetYears property
         */
        onDeleteBudgetYears() {
            this.selectInvestmentLibraryAction({selectedInvestmentLibrary: {
                    ...this.selectedInvestmentLibrary,
                    budgetYears: this.selectedInvestmentLibrary.budgetYears
                        .filter((budgetYear: InvestmentLibraryBudgetYear) =>
                            !contains(budgetYear.year, this.selectedBudgetYears))
                }
            });
        }

        /**
         * Toggles the EditBudgetsDialog modal
         */
        onEditBudgets() {
            this.editBudgetsDialogData = {
                showDialog: true,
                budgets: this.selectedInvestmentLibrary.budgetOrder,
                canOrderBudgets: true,
                criteriaBudgets: this.intermittentBudgetsCriteria,
                scenarioId: this.selectedScenarioId
            };
        }

        /**
         * Modifies the selectedInvestmentLibrary budgetOrder, budgetYears, and budgetCriteria properties with the
         * EditBudgetsDialog modal result
         */
        onSubmitEditedBudgets(editedBudgets: EditedBudget[]) {
            this.editBudgetsDialogData = clone(emptyEditBudgetsDialogData);

            if (!isNil(editedBudgets)) {
                const remainingBudgets: string[] = getPropertyValues('previousName', editedBudgets
                    .filter((budget: EditedBudget) => !budget.isNew));

                let intermittentCriteria: CriteriaDrivenBudgets[] = [];

                const deletedBudgetYearIds = [
                    ...getPropertyValues('id', this.selectedInvestmentLibrary.budgetYears
                        .filter((budgetYear: InvestmentLibraryBudgetYear) =>
                            !contains(budgetYear.budgetName, remainingBudgets)
                        )) as string[]
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

                    editedBudgets.forEach((editBudgets: EditedBudget) => {
                        intermittentCriteria.push(editBudgets.criteriaBudgets);
                    });
                }

                this.selectInvestmentLibraryAction({selectedInvestmentLibrary: {
                        ...this.selectedInvestmentLibrary,
                        budgetOrder: getPropertyValues('name', editedBudgets),
                        budgetYears: editedBudgetYears,
                        budgetCriteria: intermittentCriteria
                    }
                });

                this.saveIntermittentCriteriaDrivenBudgetAction({updateIntermittentCriteriaDrivenBudget: intermittentCriteria});
            }
        }

        /**
         * Modifies an InvestmentLibraryBudgetYear budgetAmount property
         */
        onEditBudgetYearAmount(year: number, budgetName: string, amount: number) {
            if (any(propEq('year', year), this.selectedInvestmentLibrary.budgetYears) &&
                any(propEq('budgetName', budgetName), this.selectedInvestmentLibrary.budgetYears)) {
                this.selectInvestmentLibraryAction({selectedInvestmentLibrary: {
                        ...this.selectedInvestmentLibrary,
                        budgetYears: this.selectedInvestmentLibrary.budgetYears
                            .map((budgetYear: InvestmentLibraryBudgetYear) => {
                                if (budgetYear.year === year && budgetYear.budgetName === budgetName) {
                                    return {
                                        ...budgetYear,
                                        budgetAmount: amount
                                    };
                                }
                                return budgetYear;
                            })
                    }
                });
            }
        }

        /**
         * Toggles the CreateInvestmentLibrary modal to create a new InvestmentLibrary using the selectedInvestmentLibrary
         * data
         */
        onCreateAsNewLibrary() {
            if (isNil(this.selectedInvestmentLibrary.budgetCriteria)) {
               this.selectedInvestmentLibrary.budgetCriteria = this.intermittentBudgetsCriteria;
            }

            this.createInvestmentLibraryDialogData = {
                showDialog: true,
                inflationRate: this.selectedInvestmentLibrary.inflationRate,
                description: this.selectedInvestmentLibrary.description,
                budgetOrder: this.selectedInvestmentLibrary.budgetOrder,
                budgetYears: this.selectedInvestmentLibrary.budgetYears,
                budgetCriteria: this.selectedInvestmentLibrary.budgetCriteria
            };
        }

        /**
         * Dispatches an action to create a new InvestmentLibrary in the mongo database
         */
        onCreateLibrary(createdInvestmentLibrary: InvestmentLibrary) {
            this.createInvestmentLibraryDialogData = clone(emptyCreateInvestmentLibraryDialogData);

            if (!isNil(createdInvestmentLibrary)) {
                this.createInvestmentLibraryAction({createdInvestmentLibrary: createdInvestmentLibrary})
                    .then(() => this.selectItemValue = createdInvestmentLibrary.id);
            }
        }

        /**
         * Dispatches an action to modify the selectedInvestmentLibrary data in the mongo database
         */
        onUpdateLibrary() {
            this.updateInvestmentLibraryAction({updatedInvestmentLibrary: this.selectedInvestmentLibrary});
        }

        /**
         * Dispatches an action to modify a scenario's InvestmentLibrary data in the sql server database
         */
        onApplyToScenario() {
            this.saveBudgetCriteriaAction({
                selectedScenarioId: this.selectedScenarioId, budgetCriteriaData: this.intermittentBudgetsCriteria
            }).then(() => {
                this.saveIntermittentStateToBudgetCriteriaAction({intermittentState: this.intermittentBudgetsCriteria});
            }).then(() => {
                this.saveScenarioInvestmentLibraryAction({ saveScenarioInvestmentLibraryData: {...this.selectedInvestmentLibrary, id: this.selectedScenarioId},
                    objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario })
                    .then(() => this.onDiscardChanges())
            });
        }

        /**
         * Dispatches an action to reset the selectedInvestmentLibrary with the current stateScenarioInvestmentLibrary
         */
        onDiscardChanges() {
            this.saveIntermittentCriteriaDrivenBudgetAction({updateIntermittentCriteriaDrivenBudget: this.stateBudgetCriteria});
            this.selectItemValue = null;
            setTimeout(() =>
                this.selectInvestmentLibraryAction({selectedInvestmentLibrary: this.stateScenarioInvestmentLibrary})
            );
        }

        /**
         * Formats the given value as currency
         */
        formatAsCurrency(value: any) {
            return formatAsCurrency(value);
        }

        onDeleteInvestmentLibrary() {
            this.alertBeforeDelete = {
                showDialog: true,
                heading: 'Warning',
                choice: true,
                message: 'Are you sure you want to delete?'
            };
        }

        onSubmitDeleteResponse(response: boolean) {
            this.alertBeforeDelete = clone(emptyAlertData);

            if (response) {
                this.deleteInvestmentLibraryAction({investmentLibrary: this.selectedInvestmentLibrary});
                this.onClearSelectedInvestmentLibrary();
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

    .sharing label {
        padding-top: 0.5em;
    }

    .sharing {
        padding-top: 0;
        margin: 0;
    }
</style>
