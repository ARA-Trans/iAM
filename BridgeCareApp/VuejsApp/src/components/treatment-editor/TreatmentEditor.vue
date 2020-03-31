<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn @click="createTreatmentLibraryDialogData.showDialog = true" class="ara-blue-bg white--text"
                           v-show="selectedScenarioId === '0'">
                        New Library
                    </v-btn>
                    <v-select :items="treatmentLibrariesSelectListItems"
                              class="treatment-library-select" label="Select a Treatment Library"
                              outline v-if="!hasSelectedTreatmentLibrary || selectedScenarioId !== '0'" v-model="treatmentLibrarySelectItemValue">
                    </v-select>
                    <v-text-field label="Treatment Name"
                                  v-if="hasSelectedTreatmentLibrary && selectedScenarioId === '0'" v-model="selectedTreatmentLibrary.name">
                        <template slot="append">
                            <v-btn @click="treatmentLibrarySelectItemValue = null" class="ara-orange" icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedTreatmentLibrary && selectedScenarioId === '0'">
                        Owner: {{selectedTreatmentLibrary.owner ? selectedTreatmentLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" label="Shared"
                                v-if="hasSelectedTreatmentLibrary && selectedScenarioId === '0'" v-model="selectedTreatmentLibrary.shared"/>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedTreatmentLibrary"></v-divider>
        <v-flex v-show="hasSelectedTreatmentLibrary" xs12>
            <div class="treatments-div">
                <v-layout justify-center row>
                    <v-flex xs3>
                        <v-btn @click="showCreateTreatmentDialog = true" class="ara-blue-bg white--text">
                            Add Treatment
                        </v-btn>
                        <v-select :items="treatmentsSelectListItems" clearable label="Select a Treatment"
                                  outline v-model="treatmentSelectItemValue">
                        </v-select>
                    </v-flex>
                    <v-flex xs9>
                        <div v-show="selectedTreatment.id !== '0'">
                            <v-tabs v-model="activeTab">
                                <v-tab :key="index" @click="activeTab = index" ripple
                                       v-for="(treatmentTab, index) in treatmentTabs">
                                    {{treatmentTab}}
                                </v-tab>
                                <v-tabs-items v-model="activeTab">
                                    <v-tab-item>
                                        <v-card>
                                            <v-card-text class="card-tab-content">
                                                <FeasibilityTab :feasibilityTabData="tabData"
                                                                @submit="updateSelectedTreatmentLibrary"/>
                                            </v-card-text>
                                        </v-card>
                                    </v-tab-item>
                                    <v-tab-item>
                                        <v-card>
                                            <v-card-text class="card-tab-content">
                                                <CostsTab :costsTabData="tabData"
                                                          @submit="updateSelectedTreatmentLibrary"/>
                                            </v-card-text>
                                        </v-card>
                                    </v-tab-item>
                                    <v-tab-item>
                                        <v-card>
                                            <v-card-text class="card-tab-content">
                                                <ConsequencesTab :consequencesTabData="tabData"
                                                                 @submit="updateSelectedTreatmentLibrary"/>
                                            </v-card-text>
                                        </v-card>
                                    </v-tab-item>
                                    <v-tab-item>
                                        <v-card>
                                            <v-card-text class="card-tab-content">
                                                <BudgetsTab :budgetsTabData="tabData"
                                                            @submit="updateSelectedTreatmentLibrary"/>
                                            </v-card-text>
                                        </v-card>
                                    </v-tab-item>
                                </v-tabs-items>
                            </v-tabs>
                        </div>
                    </v-flex>
                </v-layout>
            </div>
        </v-flex>
        <v-divider v-show="hasSelectedTreatmentLibrary"></v-divider>
        <v-flex v-show="hasSelectedTreatmentLibrary && selectedTreatmentLibrary.id !== stateScenarioTreatmentLibrary.id"
                xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea label="Description" no-resize outline rows="4"
                                v-model="selectedTreatmentLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout justify-end row v-show="hasSelectedTreatmentLibrary">
                <v-btn @click="onApplyToScenario" class="ara-blue-bg white--text" v-show="selectedScenarioId !== '0'">
                    Save
                </v-btn>
                <v-btn @click="onUpdateLibrary" class="ara-blue-bg white--text" v-show="selectedScenarioId === '0'">
                    Update Library
                </v-btn>
                <v-btn @click="onCreateAsNewLibrary" class="ara-blue-bg white--text">
                    Create as New Library
                </v-btn>
                <v-btn @click="onDeleteTreatmentLibrary" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Delete Library
                </v-btn>
                <v-btn @click="onDiscardChanges" class="ara-orange-bg white--text" v-show="selectedScenarioId !== '0'">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse"/>

        <CreateTreatmentLibraryDialog :dialogData="createTreatmentLibraryDialogData"
                                      @submit="onCreateTreatmentLibrary"/>

        <CreateTreatmentDialog :showDialog="showCreateTreatmentDialog" @submit="onCreateTreatment"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import CreateTreatmentLibraryDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentLibraryDialog.vue';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {
        CreateTreatmentLibraryDialogData,
        emptyCreateTreatmentLibraryDialogData
    } from '@/shared/models/modals/create-treatment-library-dialog-data';
    import {emptyTreatment, emptyTreatmentLibrary, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';
    import {hasValue} from '@/shared/utils/has-value-util';
    import CreateTreatmentDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentDialog.vue';
    import {any, append, clone, find, isNil, propEq} from 'ramda';
    import FeasibilityTab from '@/components/treatment-editor/treatment-editor-tabs/FeasibilityTab.vue';
    import CostsTab from '@/components/treatment-editor/treatment-editor-tabs/CostsTab.vue';
    import {emptyTabData, TabData} from '@/shared/models/child-components/tab-data';
    import ConsequencesTab from '@/components/treatment-editor/treatment-editor-tabs/ConsequencesTab.vue';
    import BudgetsTab from '@/components/treatment-editor/treatment-editor-tabs/BudgetsTab.vue';
    import {InvestmentLibrary} from '@/shared/models/iAM/investment';
    import {sorter} from '@/shared/utils/sorter-utils';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';

    const ObjectID = require('bson-objectid');

    @Component({
        components: {
            BudgetsTab,
            ConsequencesTab, CostsTab, FeasibilityTab, CreateTreatmentDialog, CreateTreatmentLibraryDialog, Alert
        }
    })
    export default class TreatmentEditor extends Vue {
        @State(state => state.treatmentEditor.treatmentLibraries) stateTreatmentLibraries: TreatmentLibrary[];
        @State(state => state.treatmentEditor.selectedTreatmentLibrary) stateSelectedTreatmentLibrary: TreatmentLibrary;
        @State(state => state.treatmentEditor.scenarioTreatmentLibrary) stateScenarioTreatmentLibrary: TreatmentLibrary;
        @State(state => state.investmentEditor.scenarioInvestmentLibrary) stateScenarioInvestmentLibrary: InvestmentLibrary;

        @Action('getTreatmentLibraries') getTreatmentLibrariesAction: any;
        @Action('getScenarioTreatmentLibrary') getScenarioTreatmentLibraryAction: any;
        @Action('selectTreatmentLibrary') selectTreatmentLibraryAction: any;
        @Action('createTreatmentLibrary') createTreatmentLibraryAction: any;
        @Action('updateTreatmentLibrary') updateTreatmentLibraryAction: any;
        @Action('deleteTreatmentLibrary') deleteTreatmentLibraryAction: any;
        @Action('saveScenarioTreatmentLibrary') saveScenarioTreatmentLibraryAction: any;
        @Action('getScenarioInvestmentLibrary') getScenarioInvestmentLibraryAction: any;
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        selectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
        selectedScenarioId: string = '0';
        hasSelectedTreatmentLibrary: boolean = false;
        treatmentLibrariesSelectListItems: SelectItem[] = [];
        treatmentLibrarySelectItemValue: string | null = null;
        treatmentsSelectListItems: SelectItem[] = [];
        treatmentSelectItemValue: string | null = null;
        selectedTreatment: Treatment = clone(emptyTreatment);
        activeTab: number = 0;
        treatmentTabs: string[] = ['feasibility', 'costs', 'consequences'];
        createTreatmentLibraryDialogData: CreateTreatmentLibraryDialogData = clone(emptyCreateTreatmentLibraryDialogData);
        showCreateTreatmentDialog: boolean = false;
        tabData: TabData = clone(emptyTabData);
        alertBeforeDelete: AlertData = clone(emptyAlertData);
        objectIdMOngoDBForScenario: string = '';

        /**
         * Sets component ui properties that triggers cascading ui updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/TreatmentEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;
                    if (vm.selectedScenarioId === '0') {
                        // set 'no selected scenario' error message, then redirect user to Scenarios UI
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }

                    vm.treatmentTabs = [...vm.treatmentTabs, 'budgets'];
                }

                vm.treatmentLibrarySelectItemValue = null;
                vm.getTreatmentLibrariesAction()
                    .then(() => {
                        if (vm.selectedScenarioId !== '0') {
                            vm.getScenarioTreatmentLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)})
                                .then(() =>
                                    vm.getScenarioInvestmentLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)})
                                );
                        }
                    });
            });
        }

        beforeDestroy() {
            this.setHasUnsavedChangesAction({value: false});
        }

        /**
         * Sets treatmentLibraries with the treatmentLibraries state data
         */
        @Watch('stateTreatmentLibraries')
        onStateTreatmentLibrariesChanged() {
            this.treatmentLibrariesSelectListItems = this.stateTreatmentLibraries
                .map((treatmentLibrary: TreatmentLibrary) => ({
                    text: treatmentLibrary.name,
                    value: treatmentLibrary.id.toString()
                }));
        }

        /**
         * Sets the selected treatment library based on treatmentLibrarySelectItemValue
         */
        @Watch('treatmentLibrarySelectItemValue')
        onTreatmentLibrarySelectItemValueChanged() {
            this.selectTreatmentLibraryAction({selectedLibraryId: this.treatmentLibrarySelectItemValue});
        }

        /**
         * Sets selectedTreatmentLibrary with selectedTreatmentLibrary state data
         */
        @Watch('stateSelectedTreatmentLibrary')
        onStateSelectedTreatmentLibraryChanged() {
            this.selectedTreatmentLibrary = clone(this.stateSelectedTreatmentLibrary);
        }

        /**
         * Sets/resets the component UI properties reliant on a selected treatment library
         */
        @Watch('selectedTreatmentLibrary')
        onSelectedTreatmentLibraryChanged() {
            this.setHasUnsavedChangesAction({
                value: hasUnsavedChanges(
                    'treatment', this.selectedTreatmentLibrary, this.stateSelectedTreatmentLibrary, this.stateScenarioTreatmentLibrary
                )
            });

            this.hasSelectedTreatmentLibrary = this.selectedTreatmentLibrary.id !== '0';

            this.treatmentsSelectListItems = hasValue(this.selectedTreatmentLibrary.treatments)
                ? this.selectedTreatmentLibrary.treatments.map((treatment: Treatment) => ({
                    text: treatment.name,
                    value: treatment.id
                }))
                : [];

            if (!hasValue(this.selectedTreatmentLibrary.treatments) ||
                !any(propEq('id', this.selectedTreatment.id), this.selectedTreatmentLibrary.treatments)) {
                this.treatmentSelectItemValue = null;
            }
        }

        /**
         * Sets the selected treatment
         */
        @Watch('treatmentSelectItemValue')
        onTreatmentSelectItemValueChanged() {
            let selectedTreatment: Treatment = clone(emptyTreatment);

            if (any(propEq('id', this.treatmentSelectItemValue), this.selectedTreatmentLibrary.treatments)) {
                selectedTreatment = {
                    ...find(propEq('id', this.treatmentSelectItemValue), this.selectedTreatmentLibrary.treatments) as Treatment,
                    budgets: this.getTreatmentBudgets()
                };
            }

            this.activeTab = 0;
            this.selectedTreatment = selectedTreatment;
        }

        @Watch('selectedTreatment')
        onSelectedTreatmentChanged() {
            if (this.selectedTreatment.id !== '0') {
                this.tabData = {
                    tabTreatmentLibraries: clone(this.stateTreatmentLibraries),
                    tabSelectedTreatmentLibrary: clone(this.selectedTreatmentLibrary),
                    tabSelectedTreatment: clone(this.selectedTreatment),
                    tabScenarioInvestmentLibrary: clone(this.stateScenarioInvestmentLibrary)
                };
            }
        }

        getTreatmentBudgets() {
            let budgets: string[] = [];

            if (hasValue(this.stateScenarioInvestmentLibrary.budgetOrder)) {
                budgets = clone(this.stateScenarioInvestmentLibrary.budgetOrder);
            } else if (hasValue(this.stateScenarioInvestmentLibrary.budgetYears)) {
                budgets = sorter(
                    getPropertyValues('budgetName', this.stateScenarioInvestmentLibrary.budgetYears)) as string[];
            }

            return budgets;
        }

        /**
         * Dispatches an action to update the selected treatment library in state, then resets the selected treatment
         * @param updatedSelectedTreatmentLibrary The selected treatment library data to use for updating
         */
        updateSelectedTreatmentLibrary(updatedSelectedTreatmentLibrary: TreatmentLibrary) {
            this.selectedTreatmentLibrary = {...updatedSelectedTreatmentLibrary};
            this.selectedTreatment = find(
                propEq('id', this.selectedTreatment.id), this.selectedTreatmentLibrary.treatments) as Treatment;
        }

        /**
         * Shows the CreateTreatmentLibraryDialog passing in the selected treatment library's description & treatments data
         */
        onCreateAsNewLibrary() {
            this.createTreatmentLibraryDialogData = {
                showDialog: true,
                selectedTreatmentLibraryDescription: this.selectedTreatmentLibrary.description,
                selectedTreatmentLibraryTreatments: this.selectedTreatmentLibrary.treatments
            };
        }

        /**
         * Dispatches an action to create a new treatment library on the server
         */
        onCreateTreatmentLibrary(createdTreatmentLibrary: TreatmentLibrary) {
            this.createTreatmentLibraryDialogData = clone(emptyCreateTreatmentLibraryDialogData);

            if (!isNil(createdTreatmentLibrary)) {
                this.createTreatmentLibraryAction({createdTreatmentLibrary: createdTreatmentLibrary})
                    .then(() => this.treatmentLibrarySelectItemValue = createdTreatmentLibrary.id);
            }
        }

        /**
         * Dispatches an action to add a created treatment to the selected treatment library's treatments list in state
         */
        onCreateTreatment(createdTreatment: Treatment) {
            this.showCreateTreatmentDialog = false;

            if (!isNil(createdTreatment)) {
                this.selectedTreatmentLibrary = {
                    ...this.selectedTreatmentLibrary,
                    treatments: append(createdTreatment, this.selectedTreatmentLibrary.treatments)
                };

                this.treatmentSelectItemValue = createdTreatment.id;
            }
        }

        /**
         * Dispatches an action to update the selected treatment library on the server
         */
        onUpdateLibrary() {
            this.updateTreatmentLibraryAction({updatedTreatmentLibrary: this.selectedTreatmentLibrary});
        }

        /**
         * Dispatches an action to update the scenario's treatment library data with the currently selected treatment library
         */
        onApplyToScenario() {
            this.saveScenarioTreatmentLibraryAction({
                saveScenarioTreatmentLibraryData: {
                    ...this.selectedTreatmentLibrary,
                    id: this.selectedScenarioId
                },
                objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario
            }).then(() => this.onDiscardChanges());
        }

        /**
         * Clears the selected treatment library and dispatches an action to update the selected treatment library in
         * state with the scenario treatment library (if present), otherwise an action is dispatched to get the scenario
         * treatment library from the server to update the selected treatment library in state
         */
        onDiscardChanges() {
            this.treatmentLibrarySelectItemValue = null;
            this.treatmentSelectItemValue = null;
            setTimeout(() => this.selectTreatmentLibraryAction({selectedLibraryId: this.stateScenarioTreatmentLibrary.id}));
        }

        onDeleteTreatmentLibrary() {
            this.alertBeforeDelete = {
                showDialog: true,
                heading: 'Warning',
                choice: true,
                message: 'Are you sure you want to delete?'
            };
        }

        onSubmitDeleteResponse(response: boolean) {
            this.alertBeforeDelete = clone(emptyAlertData);

            if (response) {
                this.treatmentLibrarySelectItemValue = null;
                this.deleteTreatmentLibraryAction({treatmentLibrary: this.selectedTreatmentLibrary});
            }
        }
    }
</script>

<style>
    .treatment-editor-container {
        height: 730px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .treatments-div {
        height: 355px;
    }

    .card-tab-content {
        height: 305px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .treatment-library-select {
        height: 60px;
    }

    .sharing label {
        padding-top: 0.5em;
    }

    .sharing {
        padding-top: 0;
        margin: 0;
    }
</style>
