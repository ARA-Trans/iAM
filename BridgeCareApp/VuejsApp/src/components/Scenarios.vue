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
                            <td>{{props.item.name}}</td>
                            <td>{{formatDate(props.item.createdDate)}}</td>
                            <td>{{formatDate(props.item.lastModifiedDate)}}</td>
                            <td>{{getStatus(props.item.status)}}</td>
                            <td>
                                <v-layout row wrap>
                                    <v-flex>
                                        <v-btn flat icon color="green" v-on:click="editScenario">
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
    </v-container>
</template>

<script lang="ts">
    import Vue from "vue";
    import {Component, Watch} from "vue-property-decorator";
    import {Action, State} from "vuex-class";
    import axios from "axios";

    import AppSpinner from "../shared/AppSpinner.vue";
    import * as moment from "moment";
    import {Scenario} from "@/models/scenario";
    import {hasValue} from "@/shared/utils/has-value";

    axios.defaults.baseURL = process.env.VUE_APP_URL;

    @Component({
        components: {AppSpinner}
    })
    export default class Scenarios extends Vue {
        @State(state => state.busy.isBusy) isBusy: boolean;
        @State(state => state.scenario.scenarios) scenarios: Scenario[];

        @Action("setIsBusy") setIsBusyAction: any;
        @Action("getUserScenarios") getUserScenariosAction: any;

        scenarioGridHeaders: object[] = [
            {text: "Scenario Name", align: "left", sortable: false, value: "name"},
            {text: "Date Created", sortable: false, value: "createdDate"},
            {text: "Date Last Modified", sortable: false, value: "lastModifiedDate"},
            {text: "Status", sortable: false, value: "status"},
            {text: "", sortable: false, value: "actions"}
        ];
        userScenarios: Scenario[] = [];
        sharedScenarios: Scenario[] = [];
        searchMine = "";
        searchShared = "";

        @Watch("scenarios")
        onScenariosChanged(val: Scenario[]) {
            if (hasValue(val)) {
                // filter scenarios that are the user's
                this.userScenarios = val.filter((s: Scenario) => s.shared === false);
                // filter scenarios that are shared with the user
                this.sharedScenarios = val.filter((s: Scenario) => s.shared === true);
            }

        }

        /**
         * Component has been mounted
         */
        mounted() {
            this.setIsBusyAction({isBusy: true});
            this.getUserScenariosAction({userId: 0})
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });

        }

        //TODO: need to replace this with something that will actually get the status of scenario from server
        getStatus(isCompleted: boolean) {
            return isCompleted ? "Completed" : "Running";
        }

        /**
         * Formats a date as month/day/year
         * @param unformattedDate Unformatted date
         */
        formatDate(unformattedDate: Date) {
            //@ts-ignore
            return moment(unformattedDate).format("M/D/YYYY");
        }

        editScenario() {
            this.$router.push({path: 'EditScenario'});
        }
        editSharedScenario() {
            this.$router.push({ path: 'EditScenario' });
        }
    }
</script>