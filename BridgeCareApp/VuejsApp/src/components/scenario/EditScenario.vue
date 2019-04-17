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
    import { Component } from 'vue-property-decorator';
    import { Action, State } from 'vuex-class';

    @Component
    export default class EditScenario extends Vue {
        marker: boolean = true;
        message: string = '';

        @State(state => state.breadcrumb.navigation) navigation: any[];
        @Action('setNavigation') setNavigationAction: any;

        created() {
            this.marker = true;
            this.setNavigationAction([
                {
                    text: 'Scenario dashboard',
                    to: '/Scenarios/'
                },
                {
                    text: 'Scenario editor',
                    to: '/EditScenario/'
                }
            ]);
        }

        toggleMarker() {
            this.marker = !this.marker;
        }
        editAnalysis() {
            this.$router.push('/EditAnalysis/');
        }
        editInvestment() {
            this.$router.push({ path: '/InvestmentEditor/FromScenario/' });
        }
        editPerformance() {
            this.$router.push('/PerformanceEditor/FromScenario/');
        }
    }
</script>

<style scoped>
</style>