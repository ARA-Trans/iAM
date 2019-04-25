<template>
    <v-form>
        <v-container>
            <v-layout row wrap>

                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="'fas fa-edit'"
                                  box
                                  readonly
                                  label="Analysis"
                                  type="text"
                                  @click:append="editAnalysis"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="marker ? 'fas fa-address-book' : 'home'"
                                  box
                                  readonly
                                  label="Scope"
                                  type="text"
                                  @click:append="toggleMarker"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="'fas fa-dollar-sign'"
                                  box
                                  readonly
                                  label="Investment"
                                  type="text"
                                  @click:append="editInvestment"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="'fas fa-chart-line'"
                                  box
                                  readonly
                                  label="Performance"
                                  type="text"
                                  @click:append="editPerformance"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="marker ? 'info' : 'home'"
                                  box
                                  readonly
                                  label="Commited"
                                  type="text"
                                  @click:append="toggleMarker"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="marker ? 'info' : 'home'"
                                  box
                                  readonly
                                  label="Treatments"
                                  type="text"
                                  @click:append="toggleMarker"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="marker ? 'info' : 'home'"
                                  box
                                  readonly
                                  label="Prioritization"
                                  type="text"
                                  @click:append="toggleMarker"></v-text-field>
                </v-flex>

            </v-layout>
        </v-container>
    </v-form>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import { Action, State } from 'vuex-class';
    import { Simulation } from '@/shared/models/iAM/simulation';

    @Component
    export default class EditScenario extends Vue {
        marker: boolean = true;
        message: string = '';

        @State(state => state.breadcrumb.navigation) navigation: any[];
        @Action('setNavigation') setNavigationAction: any;

        currentScenario: Simulation = { networkId: 0, networkName: '', simulationId: 0, simulationName: '' };

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                vm.currentScenario = to.query;

                vm.setNavigationAction([
                    {
                        text: 'Scenario dashboard',
                        to: '/Scenarios/'
                    },
                    {
                        text: 'Scenario editor',
                        to: {
                            path: '/EditScenario/', query: {
                                networkId: to.query.networkId,
                                simulationId: to.query.simulationId,
                                networkName: to.query.networkName,
                                simulationName: to.query.simulationName
                            }
                        }
                    }
                ]);
            });
        }

        created() {
            this.marker = true;
        }

        toggleMarker() {
            this.marker = !this.marker;
        }
        editAnalysis() {
            this.$router.push({
                path: '/EditAnalysis/', query: {
                    networkId: this.currentScenario.networkId.toString(),
                    simulationId: this.currentScenario.simulationId.toString(),
                    networkName: this.currentScenario.networkName,
                    simulationName: this.currentScenario.simulationName
                } });
        }
        editInvestment() {
            this.$router.push({
                path: '/InvestmentEditor/FromScenario/', query: {
                    networkId: this.currentScenario.networkId.toString(),
                    simulationId: this.currentScenario.simulationId.toString(),
                    networkName: this.currentScenario.networkName,
                    simulationName: this.currentScenario.simulationName
                }
            });
        }
        editPerformance() {
            this.$router.push({
                path: '/PerformanceEditor/FromScenario/', query: {
                    networkId: this.currentScenario.networkId.toString(),
                    simulationId: this.currentScenario.simulationId.toString(),
                    networkName: this.currentScenario.networkName,
                    simulationName: this.currentScenario.simulationName
                }
            });
        }
    }
</script>

<style scoped>
</style>