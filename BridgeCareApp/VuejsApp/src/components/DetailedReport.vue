<template>
    <v-container fluid grid-list-xl>
        <v-layout wrap align-center>
            <v-flex xs12 sm6 d-flex>
                <v-select :items="networks"
                          label="Select a Network"
                          item-text="networkName"
                          item-value="networkId"
                          v-on:change="getNetwork"
                          outline></v-select>
            </v-flex>
            <v-flex xs12 sm6 d-flex>
                <v-select :items="simulations"
                          label="Select a Simulation"
                          :disabled="noNetwork"
                          item-text="simulationName"
                          item-value="simulationId"
                          v-on:change="getSimulation"
                          outline></v-select>
            </v-flex>
            <v-flex xs4>
                    <v-btn color="blue-grey"
                           class="white--text"
                           :disabled="notAllSelected"
                           v-on:click="downloadReport">
                        Download
                        <v-icon right dark>cloud_download</v-icon>
                    </v-btn>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';
    import axios from 'axios'

    axios.defaults.baseURL = process.env.VUE_APP_URL

    @Component
    export default class DetailedReport extends Vue {

        networks: any[] = []
        simulations: any[] = []
        networkId: number = 0
        simulationId: number = 0
        notAllSelected: boolean = true
        noNetwork: boolean = true

        created() {
            axios
                .get('/api/Networks')
                .then(response => (response.data as Promise<any[]>))
                .then(data => {
                    for (let i = 0; i < data.length; i++) {
                        this.networks.push(data[i])
                    }
                }, error => {
                    console.log(error)
                });
        }

        getNetwork(id: any) {
            this.networkId = id
            axios
                .get(`/api/Simulations/${id}`)
                .then(response => (response.data as Promise<any[]>))
                .then(data => {
                    for (let i = 0; i < data.length; i++) {
                        this.simulations.push(data[i])
                    }
                    this.noNetwork = false
                }, error => {
                    console.log(error)
                });
        }

        getSimulation(a: any) {
            this.simulationId = a
            this.notAllSelected = false
        }

        downloadReport() {
            console.log("Network Id" + " " + this.networkId)
        }
    }
</script>