<template>
    <v-container fluid grid-list-xl>
        <v-layout class="feasibility-tab-content" fill-height>
            <v-flex xs12>
                <v-layout justify-center fill-height v-if="feasibility.id === 0">
                    <v-btn color="info" v-on:click="onCreateFeasibility">Create Feasibility</v-btn>
                </v-layout>
                <v-layout v-if="feasibility.id !== 0" justify-center column fill-height>
                    <v-btn color="error" icon v-on:click="onDeleteFeasibility"><v-icon>delete</v-icon></v-btn>
                    <v-textarea no-resize full-width outline readonly append-outer-icon="edit"
                                @click:append-outer="onEditFeasibilityCriteria"
                                v-model="feasibility.criteria">
                    </v-textarea>
                    <v-layout>
                        <v-spacer></v-spacer>
                        <v-layout justify-space-between row fill-height>
                            <v-flex xs5>
                                <v-text-field label="Years Before Any" :mask="'####'" outline
                                              v-model="feasibility.yearsBeforeAny"
                                              v-on:change="onChangeYears">
                                </v-text-field>
                            </v-flex>
                            <v-flex xs5>
                                <v-text-field label="Years Before Same" :mask="'####'" outline
                                              v-model="feasibility.yearsBeforeSame"
                                              v-on:change="onChangeYears">
                                </v-text-field>
                            </v-flex>
                        </v-layout>
                        <v-spacer></v-spacer>
                    </v-layout>
                </v-layout>
            </v-flex>
        </v-layout>

        <CriteriaEditor :dialogData="criteriaEditorDialogData" @submit="onSubmitFeasibilityCriteria" />
    </v-container>
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
    import CriteriaEditor from '../../../shared/dialogs/CriteriaEditor.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/dialogs/criteria-editor-dialog/criteria-editor-dialog-data';
    import {hasValue} from '@/shared/utils/has-value';
    import {findIndex, isNil, uniq, clone} from 'ramda';
    import {TabData} from '@/shared/models/child-components/treatment-editor/tab-data';

    @Component({
        components: {CriteriaEditor}
    })
    export default class FeasibilityTab extends Vue {
        @Prop() feasibilityTabData: TabData;

        feasibilityTabTreatmentLibraries: TreatmentLibrary[] = [];
        feasibilityTabSelectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
        feasibilityTabSelectedTreatment: Treatment = clone(emptyTreatment);
        feasibilityTabLatestFeasibilityId: number = 0;
        feasibility: Feasibility = clone(emptyFeasibility);
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

        /**
         * Sets the FeasibilityTab required UI functionality properties
         */
        @Watch('feasibilityTabData')
        onFeasibilityTabDataChanged() {
            this.feasibilityTabTreatmentLibraries = this.feasibilityTabData.tabTreatmentLibraries;
            this.feasibilityTabSelectedTreatmentLibrary = this.feasibilityTabData.tabSelectedTreatmentLibrary;
            this.feasibilityTabSelectedTreatment = this.feasibilityTabData.tabSelectedTreatment;
            this.feasibilityTabLatestFeasibilityId = this.feasibilityTabData.latestFeasibilityId;
            this.setFeasibility();
        }

        /**
         * Sets the feasibility property based on feasibilitySelectedTreatment
         */
        setFeasibility() {
            if (this.feasibilityTabSelectedTreatment.id !== 0 &&
                !isNil(this.feasibilityTabSelectedTreatment.feasibility) &&
                this.feasibilityTabSelectedTreatment.feasibility.id !== 0) {
                    this.feasibility = clone(this.feasibilityTabSelectedTreatment.feasibility);
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
                treatmentId: this.feasibilityTabSelectedTreatment.id,
                id: hasValue(this.feasibilityTabLatestFeasibilityId) ? this.feasibilityTabLatestFeasibilityId + 1 : 1
            };
            this.submitChanges(newFeasibility);
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
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
            if (!isNil(criteria)) {
                this.submitChanges({...clone(this.feasibility), criteria: criteria});
            }
        }

        /**
         * User has changed one of 'Years Before Any' or 'Years Before Same' inputs
         */
        onChangeYears() {
            this.submitChanges(clone(this.feasibility));
        }

        /**
         * 'Delete' button has been clicked
         */
        onDeleteFeasibility() {
            this.submitChanges(null);
        }

        /**
         * Submits feasibility data changes
         * @param feasibilityData The feasibility data to submit changes on
         */
        submitChanges(feasibilityData: Feasibility | null) {
            const updatedTreatmentLibrary = clone(this.feasibilityTabSelectedTreatmentLibrary);
            const updatedTreatment: Treatment = {...clone(this.feasibilityTabSelectedTreatment), feasibility: clone(feasibilityData)};
            const updatedTreatmentIndex: number = findIndex((treatment: Treatment) =>
                treatment.id === updatedTreatment.id, updatedTreatmentLibrary.treatments
            );
            updatedTreatmentLibrary.treatments[updatedTreatmentIndex] = updatedTreatment;
            this.$emit('submit', updatedTreatmentLibrary);
        }
    }
</script>

<style>
    .feasibility-tab-content {
        height: 185px;
    }
</style>