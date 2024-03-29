<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn @click="onNewLibrary" class="ara-blue-bg white--text" v-show="selectedScenarioId === '0'">
                        New Library
                    </v-btn>
                    <v-select :items="investmentLibrariesSelectListItems"
                              label="Select an Investment library"
                              outline v-if="!hasSelectedInvestmentLibrary || selectedScenarioId !== '0'"
                              v-model="selectItemValue">
                    </v-select>
                    <v-text-field label="Library Name" v-if="hasSelectedInvestmentLibrary && selectedScenarioId === '0'"
                                  v-model="selectedInvestmentLibrary.name"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]">
                        <template slot="append">
                            <v-btn @click="selectItemValue = null" class="ara-orange" icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedInvestmentLibrary && selectedScenarioId === '0'">
                        Owner: {{selectedInvestmentLibrary.owner ? selectedInvestmentLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" label="Shared"
                                v-if="hasSelectedInvestmentLibrary && selectedScenarioId === '0'"
                                v-model="selectedInvestmentLibrary.shared"/>
                </v-flex>
            </v-layout>
            <v-layout justify-center v-show="hasSelectedInvestmentLibrary">
                <v-flex xs12>
                    <v-layout justify-center>
                        <v-flex xs3>
                            <v-text-field :disabled="!hasSelectedInvestmentLibrary" :mask="'##########'" outline
                                          label="Inflation Rate (%)"
                                          v-model.number="selectedInvestmentLibrary.inflationRate"
                                          :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedInvestmentLibrary"></v-divider>
        <v-flex v-show="hasSelectedInvestmentLibrary" xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-layout justify-space-between>
                        <v-btn @click="onEditBudgets" class="ara-blue-bg white--text">
                            Edit Budgets
                        </v-btn>
                        <v-btn :disabled="selectedInvestmentLibrary.budgetOrder.length === 0" @click="onAddBudgetYear"
                               class="ara-blue-bg white--text">
                            Add Year
                        </v-btn>
                        <v-btn :disabled="selectedInvestmentLibrary.budgetOrder.length === 0"
                               @click="showSetRangeForAddingBudgetYearsDialog = true"
                               class="ara-blue-bg white--text">
                            Add Years by Range
                        </v-btn>
                        <v-btn :disabled="selectedGridRows.length === 0" @click="onDeleteBudgetYears"
                               class="ara-orange-bg white--text">
                            Delete Budget Year(s)
                        </v-btn>
                    </v-layout>
                </v-flex>
            </v-layout>
            <v-layout justify-center>
                <v-flex xs8>
                    <v-card>
                        <v-data-table :headers="budgetYearsGridHeaders" :items="budgetYearsGridData"
                                      class="elevation-1 v-table__overflow" item-key="year" select-all
                                      v-model="selectedGridRows">
                            <template slot="items" slot-scope="props">
                                <td>
                                    <v-checkbox hide-details primary v-model="props.selected"></v-checkbox>
                                </td>
                                <td v-for="header in budgetYearsGridHeaders">
                                    <div v-if="header.value !== 'year'">
                                        <v-edit-dialog :return-value.sync="props.item[header.value]"
                                                       @save="onEditBudgetYearAmount(props.item.year, header.value, props.item[header.value])"
                                                       large lazy persistent>
                                            <v-text-field readonly single-line class="sm-txt"
                                                          :value="formatAsCurrency(props.item[header.value])"
                                                          :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                            <template slot="input">
                                                <v-text-field label="Edit" single-line
                                                              v-model.number="props.item[header.value]"
                                                              v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: false}"
                                                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                            </template>
                                        </v-edit-dialog>
                                    </div>
                                    <div v-else>
                                        <span class="sm-txt">{{props.item.year}}</span>
                                    </div>
                                </td>
                            </template>
                        </v-data-table>
                    </v-card>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedInvestmentLibrary"></v-divider>
        <v-flex v-show="hasSelectedInvestmentLibrary && (stateScenarioInvestmentLibrary === null || selectedInvestmentLibrary.id !== stateScenarioInvestmentLibrary.id)"
                xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea label="Description" no-resize outline rows="4"
                                v-model="selectedInvestmentLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout justify-end row v-show="hasSelectedInvestmentLibrary">
                <v-btn :disabled="disableSubmitAction()" @click="onApplyToScenario"
                       class="ara-blue-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Save
                </v-btn>
                <v-btn :disabled="disableSubmitAction()" @click="onUpdateLibrary"
                       class="ara-blue-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Update Library
                </v-btn>
                <v-btn :disabled="disableSubmitAction()" @click="onCreateAsNewLibrary"
                       class="ara-blue-bg white--text">
                    Create as New Library
                </v-btn>
                <v-btn @click="onDeleteInvestmentLibrary" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId === '0'" :disabled="!hasSelectedInvestmentLibrary">
                    Delete Library
                </v-btn>
                <v-btn :disabled="!hasSelectedInvestmentLibrary" @click="onDiscardChanges"
                       class="ara-orange-bg white--text" v-show="selectedScenarioId !== '0'">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse"/>

        <CreateInvestmentLibraryDialog :dialogData="createInvestmentLibraryDialogData"
                                       @submit="onCreateLibrary"/>

        <SetRangeForAddingBudgetYearsDialog :showDialog="showSetRangeForAddingBudgetYearsDialog"
                                            @submit="onSubmitBudgetYearRange"/>

        <EditBudgetsDialog :dialogData="editBudgetsDialogData" @submit="onSubmitEditedBudgets"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Watch} from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {Action, State} from 'vuex-class';
    import CreateInvestmentLibraryDialog from './investment-editor-dialogs/CreateInvestmentLibraryDialog.vue';
    import SetRangeForAddingBudgetYearsDialog from './investment-editor-dialogs/SetRangeForAddingBudgetYearsDialog.vue';
    import EditBudgetsDialog from './investment-editor-dialogs/EditBudgetsDialog.vue';
    import {
        BudgetYearsGridData,
        CriteriaDrivenBudget,
        emptyInvestmentLibrary,
        InvestmentLibrary,
        InvestmentLibraryBudgetYear
    } from '@/shared/models/iAM/investment';
    import {any, clone, contains, find, findIndex, groupBy, isNil, keys, propEq, update} from 'ramda';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {hasValue} from '@/shared/utils/has-value-util';
    import moment from 'moment';
    import {
        CreateInvestmentLibraryDialogData,
        emptyCreateInvestmentLibraryDialogData
    } from '@/shared/models/modals/create-investment-library-dialog-data';
    import {
        EditBudgetsDialogData,
        emptyEditBudgetsDialogData
    } from '@/shared/models/modals/edit-budgets-dialog';
    import {getLatestPropertyValue, getPropertyValues} from '@/shared/utils/getter-utils';
    import {formatAsCurrency} from '@/shared/utils/currency-formatter';
    import Alert from '@/shared/modals/Alert.vue';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';

    const ObjectID = require('bson-objectid');

    @Component({
        components: {CreateInvestmentLibraryDialog, SetRangeForAddingBudgetYearsDialog, EditBudgetsDialog, Alert}
    })
    export default class InvestmentEditor extends Vue {
        @State(state => state.investmentEditor.investmentLibraries) stateInvestmentLibraries: InvestmentLibrary[];
        @State(state => state.investmentEditor.selectedInvestmentLibrary) stateSelectedInvestmentLibrary: InvestmentLibrary;
        @State(state => state.investmentEditor.scenarioInvestmentLibrary) stateScenarioInvestmentLibrary: InvestmentLibrary;
        @State(state => state.criteriaDrivenBudgets.budgetCriteria) stateBudgetCriteria: CriteriaDrivenBudget[];

        @Action('getInvestmentLibraries') getInvestmentLibrariesAction: any;
        @Action('getScenarioInvestmentLibrary') getScenarioInvestmentLibraryAction: any;
        @Action('selectInvestmentLibrary') selectInvestmentLibraryAction: any;
        @Action('createInvestmentLibrary') createInvestmentLibraryAction: any;
        @Action('updateInvestmentLibrary') updateInvestmentLibraryAction: any;
        @Action('deleteInvestmentLibrary') deleteInvestmentLibraryAction: any;
        @Action('saveScenarioInvestmentLibrary') saveScenarioInvestmentLibraryAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        selectedInvestmentLibrary: InvestmentLibrary = clone(emptyInvestmentLibrary);
        selectedScenarioId: string = '0';
        hasSelectedInvestmentLibrary: boolean = false;
        investmentLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string | null = '';
        budgetYearsGridHeaders: DataTableHeader[] = [
            {text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: ''}
        ];
        intermittentBudgetsCriteria: CriteriaDrivenBudget[] = [];

        budgetYearsGridData: BudgetYearsGridData[] = [];
        selectedGridRows: BudgetYearsGridData[] = [];
        selectedBudgetYears: number[] = [];
        createInvestmentLibraryDialogData: CreateInvestmentLibraryDialogData = clone(emptyCreateInvestmentLibraryDialogData);
        editBudgetsDialogData: EditBudgetsDialogData = clone(emptyEditBudgetsDialogData);
        showSetRangeForAddingBudgetYearsDialog: boolean = false;
        alertBeforeDelete: AlertData = clone(emptyAlertData);
        objectIdMOngoDBForScenario: string = '';
        rules: InputValidationRules = clone(rules);

        /**
         * Sets component UI properties that triggers cascading UI updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/InvestmentEditor/Scenario/') {
                    vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    if (vm.selectedScenarioId === '0') {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.selectItemValue = null;
                vm.getInvestmentLibrariesAction()
                    .then(() => {
                        if (vm.selectedScenarioId !== '0') {
                            vm.getScenarioInvestmentLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});
                        }
                    });
            });
        }

        beforeDestroy() {
            this.setHasUnsavedChangesAction({value: false});
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
            this.selectInvestmentLibraryAction({selectedLibraryId: this.selectItemValue});
        }

        @Watch('stateSelectedInvestmentLibrary')
        onStateSelectedInvestmentLibraryChanged() {
            this.selectedInvestmentLibrary = clone(this.stateSelectedInvestmentLibrary);
        }

        @Watch('selectedInvestmentLibrary')
        onSelectedInvestmentLibraryChanged() {
            this.setHasUnsavedChangesAction({
                value: hasUnsavedChanges(
                    'investment', this.selectedInvestmentLibrary, this.stateSelectedInvestmentLibrary, this.stateScenarioInvestmentLibrary
                )
            });

            this.selectedGridRows = [];
            this.hasSelectedInvestmentLibrary = this.selectedInvestmentLibrary.id !== '0';

            this.setGridHeaders();
            this.setGridData();
        }

        /**
         * Setter for the selectedBudgetYears object
         */
        @Watch('selectedGridRows')
        onSelectedGridRowsChanged() {
            this.selectedBudgetYears = getPropertyValues('year', this.selectedGridRows) as number[];
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

                    gridDataRow[budgets[i]] = hasValue(budgetYear) ? budgetYear.budgetAmount : null;
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

            const newBudgetYears: InvestmentLibraryBudgetYear[] = this.selectedInvestmentLibrary.criteriaDrivenBudgets
                .map((budget: CriteriaDrivenBudget) => {
                    const newBudgetYear: InvestmentLibraryBudgetYear = {
                        id: ObjectID.generate(),
                        year: nextYear,
                        budgetName: budget.budgetName,
                        budgetAmount: 0,
                        criteriaDrivenBudgetId: budget.id
                    };
                    return newBudgetYear;
                });

            this.selectedInvestmentLibrary = {
                ...this.selectedInvestmentLibrary,
                budgetYears: [
                    ...this.selectedInvestmentLibrary.budgetYears,
                    ...newBudgetYears
                ]
            };
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
                    this.selectedInvestmentLibrary.criteriaDrivenBudgets.forEach((budget: CriteriaDrivenBudget) => {
                        newBudgetYears.push({
                            id: ObjectID.generate(),
                            year: currentYear,
                            budgetName: budget.budgetName,
                            budgetAmount: 0,
                            criteriaDrivenBudgetId: budget.id
                        });
                    });
                }

                this.selectedInvestmentLibrary = {
                    ...this.selectedInvestmentLibrary,
                    budgetYears: [
                        ...this.selectedInvestmentLibrary.budgetYears,
                        ...newBudgetYears
                    ]
                };
            }
        }

        /**
         * Removes n number of InvestmentLibraryBudgetYear objects from the selectedInvestmentLibrary budgetYears property
         */
        onDeleteBudgetYears() {
            this.selectedInvestmentLibrary = {
                ...this.selectedInvestmentLibrary,
                budgetYears: this.selectedInvestmentLibrary.budgetYears
                    .filter((budgetYear: InvestmentLibraryBudgetYear) =>
                        !contains(budgetYear.year, this.selectedBudgetYears))
            };
        }

        /**
         * Toggles the EditBudgetsDialog modal
         */
        onEditBudgets() {
            this.editBudgetsDialogData = {
                showDialog: true,
                criteriaDrivenBudgets: this.selectedInvestmentLibrary.criteriaDrivenBudgets,
                scenarioId: parseInt(this.selectedScenarioId)
            };
        }

        /**
         * Modifies the selectedInvestmentLibrary budgetOrder, budgetYears, and budgetCriteria properties with the
         * EditBudgetsDialog modal result
         */
        onSubmitEditedBudgets(editedBudgets: CriteriaDrivenBudget[]) {
            this.editBudgetsDialogData = clone(emptyEditBudgetsDialogData);

            if (!isNil(editedBudgets)) {
                const editedBudgetYears: InvestmentLibraryBudgetYear[] = [];
                const remainingBudgetIds: string[] = getPropertyValues('id', editedBudgets);
                const budgetIdsToBudgetYears: any[] = remainingBudgetIds.map((budgetId: string) => ({
                    id: budgetId,
                    budgetYears: this.selectedInvestmentLibrary.budgetYears
                        .filter((budgetYear: InvestmentLibraryBudgetYear) => budgetYear.criteriaDrivenBudgetId === budgetId)
                }));
                const years = getPropertyValues(
                    'year', this.selectedInvestmentLibrary.budgetYears
                        .filter((budgetYear: InvestmentLibraryBudgetYear) => contains(budgetYear.criteriaDrivenBudgetId, remainingBudgetIds))
                );

                budgetIdsToBudgetYears.forEach((budgetIdToBudgetYears: any) => {
                    const criteriaDrivenBudget: CriteriaDrivenBudget = find(
                        propEq('id', budgetIdToBudgetYears.id), editedBudgets) as CriteriaDrivenBudget;
                    if (hasValue(budgetIdToBudgetYears.budgetYears)) {
                        budgetIdToBudgetYears.budgetYears
                            .forEach((budgetYear: InvestmentLibraryBudgetYear) => budgetYear.budgetName = criteriaDrivenBudget.budgetName);
                        editedBudgetYears.push(...budgetIdToBudgetYears.budgetYears);
                    } else {
                        years.forEach((year: number) => {
                            editedBudgetYears.push({
                                id: ObjectID.generate(),
                                year: year,
                                budgetName: criteriaDrivenBudget.budgetName,
                                budgetAmount: 0,
                                criteriaDrivenBudgetId: criteriaDrivenBudget.id
                            });
                        });
                    }
                });

                this.selectedInvestmentLibrary = {
                    ...this.selectedInvestmentLibrary,
                    budgetOrder: getPropertyValues('budgetName', editedBudgets),
                    budgetYears: editedBudgetYears,
                    criteriaDrivenBudgets: editedBudgets
                };
            }
        }

        /**
         * Modifies an InvestmentLibraryBudgetYear budgetAmount property
         */
        onEditBudgetYearAmount(year: number, budgetName: string, amount: number) {
            if (any(propEq('year', year), this.selectedInvestmentLibrary.budgetYears) &&
                any(propEq('budgetName', budgetName), this.selectedInvestmentLibrary.budgetYears)) {
                const budgetYear: InvestmentLibraryBudgetYear = find(
                    (budgetYear: InvestmentLibraryBudgetYear) => budgetYear.year === year && budgetYear.budgetName === budgetName,
                    this.selectedInvestmentLibrary.budgetYears
                ) as InvestmentLibraryBudgetYear;

                this.selectedInvestmentLibrary = {
                    ...this.selectedInvestmentLibrary,
                    budgetYears: update(
                        findIndex(propEq('id', budgetYear.id), this.selectedInvestmentLibrary.budgetYears),
                        {
                            ...budgetYear,
                            budgetAmount: hasValue(amount)
                                ? parseFloat(amount.toString().replace(/(\$*)(\,*)/g, '')) : null
                        },
                        this.selectedInvestmentLibrary.budgetYears
                    )
                };
            }
        }

        /**
         * Toggles the CreateInvestmentLibrary modal to create a new InvestmentLibrary using the selectedInvestmentLibrary
         * data
         */
        onCreateAsNewLibrary() {
            if (isNil(this.selectedInvestmentLibrary.criteriaDrivenBudgets)) {
                this.selectedInvestmentLibrary.criteriaDrivenBudgets = this.intermittentBudgetsCriteria;
            }
            this.createInvestmentLibraryDialogData = {
                showDialog: true,
                inflationRate: this.selectedInvestmentLibrary.inflationRate,
                description: this.selectedInvestmentLibrary.description,
                budgetOrder: this.selectedInvestmentLibrary.budgetOrder,
                budgetYears: this.selectedInvestmentLibrary.budgetYears,
                budgetCriteria: this.selectedInvestmentLibrary.criteriaDrivenBudgets
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
            this.saveScenarioInvestmentLibraryAction({
                saveScenarioInvestmentLibraryData: {
                    ...this.selectedInvestmentLibrary,
                    id: this.selectedScenarioId
                },
                objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario
            }).then(() => this.onDiscardChanges());
        }

        /**
         * Dispatches an action to reset the selectedInvestmentLibrary with the current stateScenarioInvestmentLibrary
         */
        onDiscardChanges() {
            this.selectItemValue = null;
            setTimeout(() => this.selectInvestmentLibraryAction({selectedLibraryId: this.stateScenarioInvestmentLibrary.id}));
        }

        /**
         * Formats the given value as currency
         */
        formatAsCurrency(value: any) {
            if (hasValue(value)) {
                return formatAsCurrency(value);
            }

            return null;
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
                this.selectItemValue = null;
                this.deleteInvestmentLibraryAction({investmentLibrary: this.selectedInvestmentLibrary});
            }
        }

        disableSubmitAction() {
            if (this.hasSelectedInvestmentLibrary) {
                const allDataIsValid: boolean = this.rules['generalRules'].valueIsNotEmpty(this.selectedInvestmentLibrary.inflationRate) === true &&
                    this.selectedInvestmentLibrary.budgetYears.every((by: InvestmentLibraryBudgetYear) => {
                        return this.rules['generalRules'].valueIsNotEmpty(by.budgetAmount) === true;
                    });
                if (this.selectedScenarioId !== '0') {
                    return !allDataIsValid;
                } else {
                    return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedInvestmentLibrary.name) === true &&
                        allDataIsValid);
                }
            }

            return true;
        }
    }
</script>

<style>
    .sharing label {
        padding-top: 0.5em;
    }

    .sharing {
        padding-top: 0;
        margin: 0;
    }

    .budget-year-amount-input {
        border: 1px solid;
        width: 100%;
    }
</style>
