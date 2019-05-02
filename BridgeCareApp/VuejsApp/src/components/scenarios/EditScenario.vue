<template>
    <v-form>
        <v-container>
            <v-layout row wrap>

                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="'fas fa-edit'"
                                  box
                                  readonly
                                  label="Analysis"
                                  type="text"
                                  @click:append="editAnalysis"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="'fas fa-dollar-sign'"
                                  box
                                  readonly
                                  label="Investment"
                                  type="text"
                                  @click:append="editInvestment"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="'fas fa-chart-line'"
                                  box
                                  readonly
                                  label="Performance"
                                  type="text"
                                  @click:append="editPerformance"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="marker ? 'info' : 'home'"
                                  box
                                  readonly
                                  label="Commited"
                                  type="text"
                                  @click:append="onShowCommittedProjectsFileUploader"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="marker ? 'info' : 'home'"
                                  box
                                  readonly
                                  label="Treatments"
                                  type="text"
                                  @click:append="toggleMarker"></v-text-field>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model="message"
                                  :append-icon="marker ? 'info' : 'home'"
                                  box
                                  readonly
                                  label="Prioritization"
                                  type="text"
                                  @click:append="toggleMarker"></v-text-field>
                </v-flex>

            </v-layout>

            <CommittedProjectsFileUploaderDialog :showDialog="showFileUploader" @submit="onUploadCommitedProjectFiles" />
        </v-container>
    </v-form>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {State, Action} from 'vuex-class';
    import {Scenario} from '@/shared/models/iAM/scenario';
    import CommittedProjectsFileUploaderDialog from '@/components/scenarios/scenarios-dialogs/CommittedProjectsFileUploaderDialog.vue';
    import {hasValue} from '@/shared/utils/has-value';
    import ScenarioService from '@/services/scenario.service';

    @Component({
        components: {CommittedProjectsFileUploaderDialog}
    })
    export default class EditScenario extends Vue {
        selectedScenarioId: number = 0;
        marker: boolean = true;
        message: string = '';

        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.scenario.selectedScenario) selectedScenario: Scenario;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('setNavigation') setNavigationAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;

        showFileUploader: boolean = false;

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                // set selectedScenarioId
                vm.selectedScenarioId = parseInt(to.query.simulationId);
                // set breadcrumbs
                vm.setNavigationAction([
                    {
                        text: 'Scenario dashboard',
                        to: '/Scenarios/'
                    },
                    {
                        text: 'Scenario editor',
                        to: {
                            path: '/EditScenario/', query: {simulationId: to.query.simulationId}
                        }
                    }
                ]);
                // check that selectedScenarioId is set
                if (isNaN(vm.selectedScenarioId) || vm.selectedScenarioId === 0) {
                    // set 'no selected scenario' error message, then redirect user to Scenarios UI
                    vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                    vm.$router.push('/Scenarios/');
                }
            });
        }

        created() {
            this.marker = true;
        }

        toggleMarker() {
            this.marker = !this.marker;
        }

        editAnalysis() {
            this.$router.push({
                path: '/EditAnalysis/', query: {simulationId: this.selectedScenarioId.toString()}
            });
        }

        editInvestment() {
            this.$router.push({
                path: '/InvestmentEditor/FromScenario/', query: {
                    simulationId: this.selectedScenarioId.toString()
                }
            });
        }

        editPerformance() {
            this.$router.push({
                path: '/PerformanceEditor/FromScenario/', query: {
                    simulationId: this.selectedScenarioId.toString()
                }
            });
        }

        onShowCommittedProjectsFileUploader() {
            this.showFileUploader = true;
        }

        onUploadCommitedProjectFiles(files: File[]) {
            if (hasValue(files)) {
                this.setIsBusyAction({isBusy: true});
                new ScenarioService().uploadCommittedProjectsFiles(files)
                    .then(() =>
                        // TODO: handle server response properly
                        this.setSuccessMessageAction({message: 'Files uploaded successfully'})
                    );
            }
        }
    }
</script>

<style scoped>
</style>