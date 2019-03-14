<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="350px">
            <v-card>
                <v-card-title>New Investment Strategy</v-card-title>
                <v-card-text>
                    <v-layout column>
                        <v-flex>
                            <v-select label="Select a Network"
                                      :items="networksSelect"
                                      outline
                                      v-model="networksSelectItem"
                                      v-on:change="onSelectNetwork">
                            </v-select>
                        </v-flex>
                        <v-flex>
                            <v-select :label="setSimulationsSelectLabel()"
                                      :items="simulationsSelect"
                                      outline
                                      v-model="simulationsSelectItem"
                                      v-on:change="onSelectSimulation"
                                      :disabled="newInvestmentStrategy.networkId === 0">
                            </v-select>
                        </v-flex>
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
    import {Network} from '@/shared/models/iAM/network';
    import {Simulation} from '@/shared/models/iAM/simulation';
    import {InvestmentStrategy, emptyInvestmentStrategy} from '@/shared/models/iAM/investment';
    import {CreateInvestmentStrategyDialogResult} from '@/shared/models/dialogs/create-investment-strategy-dialog-result';
    import {SelectItem, defaultSelectItem} from '@/shared/models/vue/select-item';
    import moment from 'moment';
    import {hasValue} from '@/shared/utils/has-value';
    import * as R from 'ramda';

    @Component
    export default class CreateInvestmentStrategyDialog extends Vue  {
        @Prop() showDialog: boolean;

        @State(state => state.network.networks) networks: Network[];
        @State(state => state.simulation.simulations) simulations: Simulation[];
        @State(state => state.investmentEditor.investmentStrategies) investmentStrategies: InvestmentStrategy[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getNetworks') getNetworksAction: any;
        @Action('getSimulations') getSimulationsAction: any;

        networksSelect: SelectItem[] = [{...defaultSelectItem}];
        networksSelectItem: SelectItem = {...defaultSelectItem};
        simulationsSelect: SelectItem[] = [{...defaultSelectItem}];
        simulationsSelectItem: SelectItem = {...defaultSelectItem};
        newInvestmentStrategy: InvestmentStrategy = {...emptyInvestmentStrategy};

        /**
         * state.network.networks has changed
         */
        @Watch('networks')
        onNetworksChanged(networks: Network[]) {
            // set networksSelect using networks property
            this.networksSelect = networks.map((network: Network) => ({
                text: network.networkName,
                value: network.networkId.toString()
            }));
        }

        /**
         * state.simulation.simulations has changed
         */
        @Watch('simulations')
        onSimulationsChanged(simulations: Simulation[]) {
            // get all simulations that don't already have an investment strategy
            const simulationsWithoutAnInvestmentStrategy = simulations.filter((simulation: Simulation) =>
                !R.any(R.propEq('simulationId', simulation.simulationId), this.investmentStrategies)
            );
            // set simulationsSelect using simulationsWithoutAnInvestmentStrategy list
            this.simulationsSelect = simulationsWithoutAnInvestmentStrategy.map((simulation: Simulation) => ({
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
                });
        }

        /**
         * A network has been selected
         */
        onSelectNetwork(value: string) {
            // set the networks select item
            this.networksSelectItem = this.networksSelect
                .find((selectItem: SelectItem) => selectItem.value === value) as SelectItem;
            // parse the incoming value as an integer
            this.newInvestmentStrategy.networkId = parseInt(value);
            // set app as busy and call the server to get all simulations for the selected network
            this.setIsBusyAction({isBusy: true});
            this.getSimulationsAction({networkId: this.newInvestmentStrategy.networkId})
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }

        onSelectSimulation(value: string) {
            // set the simulations select item
            this.simulationsSelectItem = this.simulationsSelect
                .find((selectItem: SelectItem) => selectItem.value === value) as SelectItem;
            // parse the incoming value as an integer
            this.newInvestmentStrategy.simulationId = parseInt(value);
        }

        /**
         * Sets the label for the simulations select element
         */
        setSimulationsSelectLabel() {
            if (this.newInvestmentStrategy.networkId !== 0) {
                if (this.simulationsSelect.length > 0) {
                    return 'Select a Simulation';
                }
                return 'Current Network Simulations Have Investment Strategies';
            }
            return 'Waiting on user to select network...';
        }

        /**
         * 'Save'/'Cancel' button has been clicked
         * @param isCanceled Whether or not the dialog 'Cancel' button was clicked
         */
        onSubmit(isCanceled: boolean) {
            // get the selected simulation
            const selectedSimulation: Simulation = this.simulations
                .find((simulation: Simulation) =>
                    simulation.simulationId === this.newInvestmentStrategy.simulationId
                ) as Simulation;
            // set new investment strategy name and a default budget year
            this.newInvestmentStrategy.name = hasValue(selectedSimulation) ? selectedSimulation.simulationName : '';
            this.newInvestmentStrategy.budgetYears = [{
                year: moment().year(),
                budgets: []
            }];
            // create dialog result
            const result: CreateInvestmentStrategyDialogResult = {
                canceled: isCanceled,
                newInvestmentStrategy: {...this.newInvestmentStrategy}
            };
            // reset the dialog properties
            this.resetDialogProperties();
            // emit dialog result
            this.$emit('result', result);
        }

        /**
         * Resets this dialog's properties
         */
        resetDialogProperties() {
            this.networksSelectItem = {...defaultSelectItem};
            this.simulationsSelectItem = {...defaultSelectItem};
            this.newInvestmentStrategy = {...emptyInvestmentStrategy};
        }
    }
</script>
