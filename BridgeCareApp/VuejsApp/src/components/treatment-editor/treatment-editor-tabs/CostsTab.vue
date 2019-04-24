<template>
    <v-container fluid grid-list-xl>
        <v-layout fill-height>
            <v-flex xs12>
                <v-btn color="info" v-on:click="onAddCost">Add Cost</v-btn>
                <div class="costs-data-table">
                    <v-data-table :headers="costsGridHeaders" :items="costsGridData" hide-actions
                                  class="elevation-1 fixed-header v-table__overflow">
                        <template slot="items" slot-scope="props">
                            <td>
                                <v-textarea rows="3" readonly no-resize full-width outline append-outer-icon="edit"
                                            @click:append-outer="onEditCostEquation(props.item)"
                                            v-model="props.item.equation">
                                </v-textarea>
                            </td>
                            <td>
                                <v-textarea rows="3" readonly no-resize full-width outline append-outer-icon="edit"
                                            @click:append-outer="onEditCostCriteria(props.item)"
                                            v-model="props.item.criteria">
                                </v-textarea>
                            </td>
                            <td>
                                <v-layout align-start fill-height>
                                    <v-btn color="error" icon v-on:click="onDeleteCost(props.item)">
                                        <v-icon>delete</v-icon>
                                    </v-btn>
                                </v-layout>
                            </td>
                        </template>
                    </v-data-table>
                </div>
            </v-flex>
        </v-layout>

        <EquationEditor :dialogData="equationEditorDialogData" @submit="onSubmitEditedCostEquation" />

        <CriteriaEditor :dialogData="criteriaEditorDialogData" @submit="onSubmitEditedCostCriteria"/>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {
        Cost,
        emptyCost,
        emptyTreatment,
        emptyTreatmentStrategy,
        Treatment,
        TreatmentStrategy
    } from '@/shared/models/iAM/treatment';
    import {
        emptyEquationEditorDialogData,
        EquationEditorDialogData
    } from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-data';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/dialogs/criteria-editor-dialog/criteria-editor-dialog-data';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {
        EquationEditorDialogResult
    } from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-result';
    import {isNil, findIndex, uniq} from 'ramda';
    import EquationEditor from '../../../shared/dialogs/EquationEditor.vue';
    import CriteriaEditor from '../../../shared/dialogs/CriteriaEditor.vue';
    import {getLatestPropertyValue} from '@/shared/utils/getter-utils';
    import {hasValue} from '@/shared/utils/has-value';
    import {TabData} from '@/shared/models/child-components/treatment-editor/tab-data';

    @Component({
        components: {CriteriaEditor, EquationEditor}
    })
    export default class CostsTab extends Vue {
        @Prop() costsTabData: TabData;

        costsTabTreatmentStrategies: TreatmentStrategy[] = [];
        costsTabSelectedTreatmentStrategy: TreatmentStrategy = {...emptyTreatmentStrategy};
        costsTabSelectedTreatment: Treatment = {...emptyTreatment};

        costsGridHeaders: DataTableHeader[] = [
            {text: 'Equation', value: 'equation', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '100px'}
        ];
        costsGridData: Cost[] = [];
        equationEditorDialogData: EquationEditorDialogData = {...emptyEquationEditorDialogData};
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
        selectedCost: Cost = {...emptyCost};
        allCosts: Cost[] = [];

        /**
         * Sets the CostsTab's required UI functionality properties
         */
        @Watch('costsTabData')
        onCostsTabDataChanged() {
            this.costsTabTreatmentStrategies = this.costsTabData.tabTreatmentStrategies;
            this.costsTabSelectedTreatmentStrategy = this.costsTabData.tabSelectedTreatmentStrategy;
            this.costsTabSelectedTreatment = this.costsTabData.tabSelectedTreatment;
            this.setAllCosts();
            this.setCostsGridData();
        }

        /**
         * Sets allCosts property based on costsTabTreatmentStrategies data
         */
        setAllCosts() {
            this.costsTabTreatmentStrategies.forEach((treatmentStrategy: TreatmentStrategy) => {
                if (treatmentStrategy.id === this.costsTabSelectedTreatmentStrategy.id) {
                    this.costsTabSelectedTreatmentStrategy.treatments.forEach((treatment: Treatment) => {
                        if (hasValue(treatment.costs)) {
                            this.allCosts.push(...treatment.costs);
                        }
                    });
                } else {
                    treatmentStrategy.treatments.forEach((treatment: Treatment) => {
                        if (hasValue(treatment.costs)) {
                            this.allCosts.push(...treatment.costs);
                        }
                    });
                }
            });
            this.allCosts = uniq(this.allCosts);
        }

        /**
         * Sets costsGridData property based on costsTabSelectedTreatment data
         */
        setCostsGridData() {
            if (this.costsTabSelectedTreatment.id !== 0 && this.costsTabSelectedTreatment.costs.length > 0) {
                    this.costsGridData = [...this.costsTabSelectedTreatment.costs];
            } else {
                this.costsGridData = [];
            }
        }

        /**
         * Creates a new Cost object to add to the selected treatment
         */
        onAddCost() {
            const latestId: number = hasValue(this.allCosts) ? getLatestPropertyValue('id', this.allCosts) : 0;
            const newCost: Cost = {
                ...emptyCost,
                treatmentId: this.costsTabSelectedTreatment.id,
                id: hasValue(latestId) ? latestId + 1 : 1
            };
            this.submitChanges(newCost, false);
        }

        /**
         * Sets selectedCost with the given cost parameter, then sets equationEditorDialogData with selectedCost
         * data
         * @param cost The cost to set as selectedCost
         */
        onEditCostEquation(cost: Cost) {
            this.selectedCost = {...cost};
            this.equationEditorDialogData = {
                ...emptyEquationEditorDialogData,
                showDialog: true,
                equation: this.selectedCost.equation,
                isFunction: this.selectedCost.isFunction
            };
        }

        /**
         * Updates the selectedCost.equation & selectedCost.isFunction based on the user submitted result from the
         * EquationEditor
         * @param result User's submitted EquationEditor result
         */
        onSubmitEditedCostEquation(result: EquationEditorDialogResult) {
            this.equationEditorDialogData = {...emptyEquationEditorDialogData};
            if (!isNil(result)) {
                const updatedCost: Cost = {...this.selectedCost};
                this.selectedCost = {...emptyCost};
                updatedCost.equation = result.equation;
                updatedCost.isFunction = result.isFunction;
                this.submitChanges(updatedCost, false);
            }
        }

        /**
         * Sets selectedCost with the given cost parameter, then sets criteriaEditorDialogData with selectedCost
         * data
         * @param cost The cost to set as selectedCost
         */
        onEditCostCriteria(cost: Cost) {
            this.selectedCost = {...cost};
            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.selectedCost.criteria
            };
        }

        /**
         * Updates the selectedCost.criteria based on the user submitted result from the CriteriaEditor
         * @param criteria User's submitted CriteriaEditor result
         */
        onSubmitEditedCostCriteria(criteria: string) {
            this.criteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
            if (!isNil(criteria)) {
                const updatedCost: Cost = {...this.selectedCost};
                this.selectedCost = {...emptyCost};
                updatedCost.criteria = criteria;
                this.submitChanges(updatedCost, false);
            }
        }

        /**
         * A Cost 'Delete' button has been clicked
         */
        onDeleteCost(cost: Cost) {
            this.submitChanges(cost, true);
        }

        /**
         * Submits cost data changes
         * @param costData The cost data to submit changes on
         * @param forDelete Whether or not the cost data is to be used for deleting a cost
         */
        submitChanges(costData: Cost, forDelete: boolean) {
            // update selected treatment data
            const updatedTreatment: Treatment = {...this.costsTabSelectedTreatment};
            if (forDelete) {
                updatedTreatment.costs = updatedTreatment.costs.filter((cost: Cost) => cost.id !== costData.id);
            } else {
                const updatedCostIndex: number = findIndex((cost: Cost) => cost.id === costData.id, updatedTreatment.costs);
                if (updatedCostIndex === -1) {
                    updatedTreatment.costs = [...updatedTreatment.costs, costData];
                } else {
                    updatedTreatment.costs[updatedCostIndex] = costData;
                }
            }
            // update selected treatment strategy data
            const updatedTreatmentStrategy: TreatmentStrategy = {...this.costsTabSelectedTreatmentStrategy};
            const updatedTreatmentIndex: number = findIndex(
                (treatment: Treatment) => treatment.id === updatedTreatment.id, updatedTreatmentStrategy.treatments
            );
            updatedTreatmentStrategy.treatments[updatedTreatmentIndex] = updatedTreatment;
            this.$emit('submit', updatedTreatmentStrategy);
        }
    }
</script>

<style>
    .costs-data-table {
        height: 245px;
        overflow-y: auto;
    }
</style>