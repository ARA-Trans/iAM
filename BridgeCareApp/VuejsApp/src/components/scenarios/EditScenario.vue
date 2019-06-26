<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center fill-height>
                    <v-flex xs6>
                        <v-layout column fill-height>
                            <div class="scenario-edit-text-field-div" v-on:click="onEditAnalysis">
                                <v-text-field :append-icon="'fas fa-chart-bar'" box readonly label="Analysis">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="onEditInvestment">
                                <v-text-field :append-icon="'fas fa-dollar-sign'" box readonly label="Investment">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="onEditPerformance">
                                <v-text-field :append-icon="'fas fa-chart-line'" box readonly label="Performance">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="onShowCommittedProjectsFileUploader">
                                <v-text-field :append-icon="'fas fa-tasks'" box readonly label="Commited">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="onEditTreatment">
                                <v-text-field :append-icon="'fas fa-heartbeat'" box readonly label="Treatments">
                                </v-text-field>
                            </div>
                            <div class="scenario-edit-text-field-div" v-on:click="onEditPrioritiesTargetsDeficients">
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
    import {isNil} from 'ramda';
    import {AxiosResponse} from 'axios';
    import CommittedProjectsService from '@/services/committed-projects.service';
    import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';
    import { Network } from '@/shared/models/iAM/network';
    import { hasValue } from '@/shared/utils/has-value-util';
    import FileDownload from 'js-file-download';

    @Component({
        components: { CommittedProjectsFileUploaderDialog }
    })
    export default class EditScenario extends Vue {
        @State(state => state.breadcrumb.navigation) navigation: any[];        
        @State(state => state.network.networks) networks: Network[];
        
        @Action('setNavigation') setNavigationAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;

        selectedScenarioId: number = 0;
        showFileUploader: boolean = false;
        networkId: number = 0;
        simulationName: string;
        selectedScenario: Scenario = {
            id: 0,
            simulationId: this.selectedScenarioId,
            networkId: this.networkId,
            simulationName: '',
            networkName: ''
        };

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                // set selectedScenarioId
                vm.selectedScenarioId = isNaN(to.query.selectedScenarioId) ? 0 : parseInt(to.query.selectedScenarioId);
                vm.simulationName = to.query.simulationName;
                // set breadcrumbs
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
                    }
                ]);
                // check that selectedScenarioId is set
                if (vm.selectedScenarioId === 0) {
                    // set 'no selected scenario' error message, then redirect user to Scenarios UI
                    vm.setErrorMessageAction({ message: 'Found no selected scenario for edit' });
                    vm.$router.push('/Scenarios/');
                }
            });
        }

        /**
         * Navigates user to the EditAnalysis page passing in the selected scenario's id
         */
        onEditAnalysis() {
            this.$router.push({
                path: '/EditAnalysis/', query: {selectedScenarioId: this.selectedScenarioId.toString(), simulationName: this.simulationName}
            });
        }

        /**
         * Navigates user to the InvestmentEditor page passing in the selected scenario's id
         */
        onEditInvestment() {
            this.$router.push({
                path: '/InvestmentEditor/FromScenario/', query: {
                    selectedScenarioId: this.selectedScenarioId.toString(),
                    simulationName: this.simulationName
                }
            });
        }

        /**
         * Navigates user to the PerformanceEditor page passing in the selected scenario's id
         */
        onEditPerformance() {
            this.$router.push({
                path: '/PerformanceEditor/FromScenario/', query: {selectedScenarioId: this.selectedScenarioId.toString(), simulationName: this.simulationName}
            });
        }

        /**
         * Navigates user to the TreatmentEditor page passing in the selected scenario's id
         */
        onEditTreatment() {
            this.$router.push({
                path: '/TreatmentEditor/FromScenario/', query: { selectedScenarioId: this.selectedScenarioId.toString(), simulationName: this.simulationName }
            });
        }

        /**
         * Navigates user to the PrioritiesTargetsDeficients page passing in the selected scenario's id
         */
        onEditPrioritiesTargetsDeficients() {
            this.$router.push({
                path: '/PrioritiesTargetsDeficients/', query: {selectedScenarioId: this.selectedScenarioId.toString(), simulationName: this.simulationName}
            });
        }

        /**
         * Shows the CommittedProjectsFileUploaderDialog
         */
        onShowCommittedProjectsFileUploader() {
            this.showFileUploader = true;
        }

        /**
         * Uploads the files submitted via the CommittedProjectsFileUploaderDialog (if present),
         * exports committed projects if isExport is true
         * @param files File array         
         * @param isExport boolean
         */
        onUploadCommitedProjectFiles(files: File[], isExport: boolean) {
            this.showFileUploader = false;
            if (!isNil(files)) {
                CommittedProjectsService.saveCommittedProjectsFiles(files, this.selectedScenarioId.toString(), this.networks[0].networkId.toString())
                    .then((response: AxiosResponse<any>) => {
                        if (http2XX.test(response.status.toString())) {
                            this.setSuccessMessageAction({ message: 'Successfully saved file(s)' });
                        } else {
                            this.setErrorMessageAction({ message: `Failed to save file(s)${setStatusMessage(response)}` });
                        }
                    });
            }
            if (isExport) {
                this.selectedScenario.simulationId = this.selectedScenarioId;
                this.selectedScenario.networkId = this.networks[0].networkId;
                CommittedProjectsService.ExportCommittedProjects(this.selectedScenario)
                    .then((response: AxiosResponse<any>) => {
                        if (hasValue(response) && http2XX.test(response.status.toString())) {
                            FileDownload(response.data, 'CommittedProjects.xlsx');
                        }
                        else {

                            this.setErrorMessageAction({ message: `Failed to export committed projects${setStatusMessage(response)}` });
                        }
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