<template>
    <v-container fluid grid-list-xl>
        <v-layout row wrap>
            <v-flex xs12>
                <v-card>
                    <v-card-title>
                        My Scenarios
                        <v-spacer></v-spacer>
                        <v-text-field v-model="searchMine" append-icon="search" lablel="Search" single-line
                                      hide-details>
                        </v-text-field>
                    </v-card-title>
                    <v-data-table :headers="scenarioGridHeaders" :items="userScenarios" :search="searchMine">
                        <template slot="items" slot-scope="props">
                            <td>{{props.item.simulationName}}</td>
                            <td>{{formatDate(props.item.createdDate)}}</td>
                            <td>{{formatDate(props.item.lastModifiedDate)}}</td>
                            <td>{{props.item.status}}</td>
                            <td>
                                <v-layout row wrap>
                                    <v-flex>
                                        <v-btn flat icon color="blue" v-on:click="onShowRunSimulationAlert(props.item)">
                                            <v-icon>fas fa-play</v-icon>
                                        </v-btn>
                                    </v-flex>
                                    <v-flex>
                                        <v-btn flat icon color="info" v-on:click="onShowReportsDownloaderDialog(props.item)">
                                            <v-icon>fas fa-chart-line</v-icon>
                                        </v-btn>
                                    </v-flex>
                                    <v-flex>
                                        <v-btn flat icon color="green" v-on:click="onEditScenario(props.item.scenarioId)">
                                            <v-icon>edit</v-icon>
                                        </v-btn>
                                    </v-flex>
                                    <v-flex>
                                        <v-btn flat icon color="red" v-on:click="">
                                            <v-icon>delete</v-icon>
                                        </v-btn>
                                    </v-flex>
                                </v-layout>
                            </td>
                        </template>
                        <v-alert slot="no-results" :value="true" color="error" icon="warning">
                            Your search for "{{searchMine}}" found no results.
                        </v-alert>
                    </v-data-table>
                    <v-card-actions>
                        <v-btn color="info lighten-1" v-on:click="onCreateScenario">Create new</v-btn>
                    </v-card-actions>
                </v-card>
            </v-flex>
        </v-layout>
        <v-layout row wrap>
            <v-flex xs12>
                <v-card>
                    <v-card-title>
                        Shared with Me
                        <v-spacer></v-spacer>
                        <v-text-field v-model="searchShared" append-icon="search" lablel="Search" single-line
                                      hide-details>
                        </v-text-field>
                    </v-card-title>
                    <v-data-table :headers="scenarioGridHeaders" :items="sharedScenarios" :search="searchShared">
                        <template slot="items" slot-scope="props">
                            <td>{{props.item.name}}</td>
                            <td>{{formatDate(props.item.createdDate)}}</td>
                            <td>{{formatDate(props.item.lastModifiedDate)}}</td>
                            <td>{{getStatus(props.item.status)}}</td>
                            <td>
                                <v-layout row wrap>
                                    <v-flex>
                                        <v-btn flat icon color="blue" v-on:click="onShowRunSimulationAlert(props.item)">
                                            <v-icon>fas fa-play</v-icon>
                                        </v-btn>
                                    </v-flex>
                                    <v-flex>
                                        <v-btn flat icon color="green" v-on:click="onShowReportsDownloaderDialog(props.item)">
                                            <v-icon>fas fa-chart-line</v-icon>
                                        </v-btn>
                                    </v-flex>
                                    <v-flex>
                                        <v-btn flat icon color="green" v-on:click="onEditSharedScenario(props.item.scenarioId)">
                                            <v-icon>edit</v-icon>
                                        </v-btn>
                                    </v-flex>
                                </v-layout>
                            </td>
                        </template>
                        <v-alert slot="no-results" :value="true" color="error" icon="warning">
                            Your search for "{{searchShared}}" found no results.
                        </v-alert>
                    </v-data-table>
                </v-card>
            </v-flex>
        </v-layout>
        <v-flex xs12>
            <Alert :dialogData="alertData" @submit="onSubmitAlertResult" />
        </v-flex>
        <v-flex xs12>
            <ReportsDownloaderDialog :dialogData="reportsDownloaderDialogData" />
        </v-flex>
        <v-flex xs12>
            <CreateScenarioDialog :dialogData="createScenarioDialogData" @submit="onSubmitNewScenario" />
        </v-flex>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import moment from 'moment';
    import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    import ReportsDownloaderDialog from '@/components/scenarios/scenarios-dialogs/ReportsDownloaderDialog.vue';
    import {
        emptyReportsDownloadDialogData,
        ReportsDownloaderDialogData
    } from '@/shared/models/modals/reports-downloader-dialog-data';
    import {
        CreateScenarioDialogData,
        emptyCreateScenarioDialogData
    } from '@/shared/models/modals/scenario-creation-data';
    import CreateScenarioDialog from '@/components/scenarios/scenarios-dialogs/CreateScenarioDialog.vue';
    import {Network} from '@/shared/models/iAM/network';
    import {clone} from 'ramda';

    @Component({
        components: {Alert, ReportsDownloaderDialog, CreateScenarioDialog}
    })
    export default class Scenarios extends Vue {
        @State(state => state.scenario.scenarios) scenarios: Scenario[];
        @State(state => state.authentication.userId) userId: string;
        @State(state => state.reports.names) reportNames: string[];
        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.network.networks) networks: Network[];
        
        @Action('getUserScenarios') getUserScenariosAction: any;
        @Action('getScenarios') getScenariosAction: any;
        @Action('runSimulation') runSimulationAction: any;
        @Action('setNavigation') setNavigationAction: any;
        @Action('createScenario') createNewScenarioAction: any;

        alertData: AlertData = clone(emptyAlertData);
        reportsDownloaderDialogData: ReportsDownloaderDialogData = clone(emptyReportsDownloadDialogData);
        createScenarioDialogData: CreateScenarioDialogData = clone(emptyCreateScenarioDialogData);
        scenarioGridHeaders: object[] = [
            {text: 'Scenario Name', align: 'left', sortable: false, value: 'name'},
            {text: 'Date Created', sortable: false, value: 'createdDate'},
            {text: 'Date Last Modified', sortable: false, value: 'lastModifiedDate' },
            {text: 'Status', sortable: false, value: 'status' },
            {text: '', sortable: false, value: 'actions'}
        ];
        userScenarios: Scenario[] = [];
        sharedScenarios: Scenario[] = [];
        searchMine = '';
        searchShared = '';
        networkId: number = 0;
        networkName: string = '';
        simulationId: number = 0;
        simulationName: string = '';
        currentScenario: Scenario = clone(emptyScenario);

        @Watch('scenarios')
        onScenariosChanged() {
            if (hasValue(this.scenarios)) {
                // filter scenarios that are the user's
                this.userScenarios = this.scenarios.filter((simulation: Scenario) => !simulation.shared);
                // filter scenarios that are shared with the user
                this.sharedScenarios = this.scenarios.filter((simulation: Scenario) => simulation.shared);
            }

        }

        created() {
            this.setNavigationAction([]);
        }

        /**
         * Component has been mounted
         */
        mounted() {
            this.getUserScenariosAction({ userId: this.userId });
        }

        getStatus(isCompleted: boolean) {
            return isCompleted ? 'Completed' : 'Running';
        }

        /**
         * Formats a date as month/day/year
         * @param unformattedDate Unformatted date
         */
        formatDate(unformattedDate: Date) {
            return moment(unformattedDate).format('M/D/YYYY');
        }

        /**
         * Navigates user to EditScenario page passing in the simulation id of their scenario
         * @param id Scenario simulation id
         */
        onEditScenario(id: number) {
            this.$router.push({
                path: '/EditScenario/', query: {selectedScenarioId: id.toString()}
            });
        }
        
        /**
         * Navigates user to EditScenario page passing in the simulation id of a shared scenario
         * @param id Scenario simulation id
         */
        onEditSharedScenario(id: number) {
            this.$router.push({
                path: '/EditScenario/', query: {selectedScenarioId: id.toString()}
            });
        }

        /**
         * Shows the Alert
         */
        onShowRunSimulationAlert(scenario: Scenario) {
            this.currentScenario = scenario;
            
            this.alertData = {
                showDialog: true,
                heading: 'Warning',
                choice: true,
                message: 'The simulation can take around five minutes to finish. ' +
                    'Are you sure that you want to continue?'
            };
        }

        /**
         * Takes in a boolean parameter from the AppPopupModal to determine if a scenario's simulation should be executed
         * @param runSimulation Alert result
         */
        onSubmitAlertResult(runScenarioSimulation: boolean) {
            this.alertData = clone(emptyAlertData);
            
            if (runScenarioSimulation) {
                this.runScenarioSimulation();
            }
        }

        /**
         * Dispatches an action with the currentScenario object's data in order to run a simulation on the server
         */
        runScenarioSimulation() {
            this.runSimulationAction({
                networkId: this.currentScenario.networkId,
                simulationId: this.currentScenario.scenarioId,
                networkName: this.currentScenario.networkName,
                simulationName: this.currentScenario.simulationName,
                userId: this.userId
            });
        }

        /**
         * Shows the ReportsDownloaderDialog passing in the specified scenario's data
         * @param scenario Scenario object to use for setting the ReportsDownloaderDialogData object
         */
        onShowReportsDownloaderDialog(scenario: Scenario) {
            this.reportsDownloaderDialogData = {
                showModal: true,
                names: this.reportNames,
                networkId: scenario.networkId,
                networkName: scenario.networkName,
                simulationId: scenario.scenarioId,
                simulationName: scenario.simulationName,
            };
        }

        onCreateScenario() {
            this.createScenarioDialogData.showDialog = true;
        }

        onSubmitNewScenario(value: any) {
            if (!hasValue(value)) {
                this.createScenarioDialogData.showDialog = false;
                return;
            }

            this.createNewScenarioAction({
                networkId: this.networks[0].networkId,
                networkName: this.networks[0].networkName,
                scenarioName: value.name,
                userId: this.userId
            })
            .then(() => this.createScenarioDialogData.showDialog = false);
        }
    }
</script>