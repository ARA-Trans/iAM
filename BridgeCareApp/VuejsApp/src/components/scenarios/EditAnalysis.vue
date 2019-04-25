<template>
    <v-container fluid grid-list-xl>
        <div class="analysis-container">
            <v-layout>
                <v-flex xs12>
                    <v-layout column fill-height>
                        <v-layout justify-center row fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs3>
                                <v-menu v-model="showDatePicker" :close-on-content-click="false"
                                        :nudge-right="40" lazy transition="scale-transition" offset-y full-width
                                        min-width="290px">
                                    <template slot="activator">
                                        <v-text-field v-model="analysis.startYear" label="Start year" append-icon="event"
                                                      readonly outline>
                                        </v-text-field>
                                    </template>
                                    <v-date-picker v-model="year" ref="picker" min="1950" reactive no-title
                                                   @input="onSetStartYear">
                                    </v-date-picker>
                                </v-menu>
                            </v-flex>
                            <v-flex xs3>
                                <v-text-field v-model.number="analysis.analysisPeriod" type="number"
                                              label="Analysis period" outline>
                                </v-text-field>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs3>
                                <v-select v-model="analysis.optimizationType" :items="optimizationType"
                                          label="Optimization type" outline>
                                </v-select>
                            </v-flex>
                            <v-flex xs3>
                                <v-select v-model="analysis.budgetType" :items="budgetType" label="Budget type" outline>
                                </v-select>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs3>
                                <v-text-field v-model.number="analysis.benefitLimit" type="number" label="Benefit limit" outline >
                                </v-text-field>
                            </v-flex>
                            <v-flex xs3></v-flex>
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
                <v-btn depressed color="grey" @click="onCancelAnalysisEdit">Cancel</v-btn>
            </v-layout>
        </v-footer>

        <CriteriaEditor :dialogData="criteriaEditorDialogData" @submit="onSubmitScopeCriteria" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';
    import { Action, State } from 'vuex-class';

    import moment from 'moment';
    import {Analysis, emptyAnalysis, Scenario, ScenarioAnalysisUpsertData} from '@/shared/models/iAM/scenario';
    import CriteriaEditor from '@/shared/dialogs/CriteriaEditor.vue';
    import {hasValue} from '@/shared/utils/has-value';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/dialogs/criteria-editor-dialog/criteria-editor-dialog-data';
    import {isNil} from 'ramda';

    @Component({
        components: {CriteriaEditor}
    })
    export default class EditAnalysis extends Vue {
        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.scenario.selectedScenario) selectedScenario: Scenario;

        @Action('setNavigation') setNavigationAction: any;
        @Action('applyAnalysisToScenario') applyAnalysisToScenarioAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        analysis: Analysis = {...emptyAnalysis, startYear: moment().year()};
        showDatePicker: boolean = false;
        year: string = moment().year().toString();
        maxYear: string = moment().add(49, 'years').year().toString();
        optimizationType: string[] = ['Incremental benefit/cost', 'Another one', 'The better one'];
        budgetType: string[] =  ['As budget permits', 'Another one', 'The better one'];
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};

        /**
         * Component has been created
         */
        created() {
            // set the breadcrumbs for this component ui
            this.setNavigationAction([
                {
                    text: 'Scenario dashboard',
                    to: '/Scenarios/'
                },
                {
                    text: 'Scenario editor',
                    to: '/EditScenario/'
                },
                {
                    text: 'Analysis editor',
                    to: '/EditAnalysis/'
                }
            ]);
        }

        /**
         * Component has been mounted
         */
        mounted() {
            if (this.selectedScenario.simulationId > 0 && hasValue(this.selectedScenario.analysis)) {
                // set the initial start year to the selected scenario's start year if present, otherwise use current year
                const startYear: number = this.selectedScenario.analysis.startYear > 0
                    ? this.selectedScenario.analysis.startYear
                    : moment().year();
                this.year = startYear.toString();
                this.analysis = {
                    ...this.selectedScenario.analysis,
                    startYear: startYear
                };
            } else {
                // set 'no selected scenario' error message, then redirect user to Scenarios UI
                this.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                this.$router.push('/Scenarios/');
            }
        }

        /**
         * Sets the activePicker to use 'YEAR' input if showDatePicker is true
         */
        @Watch('showDatePicker')
        onShowDatePickerChanged() {
            if (this.showDatePicker) {
                // @ts-ignore
                this.$nextTick(() => this.$refs.picker.activePicker = 'YEAR');
            }
        }

        /**
         * Sets year & analysis.startYear with user selected year
         */
        onSetStartYear(year: string) {
            this.year = year.substr(0, 4);
            this.analysis.startYear = parseInt(this.year);
            this.showDatePicker = false;
        }

        /**
         * Opens the CriteriaEditor passing in the analysis criteria
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
            const scenarioAnalysisUpsertData: ScenarioAnalysisUpsertData = {
                id: this.selectedScenario.simulationId,
                analysis: this.analysis
            };
            this.applyAnalysisToScenarioAction({scenarioAnalysisUpsertData: scenarioAnalysisUpsertData})
                .then(() => {
                    // set 'analysis applied' success message, then redirect user to EditScenario UI
                    this.setSuccessMessageAction({message: 'Analysis was applied to selected scenario'});
                    this.$router.push('/EditScenario/');
                });
        }

        /**
         * Returns user to EditScenario UI
         */
        onCancelAnalysisEdit() {
            this.$router.push('/EditScenario/');
        }
    }
</script>

<style scoped>
</style>