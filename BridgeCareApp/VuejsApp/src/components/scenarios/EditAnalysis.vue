<template>
    <v-container fluid grid-list-xl>
        <div class="analysis-container">
            <v-layout>
                <v-flex xs12>
                    <v-layout column fill-height>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs1>
                                <EditYearDialog :itemYear="analysis.startYear.toString()" :itemLabel="'Start Year'"
                                                :outline="true" @editedYear="onSetStartYear" />
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center row fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs3>
                                <v-text-field v-model.number="analysis.analysisPeriod" type="number"
                                              label="Analysis period" outline>
                                </v-text-field>
                            </v-flex>
                            <v-flex xs3>
                                <v-select v-model="analysis.weightingAttribute" :items="weightingAttributes" label="Weighting"
                                          outline>
                                </v-select>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs3>
                                <v-select v-model="analysis.optimizationType" :items="optimizationTypes"
                                          label="Optimization type" outline>
                                </v-select>
                            </v-flex>
                            <v-flex xs3>
                                <v-select v-model="analysis.budgetType" :items="budgetTypes" label="Budget type" outline>
                                </v-select>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs3>
                                <v-select v-model="analysis.benefitAttribute" :items="benefitAttributes" label="Benefit"
                                          outline>
                                </v-select>
                            </v-flex>
                            <v-flex xs3>
                                <v-text-field v-model.number="analysis.benefitLimit" type="number" label="Benefit limit" outline >
                                </v-text-field>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs6>
                                <v-textarea v-model="analysis.description" rows="5" label="Description" no-resize outline>
                                </v-textarea>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs6>
                                <v-textarea v-model="analysis.criteria" rows="5" label="Criteria" readonly no-resize outline
                                            append-outer-icon="edit" @click:append-outer="onEditScopeCriteria">
                                </v-textarea>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                    </v-layout>
                </v-flex>
            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn depressed color="primary" @click="onApplyAnalysisToScenario">Apply</v-btn>
                <v-btn depressed color="error" @click="onCancelAnalysisEdit">Cancel</v-btn>
            </v-layout>
        </v-footer>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitScopeCriteria" />
    </v-container>
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
    import AnalysisEditorService from '@/services/analysis-editor.service';
    import {AxiosResponse} from 'axios';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import EditYearDialog from '@/shared/modals/EditYearDialog.vue';

    @Component({
        components: {EditYearDialog, CriteriaEditorDialog}
    })
    export default class EditAnalysis extends Vue {
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('setNavigation') setNavigationAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getBenefitAttributes') getBenefitAttributesAction: any;

        selectedScenarioId: number = 0;
        analysis: Analysis = {...emptyAnalysis, startYear: moment().year()};
        showDatePicker: boolean = false;
        optimizationTypes: string[] = ['Incremental Benefit/Cost', 'Maximum Benefit', 'Remaining Life/Cost',
            'Conditional RSL/Cost', 'Maximum Remaining Life', 'Multi-year Incremental Benefit/Cost',
            'Multi-year Maximum Benefit', 'Multi-year Remaining Life/Cost', 'Multi-year Maximum Life'];
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

                if (vm.selectedScenarioId === 0) {
                    // set 'no selected scenario' error message, then redirect user to Scenarios UI
                    vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                    vm.$router.push('/Scenarios/');
                }
                vm.setNavigationAction([
                    {
                        text: 'Scenario dashboard',
                        to: '/Scenarios/'
                    },
                    {
                        text: 'Scenario editor',
                        to: {
                            path: '/EditScenario/', query: {selectedScenarioId: to.query.selectedScenarioId, simulationName: to.query.simulationName}
                        }
                    },
                    {
                        text: 'Analysis editor',
                        to: {
                            path: '/EditAnalysis/', query: {selectedScenarioId: to.query.selectedScenarioId, simulationName: to.query.simulationName}
                        }
                    }
                ]);
                // check that selectedScenarioId is set
                if (vm.selectedScenarioId > 0) {
                    // get the selected scenario's analysis data
                    AnalysisEditorService.getScenarioAnalysisData(vm.selectedScenarioId)
                        .then((response: AxiosResponse<Analysis>) => {
                            vm.analysis = {
                                ...response.data,
                                startYear: response.data.startYear > 0 ? response.data.startYear : moment().year()
                            };
                        });
                } else {
                    // set 'no selected scenario' error message, then redirect user to Scenarios UI
                    vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                    vm.$router.push('/Scenarios/');
                }
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
         * Sets year & analysis.startYear with user selected year
         */
        onSetStartYear(year: string) {
            this.analysis.startYear = parseInt(year);
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
         * selected scenario (will redirect user to EditScenario on a success)
         */
        onApplyAnalysisToScenario() {
            AnalysisEditorService.saveScenarioAnalysisData(this.analysis)
                .then((response: AxiosResponse<any>) => {
                    // set 'analysis applied' success message, then navigate user to EditScenario page
                    this.setSuccessMessageAction({message: 'Saved scenario analysis data'});
                    this.$router.push({
                        path: '/EditScenario/', query: {
                            selectedScenarioId: this.selectedScenarioId.toString(), simulationName: this.simulationName
                        }
                    });
                });
        }

        /**
         * Navigates user to the EditScenario page passing in the selected scenario's id
         */
        onCancelAnalysisEdit() {
            this.$router.push({
                path: '/EditScenario/', query: {selectedScenarioId: this.selectedScenarioId.toString(), simulationName: this.simulationName}
            });
        }
    }
</script>

<style scoped>
</style>