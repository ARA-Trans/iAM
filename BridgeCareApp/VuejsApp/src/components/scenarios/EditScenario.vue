<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center fill-height>
                    <v-flex xs6>
                        <v-layout column fill-height>
                            <div class="scenario-edit-text-field-div" v-on:click="editAnalysis">
                                <v-text-field :append-icon="'fas fa-chart-bar'" box readonly label="Analysis">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="editInvestment">
                                <v-text-field :append-icon="'fas fa-dollar-sign'" box readonly label="Investment">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="editPerformance">
                                <v-text-field :append-icon="'fas fa-chart-line'" box readonly label="Performance">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="onShowCommittedProjectsFileUploader">
                                <v-text-field :append-icon="'fas fa-tasks'" box readonly label="Commited">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="editTreatment">
                                <v-text-field :append-icon="'fas fa-heartbeat'" box readonly label="Treatments">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="">
                                <v-text-field :append-icon="'fas fa-copy'" box readonly label="Prioritization">
                                </v-text-field>
                            </div>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>

        <CommittedProjectsFileUploaderDialog :showDialog="showFileUploader" @submit="onUploadCommitedProjectFiles" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {State, Action} from 'vuex-class';
    import {Scenario} from '@/shared/models/iAM/scenario';
    import CommittedProjectsFileUploaderDialog from '@/components/scenarios/scenarios-dialogs/CommittedProjectsFileUploaderDialog.vue';
    import ScenarioService from '@/services/scenario.service';
    import {isNil} from 'ramda';

    @Component({
        components: {CommittedProjectsFileUploaderDialog}
    })
    export default class EditScenario extends Vue {
        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.scenario.selectedScenario) selectedScenario: Scenario;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('setNavigation') setNavigationAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;

        selectedScenarioId: number = 0;
        showFileUploader: boolean = false;

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                // set selectedScenarioId
                vm.selectedScenarioId = isNaN(to.query.simulationId) ? 0 : parseInt(to.query.simulationId);
                // set breadcrumbs
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
                    }
                ]);
                // check that selectedScenarioId is set
                if (vm.selectedScenarioId === 0) {
                    // set 'no selected scenario' error message, then redirect user to Scenarios UI
                    vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                    vm.$router.push('/Scenarios/');
                }
            });
        }

        /**
         * Navigates to EditAnalysis UI, providing simulationId context
         */
        editAnalysis() {
            this.$router.push({
                path: '/EditAnalysis/', query: {simulationId: this.selectedScenarioId.toString()}
            });
        }

        /**
         * Navigates to InvestmentEditor UI, providing simulationId context
         */
        editInvestment() {
            this.$router.push({
                path: '/InvestmentEditor/FromScenario/', query: {
                    simulationId: this.selectedScenarioId.toString()
                }
            });
        }

        /**
         * Navigates to PerformanceEditor UI, providing simulationId context
         */
        editPerformance() {
            this.$router.push({
                path: '/PerformanceEditor/FromScenario/', query: {simulationId: this.selectedScenarioId.toString()}
            });
        }

        /**
         * Navigates to TreatmentEditor UI, providing simulationId context
         */
        editTreatment() {
            this.$router.push({
                path: '/TreatmentEditor/FromScenario/', query: {simulationId: this.selectedScenarioId.toString()}
            });
        }

        /**
         * Shows the CommittedProjectsFileUploaderDialog
         */
        onShowCommittedProjectsFileUploader() {
            this.showFileUploader = true;
        }

        /**
         * Uploads the files submitted via the CommittedProjectsFileUploaderDialog (if present)
         * @param files File array
         */
        onUploadCommitedProjectFiles(files: File[]) {
            this.showFileUploader = false;
            if (!isNil(files)) {
                this.setIsBusyAction({isBusy: true});
                new ScenarioService().uploadCommittedProjectsFiles(files)
                    .then(() => {
                        this.setIsBusyAction({isBusy: false});
                        // TODO: handle server response properly
                        this.setSuccessMessageAction({message: 'Files uploaded successfully'});
                    });
            }
        }
    }
</script>

<style>
    .scenario-edit-text-field-div * {
        cursor: pointer;
    }
</style>