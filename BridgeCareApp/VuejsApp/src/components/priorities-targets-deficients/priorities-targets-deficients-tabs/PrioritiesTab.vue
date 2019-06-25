<template>
    <v-container fluid grid-list-xl>
        <div>
            <v-layout>
                <v-flex>
                    <v-btn color="info" @click="onAddPriority">Add</v-btn>
                    <v-btn color="info lighten-1" @click="onEditBudgets" :disabled="priorities.length === 0">Edit Budgets</v-btn>
                </v-flex>
            </v-layout>
            <v-layout>
                <v-flex>
                    <div class="priorities-data-table">
                        <v-data-table :headers="priorityDataTableHeaders" :items="prioritiesDataTableRows"
                                      class="elevation-1 fixed-header v-table__overflow" hide-actions>
                            <template slot="items" slot-scope="props">
                                <td v-for="header in priorityDataTableHeaders">
                                    <div v-if="header.value === 'priorityLevel'">
                                        <v-edit-dialog :return-value.sync="props.item.priorityLevel" large lazy persistent
                                                       @save="onEditPriorityProperty(props.item.priorityId, header.value, props.item[header.value])">
                                            <v-text-field readonly :value="props.item.priorityLevel"></v-text-field>
                                            <template slot="input">
                                                <v-text-field v-model="props.item.priorityLevel" label="Edit" single-line>
                                                </v-text-field>
                                            </template>
                                        </v-edit-dialog>
                                    </div>
                                    <div v-else-if="header.value === 'year'">
                                        <v-edit-dialog :return-value.sync="props.item.year" large lazy persistent
                                                       @save="onEditPriorityProperty(props.item.priorityId, header.value, props.item[header.value])">
                                            <v-text-field readonly :value="props.item.year" @click="setPriorityYear(props.item.year)">
                                            </v-text-field>
                                            <template slot="input">
                                                <EditPriorityYearDialog :itemYear="priorityYear.toString()"
                                                                        @editedYear="props.item.year = $event" />
                                            </template>
                                        </v-edit-dialog>
                                    </div>
                                    <div v-else-if="header.value === 'criteria'">
                                        <v-text-field readonly :value="props.item.criteria" append-outer-icon="edit"
                                                      @click:append-outer="onEditCriteria(props.item.priorityId, props.item.criteria)">
                                        </v-text-field>
                                    </div>
                                    <div v-else>
                                        <v-edit-dialog :return-value.sync="props.item[header.value]" large lazy persistent
                                                       @save="onEditBudgetFunding(props.item.priorityId, header.value, props.item[header.value])">
                                            <v-text-field readonly :value="props.item[header.value]"></v-text-field>
                                            <template slot="input">
                                                <v-text-field v-model="props.item[header.value]" label="Edit" single-line>
                                                </v-text-field>
                                            </template>
                                        </v-edit-dialog>
                                    </div>
                                </td>
                                <!--<td>

                                </td>
                                <td>

                                </td>-->
                            </template>
                        </v-data-table>
                    </div>
                </v-flex>
            </v-layout>
        </div>

        <CreatePriorityDialog :dialogData="createPriorityDialogData" @submit="onSubmitNewPriority" />

        <PrioritiesCriteriaEditor :dialogData="prioritiescriteriaEditorDialogData" @submit="onSubmitPriorityCriteria" />

        <EditBudgetsDialog :dialogData="editBudgetsDialogData" @submit="onSubmitEditedBudgets" />

        <v-footer>
            <v-layout class="priorities-targets-deficients-buttons" justify-end row fill-height>
                <v-btn color="info" @click="onSavePriorities">Save</v-btn>
                <v-btn color="error" @click="onCancelChangesToPriorities">Cancel</v-btn>
            </v-layout>
        </v-footer>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch, Prop} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {PrioritiesDataTableRow, Priority, PriorityFund} from '@/shared/models/iAM/priority';
    import CreatePriorityDialog from '@/components/priorities-targets-deficients/dialogs/priorities-dialogs/CreatePriorityDialog.vue';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import EditBudgetsDialog from '../../../shared/modals/EditBudgetsDialog.vue';
    import EditYearDialog from '@/components/priorities-targets-deficients/dialogs/shared/EditYearDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {clone, isNil, append, concat, isEmpty, any, propEq} from 'ramda';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {EditedBudget} from '@/shared/models/modals/edit-budgets-dialog';
    import {EditBudgetsDialogData, emptyEditBudgetsDialogData} from '@/shared/models/modals/edit-budgets-dialog';
    import {
        CreatePrioritizationDialogData,
        emptyCreatePrioritizationDialogData
    } from '@/shared/models/modals/create-prioritization-dialog-data';
    const ObjectID = require('bson-objectid');

    @Component({
        components: {
            EditBudgetsDialog,
            EditPriorityYearDialog: EditYearDialog, CreatePriorityDialog, PrioritiesCriteriaEditor: CriteriaEditorDialog}
    })
    export default class PrioritiesTab extends Vue {
        @Prop() selectedScenarioId: number;

        @State(state => state.priority.priorities) statePriorities: Priority[];

        @Action('savePriorities') savePrioritiesAction: any;

        priorities: Priority[] = [];
        priorityDataTableHeaders: DataTableHeader[] = [
            {text: 'Priority', value: 'priorityLevel', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Year', value: 'year', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''}
        ];
        prioritiesDataTableRows: PrioritiesDataTableRow[] = [];
        priorityBudgets: string[] = [];
        priorityYear: number = -1;
        selectedPriorityIndex: number = -1;
        createPriorityDialogData: CreatePrioritizationDialogData = clone(emptyCreatePrioritizationDialogData);
        prioritiescriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        editBudgetsDialogData: EditBudgetsDialogData = clone(emptyEditBudgetsDialogData);

        /**
         * Sets the priorities list property with a copy of the statePriorities list property when statePriorities list
         * changes are detected
         */
        @Watch('statePriorities')
        onStatePrioritiesChanged() {
            this.priorities = clone(this.statePriorities);
        }

        /**
         * Sets the latestPriorityId property when priorities list changes are detected
         */
        @Watch('priorities')
        onPrioritiesChanged() {
            this.setPriorityBudgets();
        }

        /**
         * Sets data table properties by calling functions to set the table columns widths, table headers, and table data
         */
        @Watch('priorityBudgets')
        onPriorityBudgetsChanged() {
            this.setTableColumnsWidth();
            this.setTableHeaders();
            this.setTableData();
        }

        /**
         * Sets the priorityBudgets property with the budget names of any priority funds found in the list of priorities
         */
        setPriorityBudgets() {
            let priorityFunds: PriorityFund[] = [];
            this.priorities.forEach((priority: Priority) => {
                priorityFunds = concat(priorityFunds, hasValue(priority.priorityFunds) ? priority.priorityFunds : []);
            });

            this.priorityBudgets = getPropertyValues('budget', priorityFunds) as string[];
        }

        /**
         * Sets the width (as a percentage) of the Priority, Year, and Criteria data table columns
         */
        setTableColumnsWidth() {
            let criteriaColumnWidth = '';
            let otherColumnsWidth = '12.4%';

            switch (this.priorityBudgets.length) {
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
            if (hasValue(this.priorityBudgets)) {
                const budgetHeaders: DataTableHeader[] = this.priorityBudgets.map((budgetName: string) => {
                    return {
                        text: budgetName,
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

            this.priorities.forEach((priority: Priority) => {
                const row: PrioritiesDataTableRow = {
                    priorityId: priority.id.toString(),
                    priorityLevel: priority.priorityLevel.toString(),
                    year: priority.year.toString(),
                    criteria: priority.criteria
                };

                this.priorityBudgets.forEach((budgetName: string) => {
                    const priorityFund: PriorityFund = priority.priorityFunds
                        .find((priorityFund: PriorityFund) => priorityFund.budget === budgetName) as PriorityFund;

                    row[budgetName] = hasValue(priorityFund.funding) ? priorityFund.funding.toString() : '0';
                });

                this.prioritiesDataTableRows.push(row);
            });
        }

        /**
         * Sets the showCreatePriorityDialog property to true
         */
        onAddPriority() {
            this.createPriorityDialogData = {
                showDialog: true,
                scenarioId: this.selectedScenarioId
            };
        }

        /**
         * Receives a Priority object result from the CreatePriorityDialog and adds it to the priorities list property
         * @param newPriority Priority object
         */
        onSubmitNewPriority(newPriority: Priority) {
            this.createPriorityDialogData = clone(emptyCreatePrioritizationDialogData);

            if (!isNil(newPriority)) {
                newPriority.id = ObjectID.generate();

                if (hasValue(this.priorityBudgets)) {
                    this.priorityBudgets.forEach((budgetName: string) => {
                        newPriority.priorityFunds.push({
                            priorityId: newPriority.id,
                            id: ObjectID.generate(),
                            budget: budgetName,
                            funding: 0
                        });
                    });
                }

                this.priorities = append(newPriority, this.priorities);
            }
        }

        onEditPriorityProperty(priorityId: any, property: string, value: any) {
            if (any(propEq('id', priorityId), this.priorities)) {
                const index: number = this.priorities.findIndex((priority: Priority) => priority.id === priorityId);
                // @ts-ignore
                this.priorities[index][property] = value;
            }
        }

        setPriorityYear(year: number) {
            this.priorityYear = year;
        }

        /**
         * Sets selectedPriority property with the specified target and sets the criteriaEditorDialogData property
         * values with the selectedPriority.criteria property
         * @param priority Priority object
         */
        onEditCriteria(priorityId: any, criteria: string) {
            this.selectedPriorityIndex = this.priorities.findIndex((p: Priority) => p.id === priorityId);

            this.prioritiescriteriaEditorDialogData = {
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
            this.prioritiescriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                const priorities = clone(this.priorities);
                priorities[this.selectedPriorityIndex].criteria = criteria;
                this.selectedPriorityIndex = -1;
                this.priorities = priorities;

            }
        }

        /**
         * Shows the EditBudgetsDialog by setting the editBudgetsDialogData.showDialog property to true & passes in the
         * budget names by setting the editBudgetsDialogData.budgets property using the priorityBudgets property
         */
        onEditBudgets() {
            this.editBudgetsDialogData = {
                showDialog: true,
                budgets: this.priorityBudgets,
                canOrderBudgets: false
            };
        }

        /**
         * Re-orders existing priority funds & adds new priority funds for all priorities based on the result (if any)
         * of the EditBudgetsDialog
         */
        onSubmitEditedBudgets(editedBudgets: EditedBudget[]) {
            this.editBudgetsDialogData = clone(emptyEditBudgetsDialogData);

            if (!isNil(editedBudgets)) {
                const updatedPriorities: Priority[] = [];

                clone(this.priorities).forEach((priority: Priority) => {
                    const editedPriorityFunds: PriorityFund[] = [];

                    if (!isEmpty(editedBudgets)) {
                        editedBudgets.forEach((editedBudget: EditedBudget) => {
                            if (editedBudget.isNew) {
                                editedPriorityFunds.push({
                                    priorityId: priority.id,
                                    id: ObjectID.generate(),
                                    budget: editedBudget.name,
                                    funding: 0
                                });
                            } else {
                                const priorityFund: PriorityFund = priority.priorityFunds
                                    .find((priorityFund: PriorityFund) =>
                                        priorityFund.budget === editedBudget.previousName
                                    ) as PriorityFund;
                                editedPriorityFunds.push({
                                    ...priorityFund,
                                    budget: editedBudget.name
                                });
                            }
                        });
                    }

                    priority.priorityFunds = editedPriorityFunds;
                    updatedPriorities.push(priority);
                });

                this.priorities = updatedPriorities;
            }
        }

        /**
         * Sets a priority fund's funding amount with the specified amount for the specified priority with the priority
         * fund that has the specified budget name
         */
        onEditBudgetFunding(priorityId: any, budgetName: string, amount: number) {
            if (any(propEq('id', priorityId), this.priorities)) {
                const priorityIndex: number = this.priorities.findIndex((priority: Priority) => priority.id === priorityId);

                if (any(propEq('budget', budgetName), this.priorities[priorityIndex].priorityFunds)) {
                    const priorityFundIndex: number = this.priorities[priorityIndex].priorityFunds
                        .findIndex((priorityFund: PriorityFund) => priorityFund.budget === budgetName);

                    this.priorities[priorityIndex].priorityFunds[priorityFundIndex].funding = amount;
                }
            }
        }

        /**
         * Sends priority data changes to the server for upsert
         */
        onSavePriorities() {
            this.savePrioritiesAction({selectedScenarioId: this.selectedScenarioId, priorityData: this.priorities});
        }

        /**
         * Discards priority data changes by resetting the priorities list with a new copy of the statePriorities list
         */
        onCancelChangesToPriorities() {
            this.priorities = clone(this.statePriorities);
        }
    }
</script>