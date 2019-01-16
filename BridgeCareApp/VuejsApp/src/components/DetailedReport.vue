<template>
    <v-container fluid grid-list-xl>
        <v-layout wrap align-center>
            <v-flex xs12 sm6 d-flex>
                <v-select :items="networks"
                          label="Select a Network"
                          item-text="networkName"
                          item-value="networkId"
                          v-on:change="getNetworkId"
                          outline>
                </v-select>
            </v-flex>
            <v-flex xs12 sm6 d-flex>
                <v-select :items="simulations"
                          label="Select a Simulation"
                          :disabled="noNetwork"
                          item-text="simulationName"
                          item-value="simulationId"
                          v-on:change="getSimulationId"
                          outline></v-select>
            </v-flex>
            <v-flex xs4>
                <v-btn color="blue-grey"
                       class="white--text"
                       :disabled="notAllSelected"
                       v-on:click="downloadReport">
                    Download Report
                    <v-icon right dark>cloud_download</v-icon>
                </v-btn>
                <!--<v-btn color="blue-grey"
                       class="white--text"
                       :disabled="notAllSelected"
                       v-on:click="runSimulation">
                    Run Simulation
                    <v-icon right dark>cloud_download</v-icon>
                </v-btn>-->
            </v-flex>
            <v-flex xs12 v-if="downloadProgress" v-model="loading">
                <ShowProgress />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';
    import axios from 'axios'

    //@ts-ignore
    import ShowProgress from './ShowProgress'

    axios.defaults.baseURL = process.env.VUE_APP_URL

    @Component({
        components: { ShowProgress }
    })
    export default class DetailedReport extends Vue {

        networks: any[] = []
        simulations: any[] = []
        networkId: number = 0
        networkName: string = ""
        simulationName: string = ""
        simulationId: number = 0
        notAllSelected: boolean = true
        noNetwork: boolean = true
        downloadProgress: boolean = false
        loading: boolean = false

        created() {
            this.downloadProgress = true
            this.loading = true
            axios
                .get('/api/Networks')
                .then(response => (response.data as Promise<any[]>))
                .then(data => {
                    for (let i = 0; i < data.length; i++) {
                        this.networks.push(data[i])
                    }
                    this.downloadProgress = false
                    this.loading = false
                }, error => {
                    this.downloadProgress = false
                    this.loading = false
                    console.log(error)
                });
        }

        getNetworkId(id: any) {
            this.networkId = id
            this.networkName = this.networks.find(_ => _.networkId == id).networkName
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

        getSimulationId(id: any) {
            this.simulationId = id
            this.simulationName = this.simulations.find(_ => _.simulationId == id).simulationName
            this.notAllSelected = false
        }

        downloadReport() {
            this.downloadProgress = true
            this.loading = true

            axios({
                method: 'post',
                url: '/api/DetailedReport',
                responseType: 'blob',
                data: {
                    NetworkId: this.networkId,
                    SimulationId: this.simulationId
                }
            })
            .then(response => {
                this.downloadProgress = false
                this.loading = false
                if (navigator.msSaveOrOpenBlob) {
                    navigator.msSaveOrOpenBlob(new Blob([response.data]), 'DetailedReport.xlsx')
                }
                else
                {
                    const url = window.URL.createObjectURL(new Blob([response.data]))
                    const link = document.createElement('a')
                    link.href = url
                    link.setAttribute('download', 'DetailedReport.xlsx')
                    document.body.appendChild(link)
                    link.click()
                }
            })
            .catch(error => {
                this.downloadProgress = false
                this.loading = false
                console.log(error)
            })
        }

        runSimulation() {
            axios({
                method: 'post',
                url: '/api/RunSimulation',
                data: {
                    NetworkId: this.networkId,
                    SimulationId: this.simulationId,
                    NetworkName: this.networkName,
                    SimulationName: this.simulationName
                }
            }).then(response => {
                console.log(response.data)
                })
                .catch(error => {
                    console.log(error)
                })
        }
    }
</script>