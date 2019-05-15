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

        <CriteriaEditor :dialogData="criteriaEditorDialogData" @submit="onSubmitScopeCriteria" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Watch} from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {State, Action} from 'vuex-class';

    import moment from 'moment';
    import {Analysis, emptyAnalysis} from '@/shared/models/iAM/scenario';
    import CriteriaEditor from '@/shared/dialogs/CriteriaEditor.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/dialogs/criteria-editor-dialog/criteria-editor-dialog-data';
    import {isNil} from 'ramda';
    import ScenarioService from '@/services/scenario.service';

    @Component({
        components: {CriteriaEditor}
    })
    export default class EditAnalysis extends Vue {
        @State(state => state.scenario.benefitAttributes) benefitAttributes: string[];

        @Action('setNavigation') setNavigationAction: any;
        @Action('setIsBusy') setIsBusyAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getBenefitAttributes') getBenefitAttributesAction: any;

        selectedScenarioId: number = 0;
        analysis: Analysis = {...emptyAnalysis, startYear: moment().year()};
        showDatePicker: boolean = false;
        year: string = moment().year().toString();
        optimizationType: string[] = ['Incremental benefit/cost', 'Another one', 'The better one'];
        budgetType: string[] =  ['As budget permits', 'Another one', 'The better one'];
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};

        /**
         * Sets component UI properties
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                vm.selectedScenarioId = isNaN(to.query.simulationId) ? 0 : parseInt(to.query.simulationId);
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
                            path: '/EditScenario/', query: {simulationId: to.query.simulationId}
                        }
                    },
                    {
                        text: 'Analysis editor',
                        to: {
                            path: '/EditAnalysis/', query: {simulationId: to.query.simulationId}
                        }
                    }
                ]);

                vm.setIsBusyAction({isBusy: true});
                new ScenarioService().getScenarioAnalysisData(vm.selectedScenarioId)
                    .then((analysis: Analysis) => {
                        vm.analysis = {
                            ...analysis,
                            startYear: analysis.startYear > 0 ? analysis.startYear : moment().year()
                        };
                        vm.getBenefitAttributesAction().then(() => vm.setIsBusyAction({isBusy: false}));
                    });
            });
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
            new ScenarioService().applyAnalysisDataToScenario(this.analysis)
                .then((dataUpserted: boolean) => {
                    if (dataUpserted) {
                        // set 'analysis applied' success message, then redirect user to EditScenario UI
                        this.setSuccessMessageAction({message: 'Analysis was applied to selected scenario'});
                        this.$router.push({
                            path: '/EditScenario/', query: {simulationId: this.selectedScenarioId.toString()}
                        });
                    } else {
                        this.setErrorMessageAction({message: 'Failed to apply analysis data to scenario'});
                    }
                });
        }

        /**
         * Returns user to EditScenario UI
         */
        onCancelAnalysisEdit() {
            this.$router.push({
                path: '/EditScenario/', query: {simulationId: this.selectedScenarioId.toString()}
            });
        }
    }
</script>

<style scoped>
</style>