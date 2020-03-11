<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout column>
                <v-layout justify-center>
                    <v-flex xs2>
                        <v-text-field v-model="analysis.startYear" label="Start Year" outline :mask="'####'"></v-text-field>
                    </v-flex>
                    <v-flex xs2>
                        <v-select v-model="analysis.weightingAttribute" :items="weightingAttributes" label="Weighting"
                                  outline>
                        </v-select>
                    </v-flex>
                    <v-flex xs2>
                        <v-select v-model="analysis.optimizationType" :items="optimizationTypes"
                                  label="Optimization type" outline>
                        </v-select>
                    </v-flex>
                </v-layout>
                <v-layout justify-center>
                    <v-flex xs2>
                        <v-select v-model="analysis.budgetType" :items="budgetTypes" label="Budget type" outline>
                        </v-select>
                    </v-flex>
                    <v-flex xs2>
                        <v-select v-model="analysis.benefitAttribute" :items="benefitAttributes" label="Benefit"
                                  outline>
                        </v-select>
                    </v-flex>
                    <v-flex xs2>
                        <v-text-field v-model.number="analysis.benefitLimit" type="number" label="Benefit limit" outline >
                        </v-text-field>
                    </v-flex>
                </v-layout>
                <v-layout justify-center>
                    <v-spacer></v-spacer>
                    <v-flex xs6>
                        <v-textarea v-model="analysis.description" rows="5" label="Description" no-resize outline>
                        </v-textarea>
                    </v-flex>
                    <v-spacer></v-spacer>
                </v-layout>
                <v-layout justify-center>
                    <v-spacer></v-spacer>
                    <v-flex xs6>
                        <v-textarea v-model="analysis.criteria" rows="5" label="Criteria" readonly no-resize outline>
                            <template slot="append-outer">
                                <v-btn icon class="edit-icon" @click="onEditScopeCriteria">
                                    <v-icon>fas fa-edit</v-icon>
                                </v-btn>
                            </template>
                        </v-textarea>
                    </v-flex>
                    <v-spacer></v-spacer>
                </v-layout>
            </v-layout>
        </v-flex>

        <v-flex xs12>
            <v-layout justify-end row>
                <v-btn class="ara-blue-bg white--text" @click="onApplyAnalysisToScenario">Save</v-btn>
                <v-btn class="ara-orange-bg white--text" @click="onCancelAnalysisEdit">Cancel</v-btn>
            </v-layout>
        </v-flex>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitScopeCriteria" />
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Watch} from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {State, Action} from 'vuex-class';
    import moment from 'moment';
    import {Analysis, emptyAnalysis} from '@/shared/models/iAM/scenario';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {isNil} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {getPropertyValues} from '@/shared/utils/getter-utils';

    @Component({
        components: {CriteriaEditorDialog}
    })
    export default class EditAnalysis extends Vue {
        @State(state => state.scenario.analysis) stateAnalysis: Analysis;
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('getScenarioAnalysis') getScenarioAnalysisAction: any;
        @Action('saveScenarioAnalysis') saveScenarioAnalysisAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        selectedScenarioId: number = 0;
        objectIdMOngoDBForScenario: string = '';
        analysis: Analysis = {...emptyAnalysis, startYear: moment().year()};
        showDatePicker: boolean = false;
        optimizationTypes: string[] = ['Incremental Benefit/Cost', 'Maximum Benefit', 'Remaining Life/Cost',
            'Maximum Remaining Life', 'Multi-year Incremental Benefit/Cost', 'Multi-year Maximum Benefit', 
            'Multi-year Remaining Life/Cost', 'Multi-year Maximum Life'];
        budgetTypes: string[] =  ['No Spending', 'As Budget Permits', 'Until Targets Met', 'Until Deficient Met',
            'Targets/Deficient Met', 'Unlimited'];
        benefitAttributes: string[] = [];
        weightingAttributes: string[] = ['None'];
        simulationName: string;
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};

        /**
         * beforeRouterEnter event handler
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                vm.selectedScenarioId = isNaN(to.query.selectedScenarioId) ? 0 : parseInt(to.query.selectedScenarioId);
                vm.simulationName = to.query.simulationName;
                vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;

                if (vm.selectedScenarioId === 0) {
                    // set 'no selected scenario' error message, then redirect user to Scenarios UI
                    vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                    vm.$router.push('/Scenarios/');
                }

                // get the selected scenario's analysis data
                vm.getScenarioAnalysisAction({selectedScenarioId: vm.selectedScenarioId});
            });
        }

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateNumericAttributes)) {
                this.setBenefitAndWeightingAttributes();
            }
        }

        @Watch('stateAnalysis')
        onStateAnalysisChanged() {
            this.analysis = {
                ...this.stateAnalysis,
                startYear: this.stateAnalysis.startYear > 0 ? this.stateAnalysis.startYear : moment().year()
            };
        }

        /**
         * Calls the setBenefitAndWeightingAttributes function if a change to stateNumericAttributes causes it to have a value
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            if (hasValue(this.stateNumericAttributes)) {
                this.setBenefitAndWeightingAttributes();
            }
        }

        /**
         * Sets the benefitAttributes & weightingAttributes lists using the numeric attributes from state
         */
        setBenefitAndWeightingAttributes() {
            const numericAttributes: string[] = getPropertyValues('name', this.stateNumericAttributes);
            this.benefitAttributes = numericAttributes;
            this.weightingAttributes = [...this.weightingAttributes, ...numericAttributes];
        }

        /**
         * Opens the CriteriaEditorDialog passing in the analysis criteria
         */
        onEditScopeCriteria() {
            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.analysis.criteria
            };
        }

        /**
         * Updates the analysis criteria (if present) with the user's submitted criteria
         */
        onSubmitScopeCriteria(criteria: string) {
            this.criteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
            if (!isNil(criteria)) {
                this.analysis.criteria = criteria;
            }
        }

        /**
         * Dispatch an action that sends the current analysis data to the server to apply the analysis data to the
         * selected scenario
         */
        onApplyAnalysisToScenario() {
            this.saveScenarioAnalysisAction({scenarioAnalysisData: this.analysis, objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario});
        }

        /**
         * Resets the analysis object with a copy of the analysis object found in state
         */
        onCancelAnalysisEdit() {
            this.analysis = {
                ...this.stateAnalysis,
                startYear: this.stateAnalysis.startYear > 0 ? this.stateAnalysis.startYear : moment().year()
            };
        }
    }
</script>
