<template>
    <v-layout class="costs-tab-content">
        <v-flex xs12>
            <v-btn class="ara-blue-bg white--text" @click="onAddCost">Add Cost</v-btn>
            <div class="costs-data-table">
                <v-data-table :headers="costsGridHeaders" :items="costsGridData" hide-actions
                              class="elevation-1 fixed-header v-table__overflow">
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-textarea rows="3" readonly no-resize full-width outline v-model="props.item.equation">
                                <template slot="append-outer">
                                    <v-btn class="ara-yellow" icon @click="onEditCostEquation(props.item)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-textarea rows="3" readonly no-resize full-width outline v-model="props.item.criteria">
                                <template slot="append-outer">
                                    <v-btn class="ara-yellow" icon @click="onEditCostCriteria(props.item)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-layout align-start>
                                <v-btn class="ara-orange" icon @click="onDeleteCost(props.item)">
                                    <v-icon>fas fa-trash</v-icon>
                                </v-btn>
                            </v-layout>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>

        <EquationEditorDialog :dialogData="equationEditorDialogData" @submit="onSubmitEditedCostEquation" />

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitEditedCostCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {
        Cost,
        emptyCost,
        emptyTreatment,
        emptyTreatmentLibrary,
        Treatment,
        TreatmentLibrary
    } from '@/shared/models/iAM/treatment';
    import {
        emptyEquationEditorDialogData,
        EquationEditorDialogData
    } from '@/shared/models/modals/equation-editor-dialog-data';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {
        EquationEditorDialogResult
    } from '@/shared/models/modals/equation-editor-dialog-result';
    import {isNil, findIndex, clone, append} from 'ramda';
    import EquationEditorDialog from '../../../shared/modals/EquationEditorDialog.vue';
    import CriteriaEditorDialog from '../../../shared/modals/CriteriaEditorDialog.vue';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {TabData} from '@/shared/models/child-components/tab-data';
    const ObjectID = require('bson-objectid');

    @Component({
        components: {CriteriaEditorDialog, EquationEditorDialog}
    })
    export default class CostsTab extends Vue {
        @Prop() costsTabData: TabData;

        costsTabTreatmentLibraries: TreatmentLibrary[] = [];
        costsTabSelectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
        costsTabSelectedTreatment: Treatment = clone(emptyTreatment);
        costsGridHeaders: DataTableHeader[] = [
            {text: 'Equation', value: 'equation', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '100px'}
        ];
        costsGridData: Cost[] = [];
        equationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        selectedCost: Cost = clone(emptyCost);

        /**
         * Sets the component's data properties
         */
        @Watch('costsTabData')
        onCostsTabDataChanged() {
            this.costsTabTreatmentLibraries = this.costsTabData.tabTreatmentLibraries;
            this.costsTabSelectedTreatmentLibrary = this.costsTabData.tabSelectedTreatmentLibrary;
            this.costsTabSelectedTreatment = this.costsTabData.tabSelectedTreatment;

            this.setCostsGridData();
        }

        /**
         * Sets the component's grid data
         */
        setCostsGridData() {
            this.costsGridData = hasValue(this.costsTabSelectedTreatment.costs)
                ? this.costsTabSelectedTreatment.costs
                : [];
        }

        /**
         * Creates a new Cost object to add to the selected treatment
         */
        onAddCost() {
            const newCost: Cost = {
                ...clone(emptyCost),
                id: ObjectID.generate()
            };

            this.submitChanges(newCost, false);
        }

        /**
         * Sets the selectedCost and shows the EquationEditorDialog passing in the selectedCost's equation & isFunction data
         * @param cost The cost to set as the selectedCost
         */
        onEditCostEquation(cost: Cost) {
            this.selectedCost = clone(cost);

            this.equationEditorDialogData = {
                ...clone(emptyEquationEditorDialogData),
                showDialog: true,
                equation: this.selectedCost.equation
            };
        }

        /**
         * Modifies the selectedCost's equation & isFunction data using the EquationEditorDialog result
         * @param result EquationEditorDialog result
         */
        onSubmitEditedCostEquation(result: EquationEditorDialogResult) {
            this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

            if (!isNil(result)) {
                const updatedCost: Cost = clone(this.selectedCost);
                this.selectedCost = clone(emptyCost);

                updatedCost.equation = result.equation;
                updatedCost.isFunction = result.isFunction;

                this.submitChanges(updatedCost, false);
            }
        }

        /**
         * Sets the selectedCost and shows the CriteriaEditorDialog passing in the selectedCost's criteria data
         * data
         * @param cost The cost to set as selectedCost
         */
        onEditCostCriteria(cost: Cost) {
            this.selectedCost = clone(cost);
            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.selectedCost.criteria
            };
        }

        /**
         * Modifies the selectedCost's criteria data using the CriteriaEditorDialog result
         * @param criteria CriteriaEditorDialog result
         */
        onSubmitEditedCostCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                const updatedCost: Cost = clone(this.selectedCost);
                this.selectedCost = clone(emptyCost);

                updatedCost.criteria = criteria;

                this.submitChanges(updatedCost, false);
            }
        }

        /**
         * Sends a Cost object that has been marked for deletion to the submitChanges function
         */
        onDeleteCost(cost: Cost) {
            this.submitChanges(cost, true);
        }

        /**
         * Modifies the selected treatment & selected treatment library with a Cost object's data changes and emits the
         * modified objects to the parent component
         * @param costData Cost object data
         * @param forDelete Whether or not the Cost object's data is marked for deletion
         */
        submitChanges(costData: Cost, forDelete: boolean) {
            if (forDelete) {
                this.costsTabSelectedTreatment.costs = this.costsTabSelectedTreatment.costs
                    .filter((cost: Cost) => cost.id !== costData.id);
            } else {
                const updatedCostIndex: number = findIndex((cost: Cost) =>
                    cost.id === costData.id, this.costsTabSelectedTreatment.costs
                );
                if (updatedCostIndex === -1) {
                    this.costsTabSelectedTreatment.costs = append(costData, this.costsTabSelectedTreatment.costs);
                } else {
                    this.costsTabSelectedTreatment.costs[updatedCostIndex] = costData;
                }
            }

            const updatedTreatmentIndex: number = findIndex((treatment: Treatment) =>
                treatment.id === this.costsTabSelectedTreatment.id,
                this.costsTabSelectedTreatmentLibrary.treatments
            );
            this.costsTabSelectedTreatmentLibrary.treatments[updatedTreatmentIndex] = this.costsTabSelectedTreatment;

            this.$emit('submit', this.costsTabSelectedTreatmentLibrary);
        }
    }
</script>

<style>
    .costs-tab-content {
        height: 185px;
    }

    .costs-data-table {
        height: 215px;
        overflow-y: auto;
    }
</style>