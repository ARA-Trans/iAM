<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center fill-height v-if="feasibility.id === 0">
                    <v-btn color="info" v-on:click="onCreateFeasibility">Create Feasibility</v-btn>
                </v-layout>
                <v-layout column fill-height v-if="feasibility.id !== 0">
                    <v-textarea no-resize full-width outline readonly append-out-icon="edit" @click:append-outer="onEditFeasibilityCriteria"
                                v-model="feasibility.criteria">
                    </v-textarea>
                    <v-flex xs3>
                        <v-layout justify-start column fill-height>
                            <v-text-field label="Years Before Any" :mask="'####'" v-model="feasibility.yearsBeforeAny">
                            </v-text-field>
                            <v-text-field label="Years Before Same" :mask="'####'" v-model="feasibility.yearsBeforeSame">
                            </v-text-field>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>

        <CriteriaEditor :dialogData="criteriaEditorDialogData" @submit="onSubmitFeasibilityCriteria" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {
        emptyFeasibility,
        emptyTreatment,
        Feasibility,
        Treatment,
        TreatmentStrategy
    } from '@/shared/models/iAM/treatment';
    import CriteriaEditor from '../../../shared/dialogs/CriteriaEditor.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/dialogs/criteria-editor-dialog/criteria-editor-dialog-data';
    import {getLatestPropertyValue, getPropertyValues} from '@/shared/utils/getter-utils';
    import {hasValue} from '@/shared/utils/has-value';
    import {findIndex, propEq, isNil} from 'ramda';

    @Component({
        components: {CriteriaEditor}
    })
    export default class FeasibilityTab extends Vue {
        @Prop() feasibilityTreatmentStrategies: TreatmentStrategy[];
        @Prop() selectedFeasibilityTreatmentStrategy: TreatmentStrategy;
        @Prop() selectedFeasibilityTreatment: Treatment;

        @Action('updateSelectedTreatmentStrategy') updateSelectedTreatmentStrategyAction: any;

        feasibility: Feasibility = {...emptyFeasibility};
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};

        /**
         * Watcher: selectedTreatment
         */
        @Watch('selectedTreatment')
        onSelectedTreatmentChanged() {
            if (this.selectedTreatment.id !== 0) {
                if (!isNil(this.selectedTreatment.feasibility) && this.selectedTreatment.feasibility.id !== 0) {
                    // set feasibility = selectedTreatment.feasibility
                    this.feasibility = {...this.selectedTreatment.feasibility} as Feasibility;
                }
            }
        }

        /**
         * 'Create Feasibility' button has been clicked
         */
        onCreateFeasibility() {
            // create a new, empty feasibility object
            const createdFeasibility: Feasibility = {...emptyFeasibility, treatmentId: this.selectedTreatment.id};
            // get all feasibilities from treatmentStrategies' treatments
            const allFeasibilities: Feasibility[] = [];
            this.treatmentStrategies.forEach((treatmentStrategy: TreatmentStrategy) => {
                allFeasibilities.push(...getPropertyValues('feasibility', treatmentStrategy.treatments));
            });
            // get the latest feasibility id from allFeasibilities
            const latestId: number = getLatestPropertyValue(
                'id', allFeasibilities.filter((feasibility: Feasibility) => !isNil(feasibility))
            );
            // set createdFeasibility.id = latestId + 1 (if present), otherwise set as 1
            createdFeasibility.id = hasValue(latestId) ? latestId + 1 : 1;
            // create a copy of selectedTreatment.treatments
            const updatedTreatments: Treatment[] = [...this.selectedTreatmentStrategy.treatments];
            // find the index of the selected treatment in updatedTreatments
            const index: number = findIndex(propEq('id', this.selectedTreatment.id), updatedTreatments);
            // update the treatment at the given index in updatedTreatments
            updatedTreatments[index] = {...this.selectedTreatment, feasibility: createdFeasibility};
            // dispatch action to update the selected treatment strategy's treatments
            this.updateSelectedTreatmentStrategyAction({
                updatedSelectedTreatmentStrategy: {
                    ...this.selectedTreatmentStrategy,
                    treatments: updatedTreatments
                }
            });
        }

        /**
         * 'Edit Criteria' button has been clicked
         */
        onEditFeasibilityCriteria() {
            // show the CriteriaEditor, passing in the feasibility's criteria
            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.feasibility.criteria
            };
        }

        /**
         * User has submitted a CriteriaEditor result
         * @param criteria The criteria submitted by the user
         */
        onSubmitFeasibilityCriteria(criteria: string) {
            // hide the CriteriaEditor
            this.criteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
            if (!isNil(criteria)) {
                // create a copy of selectedTreatment.treatments
                const updatedTreatments: Treatment[] = [...this.selectedTreatmentStrategy.treatments];
                // find the index of the selected treatment in updatedTreatments
                const index: number = findIndex(propEq('id', this.selectedTreatment.id), updatedTreatments);
                // update the treatment's feasibility's criteria at the given index in updatedTreatments
                updatedTreatments[index] = {...this.selectedTreatment, feasibility: {...this.feasibility, criteria: criteria}};
                // dispatch action to update the selected treatment strategy's treatments
                this.updateSelectedTreatmentStrategyAction({
                    updatedSelectedTreatmentStrategy: {
                        ...this.selectedTreatmentStrategy,
                        treatments: updatedTreatments
                    }
                });
            }
        }
    }
</script>