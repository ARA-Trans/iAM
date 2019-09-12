<template>
    <v-dialog v-model="dialogData.showModal" persistent scrollable max-width="500px">
        <v-card>
            <v-card-title primary-title>
                <v-layout column>
                    <v-flex>
                        <v-layout justify-center><h3 class="grey--text">Available Reports</h3></v-layout>
                    </v-flex>
                    <v-progress-linear v-if="isBusy" :indeterminate="true"></v-progress-linear>
                    <v-flex>
                        <span v-if="isBusy" class="grey--text">Downloading...</span>
                    </v-flex>
                </v-layout>
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
                <v-list-tile v-for="item in reports" :key="item" avatar :disabled="isBusy">
                    <v-layout align-start row v-if="item === 'Summary Report'">
                        <v-flex xs4>
                            <v-checkbox :value="item" :label="item" color="primary lighten-1" v-model="selectedReports"
                                        :disabled="showMissingAttributesMessage">
                            </v-checkbox>
                        </v-flex>
                        <v-flex xs1>
                            <v-menu v-if="showMissingAttributesMessage" top>
                                <template slot="activator">
                                    <v-btn icon class="ara-dark-gray"><v-icon>fas fa-info-circle</v-icon></v-btn>
                                </template>
                                <v-card>
                                    <v-card-text class="missing-attributes-card-text">
                                        <v-list>
                                            <v-subheader>MISSING SCENARIO ATTRIBUTES</v-subheader>
                                            <v-list-tile v-for="attribute in missingSummaryReportAttributes" :key="attribute">
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
                        <v-checkbox :value="item" :label="item" color="primary lighten-1" v-model="selectedReports">
                        </v-checkbox>
                    </v-layout>
                </v-list-tile>
                <v-alert :value="showError" color="error" icon="warning" outline>{{errorMessage}}</v-alert>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" :disabled="isBusy" @click="onDownload(true)">
                        Download
                    </v-btn>
                    <v-btn class="ara-orange-bg white--text" :disabled="isBusy" @click="onDownload(false)">
                        Cancel
                    </v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
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
    import {hasValue} from '@/shared/utils/has-value-util';

    @Component({
    })
    export default class ReportsDownloaderDialog extends Vue {
        @Prop() dialogData: ReportsDownloaderDialogData;

        @State(state => state.busy.isBusy) isBusy: boolean;
        @State(state => state.scenario.missingSummaryReportAttributes) missingSummaryReportAttributes: string[];

        @Action('setErrorMessage') setErrorMessageAction: any;

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

<style>
    .missing-attributes-card-text {
        max-height: 300px;
        max-width: 300px;
        overflow-y: auto;
    }
</style>