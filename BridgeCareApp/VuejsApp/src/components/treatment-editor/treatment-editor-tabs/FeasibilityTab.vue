<template>
    <v-layout class="feasibility-tab-content">
        <v-flex xs12>
            <v-layout justify-center v-if="feasibility.id === 0">
                <v-btn class="ara-blue-bg white--text" @click="onCreateFeasibility">Create Feasibility</v-btn>
            </v-layout>
            <v-layout v-if="feasibility.id !== 0" justify-center column>
                <v-textarea no-resize full-width outline readonly prepend-outer-icon="fas fa-trash"
                            v-model="feasibility.criteria">
                    <template slot="append-outer">
                        <v-layout row align-center fill-height>
                            <v-btn class="ara-orange" icon @click="onEditFeasibilityCriteria">
                                <v-icon>fas fa-edit</v-icon>
                            </v-btn>
                            <v-btn class="ara-orange" icon @click="onDeleteFeasibility">
                                <v-icon>fas fa-minus-square</v-icon>
                            </v-btn>
                        </v-layout>
                    </template>
                </v-textarea>
                <v-layout>
                    <v-spacer></v-spacer>
                    <v-layout justify-space-between row>
                        <v-flex xs5>
                            <v-text-field label="Years Before Any" :mask="'####'" outline
                                          v-model="feasibility.yearsBeforeAny"
                                          @change="onChangeYears">
                            </v-text-field>
                        </v-flex>
                        <v-flex xs5>
                            <v-text-field label="Years Before Same" :mask="'####'" outline
                                          v-model="feasibility.yearsBeforeSame"
                                          @change="onChangeYears">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                    <v-spacer></v-spacer>
                </v-layout>
            </v-layout>
        </v-flex>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitFeasibilityCriteria" />
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch, Prop} from 'vue-property-decorator';
    import {
        emptyFeasibility, emptyTreatment, emptyTreatmentLibrary,
        Feasibility,
        Treatment,
        TreatmentLibrary
    } from '@/shared/models/iAM/treatment';
    import CriteriaEditorDialog from '../../../shared/modals/CriteriaEditorDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {findIndex, isNil, clone} from 'ramda';
    import {TabData} from '@/shared/models/child-components/tab-data';
    const ObjectID = require('bson-objectid');

    @Component({
        components: {CriteriaEditorDialog}
    })
    export default class FeasibilityTab extends Vue {
        @Prop() feasibilityTabData: TabData;

        feasibilityTabTreatmentLibraries: TreatmentLibrary[] = [];
        feasibilityTabSelectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
        feasibilityTabSelectedTreatment: Treatment = clone(emptyTreatment);
        feasibility: Feasibility = clone(emptyFeasibility);
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

        /**
         * Sets the component's data properties
         */
        @Watch('feasibilityTabData')
        onFeasibilityTabDataChanged() {
            this.feasibilityTabTreatmentLibraries = this.feasibilityTabData.tabTreatmentLibraries;
            this.feasibilityTabSelectedTreatmentLibrary = this.feasibilityTabData.tabSelectedTreatmentLibrary;
            this.feasibilityTabSelectedTreatment = this.feasibilityTabData.tabSelectedTreatment;

            this.setFeasibility();
        }

        /**
         * Sets the component's grid data
         */
        setFeasibility() {
            if (this.feasibilityTabSelectedTreatment.id !== '0' &&
                !isNil(this.feasibilityTabSelectedTreatment.feasibility) &&
                this.feasibilityTabSelectedTreatment.feasibility.id !== '0') {
                    this.feasibility = this.feasibilityTabSelectedTreatment.feasibility;
            } else {
                this.feasibility = clone(emptyFeasibility);
            }
        }

        /**
         * Creates a new Feasibility object to add to the selected treatment
         */
        onCreateFeasibility() {
            const newFeasibility: Feasibility = {
                ...clone(emptyFeasibility),
                id: ObjectID.generate()
            };

            this.submitChanges(newFeasibility);
        }

        /**
         * Shows the CriteriaEditorDialog passing in the Feasibility object's criteria data
         */
        onEditFeasibilityCriteria() {
            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.feasibility.criteria
            };
        }

        /**
         * User has submitted a CriteriaEditorDialog result
         * @param criteria The criteria submitted by the user
         */
        onSubmitFeasibilityCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.feasibility.criteria = criteria;

                this.submitChanges(this.feasibility);
            }
        }

        /**
         * Sends the Feasibility object to the submitChanges function when a user modifies it's yearsBeforeAny or
         * yearsBeforeSame data
         */
        onChangeYears() {
            this.submitChanges(this.feasibility);
        }

        /**
         * Calls the submitChanges function with a null value parameter
         */
        onDeleteFeasibility() {
            const deletedFeasibility: Feasibility = {
                ...this.feasibility,
                criteria: '',
                yearsBeforeAny: 0,
                yearsBeforeSame: 0
            };
            this.submitChanges(deletedFeasibility);
        }

        /**
         * Modifies the selected treatment & selected treatment library with the Feasibility object's data changes
         * @param feasibilityData The feasibility data to submit changes on
         */
        submitChanges(feasibilityData: Feasibility) {
            this.feasibilityTabSelectedTreatment.feasibility = feasibilityData;

            const updatedTreatmentIndex: number = findIndex((treatment: Treatment) =>
                treatment.id === this.feasibilityTabSelectedTreatment.id,
                this.feasibilityTabSelectedTreatmentLibrary.treatments
            );
            this.feasibilityTabSelectedTreatmentLibrary
                .treatments[updatedTreatmentIndex] = this.feasibilityTabSelectedTreatment;

            this.$emit('submit', this.feasibilityTabSelectedTreatmentLibrary);
        }
    }
</script>

<style>
    .feasibility-tab-content {
        height: 185px;
    }
</style>