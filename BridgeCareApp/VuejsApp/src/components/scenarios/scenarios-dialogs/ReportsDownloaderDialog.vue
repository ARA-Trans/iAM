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
                        <v-progress-linear v-if="isReportDownloading" :indeterminate="true"></v-progress-linear>
                        <v-flex>
                            <span v-if="isReportDownloading" class="grey--text">Downloading the reports</span>
                        </v-flex>
                    </v-layout>
                </v-card-title>
                <v-divider></v-divider>
                <v-card-text>
                    <v-list-tile v-for="item in dialogData.names"
                                 :key="item"
                                 avatar
                                 :disabled="isReportDownloading">
                        <v-list-tile-content>
                            <v-list-tile-title>{{ item }}</v-list-tile-title>
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
                        <v-btn color="error lighten-1" :disabled="isReportDownloading" v-on:click="onCancel">
                            Cancel
                        </v-btn>
                        <v-btn color="info lighten-1" :disabled="isReportDownloading" v-on:click="onSubmit">
                            Download
                        </v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop, Watch } from 'vue-property-decorator';
    import { State, Action } from 'vuex-class';
    import { hasValue } from '@/shared/utils/has-value-util';
    import { ReportsDownloaderDialogData } from '@/shared/models/modals/reports-downloader-dialog-data';

    @Component({
    })
    export default class ReportsDownloaderDialog extends Vue {
        @Prop() dialogData: ReportsDownloaderDialogData;

        @State(state => state.reports.reportsBlob) reportsBlob: Blob;
        @State(state => state.reports.currentReportName) currentReportName: string;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getReports') getReportsAction: any;
        @Action('clearReportsBlob') clearReportsBlobAction: any;
        selectedReports: string[] = [];
        errorMessage: string = '';
        showError: boolean = false;
        isReportDownloading: boolean = false;

        @Watch('dialogData.showModal')
        onshowModalChanged(showModal: boolean) {
            if (showModal === false) {
                this.errorMessage = '';
                this.showError = false;
            }
            if (this.isReportDownloading === false) {
                this.selectedReports = [];
            }

        }

        @Watch('reportsBlob')
        onReportsBlobChanged(val: Blob) {
            if (hasValue(val)) {
                // The if condition is used to work with IE11, and the else block is for Chrome
                if (navigator.msSaveOrOpenBlob) {
                    navigator.msSaveOrOpenBlob(val, this.currentReportName);
                } else {
                    const url = window.URL.createObjectURL(val);
                    const link = document.createElement('a');
                    link.href = url;
                    link.setAttribute('download', this.currentReportName);
                    document.body.appendChild(link);
                    link.click();
                }
                // clear detailedReport state after report blob has been downloaded
                this.clearReportsBlobAction();
            }
        }

        async onSubmit() {
            if (this.selectedReports.length === 0) {
                this.errorMessage = 'Please select at least one report to download';
                this.showError = true;
                return;
            }
            for (let report in this.selectedReports) {
                // dispatch action to get report data
                this.isReportDownloading = true;
                await this.getReportsAction({
                    reportName: this.selectedReports[report],
                    networkId: this.dialogData.networkId,
                    simulationId: this.dialogData.simulationId,
                    networkName: this.dialogData.networkName,
                    simulationName: this.dialogData.simulationName
                }).then(() => {
                    this.isReportDownloading = false;
                }).catch((error: any) => {
                    this.isReportDownloading = false;
                    console.log(error);
                });
            }
        }

        onCancel() {
            this.dialogData.showModal = false;
        }
    }
</script>

<style>
</style>