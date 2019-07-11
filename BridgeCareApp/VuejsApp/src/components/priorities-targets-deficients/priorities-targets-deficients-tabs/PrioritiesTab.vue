<template>
    <v-container fluid grid-list-xl>
        <div>
            <v-layout>
                <v-flex xs3>
                    <v-btn color="info" @click="onAddPriority"
                           :disabled="budgetOrder.length === 0 || scenarioInvestmentLibrary.id === 0">
                        Add
                    </v-btn>
                    <v-tooltip v-if="scenarioInvestmentLibrary === null" top>
                        <template>
                            <v-icon slot="activator" color="error" class="fas fa-exclamation-circle"></v-icon>
                        </template>
                        <span>No applied investment library found</span>
                    </v-tooltip>
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
                                                <EditPriorityYearDialog :itemYear="priorityYear.toString()" :itemLabel="'Year'"
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
            </v-layout>
        </div>

        <CreatePriorityDialog :dialogData="createPriorityDialogData" @submit="onSubmitNewPriority" />

        <PrioritiesCriteriaEditor :dialogData="prioritiesCriteriaEditorDialogData" @submit="onSubmitPriorityCriteria" />

        <v-footer>
            <v-layout class="priorities-targets-deficients-buttons" justify-end row fill-height>
                <v-btn color="info" @click="onSavePriorities" :disabled="priorities.length === 0">Save</v-btn>
                <v-btn color="error" @click="onCancelChangesToPriorities" :disabled="priorities.length === 0">Cancel</v-btn>
            </v-layout>
        </v-footer>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch, Prop} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {
        PrioritiesDataTableRow,
        Priority,
        PriorityFund
    } from '@/shared/models/iAM/priority';
    import CreatePriorityDialog from '@/components/priorities-targets-deficients/dialogs/priorities-dialogs/CreatePriorityDialog.vue';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import EditYearDialog from '@/shared/modals/EditYearDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {clone, isNil, append, any, propEq, groupBy} from 'ramda';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {EditBudgetsDialogData, emptyEditBudgetsDialogData} from '@/shared/models/modals/edit-budgets-dialog';
    import {
        CreatePrioritizationDialogData,
        emptyCreatePrioritizationDialogData
    } from '@/shared/models/modals/create-prioritization-dialog-data';
    import {InvestmentLibrary, InvestmentLibraryBudgetYear} from '@/shared/models/iAM/investment';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
    const ObjectID = require('bson-objectid');

    @Component({
        components: {
            EditPriorityYearDialog: EditYearDialog, CreatePriorityDialog, PrioritiesCriteriaEditor: CriteriaEditorDialog}
    })
    export default class PrioritiesTab extends Vue {
        @Prop() selectedScenarioId: number;

        @State(state => state.priority.priorities) statePriorities: Priority[];
        @State(state => state.investmentEditor.scenarioInvestmentLibrary) scenarioInvestmentLibrary: InvestmentLibrary;

        @Action('savePriorities') savePrioritiesAction: any;

        priorities: Priority[] = [];
        budgetOrder: string[] = [];
        priorityDataTableHeaders: DataTableHeader[] = [
            {text: 'Priority', value: 'priorityLevel', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Year', value: 'year', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''}
        ];
        prioritiesDataTableRows: PrioritiesDataTableRow[] = [];
        priorityYear: number = -1;
        selectedPriorityIndex: number = -1;
        createPriorityDialogData: CreatePrioritizationDialogData = clone(emptyCreatePrioritizationDialogData);
        prioritiesCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        editBudgetsDialogData: EditBudgetsDialogData = clone(emptyEditBudgetsDialogData);
        rule: any = {
            fundingPercent: (value: number) => (value >= 0 && value <= 100) || 'Value range is 0 to 100'
        };

        /**
         * Sets the priorities list property with a copy of the statePriorities list property when statePriorities list
         * changes are detected
         */
        @Watch('statePriorities')
        onStatePrioritiesChanged() {
            this.priorities = clone(this.statePriorities);
        }

        /**
         * Sets data table properties by calling functions to set the table columns widths, table headers, and table data
         */
        @Watch('priorities')
        onPrioritiesChanged() {
            const priorityFunds: PriorityFund[] = [];
            this.priorities.forEach((priority: Priority) => priorityFunds.push(...priority.priorityFunds));
            this.budgetOrder = getPropertyValues('budget', priorityFunds);
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

            this.priorities.forEach((priority: Priority) => {
                const row: PrioritiesDataTableRow = {
                    priorityId: priority.id.toString(),
                    priorityLevel: priority.priorityLevel.toString(),
                    year: priority.year.toString(),
                    criteria: priority.criteria
                };

                this.budgetOrder.forEach((budgetName: any) => {
                    let amount = 100;
                    if (any(propEq('budget', budgetName), priority.priorityFunds)) {
                        const priorityFund: PriorityFund = priority.priorityFunds
                            .find((pf: PriorityFund) => pf.budget === budgetName) as PriorityFund;
                        amount = priorityFund.funding;
                    } else {

                    }
                    row[budgetName] = amount.toString();
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

                if (hasValue(this.budgetOrder)) {
                    this.budgetOrder.forEach((budgetName: string) => {
                        newPriority.priorityFunds.push({
                            priorityId: newPriority.id,
                            id: ObjectID.generate(),
                            budget: budgetName,
                            funding: 100
                        });
                    });
                }

                this.priorities = append(newPriority, this.priorities);
            }
        }

        /**
         * Sets the specified priority's property with the given value in the priorities list
         * @param priorityId Priority object's id
         * @param property Priority object's property
         * @param value The value to set for the Priority object's property
         */
        onEditPriorityProperty(priorityId: any, property: string, value: any) {
            if (any(propEq('id', priorityId), this.priorities)) {
                const index: number = this.priorities.findIndex((priority: Priority) => priority.id === priorityId);
                this.priorities = setItemPropertyValueInList(index, property, value, this.priorities);
            }
        }

        /**
         * Sets the priorityYear property with the given year value
         * @param year The year value
         */
        setPriorityYear(year: number) {
            this.priorityYear = year;
        }

        /**
         * Sets selectedPriority property with the specified target and sets the criteriaEditorDialogData property
         * values with the selectedPriority.criteria property
         * @param priorityId Priority object id
         * @param criteria Priority object criteria
         */
        onEditCriteria(priorityId: any, criteria: string) {
            this.selectedPriorityIndex = this.priorities.findIndex((p: Priority) => p.id === priorityId);

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
                const priorities = clone(this.priorities);
                priorities[this.selectedPriorityIndex].criteria = criteria;
                this.selectedPriorityIndex = -1;
                this.priorities = priorities;

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