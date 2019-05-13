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
                    <v-layout justify-space-between fill-height>
                        <v-btn color="info" v-on:click="onSubmit(true)">Submit</v-btn>
                        <v-btn color="error" v-on:click="onSubmit(false)">Cancel</v-btn>
                    </v-layout>
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
         * Sets the editBudgetsDialogGridData array using the dialogData object's budgets data property
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
         * Disables the 'Move Up' button if there are no selected grid rows, there is more than 1 selected grid row, or
         * if the selected grid row is the first grid row in the grid data list
         */
        disableMoveUpButton() {
            return !hasValue(this.selectedGridRows) ||
                   this.selectedGridRows.length > 1 ||
                   this.selectedGridRows[0].index === 0;
        }

        /**
         * Moves a selected budget to the previous index in the grid data list
         */
        onMoveBudgetUp() {
            const gridData: EditBudgetsDialogGridData[] = [...clone(this.editBudgetsDialogGridData)];

            const currentBudgetIndex = this.selectedGridRows[0].index;

            const previousBudgetIndex = currentBudgetIndex - 1;

            const currentBudget: EditBudgetsDialogGridData = {
                ...gridData[currentBudgetIndex],
                index: previousBudgetIndex
            };

            const nextBudget: EditBudgetsDialogGridData = {
                ...gridData[previousBudgetIndex],
                index: currentBudgetIndex
            };

            gridData[previousBudgetIndex] = currentBudget;

            gridData[currentBudgetIndex] = nextBudget;

            this.editBudgetsDialogGridData = gridData;

            this.selectedGridRows = [this.editBudgetsDialogGridData[previousBudgetIndex]];
        }

        /**
         * Disables the 'Move Down' button if there are no selected grid rows, there is more than 1 selected grid row,
         * or the selected grid row is the last row in the grid data list
         */
        disableMoveDownButton() {
            return !hasValue(this.selectedGridRows) ||
                   this.selectedGridRows.length > 1 ||
                   this.selectedGridRows[0].index === this.editBudgetsDialogGridData.length - 1;
        }

        /**
         * Moves a selected budget to the next index in the grid data list
         */
        onMoveBudgetDown() {
            const gridData: EditBudgetsDialogGridData[] = [...clone(this.editBudgetsDialogGridData)];

            const currentBudgetIndex = this.selectedGridRows[0].index;

            const nextBudgetIndex = currentBudgetIndex + 1;

            const currentBudget: EditBudgetsDialogGridData = {
                ...gridData[currentBudgetIndex],
                index: nextBudgetIndex
            };

            const nextBudget: EditBudgetsDialogGridData = {
                ...gridData[nextBudgetIndex],
                index: currentBudgetIndex
            };

            gridData[nextBudgetIndex] = currentBudget;

            gridData[currentBudgetIndex] = nextBudget;

            this.editBudgetsDialogGridData = gridData;

            this.selectedGridRows = [this.editBudgetsDialogGridData[nextBudgetIndex]];
        }

        /**
         * Adds a new budget to the grid data list
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

        /**
         * Modifies the budget name at the specified index in the grid data list
         */
        onEditBudgetName(newName: string, index: number) {
            this.editBudgetsDialogGridData[index] = {
                ...this.editBudgetsDialogGridData[index],
                name: newName
            };
        }

        /**
         * Disables the 'Delete' button if there are no selected grid rows
         */
        disableDeleteButton() {
            return !hasValue(this.selectedGridRows);
        }

        /**
         * Removes budgets that have been marked for deletion from the grid data list and resets the selected grid rows
         */
        onDeleteBudget() {
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

            this.selectedGridRows = [];
        }

        /**
         * Emits the modified budgets data or a null value to the parent component then resets the component's data table
         * UI properties
         */
        onSubmit(submit: boolean) {
            if (submit) {
                const editedBudgets: EditedBudget[] = this.editBudgetsDialogGridData
                    .map((budget: EditBudgetsDialogGridData) => ({
                        name: budget.name,
                        previousName: budget.previousName,
                        isNew: budget.isNew
                    }));

                this.$emit('submit', editedBudgets);
            } else {
                this.$emit('submit', null);
            }

            this.resetDialogDataTableProperties();
        }

        /**
         * Resets the component's data table UI properties
         */
        resetDialogDataTableProperties() {
            this.editBudgetsDialogGridData = [];
            this.selectedGridRows = [];
        }
    }
</script>