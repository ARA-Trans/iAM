<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent scrollable max-width="300px">
            <v-card>
                <v-toolbar>
                    <v-toolbar-title>Budgets</v-toolbar-title>
                    <v-spacer></v-spacer>
                    <v-btn fab icon v-on:click="onMoveBudgetUp" :disabled="disableMoveUpButton()">
                        <v-icon>arrow_upward</v-icon>
                    </v-btn>
                    <v-btn fab icon v-on:click="onMoveBudgetDown" :disabled="disableMoveDownButton()">
                        <v-icon>arrow_downward</v-icon>
                    </v-btn>
                    <v-btn fab icon color="green">
                        <v-icon>add</v-icon>
                    </v-btn>
                    <v-btn fab icon color="red" v-on:click="onDeleteBudget()" :disabled="disableDeleteButton()">
                        <v-icon>delete</v-icon>
                    </v-btn>
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
                    <v-btn v-on:click="onSubmit(false)" color="info">Save</v-btn>
                    <v-btn v-on:click="onSubmit(true)">Cancel</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State} from 'vuex-class';
    import {hasValue} from '@/shared/utils/has-value';
    import {EditBudgetsDialogResult} from '@/shared/models/dialogs/edit-budgets-dialog-result';
    import * as R from 'ramda';
    import {VueDataTableHeader} from '@/shared/models/vue/vue-data-table-header';
    import {EditBudgetsDialogGridData} from '@/shared/models/iAM/investment';

    interface BudgetListItem {
        name: string;
        index: number;
    }

    @Component
    export default class EditBudgetsDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.investmentEditor.budgets) stateBudgets: string[];

        editBudgetsDialogGridHeaders: VueDataTableHeader[] = [
            {text: 'Budget', value: 'name', sortable: false, align: 'left', class: '', width: ''}
        ];
        editBudgetsDialogGridData: EditBudgetsDialogGridData[] = [];
        selectedGridRows: EditBudgetsDialogGridData[] = [];

        @Watch('stateBudgets')
        onStateBudgetsChanged(budgets: string[]) {
            this.editBudgetsDialogGridData = budgets.map((budget: string, index: number) => ({
                name: budget,
                index: index
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
            this.editBudgetsDialogGridData = R.clone(this.editBudgetsDialogGridData);
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
            this.editBudgetsDialogGridData = R.clone(this.editBudgetsDialogGridData);
        }

        /**
         * 'Add' button has been clicked
         */
        onAddBudget() {
            let newBudget = 'Unnamed Budget';
            const unnamedBudgets = this.editBudgetsDialogGridData
                .filter((budget: EditBudgetsDialogGridData) => budget.name.match(/Unnamed Budget/));
            newBudget = `${newBudget} ${unnamedBudgets.length + 1}`;

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
                    !R.any(R.propEq('name', budget.name), this.selectedGridRows)
                )
                .map((budget: EditBudgetsDialogGridData, index: number) => ({
                    name: budget.name,
                    index: index
                }));
            // reset selectedBudget as defaultBudgetListItem
            this.selectedGridRows = [];
        }

        /**
         * 'Save'/'Cancel' button has been clicked
         */
        onSubmit(isCanceled: boolean) {
            const result: EditBudgetsDialogResult = {
                canceled: isCanceled,
                budgets: this.editBudgetsDialogGridData
                    .map((budget: EditBudgetsDialogGridData) => budget.name)
            };
            this.$emit('result', result);
        }
    }
</script>