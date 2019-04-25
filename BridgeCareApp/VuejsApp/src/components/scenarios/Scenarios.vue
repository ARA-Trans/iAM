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
                                        <v-btn flat icon color="blue" v-on:click="onRunSimulation(props.item)">
                                            <v-icon>fas fa-play</v-icon>
                                        </v-btn>
                                    </v-flex>
                                    <v-flex>
                                        <v-btn flat icon color="info" v-on:click="openReportDialog(props.item)">
                                            <v-icon>fas fa-chart-line</v-icon>
                                        </v-btn>
                                    </v-flex>
                                    <v-flex>
                                        <v-btn flat icon color="green" v-on:click="editScenario(props.item)">
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
                                        <v-btn flat icon color="blue" v-on:click="onRunSimulation(props.item)">
                                            <v-icon>fas fa-play</v-icon>
                                        </v-btn>
                                    </v-flex>
                                    <v-flex>
                                        <v-btn flat icon color="green" v-on:click="openReportDialog(props.item)">
                                            <v-icon>fas fa-chart-line</v-icon>
                                        </v-btn>
                                    </v-flex>
                                    <v-flex>
                                        <v-btn flat icon color="green" v-on:click="editSharedScenario">
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
            <AppModalPopup :modalData="warning" @decision="onWarningModalDecision" />
        </v-flex>
        <v-flex xs12>
            <ReportsDownload :dialogData="reportData" />
        </v-flex>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch, Prop } from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import axios from 'axios';

    import * as moment from 'moment';
    import {Scenario} from '../../shared/models/iAM/scenario';
    import { hasValue } from '../../shared/utils/has-value';
    import { Alert } from '@/shared/models/iAM/alert';
    import AppModalPopup from '@/shared/dialogs/AppModalPopup.vue';
    import ReportsDownload from '@/shared/dialogs/ReportsDownload.vue';
    import { ShowAvailableReports } from '@/shared/models/dialogs/download-reports-dialog';

    axios.defaults.baseURL = process.env.VUE_APP_URL;

    @Component({
        components: { AppModalPopup, ReportsDownload }
    })
    export default class Scenarios extends Vue {
        @State(state => state.busy.isBusy) isBusy: boolean;
        @State(state => state.scenario.scenarios) scenarios: Scenario[];
        @State(state => state.security.userId) userId: string;
        @State(state => state.reports.names) reportNames: string[];
        @State(state => state.breadcrumb.navigation) navigation: any[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getUserScenarios') getUserScenariosAction: any;
        @Action('runSimulation') runSimulationAction: any;
        @Action('setNavigation') setNavigationAction: any;

        @Prop({
            default: function () {
                return { showModal: false };
            }
        })
        warning: Alert;

        @Prop({
            default: function () {
                return { showModal: false };
            }
        })
        reportData: ShowAvailableReports;

        scenarioGridHeaders: object[] = [
            {text: 'Scenario Name', align: 'left', sortable: false, value: 'name'},
            {text: 'Date Created', sortable: false, value: 'createdDate'},
            { text: 'Date Last Modified', sortable: false, value: 'lastModifiedDate' },
            { text: 'Status', sortable: false, value: 'status' },
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
        currentItem: Scenario;

        @Watch('scenarios')
        onScenariosChanged(val: Scenario[]) {
            if (hasValue(val)) {
                // filter scenarios that are the user's
                this.userScenarios = val.filter((s: Scenario) => s.shared === false);
                // filter scenarios that are shared with the user
                this.sharedScenarios = val.filter((s: Scenario) => s.shared === true);
            }

        }

        created() {
            this.setNavigationAction([]);
        }

        /**
         * Component has been mounted
         */
        mounted() {
            this.setIsBusyAction({isBusy: true});
            this.getUserScenariosAction({ userId: this.userId })
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });

        }

        //TODO: need to replace this with something that will actually get the status of scenario from server
        getStatus(isCompleted: boolean) {
            return isCompleted ? 'Completed' : 'Running';
        }

        /**
         * Formats a date as month/day/year
         * @param unformattedDate Unformatted date
         */
        formatDate(unformattedDate: Date) {
            //@ts-ignore
            return moment(unformattedDate).format('M/D/YYYY');
        }

        editScenario(item: any) {
            this.$router.push({
                path: '/EditScenario/', query: {
                    simulationId: item.simulationId
                }
            });
        }
        editSharedScenario() {
            this.$router.push({ path: '/EditScenario/' });
        }

        /**
         * 'Run Simulation' button has been clicked
         */
        onRunSimulation(item: any) {
            this.warning.showModal = true;
            this.warning.heading = 'Warning';
            this.warning.choice = true;
            this.warning.message = 'The simulation can take around five minutes to finish. ' +
                'Are you sure that you want to continue?';
            this.currentItem = item;
        }

        /**
         * A 'warning' modal decision has been made by the user
         * @param value The user decision
         */
        onWarningModalDecision(value: boolean) {
            this.warning.showModal = false;
            if (value == true) {
                this.runSimulation();
            }
        }

        /**
         * User has chosen to run a simulation
         */
        runSimulation() {
            // dispatch action to run simulation
            this.runSimulationAction({
                networkId: this.currentItem.networkId,
                simulationId: this.currentItem.simulationId,
                networkName: this.currentItem.networkName,
                simulationName: this.currentItem.simulationName,
                userId: this.userId
            }).catch((error: any) => {
                console.log(error);
            });
        }

        openReportDialog(item: any) {
            this.reportData.showModal = true;
            this.reportData.names = this.reportNames;
            this.reportData.networkId = item.networkId;
            this.reportData.networkName = item.networkName;
            this.reportData.simulationId = item.simulationId;
            this.reportData.simulationName = item.simulationName;
        }
    }
</script>