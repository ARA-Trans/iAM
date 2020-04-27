<template>
    <v-layout class="costs-tab-content">
        <v-flex xs12>
            <v-btn @click="onAddCost" class="ara-blue-bg white--text">Add Cost</v-btn>
            <div class="costs-data-table">
                <v-data-table :headers="costsGridHeaders" :items="costsGridData" class="elevation-1 fixed-header v-table__overflow"
                              hide-actions>
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-textarea full-width no-resize outline readonly rows="3" v-model="props.item.equation">
                                <template slot="append-outer">
                                    <v-btn @click="onEditCostEquation(props.item)" class="edit-icon" icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-textarea full-width no-resize outline readonly rows="3" v-model="props.item.criteria">
                                <template slot="append-outer">
                                    <v-btn @click="onEditCostCriteria(props.item)" class="edit-icon" icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-layout align-start>
                                <v-btn @click="onDeleteCost(props.item)" class="ara-orange" icon>
                                    <v-icon>fas fa-trash</v-icon>
                                </v-btn>
                            </v-layout>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>

        <EquationEditorDialog :dialogData="equationEditorDialogData" @submit="onSubmitEditedCostEquation"/>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData"
                              @submitCriteriaEditorDialogResult="onSubmitEditedCostCriteria"/>
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
    import {EquationEditorDialogResult} from '@/shared/models/modals/equation-editor-dialog-result';
    import {append, clone, findIndex, isNil, propEq, update} from 'ramda';
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

            this.costsTabSelectedTreatmentLibrary = {
                ...this.costsTabSelectedTreatmentLibrary,
                treatments: update(
                    findIndex(propEq('id', this.costsTabSelectedTreatment.id), this.costsTabSelectedTreatmentLibrary.treatments),
                    {...this.costsTabSelectedTreatment, costs: append(newCost, this.costsTabSelectedTreatment.costs)},
                    this.costsTabSelectedTreatmentLibrary.treatments
                )
            };

            this.$emit('submit', this.costsTabSelectedTreatmentLibrary);
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
         * Modifies the selected cost in the costsTabSelectedTreatment's costs list with the equation editor result
         */
        onSubmitEditedCostEquation(result: EquationEditorDialogResult) {
            this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

            if (!isNil(result)) {
                this.costsTabSelectedTreatmentLibrary = {
                    ...this.costsTabSelectedTreatmentLibrary,
                    treatments: update(
                        findIndex(propEq('id', this.costsTabSelectedTreatment.id), this.costsTabSelectedTreatmentLibrary.treatments),
                        {
                            ...this.costsTabSelectedTreatment,
                            costs: update(
                                findIndex(propEq('id', this.selectedCost.id), this.costsTabSelectedTreatment.costs),
                                {...this.selectedCost, equation: result.equation, isFunction: result.isFunction},
                                this.costsTabSelectedTreatment.costs
                            )
                        },
                        this.costsTabSelectedTreatmentLibrary.treatments
                    )
                };

                this.$emit('submit', this.costsTabSelectedTreatmentLibrary);
            }

            this.selectedCost = clone(emptyCost);
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
         * Modifies the selected cost in the costsTabSelectedTreatment's costs list with the specified criteria
         */
        onSubmitEditedCostCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.costsTabSelectedTreatmentLibrary = {
                    ...this.costsTabSelectedTreatmentLibrary,
                    treatments: update(
                        findIndex(propEq('id', this.costsTabSelectedTreatment.id), this.costsTabSelectedTreatmentLibrary.treatments),
                        {
                            ...this.costsTabSelectedTreatment,
                            costs: update(
                                findIndex(propEq('id', this.selectedCost.id), this.costsTabSelectedTreatment.costs),
                                {...this.selectedCost, criteria: criteria},
                                this.costsTabSelectedTreatment.costs
                            )
                        },
                        this.costsTabSelectedTreatmentLibrary.treatments
                    )
                };

                this.$emit('submit', this.costsTabSelectedTreatmentLibrary);
            }

            this.selectedCost = clone(emptyCost);
        }

        /**
         * Removes a cost with the specified costId from the costsTabSelectedTreatment's costs list
         */
        onDeleteCost(costId: string) {
            this.costsTabSelectedTreatmentLibrary = {
                ...this.costsTabSelectedTreatmentLibrary,
                treatments: update(
                    findIndex(propEq('id', this.costsTabSelectedTreatment.id), this.costsTabSelectedTreatmentLibrary.treatments),
                    {
                        ...this.costsTabSelectedTreatment,
                        costs: this.costsTabSelectedTreatment.costs.filter((cost: Cost) => cost.id !== costId)
                    },
                    this.costsTabSelectedTreatmentLibrary.treatments
                )
            };

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
