<template>
    <v-container fluid grid-list-xl>
        <v-layout>
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
        emptyFeasibility,
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
    import {findIndex, isNil} from 'ramda';
    import {TabData} from '@/shared/models/child-components/treatment-editor/tab-data';

    @Component({
        components: {CriteriaEditor}
    })
    export default class FeasibilityTab extends Vue {
        @Prop() feasibilityTabData: TabData;

        feasibilityTabTreatmentStrategies: TreatmentStrategy[];
        feasibilityTabSelectedTreatmentStrategy: TreatmentStrategy;
        feasibilityTabSelectedTreatment: Treatment;
        feasibility: Feasibility = {...emptyFeasibility};
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};

        /**
         * Sets the FeasibilityTab required UI functionality properties
         */
        @Watch('feasibilityTabData')
        onFeasibilityTabDataChanged() {
            this.feasibilityTabTreatmentStrategies = this.feasibilityTabData.tabTreatmentStrategies;
            this.feasibilityTabSelectedTreatmentStrategy = this.feasibilityTabData.tabSelectedTreatmentStrategy;
            this.feasibilityTabSelectedTreatment = this.feasibilityTabData.tabSelectedTreatment;
            this.setFeasibility();
        }

        /**
         * Sets the feasibility property based on feasibilitySelectedTreatment
         */
        setFeasibility() {
            if (this.feasibilityTabSelectedTreatment.id !== 0 &&
                !isNil(this.feasibilityTabSelectedTreatment.feasibility) &&
                this.feasibilityTabSelectedTreatment.feasibility.id !== 0) {
                    this.feasibility = {...this.feasibilityTabSelectedTreatment.feasibility};
            } else {
                this.feasibility = {...emptyFeasibility};
            }
        }

        /**
         * 'Create Feasibility' button has been clicked
         */
        onCreateFeasibility() {
            // create a new, empty feasibility object
            const createdFeasibility: Feasibility = {...emptyFeasibility, treatmentId: this.feasibilityTabSelectedTreatment.id};
            // get all feasibilities from treatmentStrategies' treatments
            const allFeasibilities: Feasibility[] = [];
            this.feasibilityTabTreatmentStrategies.forEach((treatmentStrategy: TreatmentStrategy) => {
                allFeasibilities.push(...getPropertyValues('feasibility', treatmentStrategy.treatments));
            });
            // get the latest feasibility id from allFeasibilities
            const latestId: number = getLatestPropertyValue(
                'id', allFeasibilities.filter((feasibility: Feasibility) => !isNil(feasibility))
            );
            // set createdFeasibility.id = latestId + 1 (if present), otherwise set as 1
            createdFeasibility.id = hasValue(latestId) ? latestId + 1 : 1;
            this.submitChanges(createdFeasibility);
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
                this.submitChanges({...this.feasibility, criteria: criteria});
            }
        }

        /**
         * User has changed one of 'Years Before Any' or 'Years Before Same' inputs
         */
        onChangeYears() {
            this.submitChanges({...this.feasibility});
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
            const updatedTreatmentStrategy = {...this.feasibilityTabSelectedTreatmentStrategy};
            const updatedTreatment: Treatment = {...this.feasibilityTabSelectedTreatment, feasibility: feasibilityData};
            const updatedTreatmentIndex: number = findIndex((treatment: Treatment) =>
                treatment.id === updatedTreatment.id, updatedTreatmentStrategy.treatments
            );
            updatedTreatmentStrategy.treatments[updatedTreatmentIndex] = updatedTreatment;
            this.$emit('submit', updatedTreatmentStrategy);
        }
    }
</script>