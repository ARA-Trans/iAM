<template>
    <v-container fluid grid-list-xl>
        <v-layout wrap align-center>
            <v-flex xs12 sm6 d-flex>
                <v-select :items="networks"
                          label="Select a Network"
                          item-text="networkName"
                          item-value="networkId"
                          v-on:change="onSelectNetwork"
                          outline>
                </v-select>
            </v-flex>
            <v-flex xs12 sm6 d-flex>
                <v-select :items="simulations"
                          label="Select a Simulation"
                          :disabled="networkId === 0"
                          item-text="simulationName"
                          item-value="simulationId"
                          outline
                          v-on:change="onSelectSimulation">
                </v-select>
            </v-flex>
            <v-flex xs4>
                <v-btn color="blue-grey"
                       class="white--text"
                       :disabled="simulationId === 0"
                       v-on:click="onDownloadReport">
                    Download Report
                    <v-icon right dark>cloud_download</v-icon>
                </v-btn>
                <v-btn color="blue-grey"
                       class="white--text"
                       :disabled="simulationId === 0 || isBusy"
                       v-on:click="onRunSimulation">
                    Run Simulation
                    <v-icon right dark>cloud_download</v-icon>
                </v-btn>
            </v-flex>
            <v-flex xs12>
                <AppSpinner/>
            </v-flex>
            <v-flex xs12>
                <AppModalPopup :modalData="warning" @decision="onWarningModalDecision"/>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import axios from 'axios';

    import AppSpinner from '../shared/AppSpinner.vue';
    import {Network} from '@/shared/models/iAM/network';
    import { Simulation } from '@/shared/models/iAM/simulation';
    import AppModalPopup from '../shared/AppModalPopup.vue';
    import {Alert} from '@/shared/models/iAM/alert';
    import { hasValue } from '@/shared/utils/has-value';


    import { statusReference } from '@/firebase';

    axios.defaults.baseURL = process.env.VUE_APP_URL;

    @Component({
        components: {AppSpinner, AppModalPopup}
    })
    export default class DetailedReport extends Vue {
        @State(state => state.busy.isBusy) isBusy: boolean;
        @State(state => state.network.networks) networks: Network[];
        @State(state => state.simulation.simulations) simulations: Simulation[];
        @State(state => state.detailedReport.reportBlob) reportBlob: Blob;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getNetworks') getNetworksAction: any;
        @Action('getSimulations') getSimulationsAction: any;
        @Action('getDetailedReport') getDetailedReportAction: any;
        @Action('clearReportBlob') clearReportBlobAction: any;
        @Action('runSimulation') runSimulationAction: any;

        @Prop({
            default: function () {
                return {showModal: false};
            }
        })
        warning: Alert;
        created() {
            statusReference.on('value', (snapshot: any) => {
                let simulationStatus = [];
                const results = snapshot.val();
                for (let key in results) {
                    simulationStatus.push({
                        id: key,
                        status: results[key].Status
                    });
                }
                console.log(simulationStatus);
            }, (error: any) => {

            });
        }

        networkId: number = 0;
        networkName: string = '';
        simulationId: number = 0;
        simulationName: string = '';

        @Watch('reportBlob')
        onReportBlobChanged(val: Blob) {
            if (hasValue(val)) {
                // The if condition is used to work with IE11, and the else block is for Chrome
                if (navigator.msSaveOrOpenBlob) {
                    navigator.msSaveOrOpenBlob(val, 'DetailedReport.xlsx');
                } else {
                    const url = window.URL.createObjectURL(val);
                    const link = document.createElement('a');
                    link.href = url;
                    link.setAttribute('download', 'DetailedReport.xlsx');
                    document.body.appendChild(link);
                    link.click();
                }
                // clear detailedReport state after report blob has been downloaded
                this.clearReportBlobAction();
            }
        }

        /**
         * Component has been mounted
         */
        mounted() {
            // dispatch action to get networks
            this.setIsBusyAction({isBusy: true});
            this.getNetworksAction().then(() =>
                this.setIsBusyAction({isBusy: false})
            ).catch((error: any) => {
                this.setIsBusyAction({isBusy: false});
                console.log(error);
            });
        }

        /**
         * A network has been selected
         * @param networkId The selected network id
         */
        onSelectNetwork(networkId: number) {
            this.networkId = networkId;
            const selectedNetwork: Network = this.networks.find(t => t.networkId === networkId) as Network;
            this.networkName = hasValue(selectedNetwork) ? selectedNetwork.networkName : '';
            // dispatch action to get simulations
            this.setIsBusyAction({isBusy: true});
            this.getSimulationsAction({networkId: networkId}).then(() =>
                this.setIsBusyAction({isBusy: false})
            ).catch((error: any) => {
                this.setIsBusyAction({isBusy: false});
                console.log(error);
            });
        }

        /**
         * A simulation has been selected
         * @param simulationId The selected simulation id
         */
        onSelectSimulation(simulationId: number) {
            this.simulationId = simulationId;
            const selectedSimulation: Simulation = this.simulations.find((s: Simulation) => s.simulationId == simulationId) as Simulation;
            this.simulationName = hasValue(selectedSimulation) ? selectedSimulation.simulationName : '';
        }

        /**
         * 'Download Report' button has been clicked
         */
        onDownloadReport() {
            // dispatch action to get report data
            this.setIsBusyAction({isBusy: true});
            this.getDetailedReportAction({
                networkId: this.networkId,
                simulationId: this.simulationId
            }).then(() =>
                this.setIsBusyAction({isBusy: false})
            ).catch((error: any) => {
                this.setIsBusyAction({isBusy: false});
                console.log(error);
            });
        }

        /**
         * 'Run Simulation' button has been clicked
         */
        onRunSimulation() {
            this.warning.showModal = true;
            this.warning.heading = 'Warning';
            this.warning.message = 'The simulation can take around five minutes to finish. ' +
                'Are you sure that you want to continue?';
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
                networkId: this.networkId,
                simulationId: this.simulationId,
                networkName: this.networkName,
                simulationName: this.simulationName
            }).then(() =>
                this.setIsBusyAction({isBusy: false})
            ).catch((error: any) => {
                this.setIsBusyAction({ isBusy: false });
                this.setSimulationStatusAction({ status: false })
                console.log(error);
            });
        }
    }
</script>