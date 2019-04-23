<template>
    <v-layout>
        <v-dialog v-model="dialogData.showDialog" persistent scrollable max-width="300px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>Edit Budgets</h3>
                    </v-layout>
                </v-card-title>
                <v-toolbar>
                    <v-layout justify-center row fill-height>
                        <v-btn fab small icon v-on:click="onMoveBudgetUp" :disabled="disableMoveUpButton()">
                            <v-icon>arrow_upward</v-icon>
                        </v-btn>
                        <v-btn fab small icon v-on:click="onMoveBudgetDown" :disabled="disableMoveDownButton()">
                            <v-icon>arrow_downward</v-icon>
                        </v-btn>
                        <v-btn fab small icon color="green" v-on:click="onAddBudget">
                            <v-icon>add</v-icon>
                        </v-btn>
                        <v-btn fab small icon color="red" v-on:click="onDeleteBudget" :disabled="disableDeleteButton()">
                            <v-icon>delete</v-icon>
                        </v-btn>
                    </v-layout>
                </v-toolbar>
                <v-card-text style="height: 500px;">
                    <v-data-table :headers="editBudgetsDialogGridHeaders"
                                  :items="editBudgetsDialogGridData"
                                  v-model="selectedGridRows"
                                  select-all
                                  item-key="index"
                                  class="elevation-1"
                                  hide-actions>
                        <template slot="items" slot-scope="props">
                            <td>
                                <v-checkbox v-model="props.selected" primary hide-details></v-checkbox>
                            </td>
                            <td>
                                <v-edit-dialog :return-value.sync="props.item.name" large lazy persistent
                                               @save="onEditBudgetName(props.item.index)">
                                    {{props.item.name}}
                                    <template slot="input">
                                        <v-text-field v-model="props.item.name" label="Edit" single-line>
                                        </v-text-field>
                                    </template>
                                </v-edit-dialog>
                            </td>
                        </template>
                    </v-data-table>
                </v-card-text>
                <v-card-actions>
                    <v-btn v-on:click="onCancel">Cancel</v-btn>
                    <v-btn v-on:click="onSubmit" color="info">Submit</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {hasValue} from '@/shared/utils/has-value';
    import {any, propEq, clone} from 'ramda';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {EditBudgetsDialogGridData, EditedBudget} from '@/shared/models/iAM/investment';
    import {EditBudgetsDialogData} from '@/shared/models/dialogs/investment-editor-dialogs/edit-budgets-dialog-data';

    @Component
    export default class EditBudgetsDialog extends Vue {
        @Prop() dialogData: EditBudgetsDialogData;


        editBudgetsDialogGridHeaders: DataTableHeader[] = [
            {text: 'Budget', value: 'name', sortable: false, align: 'center', class: '', width: ''}
        ];
        editBudgetsDialogGridData: EditBudgetsDialogGridData[] = [];
        selectedGridRows: EditBudgetsDialogGridData[] = [];

        /**
         * Watcher: dialogData
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.editBudgetsDialogGridData = this.dialogData.budgets.map((budget: string, index: number) => ({
                name: budget,
                index: index,
                previousName: budget,
                isNew: false
            }));
        }

        /**
         * Whether or not the 'Move Up' button should be disabled
         */
        disableMoveUpButton() {
            return !hasValue(this.selectedGridRows) ||
                   this.selectedGridRows.length > 1 ||
                   this.selectedGridRows[0].index === 0;
        }

        /**
         * 'Move Up' button has been clicked
         */
        onMoveBudgetUp() {
            // create a copy of the budgets grid data
            const budgets: EditBudgetsDialogGridData[] = [...this.editBudgetsDialogGridData];
            // get the current budget index
            const currentBudgetIndex = this.selectedGridRows[0].index;
            // set the previous budget index
            const previousBudgetIndex = currentBudgetIndex - 1;
            // get the current budget
            const currentBudget: EditBudgetsDialogGridData = budgets[currentBudgetIndex];
            // get the previous budget
            const previousBudget: EditBudgetsDialogGridData = budgets[previousBudgetIndex];
            // update selected budget index as previous index
            currentBudget.index = previousBudgetIndex;
            // update previous budget index as current index
            previousBudget.index = currentBudgetIndex;
            // move selected budget to previous index in budgets list
            budgets[previousBudgetIndex] = currentBudget;
            // moved previous budget to current index in budgets list
            budgets[currentBudgetIndex] = previousBudget;
            // update editBudgetsDialogGridData with copy
            this.editBudgetsDialogGridData = budgets;
        }

        /**
         * Whether or not the 'Move Down' button should be disabled
         */
        disableMoveDownButton() {
            return !hasValue(this.selectedGridRows) ||
                   this.selectedGridRows.length > 1 ||
                   this.selectedGridRows[0].index === this.editBudgetsDialogGridData.length - 1;
        }

        /**
         * 'Move Down' button has been clicked
         */
        onMoveBudgetDown() {
            // create a copy of the budgets grid data
            const budgets: EditBudgetsDialogGridData[] = [...this.editBudgetsDialogGridData];
            // get the current budget index
            const currentBudgetIndex = this.selectedGridRows[0].index;
            // set the next budget index
            const nextBudgetIndex = currentBudgetIndex + 1;
            // get the current budget
            const currentBudget: EditBudgetsDialogGridData = budgets[currentBudgetIndex];
            // get the next budget
            const nextBudget: EditBudgetsDialogGridData = budgets[nextBudgetIndex];
            // update selected budget index as previous index
            currentBudget.index = nextBudgetIndex;
            // update next budget index as current index
            nextBudget.index = currentBudgetIndex;
            // move selected budget to next index in budgets list
            budgets[nextBudgetIndex] = currentBudget;
            // move next budget to current index in budgets list
            budgets[currentBudgetIndex] = nextBudget;
            // update editBudgetsDialogGridData with copy
            this.editBudgetsDialogGridData = budgets;
        }

        /**
         * 'Add' button has been clicked
         */
        onAddBudget() {
            let newBudget = 'Unnamed Budget';
            const unnamedBudgets = this.editBudgetsDialogGridData
                .filter((budget: EditBudgetsDialogGridData) => budget.name.match(/Unnamed Budget/));
            newBudget = `${newBudget} ${unnamedBudgets.length + 1}`;
            this.editBudgetsDialogGridData.push({
                name: newBudget,
                index: this.editBudgetsDialogGridData.length,
                previousName: '',
                isNew: true
            });
        }

        onEditBudgetName(newName: string, index: number) {
            // create a copy of the grid data
            const budgets = [...this.editBudgetsDialogGridData];
            // updated the name of the budget in the copy at the specified index
            budgets[index] = {
                ...this.editBudgetsDialogGridData[index],
                name: newName
            };
            // update the grid data with its copy
            this.editBudgetsDialogGridData = budgets;
        }

        /**
         * Whether or not the 'Delete' button should be disabled
         */
        disableDeleteButton() {
            return !hasValue(this.selectedGridRows);
        }

        /**
         * 'Delete' button has been clicked
         */
        onDeleteBudget() {
            // filter selected budget from budgets list
            this.editBudgetsDialogGridData = this.editBudgetsDialogGridData
                .filter((budget: EditBudgetsDialogGridData) =>
                    !any(propEq('name', budget.name), this.selectedGridRows)
                )
                .map((budget: EditBudgetsDialogGridData, index: number) => ({
                    name: budget.name,
                    index: index,
                    previousName: budget.previousName,
                    isNew: budget.isNew
                }));
            // reset selectedGridRows
            this.selectedGridRows = [];
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            // create a list of EditedBudget objects using editBudgetsDialogGridData
            const editedBudgets: EditedBudget[] = this.editBudgetsDialogGridData
                .map((budget: EditBudgetsDialogGridData) => ({
                    name: budget.name,
                    previousName: budget.previousName,
                    isNew: budget.isNew
                }));
            // submit editedBudgets result
            this.$emit('submit', editedBudgets);
            // reset the data table properties
            this.resetDialogDataTableProperties();
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {
            // submit null result
            this.$emit('submit', null);
            // reset the data table properties
            this.resetDialogDataTableProperties();
        }

        /**
         * Resets the data table properties of this component
         */
        resetDialogDataTableProperties() {
            this.editBudgetsDialogGridData = [];
            this.selectedGridRows = [];
        }
    }
</script>