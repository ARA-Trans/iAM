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
                <AppModalPopup :warningData="warning" @decision="onModalClicked"/>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop } from 'vue-property-decorator';
    import axios from 'axios'

    //@ts-ignore
    import AppSpinner from '../shared/AppSpinner'
    import Network from '../models/Network'
    import Simulation from '../models/Simulation'
    //@ts-ignore
    import AppModalPopup from '../shared/AppModalPopup'
    //@ts-ignore
    import IWarning from '@/models/IWarning'

    axios.defaults.baseURL = process.env.VUE_APP_URL

    @Component({
        components: { AppSpinner, AppModalPopup }
    })
    export default class DetailedReport extends Vue {

        networks: Network[] = []
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
        warning: IWarning

        mounted() {
            this.downloadProgress = true
            this.loading = true
            axios
                .get('/api/Networks')
                .then(response => (response.data as Promise<Network[]>))
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

        getNetworkId(id: number) {
            this.networkId = id
            let details = this.networks.find(t => t.networkId === id) as Network
            this.networkName = details.networkName
            axios
                .get(`/api/Simulations/${id}`)
                .then(response => (response.data as Promise<Simulation[]>))
                .then(data => {
                    for (let i = 0; i < data.length; i++) {
                        this.simulations.push(data[i])
                    }
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