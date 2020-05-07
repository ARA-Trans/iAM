<template>
    <v-layout>
        <v-dialog max-width="800px" persistent scrollable v-model="dialogData.showDialog">
            <v-card>
                <v-card-title>
                    <v-layout justify-center>
                        <h3>Edit Budgets</h3>
                    </v-layout>
                </v-card-title>
                <v-toolbar>
                    <v-layout justify-center row>
                        <v-btn @click="onAddBudget" class="ara-blue" icon>
                            <v-icon>fas fa-plus</v-icon>
                        </v-btn>
                        <v-btn :disabled="disableDeleteButton()" @click="onDeleteBudget" class="ara-orange" icon>
                            <v-icon>fas fa-trash</v-icon>
                        </v-btn>
                    </v-layout>
                </v-toolbar>
                <v-card-text style="height: 500px;">
                    <v-data-table :headers="editBudgetsDialogGridHeaders"
                                  :items="editBudgetsDialogGridData"
                                  class="elevation-1"
                                  hide-actions
                                  item-key="id"
                                  select-all
                                  v-model="selectedGridRows">
                        <template slot="items" slot-scope="props">
                            <td>
                                <v-checkbox hide-details primary v-model="props.selected"></v-checkbox>
                            </td>
                            <td>
                                <v-edit-dialog :return-value.sync="props.item.budgetName" persistent
                                               @save="onEditBudgetName(props.item)" large lazy>
                                    <v-text-field readonly single-line class="sm-txt" :value="props.item.budgetName"
                                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].budgetNameIsUnique(props.item, editBudgetsDialogGridData)]"/>
                                    <template slot="input">
                                        <v-text-field label="Edit" single-line v-model="props.item.budgetName"
                                                      :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].budgetNameIsUnique(props.item, editBudgetsDialogGridData)]"/>
                                    </template>
                                </v-edit-dialog>
                            </td>
                            <td>
                                <v-text-field readonly single-line class="sm-txt"
                                              :value="props.item.criteria">
                                    <template slot="append-outer">
                                        <v-icon @click="onEditCriteria(props.item)"
                                                class="edit-icon">
                                            fas fa-edit
                                        </v-icon>
                                    </template>
                                </v-text-field>
                            </td>
                        </template>
                    </v-data-table>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between>
                        <v-btn @click="onSubmit(true)" class="ara-blue-bg white--text" :disabled="disableSubmit()">
                            Save
                        </v-btn>
                        <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData"
                              @submitCriteriaEditorDialogResult="onSubmitCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {Action} from 'vuex-class';
    import {any, clone, isNil, update, findIndex, propEq} from 'ramda';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import {
        EditBudgetsDialogData
    } from '@/shared/models/modals/edit-budgets-dialog';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {CriteriaDrivenBudget, emptyCriteriaDrivenBudget} from '@/shared/models/iAM/investment';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';
    import ObjectID from 'bson-objectid';

    @Component({
        components: {
            CriteriaEditorDialog
        }
    })
    export default class EditBudgetsDialog extends Vue {
        @Prop() dialogData: EditBudgetsDialogData;

        @Action('setErrorMessage') setErrorMessageAction: any;

        editBudgetsDialogGridHeaders: DataTableHeader[] = [
            {text: 'Budget', value: 'budgetName', sortable: false, align: 'center', class: '', width: ''},
            {text: 'Criteria', value: 'criteria', sortable: false, align: 'center', class: '', width: ''}
        ];
        editBudgetsDialogGridData: CriteriaDrivenBudget[] = [];
        selectedGridRows: CriteriaDrivenBudget[] = [];
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        selectedCriteriaDrivenBudget: CriteriaDrivenBudget = clone(emptyCriteriaDrivenBudget);
        rules: InputValidationRules = {...rules};

        /**
         * Sets the editBudgetsDialogGridData array using the dialogData object's budgets data property
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.editBudgetsDialogGridData = clone(this.dialogData.criteriaDrivenBudgets);
        }


        /**
         * Adds a new budget to the grid data list
         */
        onAddBudget() {
            const unnamedBudgets = this.editBudgetsDialogGridData
                .filter((budget: CriteriaDrivenBudget) => budget.budgetName.match(/Unnamed Budget/));

            this.editBudgetsDialogGridData.push({
                id: ObjectID.generate(),
                budgetName: `Unnamed Budget ${unnamedBudgets.length + 1}`,
                criteria: '',
                scenarioId: this.dialogData.scenarioId
            });
        }

        /**
         * Modifies the budget name at the specified index in the grid data list
         */
        onEditBudgetName(budget: CriteriaDrivenBudget) {
            this.editBudgetsDialogGridData = update(
                findIndex(propEq('id', budget.id), this.editBudgetsDialogGridData),
                {...budget},
                this.editBudgetsDialogGridData
            );
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
                .filter((budget: CriteriaDrivenBudget) => !any(propEq('id', budget.id), this.selectedGridRows));

            this.selectedGridRows = [];
        }

        /**
         * Emits the modified budgets data or a null value to the parent component then resets the component's data table
         * UI properties
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.editBudgetsDialogGridData);
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

        onEditCriteria(criteriaDrivenBudget: CriteriaDrivenBudget) {
            this.selectedCriteriaDrivenBudget = clone(criteriaDrivenBudget);

            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.selectedCriteriaDrivenBudget.criteria
            };
        }

        onSubmitCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.editBudgetsDialogGridData = update(
                    findIndex(propEq('id', this.selectedCriteriaDrivenBudget.id), this.editBudgetsDialogGridData),
                    {...this.selectedCriteriaDrivenBudget, criteria: criteria},
                    this.editBudgetsDialogGridData
                );

                this.selectedCriteriaDrivenBudget = clone(emptyCriteriaDrivenBudget);
            }
        }

        disableSubmit() {
            const allDataIsValid: boolean = this.editBudgetsDialogGridData.every((budget: CriteriaDrivenBudget) => {
                return this.rules['generalRules'].valueIsNotEmpty(budget.budgetName) === true &&
                    this.rules['investmentRules'].budgetNameIsUnique(budget, this.editBudgetsDialogGridData) === true;
            });

            return !allDataIsValid;
        }
    }
</script>
