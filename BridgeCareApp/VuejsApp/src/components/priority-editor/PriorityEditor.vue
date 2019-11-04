<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onNewLibrary">
                        New Library
                    </v-btn>
                    <v-select v-if="!hasSelectedPriorityLibrary || selectedScenarioId !== '0'"
                              :items="priorityLibrariesSelectListItems" label="Select a Priority Library" outline
                              v-model="selectItemValue">
                    </v-select>
                    <v-text-field v-if="hasSelectedPriorityLibrary && selectedScenarioId === '0'" label="Library Name"
                                  v-model="selectedPriorityLibrary.name">
                        <template slot="append">
                            <v-btn class="ara-orange" icon @click="onClearSelectedPriorityLibrary">
                                <v-icon>fas fa-times</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                </v-flex>
            </v-layout>
            <v-flex xs3 v-show="hasSelectedPriorityLibrary">
                <v-btn class="ara-blue-bg white--text" @click="onAddPriority">
                    Add
                </v-btn>
            </v-flex>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedPriorityLibrary">
            <div class="priorities-data-table">
                <v-data-table :headers="priorityDataTableHeaders" :items="prioritiesDataTableRows"
                              class="elevation-1 v-table__overflow">
                    <template slot="items" slot-scope="props">
                        <td v-for="header in priorityDataTableHeaders">
                            <div v-if="header.value === 'priorityLevel' || header.value === 'year'">
                                <v-edit-dialog :return-value.sync="props.item[header.value]" large lazy persistent
                                               @save="onEditPriorityProperty(props.item.priorityId, header.value, props.item[header.value])">
                                    <v-text-field readonly :value="props.item[header.value]"></v-text-field>
                                    <template slot="input">
                                        <v-text-field v-model="props.item[header.value]" label="Edit" single-line>
                                        </v-text-field>
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-else-if="header.value === 'criteria'">
                                <div v-if="budgetOrder.length > 5">
                                    <v-layout row>
                                        <v-menu v-show="props.item.criteria !== ''" left min-width="500px" min-height="500px">
                                            <template slot="activator">
                                                <v-btn icon class="ara-blue"><v-icon>fas fa-eye</v-icon></v-btn>
                                            </template>
                                            <v-card>
                                                <v-card-text>
                                                    <v-textarea rows="5" no-resize readonly full-width outline
                                                                :value="props.item.criteria">
                                                    </v-textarea>
                                                </v-card-text>
                                            </v-card>
                                        </v-menu>
                                        <v-btn icon class="edit-icon" @click="onEditCriteria(props.item.priorityId, props.item.criteria)">
                                            <v-icon>fas fa-edit</v-icon>
                                        </v-btn>
                                    </v-layout>
                                </div>
                                <div v-else>
                                    <v-text-field :id="'priority' + props.item.priorityId" readonly :value="props.item.criteria">
                                        <template slot="append-outer">
                                            <v-layout row class="criteria-input-icons">
                                                <v-menu left min-width="500px"
                                                        min-height="500px">
                                                    <template slot="activator">
                                                        <v-btn :id="'priority' + props.item.priorityId + 'EyeIcon'" hidden icon class="ara-blue"><v-icon>fas fa-eye</v-icon></v-btn>
                                                    </template>
                                                    <v-card>
                                                        <v-card-text>
                                                            <v-textarea rows="5" no-resize readonly full-width outline
                                                                        :value="props.item.criteria">
                                                            </v-textarea>
                                                        </v-card-text>
                                                    </v-card>
                                                </v-menu>
                                                <v-icon class="edit-icon"
                                                        @click="onEditCriteria(props.item.priorityId, props.item.criteria)">
                                                    fas fa-edit
                                                </v-icon>
                                            </v-layout>
                                        </template>
                                    </v-text-field>
                                </div>
                            </div>
                            <div v-else>
                                <v-edit-dialog :return-value.sync="props.item[header.value]" large lazy persistent
                                               @save="onEditBudgetFunding(props.item.priorityId, header.value, props.item[header.value])">
                                    <v-text-field readonly :value="props.item[header.value]" :rules="[rule.fundingPercent]">
                                    </v-text-field>
                                    <template slot="input">
                                        <v-text-field v-model="props.item[header.value]" label="Edit" single-line
                                                      :rules="[rule.fundingPercent]" :mask="'###'">
                                        </v-text-field>
                                    </template>
                                </v-edit-dialog>
                            </div>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>
        <v-flex xs12>
            <v-layout v-show="hasSelectedPriorityLibrary" justify-end row>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-blue-bg white--text" @click="onApplyToScenario"
                       :disabled="!hasSelectedPriorityLibrary">
                    Apply
                </v-btn>
                <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onUpdateLibrary"
                       :disabled="!hasSelectedPriorityLibrary">
                    Update Library
                </v-btn>
                <v-btn class="ara-blue-bg white--text" @click="onCreateAsNewLibrary" :disabled="!hasSelectedPriorityLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-orange-bg white--text" @click="onDiscardChanges"
                       :disabled="!hasSelectedPriorityLibrary">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <CreatePriorityLibraryDialog :dialogData="createPriorityLibraryDialogData" @submit="onCreatePriorityLibrary" />

        <CreatePriorityDialog :showDialog="showCreatePriorityDialog" @submit="onSubmitNewPriority" />

        <PrioritiesCriteriaEditor :dialogData="prioritiesCriteriaEditorDialogData" @submit="onSubmitPriorityCriteria" />
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {
        emptyPriorityLibrary,
        PrioritiesDataTableRow,
        Priority,
        PriorityFund, PriorityLibrary
    } from '@/shared/models/iAM/priority';
    import CreatePriorityDialog from '@/components/priority-editor/priority-editor-dialogs/CreatePriorityDialog.vue';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {clone, isNil, append, any, propEq} from 'ramda';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {EditBudgetsDialogData, emptyEditBudgetsDialogData} from '@/shared/models/modals/edit-budgets-dialog';
    import {InvestmentLibrary} from '@/shared/models/iAM/investment';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {
        CreatePriorityLibraryDialogData,
        emptyCreatePriorityLibraryDialogData
    } from '@/shared/models/modals/create-priority-library-dialog-data';
    import {sortByProperty} from '@/shared/utils/sorter-utils';
    import CreatePriorityLibraryDialog from '@/components/priority-editor/priority-editor-dialogs/CreatePriorityLibraryDialog.vue';
