<template>
    <v-container class="grey lighten-4">
        <v-layout row wrap>
            <v-flex xs4></v-flex>
            <v-flex xs4>
                <v-card>
                    <v-btn
                           color="blue-grey"
                           class="white--text">
                        Download
                        <v-icon right dark>cloud_download</v-icon>
                    </v-btn>
                </v-card>
            </v-flex>
            <v-flex xs4></v-flex>
        </v-layout>
        <v-card>
            <v-card-title>
                First Report
                <v-spacer></v-spacer>
                <v-text-field v-model="search"
                              append-icon="search"
                              label="Search"
                              single-line
                              hide-details></v-text-field>
            </v-card-title>
            <v-data-table :headers="headers"
                          :items="reportData"
                          :search="search">
                <template slot="items" slot-scope="props" class="align: left">
                    <td>{{ props.item.treatment }}</td>
                    <td class="text-xs-left">{{ props.item.budget }}</td>
                    <td class="text-xs-left">{{ props.item.cosT_ }}</td>
                    <td class="text-xs-left">{{ props.item.remaininG_LIFE }}</td>
                    <td class="text-xs-left">{{ props.item.benefit }}</td>
                    <td class="text-xs-left">{{ props.item.area }}</td>
                </template>
                <v-alert slot="no-results" :value="true" color="error" icon="warning">
                    Your search for "{{ search }}" found no results.
                </v-alert>
            </v-data-table>
        </v-card>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';

    import axios from 'axios'

    axios.defaults.baseURL = process.env.VUE_APP_URL

    interface Data {
        treatment: string;
        budget: string;
        cosT_: string;
        remaininG_LIFE: string;
        benefit: string;
        area: string;
    }

    @Component
    export default class ReportData extends Vue {

        reportData: any[] = [];

        data() {
            return {
                search: '',
                headers: [{
                    text: 'Treatment',
                    align: 'left',
                    sortable: false,
                    value: 'treatment'
                },
                    { text: 'Budget', align: 'left', value: 'budget' },
                    { text: 'Cost', align: 'left', value: 'cosT_' },
                    { text: 'Remaining Life', align: 'left', value: 'remaininG_LIFE' },
                    { text: 'Benefit', align: 'left', value: 'benefit' },
                    { text: 'Area', align: 'left', value: 'area' }
                ]
            }
        }

        mounted() {
            axios
                .get('/api/REPORT_13_9')
                .then(response => (response.data as Promise<any[]>))
                .then(data => {
                    this.reportData = data
                }, error => {
                    console.log(error);
                });
        }

    }
</script>