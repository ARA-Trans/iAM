<template>
    <v-layout column>
        <v-flex xs12>
            <v-card elevation=5>
                <v-flex xs10>
                    <v-layout>
                        <div>
                            <v-data-table :headers="rollupGridHeader"
                                          :items="adminRollup"
                                          :items-per-page="5"
                                          class="elevation-1"
                                          hide-actions>
                                <template slot="items" slot-scope="props">
                                    <td>{{ props.item.networkName }}</td>
                                    <td>{{ props.item.createdDate }}</td>
                                    <td>{{ props.item.lastModifiedDate }}</td>
                                    <td>{{ props.item.rollupStatus }}</td>
                                    <td>
                                        <v-layout row wrap>
                                            <v-flex>
                                                <v-btn @click="onShowRunRollupAlert(props.item)" class="green--text darken-2"
                                                       icon>
                                                    <v-icon>fas fa-play</v-icon>
                                                </v-btn>
                                            </v-flex>
                                        </v-layout>
                                    </td>
                                </template>
                            </v-data-table>
                        </div>
                        <div class="pad-button">
                            <v-btn @click="onLoadNetworks()" color="green darken-2 white--text" round>Load networks
                            </v-btn>
                        </div>
                    </v-layout>
                </v-flex>
            </v-card>
        </v-flex>
            <v-flex x12>
            <v-card elevation=5 color="blue lighten-5">
                <v-card-title>
                    <v-flex xs4>
                        <v-chip color="ara-blue-bg" text-color="white">
                            My Scenarios
                            <v-icon right>star</v-icon>
                        </v-chip>
                    </v-flex>
                    <v-flex xs6>
                        <v-text-field append-icon="fas fa-search" hide-details lablel="Search" single-line
                                      v-model="searchMine">
                        </v-text-field>
                    </v-flex>
                    <v-flex xs2>
                        <v-btn @click="onUpdateScenarioList()" class="ara-blue-bg white--text" round>
                            Load legacy scenarios
                        </v-btn>
                    </v-flex>
                </v-card-title>
                <v-data-table :headers="scenarioGridHeaders" :items="userScenarios" :search="searchMine">
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-edit-dialog :return-value.sync="props.item.simulationName"
                                           @save="onEditScenarioName(props.item.simulationName, props.item.id, props.item.simulationId)" large lazy
                                           persistent>
                                {{props.item.simulationName}}
                                <template slot="input">
                                    <v-text-field label="Edit"
                                                  single-line
                                                  v-model="props.item.simulationName"></v-text-field>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>{{props.item.creator ? props.item.creator : '[ Unknown ]'}}</td>
                        <td>{{props.item.owner ? props.item.owner : '[ No Owner ]'}}</td>
                        <td>{{formatDate(props.item.createdDate)}}</td>
                        <td>{{formatDate(props.item.lastModifiedDate)}}</td>
                        <td>{{formatDate(props.item.lastRun)}}</td>
                        <td>{{props.item.status}}</td>
                        <td>
                            <v-layout nowrap row>
                                <v-flex>
                                    <v-btn @click="onShowRunSimulationAlert(props.item)" class="ara-blue" icon
                                           title="Run Analysis">
                                        <v-icon>fas fa-play</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onShowReportsDownloaderDialog(props.item)" class="ara-blue" icon
                                           title="Reports">
                                        <v-icon>fas fa-chart-line</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onEditScenario(props.item.simulationId, props.item.simulationName, props.item.id)" class="edit-icon"
                                           icon
                                           title="Settings">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onShareScenario(props.item)" class="ara-blue"
                                           icon
                                           title="Share">
                                        <v-icon>fas fa-users</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onCloneScenario(props.item.simulationId)" class="ara-blue"
                                           icon
                                           title="Clone">
                                        <v-icon>fas fa-paste</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onDeleteScenario(props.item.simulationId, props.item.id)" class="ara-orange"
                                           icon
                                           title="Delete">
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </v-flex>
                            </v-layout>
                        </td>
                    </template>
                    <v-alert :value="true" class="ara-orange-bg" icon="fas fa-exclamation" slot="no-results">
                        Your search for "{{searchMine}}" found no results.
                    </v-alert>
                </v-data-table>
                <v-card-actions color="white">
                    <div style="width:2em"/>
                    <v-btn @click="onCreateScenario" color="green darken-2 white--text">Create new scenario</v-btn>
                </v-card-actions>
            </v-card>
        </v-flex>

        <v-flex xs12>
            <v-card elevation=5 color="blue lighten-3">
                <v-card-title>
                    <v-flex xs4>
                        <v-chip class="ara-blue-bg white--text">
                            Shared with Me
                            <v-icon right>share</v-icon>
                        </v-chip>
                    </v-flex>
                    <v-spacer/>
                    <v-flex xs6>
                        <v-text-field append-icon="fas fa-search" hide-details lablel="Search" single-line
                                      v-model="searchShared">
                        </v-text-field>
                    </v-flex>
                </v-card-title>
                <v-data-table :headers="scenarioGridHeaders" :items="sharedScenarios" :search="searchShared">
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-edit-dialog :return-value.sync="props.item.simulationName"
                                           @save="onEditScenarioName(props.item.simulationName, props.item.id, props.item.simulationId)" large lazy
                                           persistent>
                                {{props.item.simulationName}}
                                <template slot="input">
                                    <v-text-field label="Edit"
                                                  single-line
                                                  v-model="props.item.simulationName"></v-text-field>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>{{props.item.creator ? props.item.creator : '[ Unknown ]'}}</td>
                        <td>{{props.item.owner ? props.item.owner : '[ No Owner ]'}}</td>
                        <td>{{formatDate(props.item.createdDate)}}</td>
                        <td>{{formatDate(props.item.lastModifiedDate)}}</td>
                        <td>{{formatDate(props.item.lastRun)}}</td>
                        <td>{{props.item.status}}</td>
                        <td>
                            <v-layout nowrap row>
                                <v-flex>
                                    <v-btn @click="onShowRunSimulationAlert(props.item)" class="ara-blue" flat icon
                                           title="Run Analysis">
                                        <v-icon>fas fa-play</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onShowReportsDownloaderDialog(props.item)" class="ara-blue" flat icon
                                           title="Reports">
                                        <v-icon>fas fa-chart-line</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onEditScenario(props.item.simulationId, props.item.simulationName, props.item.id)" class="edit-icon" flat
                                           icon
                                           title="Settings">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onShareScenario(props.item)" class="ara-blue" flat icon
                                           title="Share">
                                        <v-icon>fas fa-users</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onCloneScenario(props.item.simulationId)" class="ara-blue"
                                           icon
                                           title="Clone">
                                        <v-icon>fas fa-paste</v-icon>
                                    </v-btn>
                                </v-flex>
                                <v-flex>
                                    <v-btn @click="onDeleteScenario(props.item.simulationId, props.item.id)" class="ara-orange"
                                           icon
                                           title="Delete">
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </v-flex>
                            </v-layout>
                        </td>
                    </template>
                    <v-alert :value="true" class="ara-orange-bg" icon="fas fa-exclamation" slot="no-results">
                        Your search for "{{searchShared}}" found no results.
                    </v-alert>
                </v-data-table>
            </v-card>
        </v-flex>

        <Alert :dialogData="alertData" @submit="onSubmitAlertResult"/>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitResponse"/>
        <Alert :dialogData="alertBeforeRunRollup" @submit="onSubmitRollupDecision"/>

        <CreateScenarioDialog :showDialog="showCreateScenarioDialog" @submit="onSubmitNewScenario"/>

        <ReportsDownloaderDialog :dialogData="reportsDownloaderDialogData"/>

        <ShareScenarioDialog :scenario="sharingScenario" :showDialog="showShareScenarioDialog"
                             @submit="onSubmitShareScenario"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import moment from 'moment';
    import {emptyScenario, Scenario, ScenarioUser} from '@/shared/models/iAM/scenario';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    import ReportsDownloaderDialog from '@/components/scenarios/scenarios-dialogs/ReportsDownloaderDialog.vue';
    import {
        emptyReportsDownloadDialogData,
        ReportsDownloaderDialogData
    } from '@/shared/models/modals/reports-downloader-dialog-data';
    import {ScenarioCreationData} from '@/shared/models/modals/scenario-creation-data';
    import CreateScenarioDialog from '@/components/scenarios/scenarios-dialogs/CreateScenarioDialog.vue';
    import ShareScenarioDialog from '@/components/scenarios/scenarios-dialogs/ShareScenarioDialog.vue';
    import {Network} from '@/shared/models/iAM/network';
    import {any, clone} from 'ramda';
    import {Simulation} from '@/shared/models/iAM/simulation';
    import {emptyRollup, Rollup} from '@/shared/models/iAM/rollup';
    import {getUserName} from '@/shared/utils/get-user-info';

    @Component({
        components: {Alert, ReportsDownloaderDialog, CreateScenarioDialog, ShareScenarioDialog}
    })
    export default class Scenarios extends Vue {
        @State(state => state.scenario.scenarios) scenarios: Scenario[];
        @State(state => state.authentication.userId) userId: string;
        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.network.networks) networks: Network[];
        @State(state => state.authentication.authenticated) authenticated: boolean;
        @State(state => state.rollup.rollups) rollups: Rollup[];
        @State(state => state.authentication.isAdmin) isAdmin: boolean;
        @State(state => state.authentication.isCWOPA) isCWOPA: boolean;

        @Action('getMongoScenarios') getMongoScenariosAction: any;
        @Action('getLegacyScenarios') getLegacyScenariosAction: any;
        @Action('runSimulation') runSimulationAction: any;
        @Action('createScenario') createScenarioAction: any;
        @Action('deleteScenario') deleteScenarioAction: any;
        @Action('updateScenario') updateScenarioAction: any;
        @Action('updateScenarioUsers') updateScenarioUsersAction: any;
        @Action('getSummaryReportMissingAttributes') getSummaryReportMissingAttributesAction: any;
        @Action('getMongoRollups') getMongoRollupsAction: any;
        @Action('rollupNetwork') rollupNetworkAction: any;
        @Action('getLegacyNetworks') getLegacyNetworksAction: any;
        @Action('cloneScenario') cloneScenarioAction: any;

        alertData: AlertData = clone(emptyAlertData);
        alertBeforeDelete: AlertData = clone(emptyAlertData);
        alertBeforeRunRollup: AlertData = clone(emptyAlertData);
        reportsDownloaderDialogData: ReportsDownloaderDialogData = clone(emptyReportsDownloadDialogData);
        showCreateScenarioDialog: boolean = false;
        showShareScenarioDialog: boolean = false;
        scenarioGridHeaders: object[] = [
            {text: 'Scenario Name', align: 'left', sortable: true, value: 'simulationName'},
            {text: 'Creator', sortable: false, value: 'creator'},
            {text: 'Owner', sortable: false, value: 'owner'},
            {text: 'Date Created', sortable: true, value: 'createdDate'},
            {text: 'Date Last Modified', sortable: true, value: 'lastModifiedDate'},
            {text: 'Date Last Run', sortable: true, value: 'lastRun'},
            {text: 'Status', sortable: false, value: 'status'},
            {text: '', sortable: false, value: 'actions'}
        ];
        rollupGridHeader: object[] = [
            {text: 'Network name', align: 'left', sortable: false, value: 'rollupName'},
            {text: 'Date Created', sortable: false, value: 'createdDate'},
            {text: 'Date Last Modified', sortable: false, value: 'lastModifiedDate'},
            {text: 'Status', sortable: false, value: 'rollupStatus'},
            {text: '', sortable: false, value: 'actions'}
        ];
        userScenarios: Scenario[] = [];
        adminRollup: any[] = [];
        sharedScenarios: Scenario[] = [];
        searchMine = '';
        searchShared = '';
        networkId: number = 0;
        networkName: string = '';
        simulationId: number = 0;
        simulationName: string = '';
        scenarioId: string = '';
        currentScenario: Scenario = clone(emptyScenario);
        currentRollup: Rollup = clone(emptyRollup);
        sharingScenario: Scenario = clone(emptyScenario);

        @Watch('scenarios')
        onScenariosChanged() {
            if (hasValue(this.scenarios)) {
                const username: string = getUserName();
                // filter scenarios that are the user's
                this.userScenarios = this.scenarios.filter((simulation: Scenario) => simulation.owner === username);
                // filter scenarios that are shared with the user
                const scenarioUserMatch = (user: ScenarioUser) =>
                    user.username === username || user.username === null || user.username === undefined;
                const sharedScenarioFilter = (simulation: Scenario) => simulation.owner !== username &&
                    (this.isAdmin || this.isCWOPA || any(scenarioUserMatch, simulation.users));
                this.sharedScenarios = this.scenarios.filter(sharedScenarioFilter);
            } else {
                this.userScenarios = [];
            }

        }

        @Watch('rollups')
        onRollupsChanged() {
            if (hasValue(this.rollups)) {
                this.adminRollup = this.rollups;
            } else {
                this.adminRollup = [];
            }

        }

        @Watch('authenticated')
        onAuthenticated() {
            if (this.authenticated) {
                this.getMongoScenariosAction({userId: this.userId});
                this.getMongoRollupsAction({});
            }
        }

        /**
         * Component has been mounted
         */
        mounted() {
            if (this.authenticated) {
                this.getMongoScenariosAction({userId: this.userId});
                this.getMongoRollupsAction({});
            }
        }

        onUpdateScenarioList() {
            this.getLegacyScenariosAction();
        }

        onLoadNetworks() {
            this.getLegacyNetworksAction({networks: this.adminRollup});
        }

        /**
         * Formats a date as month/day/year
         * @param unformattedDate Unformatted date
         */
        formatDate(unformattedDate: Date) {
            return hasValue(unformattedDate) ? moment(unformattedDate).format('M/D/YYYY') : null;
        }

        /**
         * Navigates user to EditScenario page passing in the simulation id of their scenario
         * @param id Scenario simulation id
         */
        onEditScenario(id: number, simulationName: string, objectIdMongodb: string) {
            this.$router.push({
                path: '/EditScenario/',
                query: {
                    selectedScenarioId: id.toString(),
                    simulationName: simulationName,
                    objectIdMOngoDBForScenario: objectIdMongodb
                }
            });
        }

        onDeleteScenario(simulationId: number, id: string) {

            this.alertBeforeDelete = {
                showDialog: true,
                heading: 'Warning',
                choice: true,
                message: 'Are you sure you want to delete?'
            };

            this.simulationId = simulationId;
            this.scenarioId = id;
        }

        onSubmitResponse(response: boolean) {
            this.alertBeforeDelete = clone(emptyAlertData);

            if (response) {
                this.deleteScenario();
            }
        }

        deleteScenario() {
            this.deleteScenarioAction({
                simulationId: this.simulationId,
                scenarioId: this.scenarioId
            });
        }

        onCloneScenario(scenarioId: number) {
            this.cloneScenarioAction({scenarioId});
        }

        /**
         * Navigates user to EditScenario page passing in the simulation id of a shared scenario
         * @param id Scenario simulation id
         */
        onEditSharedScenario(id: number, simulationName: string) {
            this.$router.push({
                path: '/EditScenario/', query: {selectedScenarioId: id.toString(), simulationName: simulationName}
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
                message: 'Only one simulation can be run at a time. The model run you are about to queue will be ' +
                    'executed in the order in which it was received.'
            };
        }

        /**
         * Shows the Alert
         */
        onShowRunRollupAlert(rollup: Rollup) {
            this.currentRollup = rollup;
            this.alertBeforeRunRollup = {
                showDialog: true,
                heading: 'Warning',
                choice: true,
                message: 'The rollup can take around five minutes to finish. ' +
                    'Are you sure that you want to continue?'
            };
        }

        onSubmitRollupDecision(response: boolean) {
            this.alertBeforeRunRollup = clone(emptyAlertData);

            if (response) {
                this.rollupNetwork();
            }
        }

        /**
         * Takes in a boolean parameter from the AppPopupModal to determine if a scenario's simulation should be executed
         * @param runScenarioSimulation Alert result
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
                selectedScenario: this.currentScenario,
                userId: this.userId
            });
        }

        rollupNetwork() {
            this.rollupNetworkAction({
                selectedNetwork: this.currentRollup
            });
        }

        /**
         * Shows the ReportsDownloaderDialog passing in the specified scenario's data
         * @param scenario Scenario object to use for setting the ReportsDownloaderDialogData object
         */
        onShowReportsDownloaderDialog(scenario: Scenario) {
            this.getSummaryReportMissingAttributesAction({
                    selectedScenarioId: scenario.simulationId, selectedNetworkId: this.networks[0].networkId
                }
            ).then(() => {
                setTimeout(() => {
                    this.reportsDownloaderDialogData = {
                        showModal: true,
                        scenario: scenario
                    };
                });
            });
        }

        onCreateScenario() {
            this.showCreateScenarioDialog = true;
        }

        onShareScenario(scenario: Scenario) {
            this.showShareScenarioDialog = true;
            this.sharingScenario = scenario;
        }

        onEditScenarioName(scenarioName: string, id: string, simulationId: any) {
            var scenarioData: Simulation = {
                simulationId: simulationId,
                simulationName: scenarioName,
                networkId: this.networks[0].networkId,
                networkName: this.networks[0].networkName
            };
            this.updateScenarioAction({
                updateScenarioData: scenarioData,
                scenarioId: id
            });
        }

        onSubmitNewScenario(createScenarioData: ScenarioCreationData) {
            this.showCreateScenarioDialog = false;

            if (hasValue(createScenarioData)) {
                this.createScenarioAction({
                    createScenarioData: {...createScenarioData, networkId: this.networks[0].networkId},
                    userId: this.userId
                });
            }
        }

        onSubmitShareScenario(scenarioUsers: ScenarioUser[]) {
            this.showShareScenarioDialog = false;

            if (scenarioUsers !== null) {
                this.sharingScenario.users = scenarioUsers;

                this.updateScenarioUsersAction({
                    scenario: this.sharingScenario
                });
            }

            this.sharingScenario = clone(emptyScenario);
        }
    }
</script>

<style>
    .pad-button {
        padding-top: 33px;
    }
</style>
