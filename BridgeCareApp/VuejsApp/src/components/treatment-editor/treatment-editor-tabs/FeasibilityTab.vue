<template>
    <v-layout class="feasibility-tab-content">
        <v-flex xs12>
            <v-layout column justify-center v-if="feasibility.id !== '0'">
                <v-textarea full-width no-resize outline prepend-outer-icon="fas fa-trash" readonly
                            v-model="feasibility.criteria">
                    <template slot="append-outer">
                        <v-layout align-center fill-height row>
                            <v-btn @click="onEditFeasibilityCriteria" class="edit-icon" icon>
                                <v-icon>fas fa-edit</v-icon>
                            </v-btn>
                            <v-btn @click="onDeleteFeasibility" class="ara-orange" icon>
                                <v-icon>fas fa-minus-square</v-icon>
                            </v-btn>
                        </v-layout>
                    </template>
                </v-textarea>
                <v-layout>
                    <v-spacer></v-spacer>
                    <v-layout justify-space-between row>
                        <v-flex xs5>
                            <v-text-field :mask="'####'" @change="onChangeYears" label="Years Before Any"
                                          outline
                                          v-model="feasibility.yearsBeforeAny">
                            </v-text-field>
                        </v-flex>
                        <v-flex xs5>
                            <v-text-field :mask="'####'" @change="onChangeYears" label="Years Before Same"
                                          outline
                                          v-model="feasibility.yearsBeforeSame">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                    <v-spacer></v-spacer>
                </v-layout>
            </v-layout>
        </v-flex>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData"
                              @submitCriteriaEditorDialogResult="onSubmitFeasibilityCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {
        emptyFeasibility,
        emptyTreatment,
        emptyTreatmentLibrary,
        Feasibility,
        Treatment,
        TreatmentLibrary
    } from '@/shared/models/iAM/treatment';
    import CriteriaEditorDialog from '../../../shared/modals/CriteriaEditorDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {clone, findIndex, isNil, propEq, update} from 'ramda';
    import {TabData} from '@/shared/models/child-components/tab-data';
    import {hasValue} from '@/shared/utils/has-value-util';

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
            this.feasibility = hasValue(this.feasibilityTabSelectedTreatment.feasibility)
                ? this.feasibility = clone(this.feasibilityTabSelectedTreatment.feasibility)
                : {...emptyFeasibility, id: ObjectID.generate()};
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
                this.feasibilityTabSelectedTreatmentLibrary = {
                    ...this.feasibilityTabSelectedTreatmentLibrary,
                    treatments: update(
                        findIndex(propEq('id', this.feasibilityTabSelectedTreatment.id), this.feasibilityTabSelectedTreatmentLibrary.treatments),
                        {
                            ...this.feasibilityTabSelectedTreatment,
                            feasibility: {...this.feasibility, criteria: criteria}
                        },
                        this.feasibilityTabSelectedTreatmentLibrary.treatments
                    )
                };

                this.$emit('submit', this.feasibilityTabSelectedTreatmentLibrary);
            }
        }

        /**
         * Sends the Feasibility object to the submitChanges function when a user modifies it's yearsBeforeAny or
         * yearsBeforeSame data
         */
        onChangeYears() {
            this.feasibilityTabSelectedTreatmentLibrary = {
                ...this.feasibilityTabSelectedTreatmentLibrary,
                treatments: update(
                    findIndex(propEq('id', this.feasibilityTabSelectedTreatment.id), this.feasibilityTabSelectedTreatmentLibrary.treatments),
                    {...this.feasibilityTabSelectedTreatment, feasibility: {...this.feasibility}},
                    this.feasibilityTabSelectedTreatmentLibrary.treatments
                )
            };

            this.$emit('submit', this.feasibilityTabSelectedTreatmentLibrary);
        }

        /**
         * Calls the submitChanges function with a null value parameter
         */
        onDeleteFeasibility() {
            const deletedFeasibility: Feasibility = {
                ...this.feasibility, criteria: '', yearsBeforeAny: 0, yearsBeforeSame: 0
            };

            this.feasibilityTabSelectedTreatmentLibrary = {
                ...this.feasibilityTabSelectedTreatmentLibrary,
                treatments: update(
                    findIndex(propEq('id', this.feasibilityTabSelectedTreatment.id), this.feasibilityTabSelectedTreatmentLibrary.treatments),
                    {...this.feasibilityTabSelectedTreatment, feasibility: deletedFeasibility},
                    this.feasibilityTabSelectedTreatmentLibrary.treatments
                )
            };

            this.$emit('submit', this.feasibilityTabSelectedTreatmentLibrary);
        }
    }
</script>

<style>
    .feasibility-tab-content {
        height: 185px;
    }
</style>