import prepend from 'ramda/es/prepend';
    const ObjectID = require('bson-objectid');

    @Component({
        components: {
            CreatePriorityLibraryDialog, CreatePriorityDialog, PrioritiesCriteriaEditor: CriteriaEditorDialog}
    })
    export default class PriorityEditor extends Vue {
        @State(state => state.priority.priorityLibraries) statePriorityLibraries: PriorityLibrary[];
        @State(state => state.priority.selectedPriorityLibrary) stateSelectedPriorityLibrary: PriorityLibrary;
        @State(state => state.priority.scenarioPriorityLibrary) stateScenarioPriorityLibrary: PriorityLibrary;

        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getPriorityLibraries') getPriorityLibrariesAction: any;
        @Action('getScenarioPriorityLibrary') getScenarioPriorityLibraryAction: any;
        @Action('selectPriorityLibrary') selectPriorityLibraryAction: any;
        @Action('updateSelectedPriorityLibrary') updateSelectedPriorityLibraryAction: any;
        @Action('createPriorityLibrary') createPriorityLibraryAction: any;
        @Action('updatePriorityLibrary') updatePriorityLibraryAction: any;
        @Action('saveScenarioPriorityLibrary') saveScenarioPriorityLibraryAction: any;

        selectedScenarioId: string = '0';
        hasSelectedPriorityLibrary: boolean = false;
        priorityLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string = '';
        priorityLibraries: PriorityLibrary[] = [];
        selectedPriorityLibrary: PriorityLibrary = clone(emptyPriorityLibrary);
        scenarioPriorityLibrary: PriorityLibrary = clone(emptyPriorityLibrary);
        budgetOrder: string[] = [];
        priorityDataTableHeaders: DataTableHeader[] = [
            {text: 'Priority', value: 'priorityLevel', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Year', value: 'year', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''}
        ];
        prioritiesDataTableRows: PrioritiesDataTableRow[] = [];
        selectedPriorityIndex: number = -1;
        createPriorityLibraryDialogData: CreatePriorityLibraryDialogData = clone(emptyCreatePriorityLibraryDialogData);
        showCreatePriorityDialog: boolean = false;
        prioritiesCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        editBudgetsDialogData: EditBudgetsDialogData = clone(emptyEditBudgetsDialogData);
        rule: any = {
            fundingPercent: (value: number) => (value >= 0 && value <= 100) || 'Value range is 0 to 100'
        };

        /**
         * Sets component UI properties that triggers cascading UI updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/PriorityEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    if (vm.selectedScenarioId === '0') {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.onClearSelectedPriorityLibrary();
                setTimeout(() => {
                    vm.getPriorityLibrariesAction()
                        .then(() => {
                            if (vm.selectedScenarioId !== '0') {
                                vm.getScenarioPriorityLibraryAction({
                                    selectedScenarioId: parseInt(vm.selectedScenarioId)
                                });
                            }
                        });
                });
            });
        }

        /**
         * Resets component UI properties that triggers cascading UI updates
         */
        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === '/PriorityEditor/Library/') {
                this.selectedScenarioId = '0';
                this.onClearSelectedPriorityLibrary();
                next();
            }
        }

        updated() {
            if (hasValue(this.selectedPriorityLibrary) && hasValue(this.selectedPriorityLibrary.priorities) &&
            this.budgetOrder.length <= 5) {
                this.selectedPriorityLibrary.priorities.forEach(priority => this.showEyeIcon(priority.id));
            }
        }

        /**
         * Sets the priorityLibraries list property with a copy of the statePriorityLibraries list property when
         * statePriorityLibraries list changes are detected
         */
        @Watch('statePriorityLibraries')
        onStatePriorityLibrariesChanged() {
            this.priorityLibraries = clone(this.statePriorityLibraries);
        }

        @Watch('priorityLibraries')
        onPriorityLibrariesChanged() {
            this.priorityLibrariesSelectListItems = this.priorityLibraries.map((priorityLibrary: PriorityLibrary) => ({
                text: priorityLibrary.name,
                value: priorityLibrary.id.toString()
            }));
        }

        /**
         * Sets the scenarioPriorityLibrary object = copy of scenarioPriorityLibrary object
         */
        @Watch('stateSelectedPriorityLibrary')
        onStateSelectedPriorityLibraryChanged() {
            this.selectedPriorityLibrary = clone(this.stateSelectedPriorityLibrary);
        }

        /**
         * Sets the selectedScenarioPriorityLibrary object = copy of stateScenarioSelectedPriorityLibrary object
         */
        @Watch('stateScenarioPriorityLibrary')
        onStateScenarioSelectedPriorityLibraryChanged() {
            this.scenarioPriorityLibrary = clone(this.stateScenarioPriorityLibrary);
        }

        @Watch('selectItemValue')
        onSelectItemValueChanged() {
            this.selectPriorityLibraryAction({priorityLibraryId: this.selectItemValue});
        }

        /**
         * Sets/resets the component UI properties reliant on a selected priority library
         */
        @Watch('selectedPriorityLibrary')
        onSelectedPriorityLibraryChanged() {
            if (hasValue(this.selectedPriorityLibrary) && this.selectedPriorityLibrary.id !== '0') {
                this.hasSelectedPriorityLibrary = true;

                const priorityFunds: PriorityFund[] = [];
                this.selectedPriorityLibrary.priorities
                    .forEach((priority: Priority) => priorityFunds.push(...priority.priorityFunds));
                this.budgetOrder = getPropertyValues('budget', priorityFunds);
            } else {
                this.hasSelectedPriorityLibrary = false;
                this.budgetOrder = [];
            }
        }

        /**
         * Sets data table properties by calling functions to set the table columns widths, table headers, and table data
         */
        @Watch('budgetOrder')
        onPriorityBudgetsChanged() {
            this.setTableColumnsWidth();
            this.setTableHeaders();
            this.setTableData();
        }

        onClearSelectedPriorityLibrary() {
            this.selectItemValue = hasValue(this.selectItemValue) ? '' : '0';
        }

        /**
         * Sets the width (as a percentage) of the Priority, Year, and Criteria data table columns
         */
        setTableColumnsWidth() {
            let criteriaColumnWidth = '';
            let otherColumnsWidth = '12.4%';

            switch (this.budgetOrder.length) {
                case 0:
                    criteriaColumnWidth = '75%';
                    break;
                case 1:
                    criteriaColumnWidth = '65%';
                    break;
                case 2:
                    criteriaColumnWidth = '55%';
                    break;
                case 3:
                    criteriaColumnWidth = '45%';
                    break;
                case 4:
                    criteriaColumnWidth = '35%';
                    break;
                case 5:
                    criteriaColumnWidth = '25%';
                    break;
                default:
                    otherColumnsWidth = '';
            }

            this.priorityDataTableHeaders[0].width = otherColumnsWidth;
            this.priorityDataTableHeaders[1].width = otherColumnsWidth;
            this.priorityDataTableHeaders[2].width = criteriaColumnWidth;
        }

        /**
         * Sets the table headers by adding additional headers if there are priority funds (additional columns are for
         * priority fund budgets)
         */
        setTableHeaders() {
            if (hasValue(this.budgetOrder)) {
                const budgetHeaders: DataTableHeader[] = this.budgetOrder.map((budgetName: string) => {
                    return {
                        text: `${budgetName} %`,
                        value: budgetName,
                        sortable: true,
                        align: 'left',
                        class: '',
                        width: ''
                    } as DataTableHeader;
                });

                this.priorityDataTableHeaders = [
                    this.priorityDataTableHeaders[0],
                    this.priorityDataTableHeaders[1],
                    this.priorityDataTableHeaders[2],
                    ...budgetHeaders
                ];
            }
        }

        /**
         * Sets the table data for the table rows
         */
        setTableData() {
            this.prioritiesDataTableRows = [];

            if (hasValue(this.selectedPriorityLibrary)) {
                this.selectedPriorityLibrary.priorities.forEach((priority: Priority) => {
                    const row: PrioritiesDataTableRow = {
                        priorityId: priority.id.toString(),
                        priorityLevel: priority.priorityLevel.toString(),
                        year: priority.year.toString(),
                        criteria: priority.criteria
                    };

                    this.budgetOrder.forEach((budgetName: any) => {
                        let amount = '';
                        if (any(propEq('budget', budgetName), priority.priorityFunds)) {
                            const priorityFund: PriorityFund = priority.priorityFunds
                                .find((pf: PriorityFund) => pf.budget === budgetName) as PriorityFund;
                            amount = priorityFund.funding.toString();
                        }
                        row[budgetName] = amount.toString();
                    });

                    this.prioritiesDataTableRows.push(row);
                });
            }
        }

        /**
         * Shows the CreatePriorityLibraryDialog to allow a user to create a new priority library
         */
        onNewLibrary() {
            this.createPriorityLibraryDialogData = {
                showDialog: true,
                description: '',
                priorities: []
            };
        }

        /**
         * Sets the showCreatePriorityDialog property to true
         */
        onAddPriority() {
            this.showCreatePriorityDialog = true;
        }

        /**
         * Receives a Priority object result from the CreatePriorityDialog and adds it to the priorities list property
         * @param newPriority Priority object
         */
        onSubmitNewPriority(newPriority: Priority) {
            this.showCreatePriorityDialog = false;

            if (!isNil(newPriority)) {
                if (hasValue(this.budgetOrder)) {
                    this.budgetOrder.forEach((budgetName: string) => {
                        newPriority.priorityFunds.push({
                            id: ObjectID.generate(),
                            budget: budgetName,
                            funding: 100
                        });
                    });
                }

                this.updateSelectedPriorityLibraryAction({updatedSelectedPriorityLibrary: {
                    ...this.selectedPriorityLibrary,
                    priorities: prepend(newPriority, this.selectedPriorityLibrary.priorities)
                }});
            }
        }

        /**
         * Sets the specified priority's property with the given value in the priorities list
         * @param priorityId Priority object's id
         * @param property Priority object's property
         * @param value The value to set for the Priority object's property
         */
        onEditPriorityProperty(priorityId: any, property: string, value: any) {
            if (any(propEq('id', priorityId), this.selectedPriorityLibrary.priorities)) {
                const index: number = this.selectedPriorityLibrary.priorities
                    .findIndex((priority: Priority) => priority.id === priorityId);

                const setValue = hasValue(value) ? value : 0;

                this.selectedPriorityLibrary.priorities = setItemPropertyValueInList(
                    index, property, setValue, this.selectedPriorityLibrary.priorities
                );

                this.updateSelectedPriorityLibraryAction({updatedSelectedPriorityLibrary: this.selectedPriorityLibrary});
            }
        }

        /**
         * Sets selectedPriority property with the specified target and sets the criteriaEditorDialogData property
         * values with the selectedPriority.criteria property
         * @param priorityId Priority object id
         * @param criteria Priority object criteria
         */
        onEditCriteria(priorityId: any, criteria: string) {
            this.selectedPriorityIndex = this.selectedPriorityLibrary.priorities
                .findIndex((p: Priority) => p.id === priorityId);

            this.prioritiesCriteriaEditorDialogData = {
                showDialog: true,
                criteria: criteria
            };
        }

        /**
         * Receives a criteria string result from the CriteriaEditor and sets the selectedPriority.criteria property
         * with it (if present)
         * @param criteria CriteriaEditor criteria string result
         */
        onSubmitPriorityCriteria(criteria: string) {
            this.prioritiesCriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectedPriorityLibrary.priorities[this.selectedPriorityIndex].criteria = criteria;

                this.selectedPriorityIndex = -1;

                this.updateSelectedPriorityLibraryAction({updatedSelectedPriorityLibrary: this.selectedPriorityLibrary});
            }
        }

        showEyeIcon(priorityId: any) {
            const inputElementId = `priority${priorityId}`;
            const inputElement = document.getElementById(inputElementId) as HTMLInputElement;
            const eyeIconElementId = `${inputElementId}EyeIcon`;
            const eyeIconElement = document.getElementById(eyeIconElementId) as HTMLElement;
            eyeIconElement.hidden = inputElement.scrollWidth <= inputElement.clientWidth;
        }

        /**
         * Sets a priority fund's funding amount with the specified amount for the specified priority with the priority
         * fund that has the specified budget name
         */
        onEditBudgetFunding(priorityId: any, budgetName: string, amount: number) {
            if (any(propEq('id', priorityId), this.selectedPriorityLibrary.priorities)) {
                const priorityIndex: number = this.selectedPriorityLibrary.priorities.
                findIndex((priority: Priority) => priority.id === priorityId);

                if (any(propEq('budget', budgetName), this.selectedPriorityLibrary.priorities[priorityIndex].priorityFunds)) {
                    const priorityFundIndex: number = this.selectedPriorityLibrary.priorities[priorityIndex].priorityFunds
                        .findIndex((priorityFund: PriorityFund) => priorityFund.budget === budgetName);

                    this.selectedPriorityLibrary.priorities[priorityIndex].priorityFunds[priorityFundIndex].funding = amount;

                    this.updateSelectedPriorityLibraryAction({updatedSelectedPriorityLibrary: this.selectedPriorityLibrary});
                }
            }
        }

        /**
         * Shows the CreatePriorityLibraryDialog and passes the selected priority library's data to it to allow a user to
         * create a new priority library with this data
         */
        onCreateAsNewLibrary() {
            this.createPriorityLibraryDialogData = {
                showDialog: true,
                description: this.selectedPriorityLibrary.description,
                priorities: this.selectedPriorityLibrary.priorities
            };
        }

        /**
         * Dispatches an action with a user's submitted CreatePriorityLibraryDialog result in order to create a new
         * priority library on the server
         * @param createdPriorityLibrary PriorityLibrary object data
         */
        onCreatePriorityLibrary(createdPriorityLibrary: PriorityLibrary) {
            this.createPriorityLibraryDialogData = clone(emptyCreatePriorityLibraryDialogData);

            if (!isNil(createdPriorityLibrary)) {
                createdPriorityLibrary = this.setIdsForNewPriorityLibraryRelatedData(createdPriorityLibrary);

                this.createPriorityLibraryAction({createdPriorityLibrary: createdPriorityLibrary})
                    .then(() => {
                        this.selectItemValue = '0';
                        setTimeout(() => {
                            this.selectItemValue = createdPriorityLibrary.id.toString();
                        });
                    });
            }
        }

        /**
         * Sets the ids for the createdPriorityLibrary object's priorities and the priority funds sub-data
         */
        setIdsForNewPriorityLibraryRelatedData(createdPriorityLibrary: PriorityLibrary) {
            if (hasValue(createdPriorityLibrary.priorities)) {
                createdPriorityLibrary.priorities = sortByProperty('id', createdPriorityLibrary.priorities)
                    .map((priority: Priority) => {
                        priority.id = ObjectID.generate();
                        if (hasValue(priority.priorityFunds)) {
                            priority.priorityFunds = sortByProperty('id', priority.priorityFunds)
                                .map((priorityFund: PriorityFund) => {
                                    priorityFund.id = ObjectID.generate();
                                    return priorityFund;
                                });
                        }
                        return priority;
                    });
            }

            return createdPriorityLibrary;
        }

        /**
         * Dispatches an action with the selected priority library data in order to update the selected priority library
         * on the server
         */
        onUpdateLibrary() {
            this.updatePriorityLibraryAction({updatedPriorityLibrary: this.selectedPriorityLibrary});
        }

        /**
         * Dispatches an action with the selected priority library data in order to update the selected scenario's
         * priority library data on the server
         */
        onApplyToScenario() {
            const appliedPriorityLibrary: PriorityLibrary = {
                ...this.selectedPriorityLibrary,
                id: this.selectedScenarioId,
                name: this.scenarioPriorityLibrary.name
            };

            this.saveScenarioPriorityLibraryAction({saveScenarioPriorityLibraryData: appliedPriorityLibrary})
                .then(() => {
                    this.selectItemValue = '0';
                    setTimeout(() => {
                        this.updateSelectedPriorityLibraryAction({
                            updatedSelectedPriorityLibrary: this.scenarioPriorityLibrary
                        });
                    });
                });
        }

        /**
         * Clears the selected priority library and dispatches an action to update the selected priority library in state
         * with the scenario priority library (if present), otherwise an action is dispatched to get the scenario priority
         * library from the server to update the selected priority library in state
         */
        onDiscardChanges() {
            this.selectItemValue = '0';

            if (this.scenarioPriorityLibrary.id !== '0') {
                setTimeout(() => {
                    this.updateSelectedPriorityLibraryAction({
                        updatedPriorityLibrary: this.scenarioPriorityLibrary
                    });
                });
            } else {
                setTimeout(() => {
                    this.getScenarioPriorityLibraryAction({selectedScenarioId: this.selectedScenarioId});
                });
            }
        }
    }
</script>

<style>
    .criteria-input-icons {
        margin-left: 1px;
    }
</style>