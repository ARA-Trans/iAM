<template>
    <v-container fluid grid-list-xl>
        <div class="analysis-container">
            <v-layout>
                <v-flex xs12>
                    <v-layout column fill-height>
                        <v-layout justify-center row fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs2>
                                <v-menu v-model="menu" ref="menu" :close-on-content-click="false" :nudge-right="40" lazy
                                        transition="scale-transition" offset-y full-width min-width="290px">
                                    <template slot="activator">
                                        <v-text-field v-model="analysis.startYear" label="Start year" append-icon="event"
                                                      readonly outline>
                                        </v-text-field>
                                    </template>
                                    <v-date-picker v-model="analysis.startYear" ref="picker" min="1950" :max="maxYear"
                                                   reactive no-title @input="onSetStartYear">
                                    </v-date-picker>
                                </v-menu>
                            </v-flex>
                            <v-flex xs2>
                                <v-text-field v-model.number="analysis.analysisPeriod" type="number" label="Analysis period" outline>
                                </v-text-field>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs2>
                                <v-select v-model="analysis.optimizationType" :items="optimizationType" label="Optimization type"
                                          outline>
                                </v-select>
                            </v-flex>
                            <v-flex xs2>
                                <v-select v-model="analysis.budgetType" :items="budgetType" label="Budget type" outline>
                                </v-select>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs2>
                                <v-text-field v-model.number="analysis.benefitLimit" type="number" label="Benefit limit" outline >
                                </v-text-field>
                            </v-flex>
                            <v-flex xs2></v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs4>
                                <v-textarea v-model="analysis.description" rows="5" label="Description" no-resize outline>
                                </v-textarea>
                            </v-flex>
                            <v-spacer></v-spacer>
                        </v-layout>
                        <v-layout justify-center fill-height>
                            <v-spacer></v-spacer>
                            <v-flex xs4>
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
                <v-btn depressed color="primary">Apply</v-btn>
                <v-btn depressed color="grey" @click="cancel">Cancel</v-btn>
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
    import {Analysis, emptyAnalysis, Scenario} from '@/shared/models/iAM/scenario';
    import CriteriaEditor from '@/shared/dialogs/CriteriaEditor.vue';
    import {hasValue} from '@/shared/utils/has-value';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from "@/shared/models/dialogs/criteria-editor-dialog/criteria-editor-dialog-data";
    import {isNil} from 'ramda';

    @Component({
        components: {CriteriaEditor}
    })
    export default class EditAnalysis extends Vue {
        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.scenario.selectedScenario) selectedScenario: Scenario;

        @Action('setNavigation') setNavigationAction: any;

        analysis: Analysis = {...emptyAnalysis, startYear: moment().year()};
        menu: boolean = false;
        date: string = '';
        maxYear: string = moment().add(50, 'years').year().toString();
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};

        data() {
            return {
                analysisPeriod: 0,
                benefitLimit: 0,
                optimizationType: ['Incremental benefit/cost', 'Another one', 'The better one'],
                budgetType: ['As budget permits', 'Another one', 'The better one'],
            };
        }

        created() {
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

        mounted() {
            if (hasValue(this.selectedScenario) && hasValue(this.selectedScenario.analysis)) {
                this.analysis = {
                    ...this.selectedScenario.analysis,
                    startYear: hasValue(this.selectedScenario.analysis.startYear)
                        ? this.selectedScenario.analysis.startYear
                        : moment().year()
                };
            }
        }

        @Watch('selectedScenario')
        onSelectedScenarioChanged() {
            if (this.selectedScenario.simulationId > 0) {
                this.analysis = {...this.selectedScenario.analysis, startYear: moment().year()};
            }
        }
        
        @Watch('menu')
        onDateChanged(val: string) {
            //@ts-ignore
            val && this.$nextTick(() => (this.$refs.picker.activePicker = 'YEAR'));
        }

        onSetStartYear(date: string) {
            this.analysis.startYear = moment(date).year();
            //@ts-ignore
            this.$refs.picker.activePicker = 'YEAR';
            this.menu = false;
        }

        cancel() {
            this.$router.push('/EditScenario/');
        }

        onEditScopeCriteria() {
            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.analysis.criteria
            }
        }

        onSubmitScopeCriteria(criteria: string) {
            this.criteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
            if (!isNil(criteria)) {
                this.analysis.criteria = criteria;
            }
        }
    }
</script>

<style scoped>
</style>