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
                                        <v-btn flat icon color="green" v-on:click="">
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
                                        <v-btn flat icon color="green" v-on:click="">
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
    import {Component} from "vue-property-decorator";
    import axios from "axios";

    import AppSpinner from "../shared/AppSpinner.vue";
    import * as moment from "moment";
    import {IScenario} from "@/models/scenario";

    axios.defaults.baseURL = process.env.VUE_APP_URL;

    @Component({
        components: {AppSpinner}
    })
    export default class Scenario extends Vue {
        scenarioGridHeaders: object[] = [
            {text: "Scenario Name", align: "left", sortable: false, value: "name"},
            {text: "Date Created", sortable: false, value: "createdDate"},
            {text: "Date Last Modified", sortable: false, value: "lastModifiedDate"},
            {text: "Status", sortable: false, value: "status"},
            {text: "", sortable: false, value: "actions"}
        ];
        userScenarios: IScenario[] = [];
        sharedScenarios: IScenario[] = [];
        searchMine = "";
        searchShared = "";

        /**
         * Vue component before creation
         */
        beforeCreate() {
            // TODO: get user id (from store?), for now just mock user id
            // get the user scenarios
            this.$store.dispatch({
                type: "getUserScenarios",
                userId: 1234
            });
        }

        /**
         * Vue component has been created
         */
        created() {
            // get the scenarios in state
            const scenarios: IScenario[] = this.$store.getters.scenarios;
            // filter scenarios that are the user's
            this.userScenarios = scenarios.filter((s: IScenario) => s.shared === false);
            // filter scenarios that are shared with the user
            this.sharedScenarios = scenarios.filter((s: IScenario) => s.shared === true);
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
    }
</script>