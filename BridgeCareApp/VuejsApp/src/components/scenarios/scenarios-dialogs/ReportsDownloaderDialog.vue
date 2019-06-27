<template>
    <v-layout row justify-center>
        <v-dialog v-model="dialogData.showModal" persistent scrollable max-width="600px">
            <v-card>
                <v-card-title primary-title>
                    <v-layout column fill-height>
                        <v-flex>
                            <v-layout justify-center fill-height>
                                <h3 class="grey--text">Available Reports</h3>
                            </v-layout>
                        </v-flex>
                        <v-progress-linear v-if="isBusy" :indeterminate="true"></v-progress-linear>
                        <v-flex>
                            <span v-if="isBusy" class="grey--text">Downloading the reports</span>
                        </v-flex>
                    </v-layout>
                </v-card-title>
                <v-divider></v-divider>
                <v-card-text>
                    <v-list-tile v-for="item in reports"
                                 :key="item"
                                 avatar
                                 :disabled="isBusy">
                        <v-list-tile-content>
                            <v-list-tile-title>{{item}}</v-list-tile-title>
                        </v-list-tile-content>
                        <v-list-tile-action>
                            <v-checkbox :value="item" color="primary lighten-1" v-model="selectedReports"></v-checkbox>
                        </v-list-tile-action>
                    </v-list-tile>
                    <v-alert :value="showError"
                             color="error"
                             icon="warning"
                             outline>{{errorMessage}}</v-alert>
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn color="info lighten-1" :disabled="isBusy" v-on:click="onDownload(true)">
                            Download
                        </v-btn>
                        <v-btn color="error lighten-1" :disabled="isBusy" v-on:click="onDownload(false)">
                            Cancel
                        </v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {ReportsDownloaderDialogData} from '@/shared/models/modals/reports-downloader-dialog-data';
    import FileDownload from 'js-file-download';
    import ReportsService from '@/services/reports.service';
    import {AxiosResponse} from 'axios';
    import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
    import {clone} from 'ramda';

    @Component({
    })
    export default class ReportsDownloaderDialog extends Vue {
        @Prop() dialogData: ReportsDownloaderDialogData;

        @State(state => state.busy.isBusy) isBusy: boolean;

        @Action('setErrorMessage') setErrorMessageAction: any;

        selectedScenarioData: Scenario = clone(emptyScenario);
        reports: string[] = ['Detailed Report', 'Summary Report'];
        selectedReports: string[] = [];
        errorMessage: string = '';
        showError: boolean = false;

        @Watch('dialogData.showModal')
        onshowModalChanged(showModal: boolean) {
            if (showModal) {
                this.errorMessage = '';
                this.showError = false;
                this.selectedScenarioData.networkId = this.dialogData.scenario.networkId;
                this.selectedScenarioData.simulationId = this.dialogData.scenario.simulationId;
            }
            if (!this.isBusy) {
                this.selectedReports = [];
            }

        }

        async onDownload(download: boolean) {
            if (download) {
                if (this.selectedReports.length === 0) {
                    this.errorMessage = 'Please select at least one report to download';
                    this.showError = true;
                } else {
                    for (let report of this.selectedReports) {
                        switch (report) {
                            case 'Detailed Report': {
                                console.log('seriously?');
                                await ReportsService.getDetailedReport(this.selectedScenarioData)
                                    .then((response: AxiosResponse<any>) => {
                                        FileDownload(response.data, 'DetailedReport.xlsx');
                                    });
                                break;
                            }
                            case 'Summary Report': {
                                await ReportsService.getSummaryReport(this.selectedScenarioData)
                                    .then((response: AxiosResponse<any>) => {
                                        FileDownload(response.data, 'SummaryReport.xlsx');
                                    });
                                break;
                            }
                        }
                    }
                }
            } else {
                this.dialogData.showModal = false;
            }
        }
    }
</script>
