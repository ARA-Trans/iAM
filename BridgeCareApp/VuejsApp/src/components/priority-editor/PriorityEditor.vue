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
                <v-btn class="ara-blue-bg white--text" @click="showCreatePriorityDialog = true">Add</v-btn>
                <v-btn class="ara-orange-bg white--text" @click="onDeletePriorities" :disabled="selectedPriorityIds.length === 0">
                    Delete
                </v-btn>
            </v-flex>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedPriorityLibrary">
            <div class="priorities-data-table">
                <v-data-table :headers="priorityDataTableHeaders" :items="prioritiesDataTableRows"
                              v-model="selectedPriorityRows" select-all item-key="id"
                              class="elevation-1 v-table__overflow">
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-checkbox v-model="props.selected" primary hide-details></v-checkbox>
                        </td>
                        <td v-for="header in priorityDataTableHeaders">
                            <div v-if="header.value === 'priorityLevel' || header.value === 'year'">
                                <v-edit-dialog @save="onEditPriorityProperty(props.item, header.value, props.item[header.value])"
                                               :return-value.sync="props.item[header.value]" large lazy persistent>
                                    <input type="text" class="output" readonly :value="props.item[header.value]" />
                                    <template slot="input">
                                        <v-text-field v-model="props.item[header.value]" label="Edit" single-line>
                                        </v-text-field>
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-else-if="header.value === 'criteria'">
                                <v-layout align-center row style="flex-wrap:nowrap">
                                    <v-menu bottom min-width="500px" min-height="500px">
                                        <template slot="activator">
                                            <v-btn v-if="budgets.length > 5" icon class="ara-blue">
                                                <v-icon>fas fa-eye</v-icon>
                                            </v-btn>
                                            <input v-else type="text" class="output priority-criteria-output" readonly
                                                   :value="props.item.criteria" />
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea rows="5" no-resize readonly full-width outline
                                                            :value="props.item.criteria">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn icon class="edit-icon" @click="onEditPriorityCriteria(props.item)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-layout>
                            </div>
                            <div v-else>
                                <v-edit-dialog @save="onEditPriorityFundAmount(props.item, header.value, props.item[header.value])"
                                               :return-value.sync="props.item[header.value]" large lazy persistent>
                                    <input class="output" type="text" readonly :value="props.item[header.value]"
                                           :rules="[rule.fundingPercent]" />
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
        <v-flex xs12 v-show="hasSelectedPriorityLibrary && selectedPriorityLibrary.id !== stateScenarioPriorityLibrary.id">
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea rows="4" no-resize outline label="Description" v-model="selectedPriorityLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout v-show="hasSelectedPriorityLibrary" justify-end row>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-blue-bg white--text" @click="onApplyPriorityLibraryToScenario">
                    Save
                </v-btn>
                <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onUpdatePriorityLibrary">
                    Update Library
                </v-btn>
                <v-btn class="ara-blue-bg white--text" @click="onAddAsNewPriorityLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-orange-bg white--text" @click="onDiscardPriorityLibraryChanges">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <CreatePriorityLibraryDialog :dialogData="createPriorityLibraryDialogData" @submit="onCreateNewPriorityLibrary" />

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
        emptyPriority,
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
    import {clone, isNil, contains, any, propEq, flatten, findIndex, update, find, prepend} from 'ramda';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {EditBudgetsDialogData, emptyEditBudgetsDialogData} from '@/shared/models/modals/edit-budgets-dialog';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {setItemPropertyValue} from '@/shared/utils/setter-utils';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {
        CreatePriorityLibraryDialogData,
        emptyCreatePriorityLibraryDialogData
    } from '@/shared/models/modals/create-priority-library-dialog-data';
    import {sorter} from '@/shared/utils/sorter-utils';
    import CreatePriorityLibraryDialog from '@/components/priority-editor/priority-editor-dialogs/CreatePriorityLibraryDialog.vue';
    import {InvestmentLibrary} from '@/shared/models/iAM/investment';
    import {Attribute} from '@/shared/models/iAM/attribute';
    const ObjectID = require('bson-objectid');

    @Component({
        components: {
            CreatePriorityLibraryDialog, CreatePriorityDialog, PrioritiesCriteriaEditor: CriteriaEditorDialog}
    })
    export default class PriorityEditor extends Vue {
        @State(state => state.priorityEditor.priorityLibraries) statePriorityLibraries: PriorityLibrary[];
        @State(state => state.priorityEditor.selectedPriorityLibrary) stateSelectedPriorityLibrary: PriorityLibrary;
        @State(state => state.priorityEditor.scenarioPriorityLibrary) stateScenarioPriorityLibrary: PriorityLibrary;
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getPriorityLibraries') getPriorityLibrariesAction: any;
        @Action('selectPriorityLibrary') selectPriorityLibraryAction: any;
        @Action('createPriorityLibrary') createPriorityLibraryAction: any;
        @Action('updatePriorityLibrary') updatePriorityLibraryAction: any;
        @Action('getScenarioPriorityLibrary') getScenarioPriorityLibraryAction: any;
        @Action('saveScenarioPriorityLibrary') saveScenarioPriorityLibraryAction: any;
        @Action('getScenarioInvestmentLibrary') getScenarioInvestmentLibraryAction: any;

        selectedScenarioId: string = '0';
        hasSelectedPriorityLibrary: boolean = false;
        priorityLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string = '';
        selectedPriorityLibrary: PriorityLibrary = clone(emptyPriorityLibrary);
        budgets: string[] = [];
        prioritiesDataTableRows: PrioritiesDataTableRow[] = [];
        priorityDataTableHeaders: DataTableHeader[] = [
            {text: 'Priority', value: 'priorityLevel', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Year', value: 'year', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''}
        ];
        selectedPriorityRows: PrioritiesDataTableRow[] = [];
        selectedPriorityIds: string[] = [];
        selectedPriority: Priority = clone(emptyPriority);
        showCreatePriorityDialog: boolean = false;
        prioritiesCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        createPriorityLibraryDialogData: CreatePriorityLibraryDialogData = clone(emptyCreatePriorityLibraryDialogData);
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

        /**
         * Sets the component's priorityLibrariesSelectListItems array using the priority libraries data in state
         */
        @Watch('statePriorityLibraries')
        onStatePriorityLibrariesChanged() {
            this.priorityLibrariesSelectListItems = this.statePriorityLibraries.map((priorityLibrary: PriorityLibrary) => ({
                text: priorityLibrary.name,
                value: priorityLibrary.id
            }));
        }

        /**
         * Dispatches an action to set the selected priority library in state
         */
        @Watch('selectItemValue')
        onSelectItemValueChanged() {
            const selectedPriorityLibrary: PriorityLibrary = find(
                propEq('id', this.selectItemValue), this.statePriorityLibraries
            ) as PriorityLibrary;

            this.selectPriorityLibraryAction({
                selectedPriorityLibrary: hasValue(selectedPriorityLibrary) ? selectedPriorityLibrary : emptyPriorityLibrary
            });
        }

        /**
         * Sets the components scenarioPriorityLibrary object with the selected priority library data in state
         */
        @Watch('stateSelectedPriorityLibrary')
        onStateSelectedPriorityLibraryChanged() {
            this.selectedPriorityLibrary = clone(this.stateSelectedPriorityLibrary);
            this.hasSelectedPriorityLibrary = this.selectedPriorityLibrary.id !== '0';
            this.budgets = getPropertyValues('budget',
                flatten(getPropertyValues('priorityFunds', this.selectedPriorityLibrary.priorities))
            );
            this.setTableCriteriaColumnWidth();
            this.setTableHeaders();
            this.setTableData();
        }

        /**
         * Sets the component's selectedPriorityIds array using the selectedPriorityRows array
         */
        @Watch('selectedPriorityRows')
        onSelectedPriorityRowsChanged() {
            this.selectedPriorityIds = getPropertyValues('id', this.selectedPriorityRows) as string[];
        }

        /**
         * Sets selectItemValue to '' or '0' to de-select the selected priority library
         */
        onClearSelectedPriorityLibrary() {
            this.selectItemValue = hasValue(this.selectItemValue) ? '' : '0';
        }

        /**
         * Sets the width (as a percentage) of the Priority, Year, and Criteria data table columns
         */
        setTableCriteriaColumnWidth() {
            let criteriaColumnWidth = '';

            switch (this.budgets.length) {
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
            }

            this.priorityDataTableHeaders[2].width = criteriaColumnWidth;
        }

        /**
         * Sets the table headers by adding additional headers if there are priority funds (additional columns are for
         * priority fund budgets)
         */
        setTableHeaders() {
            if (hasValue(this.budgets)) {
                const budgetHeaders: DataTableHeader[] = this.budgets.map((budget: string) => {
                    return {
                        text: `${budget} %`,
                        value: budget,
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

            if (this.hasSelectedPriorityLibrary) {
                this.prioritiesDataTableRows = this.selectedPriorityLibrary.priorities.map((priority: Priority) => {
                    const row: PrioritiesDataTableRow = {
                        id: priority.id,
                        priorityLevel: priority.priorityLevel.toString(),
                        year: priority.year === null || priority.year === undefined ? '' : priority.year.toString(),
                        criteria: priority.criteria
                    };

                    this.budgets.forEach((budget: any) => {
                        let amount = '';
                        if (any(propEq('budget', budget), priority.priorityFunds)) {
                            const priorityFund: PriorityFund = priority.priorityFunds
                                .find((pf: PriorityFund) => pf.budget === budget) as PriorityFund;
                            amount = priorityFund.funding.toString();
                        }
                        row[budget] = amount.toString();
                    });

                    return row;
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
         * Adds the CreatePriorityDialog's result to the selected priority library's priorities list
         * @param newPriority CreatePriorityDialog's Priority object result
         */
        onSubmitNewPriority(newPriority: Priority) {
            this.showCreatePriorityDialog = false;

            if (!isNil(newPriority)) {
                if (hasValue(this.budgets)) {
                    this.budgets.forEach((budgetName: string) => {
                        newPriority.priorityFunds.push({
                            id: ObjectID.generate(),
                            budget: budgetName,
                            funding: 100
                        });
                    });
                }

                this.selectPriorityLibraryAction({selectedPriorityLibrary: {
                        ...this.selectedPriorityLibrary,
                        priorities: prepend(newPriority, this.selectedPriorityLibrary.priorities)
                    }
                });
            }
        }

        /**
         * Sets the specified priority's property with the given value in the priorities list
         * @param priorityRow PrioritiesDataTableRow object
         * @param property Selected Priority object's property
         * @param value Value to set on the selected Priority object's property
         */
        onEditPriorityProperty(priorityRow: PrioritiesDataTableRow, property: string, value: any) {
            if (any(propEq('id', priorityRow.id), this.selectedPriorityLibrary.priorities)) {
                const priority: Priority = find(
                    propEq('id', priorityRow.id), this.selectedPriorityLibrary.priorities
                ) as Priority;

                this.selectPriorityLibraryAction({selectedPriorityLibrary: {
                        ...this.selectedPriorityLibrary,
                        priorities: update(
                            findIndex(propEq('id', priorityRow.id), this.selectedPriorityLibrary.priorities),
                            setItemPropertyValue(property, value, priority) as Priority,
                            this.selectedPriorityLibrary.priorities
                        )
                    }
                });
            }
        }

        /**
         * Updates the selected priority's priority fund's funding amount (where budget = budgetName)
         * @param priorityRow PrioritiesDataTableRow object
         * @param budget A PriorityFund object's budget property value (used to find the object)
         * @param amount Value to set on the found PriorityFund object's amount property
         */
        onEditPriorityFundAmount(priorityRow: PrioritiesDataTableRow, budget: string, amount: number) {
            const priority: Priority = find(propEq('id', priorityRow.id), this.selectedPriorityLibrary.priorities) as Priority;
            const priorityFund: PriorityFund = find(propEq('budget', budget), priority.priorityFunds) as PriorityFund;

            this.selectPriorityLibraryAction({selectedPriorityLibrary: {
                    ...this.selectedPriorityLibrary,
                    priorities: update(
                        findIndex(propEq('id', priority.id), this.selectedPriorityLibrary.priorities),
                        {...priority, priorityFunds: update(
                            findIndex(propEq('id', priorityFund.id), priority.priorityFunds),
                            setItemPropertyValue('funding', amount, priorityFund) as PriorityFund,
                            priority.priorityFunds
                        )} as Priority,
                        this.selectedPriorityLibrary.priorities
                    )
                }
            });
        }

        /**
         * Enables the CriteriaEditorDialog and sends to it the selected priority's criteria
         * @param priorityRow PrioritiesDataTableRow object
         */
        onEditPriorityCriteria(priorityRow: PrioritiesDataTableRow) {
            this.selectedPriority = find(propEq('id', priorityRow.id), this.selectedPriorityLibrary.priorities) as Priority;

            this.prioritiesCriteriaEditorDialogData = {
                showDialog: true,
                criteria: priorityRow.criteria
            };
        }

        /**
         * Updates the selected priority's criteria with the CriteriaEditorDialog's result
         * @param criteria CriteriaEditorDialog result
         */
        onSubmitPriorityCriteria(criteria: string) {
            this.prioritiesCriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectPriorityLibraryAction({selectedPriorityLibrary: {
                        ...this.selectedPriorityLibrary,
                        priorities: update(
                            findIndex(propEq('id', this.selectedPriority.id), this.selectedPriorityLibrary.priorities),
                            setItemPropertyValue('criteria', criteria, this.selectedPriority),
                            this.selectedPriorityLibrary.priorities
                        )
                    }
                });
            }

            this.selectedPriority = clone(emptyPriority);
        }

        /**
         * Enables the CreatePriorityLibraryDialog and sends to it the selected priority library's description &
         * priorities data
         */
        onAddAsNewPriorityLibrary() {
            this.createPriorityLibraryDialogData = {
                showDialog: true,
                description: this.selectedPriorityLibrary.description,
                priorities: this.selectedPriorityLibrary.priorities
            };
        }

        /**
         * Dispatches an action to create a new priority library in the mongo database
         * @param createdPriorityLibrary New PriorityLibrary object data
         */
        onCreateNewPriorityLibrary(createdPriorityLibrary: PriorityLibrary) {
            this.createPriorityLibraryDialogData = clone(emptyCreatePriorityLibraryDialogData);

            if (!isNil(createdPriorityLibrary)) {
                this.createPriorityLibraryAction({createdPriorityLibrary: createdPriorityLibrary})
                    .then(() => this.selectItemValue = createdPriorityLibrary.id);
            }
        }

        /**
         * Dispatches an action to update the scenario's priority library data in the sql server database
         */
        onApplyPriorityLibraryToScenario() {
            this.saveScenarioPriorityLibraryAction({
                saveScenarioPriorityLibraryData: {...this.selectedPriorityLibrary, id: this.selectedScenarioId}
            }).then(() => this.onDiscardPriorityLibraryChanges());
        }

        /**
         * Dispatches an action to update the selected priority library with the selected priorities removed (filtered)
         * from the priorities list
         */
        onDeletePriorities() {
            this.selectPriorityLibraryAction({selectedPriorityLibrary: {
                    ...this.selectedPriorityLibrary,
                    priorities: this.selectedPriorityLibrary.priorities
                        .filter((priority: Priority) => !contains(priority.id, this.selectedPriorityIds))
                }
            });
        }

        /**
         * Dispatches an action to clear changes made to the selected priority library
         */
        onDiscardPriorityLibraryChanges() {
            this.onClearSelectedPriorityLibrary();
            setTimeout(() => {
                this.selectPriorityLibraryAction({selectedPriorityLibrary: this.stateScenarioPriorityLibrary});
            });
        }

        /**
         * Dispatches an action to update the selected priority library data in the mongo database
         */
        onUpdatePriorityLibrary() {
            this.updatePriorityLibraryAction({updatedPriorityLibrary: this.selectedPriorityLibrary});
        }
    }
</script>

<style>
    .criteria-input-icons {
        margin-left: 1px;
    }

    .priorities-data-table {
        height: 425px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .priorities-data-table .v-menu--inline, .priority-criteria-output {
        width: 100%;
    }
</style>