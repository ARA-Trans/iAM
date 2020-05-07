<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout fixed justify-space-between>
                <div>
                    <v-tabs>
                        <v-tab :key="navigationTab.tabName"
                               :to="navigationTab.navigation"
                               v-for="navigationTab in visibleNavigationTabs()">
                            {{navigationTab.tabName}}
                            <v-icon right>{{navigationTab.tabIcon}}</v-icon>
                        </v-tab>
                    </v-tabs>
                </div>
                <div>
                    <v-layout>
                        <div>
                            <v-btn class="ara-blue-bg white--text">
                                Run Scenario
                                <v-icon class="white--text" right>fas fa-play</v-icon>
                            </v-btn>
                        </div>
                        <div>
                            <v-btn @click="onShowCommittedProjectsFileUploader" class="ara-blue-bg white--text">
                                Committed Projects
                                <v-icon class="white--text" right>fas fa-cloud-upload-alt</v-icon>
                            </v-btn>
                        </div>
                    </v-layout>
                </div>
            </v-layout>
        </v-flex>

        <v-flex xs12>
            <v-container fluid grid-list-xs>
                <router-view></router-view>
            </v-container>
        </v-flex>

        <CommittedProjectsFileUploaderDialog :showDialog="showFileUploader" @submit="onUploadCommittedProjectFiles"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
    import CommittedProjectsFileUploaderDialog from '@/components/scenarios/scenarios-dialogs/CommittedProjectsFileUploaderDialog.vue';
    import {any, isNil, clone} from 'ramda';
    import {AxiosResponse} from 'axios';
    import CommittedProjectsService from '@/services/committed-projects.service';
    import {Network} from '@/shared/models/iAM/network';
    import FileDownload from 'js-file-download';
    import {NavigationTab} from '@/shared/models/iAM/navigation-tab';
    import {CommittedProjectsDialogResult} from '@/shared/models/modals/committed-projects-dialog-result';

    @Component({
        components: {CommittedProjectsFileUploaderDialog}
    })
    export default class EditScenario extends Vue {
        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.network.networks) networks: Network[];
        @State(state => state.authentication.isAdmin) isAdmin: boolean;
        @State(state => state.scenario.selectedScenario) stateSelectedScenario: Scenario;

        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setSelectedScenarioName') setSelectedScenarioNameAction: any;
        @Action('selectScenario') selectScenarioAction: any;

        selectedScenarioId: number = 0;
        showFileUploader: boolean = false;
        networkId: number = 0;
        selectedScenario: Scenario = clone(emptyScenario);
        navigationTabs: NavigationTab[] = [];

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                // set selectedScenarioId
                vm.selectedScenarioId = isNaN(to.query.selectedScenarioId) ? 0 : parseInt(to.query.selectedScenarioId);
                vm.simulationName = to.query.simulationName;

                // check that selectedScenarioId is set
                if (vm.selectedScenarioId === 0) {
                    // set 'no selected scenario' error message, then redirect user to Scenarios UI
                    vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                    vm.$router.push('/Scenarios/');
                } else {
                    vm.setSelectedScenarioNameAction({selectedScenarioName: to.query.simulationName});
                    vm.selectScenarioAction({simulationId: vm.selectedScenarioId});
                    vm.navigationTabs = [
                        {
                            tabName: 'Analysis',
                            tabIcon: 'fas fa-chart-bar',
                            navigation: {
                                path: '/EditAnalysis/',
                                query: {
                                    selectedScenarioId: to.query.selectedScenarioId,
                                    simulationName: to.query.simulationName,
                                    objectIdMOngoDBForScenario: to.query.objectIdMOngoDBForScenario
                                }
                            }
                        },
                        {
                            tabName: 'Investment',
                            tabIcon: 'fas fa-dollar-sign',
                            navigation: {
                                path: '/InvestmentEditor/Scenario/',
                                query: {
                                    selectedScenarioId: to.query.selectedScenarioId,
                                    simulationName: to.query.simulationName,
                                    objectIdMOngoDBForScenario: to.query.objectIdMOngoDBForScenario
                                }
                            }
                        },
                        {
                            tabName: 'Performance',
                            tabIcon: 'fas fa-chart-line',
                            navigation: {
                                path: '/PerformanceEditor/Scenario/',
                                query: {
                                    selectedScenarioId: to.query.selectedScenarioId,
                                    simulationName: to.query.simulationName,
                                    objectIdMOngoDBForScenario: to.query.objectIdMOngoDBForScenario
                                }
                            }
                        },
                        {
                            tabName: 'Treatment',
                            tabIcon: 'fas fa-tools',
                            navigation: {
                                path: '/TreatmentEditor/Scenario/',
                                query: {
                                    selectedScenarioId: to.query.selectedScenarioId,
                                    simulationName: to.query.simulationName,
                                    objectIdMOngoDBForScenario: to.query.objectIdMOngoDBForScenario
                                }
                            }
                        },
                        {
                            tabName: 'Priority',
                            tabIcon: 'fas fa-copy',
                            navigation: {
                                path: '/PriorityEditor/Scenario/',
                                query: {
                                    selectedScenarioId: to.query.selectedScenarioId,
                                    simulationName: to.query.simulationName,
                                    objectIdMOngoDBForScenario: to.query.objectIdMOngoDBForScenario
                                }
                            }
                        },
                        {
                            tabName: 'Target',
                            tabIcon: 'fas fa-bullseye',
                            navigation: {
                                path: '/TargetEditor/Scenario/',
                                query: {
                                    selectedScenarioId: to.query.selectedScenarioId,
                                    simulationName: to.query.simulationName,
                                    objectIdMOngoDBForScenario: to.query.objectIdMOngoDBForScenario
                                }
                            }
                        },
                        {
                            tabName: 'Deficient',
                            tabIcon: 'fas fa-level-down-alt',
                            navigation: {
                                path: '/DeficientEditor/Scenario/',
                                query: {
                                    selectedScenarioId: to.query.selectedScenarioId,
                                    simulationName: to.query.simulationName,
                                    objectIdMOngoDBForScenario: to.query.objectIdMOngoDBForScenario
                                }
                            }
                        },
                        {
                            tabName: 'Remaining Life Limit',
                            tabIcon: 'fas fa-business-time',
                            visible: vm.isAdmin,
                            navigation: {
                                path: '/RemainingLifeLimitEditor/Scenario/',
                                query: {
                                    selectedScenarioId: to.query.selectedScenarioId,
                                    simulationName: to.query.simulationName,
                                    objectIdMOngoDBForScenario: to.query.objectIdMOngoDBForScenario
                                }
                            }
                        },
                        {
                            tabName: 'Cash Flow',
                            tabIcon: 'fas fa-money-bill-wave',
                            navigation: {
                                path: '/CashFlowEditor/Scenario/',
                                query: {
                                    selectedScenarioId: to.query.selectedScenarioId,
                                    simulationName: to.query.simulationName,
                                    objectIdMOngoDBForScenario: to.query.objectIdMOngoDBForScenario
                                }
                            }
                        }
                    ];

                    // get the window href
                    const href = window.location.href;
                    // check each NavigationTab object to see if it has a matching navigation path with the href
                    const hasChildPath = any(
                        (navigationTab: NavigationTab) => href.indexOf(navigationTab.navigation.path) !== -1,
                        vm.navigationTabs
                    );
                    // if no matching navigation path was found in the href, then route with path of first navigationTabs entry
                    if (!hasChildPath) {
                        vm.$router.push(vm.navigationTabs[0].navigation);
                    }
                }
            });
        }

        @Watch('stateSelectedScenario')
        onStateSelectedScenarioChanged() {
            this.selectedScenario = clone(this.stateSelectedScenario);
        }

        beforeDestroy() {
            this.setSelectedScenarioNameAction({selectedScenarioName: ''});
        }

        /**
         * Shows the CommittedProjectsFileUploaderDialog
         */
        onShowCommittedProjectsFileUploader() {
            this.showFileUploader = true;
        }

        /**
         * Uploads the files submitted via the CommittedProjectsFileUploaderDialog (if present),
         * exports committed projects if isExport is true
         * @param result CommmittedProjectsDialogResult object
         * @param isExport boolean
         */
        onUploadCommittedProjectFiles(result: CommittedProjectsDialogResult, isExport: boolean) {
            this.showFileUploader = false;
            if (!isNil(result)) {
                CommittedProjectsService
                    .saveCommittedProjectsFiles(result.files, result.applyNoTreatment, this.selectedScenarioId.toString(), this.networks[0].networkId.toString())
                    .then((response: AxiosResponse<any>) => {
                        this.setSuccessMessageAction({message: 'Successfully uploaded committed projects. You will receive an email when the projects have been fully processed.'});
                    });
            }
            if (isExport) {
                this.selectedScenario.simulationId = this.selectedScenarioId;
                this.selectedScenario.networkId = this.networks[0].networkId;
                CommittedProjectsService.ExportCommittedProjects(this.selectedScenario)
                    .then((response: AxiosResponse<any>) => {
                        FileDownload(response.data, 'CommittedProjects.xlsx');
                    });
            }
        }

        visibleNavigationTabs() {
            return this.navigationTabs.filter(navigationTab => navigationTab.visible === undefined || navigationTab.visible);
        }
    }
</script>

<style>
    .child-router-div {
        height: 100%;
        overflow: auto;
    }
</style>
