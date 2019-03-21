<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="350px">
            <v-card>
                <v-card-title>New Performance Strategy</v-card-title>
                <v-card-text>
                    <v-layout column>
                        <v-select label="Select a Network"
                                  :items="networksSelectItems"
                                  outline
                                  v-model="networksSelectItem"
                                  v-on:change="onSelectNetwork">
                        </v-select>
                        <v-select :label="setSimulationsSelectLabel()"
                                  :items="simulationsSelect"
                                  outline
                                  v-model="simulationsSelectItems"
                                  v-on:change="onSelectSimulation"
                                  :disabled="newPerformanceStrategy.networkId === 0">
                        </v-select>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-btn v-on:click="onSubmit(false)" color="info">Save</v-btn>
                    <v-btn v-on:click="onSubmit(true)">Cancel</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {Network} from "../../../shared/models/iAM/network";
    import {Simulation} from "../../../shared/models/iAM/simulation";
    import {PerformanceStrategy, emptyPerformanceStrategy} from "../../../shared/models/iAM/performance";
    import {SelectItem, defaultSelectItem} from "../../../shared/models/vue/select-item";

    @Component
    export default class CreatePerformanceStrategyDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.network.networks) networks: Network[];
        @State(state => state.simulation.simulations) simulations: Simulation[];
        @State(state => state.performanceEditor.performanceStrategies) performanceStrategies: PerformanceStrategy[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getNetworks') getNetworksAction: any;
        @Action('getSimulations') getSimulationsAction: any;

        networksSelectItems: SelectItem[] = [];
        simulationsSelectItems: SelectItem[] = [];
        newPerformanceStrategy = {...emptyPerformanceStrategy};

        /**
         * state.network.networks has changed
         * @param networks List of network objects in state
         */
        @Watch('networks')
        onNetworksChanged(networks: Network[]) {
            // set networksSelectItems using networks
            this.networksSelectItems = networks.map((network: Network) => ({
                text: network.networkName,
                value: network.networkId.toString()
            }));
        }

        /**
         * state.simulation.simulations has changed
         * @param simulations List of simulation objects in state
         */
        @Watch('simulations')
        onSimulationsChanged(simulations: Simulation[]) {
            // set simulationsSelectItems using simulations
            this.simulationsSelectItems = simulations.map((simulation: Simulation) => ({
                text: simulation.simulationName,
                value: simulation.simulationId.toString()
            }));
        }

        /**
         * Component has been mounted
         */
        mounted() {
            // set app as busy and call the server to get all networks
            this.setIsBusyAction({isBusy: true});
            this.getNetworksAction()
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                })
        }

        /**
         * A network has been selected
         * @param value The selected network's id
         */
        onSelectNetwork(value: string) {
            // parse the incoming value as an integer and set it on newPerformanceStrategy.networkId
            this.newPerformanceStrategy.networkId = parseInt(value);
            // set app as busy and call the server to get all simulations for the selected network
            this.setIsBusyAction({isBusy: true});
            this.getSimulationsAction({networkId: this.newPerformanceStrategy.networkId})
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }

        /**
         * A simulation has been selected
         * @param value The selected simulation's id
         */
        onSelectSimulation(value: string) {
            // parse the incoming value as an integer and set it on newPerformanceStrategy.simulationId
            this.newPerformanceStrategy.simulationId = parseInt(value);
        }

        setSimulationsSelectLabel() {
            if (this.newPerformanceStrategy.networkId !== 0) {
                if (this.simulationsSelectItems.length > 0) {
                    return 'Select a Simulation';
                }
                return 'Current Network Simulations Have Investment Strategies';
            }
        }
    }
</script>