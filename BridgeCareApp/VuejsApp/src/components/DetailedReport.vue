<template>
    <v-container fluid grid-list-xl>
        <v-layout wrap align-center>
            <v-flex xs12 sm6 d-flex>
                <v-select :items="networks"
                          label="Select a Network"
                          item-text="networkName"
                          item-value="networkId"
                          v-on:change="getSimulations"
                          outline>
                </v-select>
            </v-flex>
            <v-flex xs12 sm6 d-flex>
                <v-select :items="simulations"
                          label="Select a Simulation"
                          :disabled="disableSimulationDropDown"
                          item-text="simulationName"
                          item-value="simulationId"
                          v-on:change="getSimulationId"
                          outline></v-select>
            </v-flex>
            <v-flex xs4>
                <v-btn color="blue-grey"
                       class="white--text"
                       :disabled="disableDownloadReport"
                       v-on:click="downloadReport">
                    Download Report
                    <v-icon right dark>cloud_download</v-icon>
                </v-btn>
                <v-btn color="blue-grey"
                       class="white--text"
                       :disabled="disableRunSimulation"
                       v-on:click="fillWarningModal">
                    Run Simulation
                    <v-icon right dark>cloud_download</v-icon>
                </v-btn>
            </v-flex>
            <v-flex xs12 v-if="downloadProgress" v-model="loading">
                <AppSpinner />
            </v-flex>
            <v-flex xs12>
                <AppModalPopup :modalData="warning" @decision="onModalClicked"/>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop } from 'vue-property-decorator';
    import axios from 'axios'

    import AppSpinner from '../shared/AppSpinner.vue'
    import INetwork from '@/models/INetwork'
    import Simulation from '../models/Simulation'
    import AppModalPopup from '../shared/AppModalPopup.vue'
    import { IAlert } from '@/models/IAlert'

    axios.defaults.baseURL = process.env.VUE_APP_URL

    @Component({
        components: { AppSpinner, AppModalPopup }
    })
    export default class DetailedReport extends Vue {

        networks: INetwork[] = []
        simulations: Simulation[] = []
        networkId: number = 0
        networkName: string = ""
        simulationName: string = ""
        simulationId: number = 0
        disableDownloadReport: boolean = true
        disableRunSimulation: boolean = true
        disableSimulationDropDown: boolean = true
        downloadProgress: boolean = false
        loading: boolean = false

        @Prop({ default: function () { return { showModal: false } } })
        warning: IAlert

        mounted() {
            this.downloadProgress = true
            this.loading = true
            axios
                .get('/api/Networks')
                .then(response => (response.data as Promise<INetwork[]>))
                .then(data => {
                    this.$store.dispatch({
                        type: 'networks',
                        data: data
                    })
                    this.networks = this.$store.getters.networks as INetwork[]
                    this.downloadProgress = false
                    this.loading = false
                }, error => {
                    this.downloadProgress = false
                    this.loading = false
                    console.log(error)
                });
        }

        getSimulations(id: number) {
            this.networkId = id
            let details = this.networks.find(t => t.networkId === id) as INetwork
            this.networkName = details.networkName
            axios
                .get(`/api/Simulations/${id}`)
                .then(response => (response.data as Promise<Simulation[]>))
                .then(data => {
                    this.$store.dispatch({
                        type: 'simulations',
                        data: data
                    })
                    this.simulations = this.$store.getters.simulations as Simulation[]
                    this.disableSimulationDropDown = false
                }, error => {
                    console.log(error)
                });
        }

        getSimulationId(id: any) {
            this.simulationId = id
            let detail = this.simulations.find(_ => _.simulationId == id) as Simulation
            this.simulationName = detail.simulationName
            this.disableDownloadReport = false
            this.disableRunSimulation = false
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
                    // The if condition is used to work with IE11, and the else block is for Chrome
                    if (navigator.msSaveOrOpenBlob) {
                        navigator.msSaveOrOpenBlob(new Blob([response.data]), 'DetailedReport.xlsx')
                    }
                    else {
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

        fillWarningModal() {
            this.warning.showModal = true
            this.warning.heading = 'Warning'
            this.warning.message = 'The simulation can take around one and an half hours to finish. Are you sure that you want to continue?'
        }

        onModalClicked(value: boolean) {
            this.warning.showModal = false
            if (value == true) {
                this.runSimulation()
            }
        }

        runSimulation() {
            this.disableRunSimulation = true
            this.downloadProgress = true
            this.loading = true
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
                this.disableRunSimulation = false
                this.downloadProgress = false
                this.loading = false
                console.log(response.data)
            })
                .catch(error => {
                    this.disableRunSimulation = false
                    this.downloadProgress = false
                    this.loading = false
                    console.log(error)
                })
        }
    }
</script>