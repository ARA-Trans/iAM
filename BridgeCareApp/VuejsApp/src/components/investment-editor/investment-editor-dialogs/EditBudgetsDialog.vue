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
                                <v-edit-dialog :return-value.sync="props.item.name" large lazy persistent>
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
            // get current index of selected budget
            const currentIndex = this.selectedGridRows[0].index;
            // get previous index
            const previousIndex = currentIndex - 1;
            // get previous index budget
            const previousIndexBudget = this.editBudgetsDialogGridData[previousIndex];
            // update selected budget index as previous index
            this.selectedGridRows[0].index = previousIndex;
            // update previous index budget index as current index
            previousIndexBudget.index = currentIndex;
            // move selected budget to previous index in budgets list
            this.editBudgetsDialogGridData[previousIndex] = this.selectedGridRows[0];
            // moved previous index budget to current index in budgets list
            this.editBudgetsDialogGridData[currentIndex] = previousIndexBudget;
            this.editBudgetsDialogGridData = clone(this.editBudgetsDialogGridData);
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
            // get current index of selected budget
            const currentIndex = this.selectedGridRows[0].index;
            // get next index
            const nextIndex = currentIndex + 1;
            // get next index budget
            const nextIndexBudget = this.editBudgetsDialogGridData[nextIndex];
            // update selected budget index as next index
            this.selectedGridRows[0].index = nextIndex;
            // update next index budget index as current index
            nextIndexBudget.index = currentIndex;
            // move selected budget to next index in budgets list
            this.editBudgetsDialogGridData[nextIndex] = this.selectedGridRows[0];
            // move next index budget to current index in budgets list
            this.editBudgetsDialogGridData[currentIndex] = nextIndexBudget;
            this.editBudgetsDialogGridData = clone(this.editBudgetsDialogGridData);
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
                previousName: newBudget,
                isNew: true
            });
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
            // reset selectedBudget as defaultBudgetListItem
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
            this.resetsDialogDataTableProperties();
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {
            // submit null result
            this.$emit('submit', null);
            // reset the data table properties
            this.resetsDialogDataTableProperties();
        }

        /**
         * Resets the data table properties of this component
         */
        resetsDialogDataTableProperties() {
            this.editBudgetsDialogGridData = [];
            this.selectedGridRows = [];
        }
    }
</script>