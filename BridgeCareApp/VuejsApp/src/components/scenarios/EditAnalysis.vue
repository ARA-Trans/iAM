<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout column>
                <v-layout justify-center>
                    <v-flex xs2>
                        <v-text-field :mask="'####'" @input="onSetAnalysisProperty('startYear', $event)"
                                      label="Start Year"
                                      outline
                                      :value="analysis.startYear"></v-text-field>
                    </v-flex>
                    <v-flex xs2>
                        <v-select :items="weightingAttributes"
                                  @change="onSetAnalysisProperty('weightingAttribute', $event)" label="Weighting"
                                  outline
                                  :value="analysis.weightingAttribute">
                        </v-select>
                    </v-flex>
                    <v-flex xs2>
                        <v-select :items="optimizationTypes" @change="onSetAnalysisProperty('optimizationType', $event)"
                                  label="Optimization type" outline
                                  :value="analysis.optimizationType">
                        </v-select>
                    </v-flex>
                </v-layout>
                <v-layout justify-center>
                    <v-flex xs2>
                        <v-select :items="budgetTypes" @change="onSetAnalysisProperty('budgetType', $event)"
                                  label="Budget type" outline
                                  :value="analysis.budgetType">
                        </v-select>
                    </v-flex>
                    <v-flex xs2>
                        <v-select :items="benefitAttributes" @change="onSetAnalysisProperty('benefitAttribute', $event)"
                                  label="Benefit"
                                  outline
                                  :value="analysis.benefitAttribute">
                        </v-select>
                    </v-flex>
                    <v-flex xs2>
                        <v-text-field @input="onSetAnalysisProperty('benefitLimit', $event)" label="Benefit limit"
                                      outline
                                      type="number"
                                      :value.number="analysis.benefitLimit">
                        </v-text-field>
                    </v-flex>
                </v-layout>
                <v-layout justify-center>
                    <v-spacer></v-spacer>
                    <v-flex xs6>
                        <v-textarea @input="onSetAnalysisProperty('description', $event)" label="Description" no-resize
                                    outline rows="5"
                                    :value="analysis.description">
                        </v-textarea>
                    </v-flex>
                    <v-spacer></v-spacer>
                </v-layout>
                <v-layout justify-center>
                    <v-spacer></v-spacer>
                    <v-flex xs6>
                        <v-textarea label="Criteria" no-resize outline readonly rows="5" v-model="analysis.criteria">
                            <template slot="append-outer">
                                <v-btn @click="onEditScopeCriteria" class="edit-icon" icon>
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
                <v-btn @click="onApplyToScenario" class="ara-blue-bg white--text">Save</v-btn>
                <v-btn @click="onCancelAnalysisEdit" class="ara-orange-bg white--text">Cancel</v-btn>
            </v-layout>
        </v-flex>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData"
                              @submitCriteriaEditorDialogData="onSubmitScopeCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Watch} from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {Action, State} from 'vuex-class';
    import moment from 'moment';
    import {Analysis, emptyAnalysis} from '@/shared/models/iAM/scenario';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {clone, equals, isNil} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {setItemPropertyValue} from '@/shared/utils/setter-utils';

    @Component({
        components: {CriteriaEditorDialog}
    })
    export default class EditAnalysis extends Vue {
        @State(state => state.scenario.analysis) stateAnalysis: Analysis;
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('getScenarioAnalysis') getScenarioAnalysisAction: any;
        @Action('saveScenarioAnalysis') saveScenarioAnalysisAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        selectedScenarioId: string = '0';
        objectIdMOngoDBForScenario: string = '';
        analysis: Analysis = {...emptyAnalysis, startYear: moment().year()};
        optimizationTypes: string[] = ['Incremental Benefit/Cost', 'Maximum Benefit', 'Remaining Life/Cost',
            'Maximum Remaining Life', 'Multi-year Incremental Benefit/Cost', 'Multi-year Maximum Benefit',
            'Multi-year Remaining Life/Cost', 'Multi-year Maximum Life'];
        budgetTypes: string[] = ['No Spending', 'As Budget Permits', 'Until Targets Met', 'Until Deficient Met',
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
                vm.selectedScenarioId = to.query.selectedScenarioId;
                vm.simulationName = to.query.simulationName;
                vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;

                if (vm.selectedScenarioId === '0') {
                    // set 'no selected scenario' error message, then redirect user to Scenarios UI
                    vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                    vm.$router.push('/Scenarios/');
                }

                // get the selected scenario's analysis data
                vm.getScenarioAnalysisAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});
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

        beforeDestroy() {
            this.setHasUnsavedChangesAction({value: false});
        }

        @Watch('stateAnalysis')
        onStateAnalysisChanged() {
            this.analysis = clone(this.stateAnalysis);
        }

        @Watch('analysis')
        onAnalysisChanged() {
            this.setHasUnsavedChangesAction({
                value: !equals(this.analysis, emptyAnalysis) && !equals(this.analysis, this.stateAnalysis)
            });
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

        onSetAnalysisProperty(property: string, value: any) {
            this.analysis = setItemPropertyValue(property, value, this.analysis);
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
                this.analysis = {...this.analysis, criteria: criteria};
            }
        }

        /**
         * Dispatch an action that sends the current analysis data to the server to apply the analysis data to the
         * selected scenario
         */
        onApplyToScenario() {
            this.saveScenarioAnalysisAction({
                scenarioAnalysisData: this.analysis,
                objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario
            });
        }

        /**
         * Resets the analysis object with a copy of the analysis object found in state
         */
        onCancelAnalysisEdit() {
            this.analysis = clone(this.stateAnalysis);
        }
    }
</script>
