<template>
    <v-container fluid grid-list-xl>
        <div class="priorities-targets-deficients-container">
            <v-layout class="priorities-targets-deficients-layout">
                <v-tabs v-model="activeTab">
                    <v-tab v-for="(tab, index) in tabs" :key="index" ripple @click="setAsActiveTab(index)">
                        {{tab}}
                    </v-tab>
                    <v-tabs-items v-model="activeTab">
                        <v-tab-item>
                            <v-card>
                                <v-card-text class="priorities-targets-deficients-card-text">
                                    <PrioritiesTab />
                                </v-card-text>
                            </v-card>
                        </v-tab-item>
                        <v-tab-item>
                            <v-card>
                                <v-card-text class="priorities-targets-deficients-card-text">
                                    <TargetsTab />
                                </v-card-text>
                            </v-card>
                        </v-tab-item>
                        <v-tab-item>
                            <v-card>
                                <v-card-text class="priorities-targets-deficients-card-text">
                                    <DeficientsTab />
                                </v-card-text>
                            </v-card>
                        </v-tab-item>
                    </v-tabs-items>
                </v-tabs>
            </v-layout>
        </div>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Action} from 'vuex-class';
    import PrioritiesTab from '@/components/priorities-targets-deficients/priorities-targets-deficients-tabs/PrioritiesTab.vue';
    import TargetsTab from '@/components/priorities-targets-deficients/priorities-targets-deficients-tabs/TargetsTab.vue';
    import DeficientsTab from '@/components/priorities-targets-deficients/priorities-targets-deficients-tabs/DeficientsTab.vue';

    @Component({
        components: {DeficientsTab, TargetsTab, PrioritiesTab}
    })
    export default class PrioritiesTargetsDeficients extends Vue {
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setNavigation') setNavigationAction: any;
        @Action('getPriorities') getPrioritiesAction: any;
        @Action('getTargets') getTargetsAction: any;
        @Action('getDeficients') getDeficientsAction: any;

        selectedScenarioId: number = 0;
        tabs: string[] = ['priorities', 'targets', 'deficients'];
        activeTab: number = 0;

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                vm.selectedScenarioId = isNaN(parseInt(to.query.selectedScenarioId)) ? 0 : parseInt(to.query.selectedScenarioId);
                if (vm.selectedScenarioId === 0) {
                    vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                    vm.$router.push('/Scenarios/');
                }

                vm.setNavigationAction([
                    {
                        text: 'Scenario dashboard',
                        to: {path: '/Scenarios/', query: {}}
                    },
                    {
                        text: 'Scenario editor',
                        to: {path: '/EditScenario/', query: {selectedScenarioId: to.query.selectedScenarioId}}
                    },
                    {
                        text: 'Priorities, Targets, & Deficients',
                        to: {path: '/PrioritiesTargetsDeficients/', query: {selectedScenarioId: to.query.selectedScenarioId}}
                    }
                ]);

                vm.getPrioritiesAction({selectedScenarioId: vm.selectedScenarioId});
                vm.getTargetsAction({selectedScenarioId: vm.selectedScenarioId});
                vm.getDeficientsAction({selectedScenarioId: vm.selectedScenarioId});
            });
        }

        /**
         * Sets the activeTab to the specified tab index
         * @param tabIndex The tab index of one of the specified tabs in the tabs array
         */
        setAsActiveTab(tabIndex: number) {
            this.activeTab = tabIndex;
        }
    }
</script>

<style>
    .priorities-targets-deficients-container {
        height: 730px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .priorities-targets-deficients-layout {
        padding: 15px;
    }

    .priorities-targets-deficients-card-text {
        height: 660px;
    }

    .priorities-data-table, .targets-data-table, .deficients-data-table {
        height: 490px;
        overflow-y: auto;
    }

    .priorities-targets-deficients-buttons {
        background: white;
    }
</style>
