﻿<template>
    <v-dialog max-width="500px" persistent scrollable v-model="dialogData.showModal">
        <v-card>
            <v-card-title primary-title>
                <v-layout column>
                    <v-flex>
                        <v-layout justify-center><h3 class="grey--text">Available Reports</h3></v-layout>
                    </v-flex>
                    <v-progress-linear :indeterminate="true" v-if="isBusy"></v-progress-linear>
                    <v-flex>
                        <span class="grey--text" v-if="isBusy">Downloading...</span>
                    </v-flex>
                </v-layout>
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
                <v-flex>
                    <v-btn :disabled="showMissingAttributesMessage" @click="generateSummaryReport()"
                           class="green darken-2 white--text">
                        Generate summary report
                        <v-icon right>star</v-icon>
                    </v-btn>
                </v-flex>
                <v-divider></v-divider>
                <v-list-tile :disabled="isBusy" :key="item" avatar v-for="item in reports">
                    <v-layout align-start row v-if="item === 'Summary Report'">
                        <v-flex xs4>
                            <v-checkbox :disabled="showMissingAttributesMessage" :label="item" :value="item" color="primary lighten-1"
                                        v-model="selectedReports">
                            </v-checkbox>
                        </v-flex>
                        <v-flex xs1>
                            <v-menu top v-if="showMissingAttributesMessage">
                                <template slot="activator">
                                    <v-btn class="ara-dark-gray" icon>
                                        <v-icon>fas fa-info-circle</v-icon>
                                    </v-btn>
                                </template>
                                <v-card>
                                    <v-card-text class="missing-attributes-card-text">
                                        <v-list>
                                            <v-subheader>MISSING SCENARIO ATTRIBUTES</v-subheader>
                                            <v-list-tile :key="attribute"
                                                         v-for="attribute in missingSummaryReportAttributes">
                                                <v-list-tile-content>
                                                    <v-list-tile-title>{{attribute}}</v-list-tile-title>
                                                </v-list-tile-content>
                                            </v-list-tile>
                                        </v-list>
                                    </v-card-text>
                                </v-card>
                            </v-menu>
                        </v-flex>
                        <v-spacer></v-spacer>
                    </v-layout>
                    <v-layout align-start row v-else>
                        <v-checkbox :label="item" :value="item" color="primary lighten-1" v-model="selectedReports">
                        </v-checkbox>
                    </v-layout>
                </v-list-tile>
                <v-alert :value="showError" color="error" icon="warning" outline>{{errorMessage}}</v-alert>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="isBusy" @click="onDownload(true)" class="ara-blue-bg white--text">
                        Download
                    </v-btn>
                    <v-btn :disabled="isBusy" @click="onDownload(false)" class="ara-orange-bg white--text">
                        Close
                    </v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import {ReportsDownloaderDialogData} from '@/shared/models/modals/reports-downloader-dialog-data';
    import FileDownload from 'js-file-download';
    import ReportsService from '@/services/reports.service';
    import {AxiosResponse} from 'axios';
    import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
    import {clone} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';

    @Component({})
    export default class ReportsDownloaderDialog extends Vue {
        @Prop() dialogData: ReportsDownloaderDialogData;

        @State(state => state.busy.isBusy) isBusy: boolean;
        @State(state => state.scenario.missingSummaryReportAttributes) missingSummaryReportAttributes: string[];

        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('clearSummaryReportMissingAttributes') clearSummaryReportMissingAttributesAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;

        selectedScenarioData: Scenario = clone(emptyScenario);
        reports: string[] = ['Detailed Report', 'Summary Report'];
        selectedReports: string[] = [];
        errorMessage: string = '';
        showError: boolean = false;
        showMissingAttributesMessage: boolean = false;

        @Watch('dialogData.showModal')
        onshowModalChanged(showModal: boolean) {
            if (showModal) {
                this.errorMessage = '';
                this.showError = false;
                this.selectedScenarioData.networkId = this.dialogData.scenario.networkId;
                this.selectedScenarioData.simulationId = this.dialogData.scenario.simulationId;
                this.selectedScenarioData.simulationName = this.dialogData.scenario.simulationName;
            }
            if (!this.isBusy) {
                this.selectedReports = [];
            }

        }

        @Watch('missingSummaryReportAttributes')
        onMissingSummaryReportAttributesChanged() {
            this.showMissingAttributesMessage = hasValue(this.missingSummaryReportAttributes);
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
                                await ReportsService.getDetailedReport(this.selectedScenarioData)
                                    .then((response: AxiosResponse<any>) => {
                                        FileDownload(response.data, 'DetailedReport.xlsx');
                                    });
                                break;
                            }
                            case 'Summary Report': {
                                await ReportsService.downloadSummaryReport(this.selectedScenarioData)
                                    .then((response: AxiosResponse<any>) => {
                                        if (response == undefined) {
                                            this.setErrorMessageAction({message: 'Summary report does not exists on the target path. Please generate the report before downloading'});
                                        } else {
                                            this.setSuccessMessageAction({message: 'Report has been downloaded'});
                                        }
                                        FileDownload(response.data, 'SummaryReport.xlsx');
                                    });
                                break;
                            }
                        }
                    }
                }
            } else {
                this.clearSummaryReportMissingAttributesAction();
                this.dialogData.showModal = false;
            }
        }

        async generateSummaryReport() {
            await ReportsService.getSummaryReport(this.selectedScenarioData)
                .then((response: AxiosResponse<any>) => {
                    this.setSuccessMessageAction({message: 'Report generation started, please check the dashboard for status update'});
                });
        }
    }
</script>

<style>
    .missing-attributes-card-text {
        max-height: 300px;
        max-width: 300px;
        overflow-y: auto;
    }
</style>
