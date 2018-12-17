<template>
    <v-container class="grey lighten-4">
        <v-layout row wrap>
            <v-flex xs4></v-flex>
            <v-flex xs4>
                <v-card>
                    <v-btn :loading="loading3"
                           :disabled="loading3"
                           color="blue-grey"
                           class="white--text"
                           @click="loader = 'loading3'">
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
                          :items="desserts"
                          :search="search">
                <template slot="items" slot-scope="props" class="align: left">
                    <td>{{ props.item.name }}</td>
                    <td class="text-xs-left">{{ props.item.reportType }}</td>
                    <td class="text-xs-left">{{ props.item.validity }}</td>
                    <td class="text-xs-left">{{ props.item.purpose }}</td>
                    <td class="text-xs-left">{{ props.item.from }}</td>
                    <td class="text-xs-left">{{ props.item.to }}</td>
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

    @Component
    export default class ReportData extends Vue {

        loader: any = null;

        data() {
            return {
                search: '',
                headers: [{
                    text: 'Report name',
                    align: 'left',
                    sortable: false,
                    value: 'name'
                },
                    { text: 'Report type', align: 'left', value: 'type' },
                    { text: 'Validity', align: 'left', value: 'valid' },
                    { text: 'Purpose', align: 'left', value: 'purpose' },
                    { text: 'From', align: 'left', value: 'from' },
                    { text: 'To', align: 'left', value: 'to' }
                ],
                desserts: [
                    {
                        value: false,
                        name: 'First report',
                        reportType: 159,
                        validity: 'true',
                        purpose: 30,
                        from: 'Saransh',
                        to: 'Some person'
                    },
                    {
                        value: false,
                        name: 'Second report',
                        reportType: 237,
                        validity: 'true',
                        purpose: 37,
                        from: 'Brendon',
                        to: 'Some person'
                    },
                    {
                        value: false,
                        name: 'Third report',
                        reportType: 262,
                        validity: 'true',
                        purpose: 23,
                        from: 'Chad',
                        to: 'Some person'
                    },
                    {
                        value: false,
                        name: 'Fourth report',
                        reportType: 305,
                        validity: 'true',
                        purpose: 67,
                        from: 'William',
                        to: 'Some person'
                    },
                    {
                        value: false,
                        name: 'Fifth report',
                        reportType: 356,
                        validity: 'true',
                        purpose: 49,
                        from: 'Jake',
                        to: 'Some person'
                    },
                    {
                        value: false,
                        name: 'First report',
                        reportType: 375,
                        validity: 'true',
                        purpose: 94,
                        from: 'ARA',
                        to: 'Some person'
                    },
                    {
                        value: false,
                        name: 'Seventh report',
                        reportType: 392,
                        validity: 'true',
                        purpose: 98,
                        from: 'Some person',
                        to: 'Some person'
                    },
                    {
                        value: false,
                        name: 'Eighth report',
                        reportType: 408,
                        validity: 'true',
                        purpose: 87,
                        from: 6.5,
                        to: 'Some person'
                    },
                    {
                        value: false,
                        name: 'Ninth report',
                        reportType: 452,
                        validity: 'true',
                        purpose: 51,
                        from: 4.9,
                        to: 'Some person'
                    },
                    {
                        value: false,
                        name: 'Tenth report',
                        reportType: 518,
                        validity: 'true',
                        purpose: 65,
                        from: 7,
                        to: 'Some person'
                    }
                ]
            }
        }

    }
</script>