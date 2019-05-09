<template>
    <v-container fluid grid-list-xl>
        <div class="treatment-editor-container">
            <v-layout column>
                <v-flex xs12>
                    <v-layout justify-center fill-height>
                        <v-flex xs3>
                            <v-btn v-show="selectedScenarioId === 0" color="info" v-on:click="onNewLibrary">
                                New Library
                            </v-btn>
                            <v-chip label v-show="selectedScenarioId > 0" color="indigo" text-color="white">
                                <v-icon left v-model="scenarioName">label</v-icon>Scenario name: {{scenarioName}}
                            </v-chip>
                            <v-select v-if="!hasSelectedTreatmentLibrary || selectedScenarioId > 0"
                                      :items="treatmentLibrariesSelectListItems" label="Select a Treatment Library"
                                      outline v-model="treatmentLibrarySelectItemValue">
                            </v-select>
                            <v-text-field v-if="hasSelectedTreatmentLibrary && selectedScenarioId === 0"
                                          label="Treatment Name" append-icon="clear"
                                          v-model="selectedTreatmentLibrary.name"
                                          @click:append="onClearSelectedTreatmentLibrary">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedTreatmentLibrary"></v-divider>
                <v-flex xs12 v-if="hasSelectedTreatmentLibrary">
                    <div class="treatments-div">
                        <v-layout justify-center row fill-height>
                            <v-flex xs3>
                                <v-btn color="info" v-on:click="onAddTreatment">
                                    Add Treatment
                                </v-btn>
                                <v-select :items="treatmentsSelectListItems" label="Select a Treatment" outline
                                          clearable v-model="treatmentSelectItemValue">
                                </v-select>
                            </v-flex>
                            <v-flex xs9>
                                <div v-show="selectedTreatment.id !== 0">
                                    <v-tabs v-model="activeTab">
                                        <v-tab v-for="(treatmentTab, index) in treatmentTabs" :key="index" ripple
                                               v-on:click="setAsActiveTab(index)">
                                            {{treatmentTab}}
                                        </v-tab>
                                        <v-tabs-items v-model="activeTab">
                                            <v-tab-item>
                                                <v-card>
                                                    <v-card-text class="card-tab-content">
                                                        <FeasibilityTab :feasibilityTabData="tabData" @submit="updateSelectedTreatmentLibrary" />
                                                    </v-card-text>
                                                </v-card>
                                            </v-tab-item>
                                            <v-tab-item>
                                                <v-card>
                                                    <v-card-text class="card-tab-content">
                                                        <CostsTab :costsTabData="tabData" @submit="updateSelectedTreatmentLibrary" />
                                                    </v-card-text>
                                                </v-card>
                                            </v-tab-item>
                                            <v-tab-item>
                                                <v-card>
                                                    <v-card-text class="card-tab-content">
                                                        <ConsequencesTab :consequencesTabData="tabData" @submit="updateSelectedTreatmentLibrary" />
                                                    </v-card-text>
                                                </v-card>
                                            </v-tab-item>
                                            <v-tab-item>
                                                <v-card>
                                                    <v-card-text class="card-tab-content">
                                                        <UnderConstruction />
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
                <v-divider v-if="hasSelectedTreatmentLibrary"></v-divider>
                <v-flex xs12 v-if="hasSelectedTreatmentLibrary">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-textarea rows="4" no-resize outline full-width
                                        :label="selectedTreatmentLibrary.description === '' ? 'Description' : ''"
                                        v-model="selectedTreatmentLibrary.description">
                            </v-textarea>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn v-show="selectedScenarioId > 0" color="error lighten-1" v-on:click="onDiscardChanges"
                       :disabled="!hasSelectedTreatmentLibrary">
                    Discard Changes
                </v-btn>
                <v-btn color="info lighten-2" v-on:click="onCreateAsNewLibrary" :disabled="!hasSelectedTreatmentLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId === 0" color="info lighten-1" v-on:click="onUpdateLibrary"
                       :disabled="!hasSelectedTreatmentLibrary">
                    Update Library
                </v-btn>
                <v-btn v-show="selectedScenarioId > 0" color="info" v-on:click="onApplyToScenario"
                       :disabled="!hasSelectedTreatmentLibrary">
                    Apply
                </v-btn>
            </v-layout>
        </v-footer>

        <CreateTreatmentLibraryDialog :dialogData="createTreatmentLibraryDialogData"
                                      @submit="onCreateTreatmentLibrary" />

        <CreateTreatmentDialog :showDialog="showCreateTreatmentDialog" @submit="onCreateTreatment" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import CreateTreatmentLibraryDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentLibraryDialog.vue';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {
        CreateTreatmentLibraryDialogData,
        emptyCreateTreatmentLibraryDialogData
    } from '@/shared/models/dialogs/treatment-editor-dialogs/create-treatment-library-dialog-data';
    import {
        Consequence,
        Cost,
        emptyTreatment,
        Feasibility,
        Treatment,
        TreatmentLibrary
    } from '@/shared/models/iAM/treatment';
    import {hasValue} from '@/shared/utils/has-value';
    import CreateTreatmentDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentDialog.vue';
    import {isNil, append, any, propEq, clone, uniq} from 'ramda';
    import {getLatestPropertyValue} from '@/shared/utils/getter-utils';
    import FeasibilityTab from '@/components/treatment-editor/treatment-editor-tabs/FeasibilityTab.vue';
    import CostsTab from '@/components/treatment-editor/treatment-editor-tabs/CostsTab.vue';
    import {TabData, emptyTabData} from '@/shared/models/child-components/treatment-editor/tab-data';
    import ConsequencesTab from '@/components/treatment-editor/treatment-editor-tabs/ConsequencesTab.vue';
    import UnderConstruction from '@/components/UnderConstruction.vue';
    import {sortByProperty} from '@/shared/utils/sorter';

    @Component({
        components: {
            UnderConstruction,
            ConsequencesTab, CostsTab, FeasibilityTab, CreateTreatmentDialog, CreateTreatmentLibraryDialog}
    })
    export default class TreatmentEditor extends Vue {
        @State(state => state.treatmentEditor.treatmentLibraries) treatmentLibraries: TreatmentLibrary[];
        @State(state => state.treatmentEditor.selectedTreatmentLibrary) selectedTreatmentLibrary: TreatmentLibrary;
        @State(state => state.treatmentEditor.scenarioTreatmentLibrary) scenarioTreatmentLibrary: TreatmentLibrary;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('setNavigation') setNavigationAction: any;
        @Action('getTreatmentLibraries') getTreatmentLibrariesAction: any;
        @Action('getScenarioTreatmentLibrary') getScenarioTreatmentLibraryAction: any;
        @Action('selectTreatmentLibrary') selectTreatmentLibraryAction: any;
        @Action('updateSelectedTreatmentLibrary') updateSelectedTreatmentLibraryAction: any;
        @Action('createTreatmentLibrary') createTreatmentLibraryAction: any;
        @Action('updateTreatmentLibrary') updateTreatmentLibraryAction: any;
        @Action('upsertScenarioTreatmentLibrary') upsertScenarioTreatmentLibraryAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;

        selectedScenarioId: number = 0;
        scenarioName: string = '';
        hasSelectedTreatmentLibrary: boolean = false;
        treatmentLibrariesSelectListItems: SelectItem[] = [];
        treatmentLibrarySelectItemValue: string = '';
        treatmentsSelectListItems: SelectItem[] = [];
        treatmentSelectItemValue: string = '';
        selectedTreatment: Treatment = clone(emptyTreatment);
        activeTab: number = 0;
        treatmentTabs: string[] = ['feasibility', 'costs', 'consequences', 'budgets'];
        createTreatmentLibraryDialogData: CreateTreatmentLibraryDialogData = clone(emptyCreateTreatmentLibraryDialogData);
        showCreateTreatmentDialog: boolean = false;
        tabData: TabData = clone(emptyTabData);
        allTreatments: Treatment[] = [];
        latestTreatmentId: number = 0;
        latestFeasibilityId: number = 0;
        latestCostId: number = 0;
        latestConsequenceId: number = 0;

        /**
         * Sets component ui properties that triggers cascading ui updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/TreatmentEditor/FromScenario/') {
                    vm.selectedScenarioId = isNaN(parseInt(to.query.simulationId)) ? 0 : parseInt(to.query.simulationId);
                    if (vm.selectedScenarioId === 0) {
                        // set 'no selected scenario' error message, then redirect user to Scenarios UI
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                    vm.setNavigationAction([
                        {
                            text: 'Scenario Dashboard',
                            to: {path: '/Scenarios/', query: {}}
                        },
                        {
                            text: 'Scenario Editor',
                            to: {path: '/EditScenario/', query: {simulationId: to.query.simulationId}}
                        },
                        {
                            text: 'Treatment Editor',
                            to: {path: '/TreatmentEditor/FromScenario/', query: {simulationId: to.query.simulationId}}
                        }
                    ]);
                }
                vm.onClearSelectedTreatmentLibrary();
                setTimeout(() => {
                    vm.setIsBusyAction({isBusy: true});
                    vm.getTreatmentLibrariesAction()
                        .then(() => {
                            if (vm.selectedScenarioId > 0) {
                                vm.getScenarioTreatmentLibraryAction({selectedScenarioId: vm.selectedScenarioId})
                                    .then(() => vm.setIsBusyAction({isBusy: false}));
                            } else {
                                vm.setIsBusyAction({isBusy: false});
                            }
                        });
                });
            });
        }

        /**
         * Resets component ui properties that triggers cascading ui updates
         */
        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === '/TreatmentEditor/Library/') {
                this.selectedScenarioId = 0;
                this.onClearSelectedTreatmentLibrary();
                next();
            }
        }

        /**
         * Sets the treatmentLibrariesSelectListItems using the treatmentLibraries from state
         */
        @Watch('treatmentLibraries')
        onTreatmentLibrariesChanged() {
            // set treatmentLibrariesSelectListItems' list of SelectItems using treatmentLibraries
            this.treatmentLibrariesSelectListItems = this.treatmentLibraries
                .map((treatmentLibrary: TreatmentLibrary) => ({
                    text: treatmentLibrary.name,
                    value: treatmentLibrary.id.toString()
                }));
            // set allTreatments with all the treatments from all treatment libraries
            this.setAllTreatments();
        }

        /**
         * Sets the scenarioName when a scenarioTreatmentLibrary has been set
         */
        @Watch('scenarioTreatmentLibrary')
        onScenarioTreatmentLibraryChanged() {
            if (this.scenarioTreatmentLibrary.id > 0) {
                this.scenarioName = this.scenarioTreatmentLibrary.name;
                // set all treatments, making sure to add all of the scenario treatment library's treatments
                this.setAllTreatments();
            }
        }

        /**
         * Sets the selected treatment library based on treatmentLibrarySelectItemValue
         */
        @Watch('treatmentLibrarySelectItemValue')
        onTreatmentLibrarySelectItemValueChanged() {
            const id: number = hasValue(this.treatmentLibrarySelectItemValue)
                ? parseInt(this.treatmentLibrarySelectItemValue)
                : 0;
            if (id !== this.selectedTreatmentLibrary.id) {
                this.selectTreatmentLibraryAction({treatmentLibraryId: id});
            }
        }

        /**
         * Sets/resets component properties that rely on the state of whether or not a treatment library has been selected
         */
        @Watch('selectedTreatmentLibrary')
        onSelectedTreatmentLibraryChanged() {
            if (this.selectedTreatmentLibrary.id !== 0) {
                // set linked component properties
                this.hasSelectedTreatmentLibrary = true;
                if (this.selectedTreatment.treatmentLibraryId !== this.selectedTreatmentLibrary.id) {
                    this.treatmentSelectItemValue = hasValue(this.treatmentSelectItemValue) ? '' : '0';
                }
                this.treatmentsSelectListItems = this.selectedTreatmentLibrary.treatments
                    .map((treatment: Treatment) => ({
                        text: treatment.name,
                        value: treatment.id.toString()
                    }));
            } else {
                // reset linked component properties
                this.hasSelectedTreatmentLibrary = false;
                this.treatmentSelectItemValue = hasValue(this.treatmentSelectItemValue) ? '' : '0';
                this.treatmentsSelectListItems = [];
            }
            // set allTreatments with all the treatments from all treatment libraries
            this.setAllTreatments();
        }

        /**
         * Parses treatmentSelectItemValue as an int (if present), then calls the
         * setSelectedTreatment function while passing in the appropriate id parameter
         */
        @Watch('treatmentSelectItemValue')
        onTreatmentSelectItemValueChanged() {
            const id: number = hasValue(this.treatmentSelectItemValue)
                ? parseInt(this.treatmentSelectItemValue)
                : 0;
            if (id !== this.selectedTreatment.id) {
                this.setSelectedTreatment(id);
            }
        }

        /**
         * Sets each list of allFeasibilityIds, allCostsIds, and allConsequencesIds using allTreatmentsIds
         */
        @Watch('allTreatments')
        onAllTreatmentsChanged() {
            // set latestTreatmentId
            this.latestTreatmentId = getLatestPropertyValue('id', this.allTreatments);
            // get all of each: feasibilities, costs, and consequences
            const allFeasibilities: Feasibility[] = [];
            const allCosts: Cost[] = [];
            const allConsequences: Consequence [] = [];
            this.allTreatments.forEach((treatment: Treatment) => {
                if (hasValue(treatment.feasibility)) {
                    allFeasibilities.push(treatment.feasibility as Feasibility);
                }
                allCosts.push(...treatment.costs);
                allConsequences.push(...treatment.consequences);
            });
            // set the 'latest' id for each: feasibilities, costs, and consequences
            this.latestFeasibilityId = getLatestPropertyValue('id', allFeasibilities);
            this.latestCostId = getLatestPropertyValue('id', allCosts);
            this.latestConsequenceId = getLatestPropertyValue('id', allConsequences);
        }

        /**
         * Sets allTreatments with treatments from the scenario treatment library (if present) and the list of
         * treatment libraries
         */
        setAllTreatments() {
            const treatments: Treatment[] = [];
            // add the treatments from the scenario treatment library (if present)
            if (this.scenarioTreatmentLibrary.id > 0) {
                treatments.push(...this.scenarioTreatmentLibrary.treatments);
            }
            // add the treatments from the selected treatment library (if present)
            if (this.selectedTreatmentLibrary.id > 0) {
                treatments.push(...this.selectedTreatmentLibrary.treatments);
            }
            // add the treatments from each treatment library in the list of treatment libraries
            this.treatmentLibraries.forEach((treatmentLibrary: TreatmentLibrary) =>
                treatments.push(...treatmentLibrary.treatments)
            );
            // remove duplicates and set allTreatments
            this.allTreatments = uniq(treatments);
        }

        /**
         * Sets selectedTreatment property based on the value of the id parameter
         * @param id The id of the selected treatment
         */
        setSelectedTreatment(id: number) {
            if (any(propEq('id', id), this.selectedTreatmentLibrary.treatments as Treatment[])) {
                this.selectedTreatment = clone(
                    this.selectedTreatmentLibrary.treatments.find((t: Treatment) => t.id === id) as Treatment
                );
            } else {
                this.selectedTreatment = clone(emptyTreatment);
            }
            this.setTabData();
        }

        /**
         * Sets data for each of the child component tabs
         */
        setTabData() {
            this.tabData = {
                tabTreatmentLibraries: hasValue(this.treatmentLibraries) ? clone(this.treatmentLibraries) : [],
                tabSelectedTreatmentLibrary: clone(this.selectedTreatmentLibrary),
                tabSelectedTreatment: clone(this.selectedTreatment),
                latestFeasibilityId: clone(this.latestFeasibilityId),
                latestCostId: clone(this.latestCostId),
                latestConsequenceId: clone(this.latestConsequenceId)
            };
        }

        /**
         * Clears the selected treatment library by setting treatmentLibrarySelectItemValue to an empty value or 0
         */
        onClearSelectedTreatmentLibrary() {
            this.treatmentLibrarySelectItemValue = hasValue(this.treatmentLibrarySelectItemValue) ? '' : '0';
            // reset activeTab to first tab
            this.activeTab = 0;
        }

        /**
         * 'New Library' button has been clicked
         */
        onNewLibrary() {
            // show the CreateTreatmentLibraryDialog component
            this.createTreatmentLibraryDialogData = {
                ...clone(emptyCreateTreatmentLibraryDialogData),
                showDialog: true
            };
        }

        /**
         * 'Add Treatment' button has been clicked
         */
        onAddTreatment() {
            // show the CreateTreatmentDialog component
            this.showCreateTreatmentDialog = true;
        }

        /**
         * A treatment tab has been clicked
         */
        setAsActiveTab(treatmentTab: number) {
            this.activeTab = treatmentTab;
        }

        /**
         * Updates a selected treatment library by dispatching the updateSelectedTreatmentLibraryAction with the given
         * treatment library parameter
         * @param updatedSelectedTreatmentLibrary The selected treatment data to use for updating
         */
        updateSelectedTreatmentLibrary(updatedSelectedTreatmentLibrary: TreatmentLibrary) {
            this.updateSelectedTreatmentLibraryAction({
                updatedSelectedTreatmentLibrary: updatedSelectedTreatmentLibrary
            }).then(() => {
                setTimeout(() => {
                    this.setSelectedTreatment(this.selectedTreatment.id);
                });
            });
        }

        /**
         * Resets the component ui by discarding the user's changes and resetting the selected treatment library with
         * the current scenario's treatment library data
         */
        onDiscardChanges() {
            this.onClearSelectedTreatmentLibrary();
            if (this.scenarioTreatmentLibrary.id > 0) {
                setTimeout(() => {
                    this.updateSelectedTreatmentLibraryAction({
                        updatedSelectedTreatmentLibrary: clone(this.scenarioTreatmentLibrary)
                    });
                });
            } else {
                setTimeout(() => {
                    this.setIsBusyAction({isBusy: true});
                    this.getScenarioTreatmentLibraryAction({selectedScenarioId: this.selectedScenarioId})
                        .then(() => this.setIsBusyAction({isBusy: false}));
                });
            }
        }

        /**
         * 'Create as New Library' button has been clicked
         */
        onCreateAsNewLibrary() {
            // show the CreateTreatmentLibraryDialog component and include selectedTreatmentLibrary.description &
            // selectedTreatmentLibrary.treatments
            this.createTreatmentLibraryDialogData = {
                showDialog: true,
                selectedTreatmentLibraryDescription: this.selectedTreatmentLibrary.description,
                selectedTreatmentLibraryTreatments: this.selectedTreatmentLibrary.treatments
            };
        }

        /**
         * 'Update Library' button has been clicked
         */
        onUpdateLibrary() {
            // set isBusy to true, then dispatch action to update selected treatment library on the server
            this.setIsBusyAction({isBusy: true});
            this.updateTreatmentLibraryAction({updatedTreatmentLibrary: this.selectedTreatmentLibrary})
                .then(() => {
                    this.setIsBusyAction({isBusy: false});
                    this.setSuccessMessageAction({message: 'Treatment library updated successfully'});
                });
        }

        /**
         * 'Apply' button has been clicked
         */
        onApplyToScenario() {
            this.setIsBusyAction({isBusy: true});
            this.upsertScenarioTreatmentLibraryAction({
                upsertedScenarioTreatmentLibrary: clone(this.selectedTreatmentLibrary)
            })
            .then(() => {
                this.setIsBusyAction({isBusy: false});
                this.setSuccessMessageAction({message: 'Scenario treatment library updated successfully'});
            });
        }

        onCreateTreatmentLibrary(createdTreatmentLibrary: TreatmentLibrary) {
            // hide the CreateTreatmentLibraryDialog component
            this.createTreatmentLibraryDialogData =  clone(emptyCreateTreatmentLibraryDialogData);
            if (!isNil(createdTreatmentLibrary)) {
                // get the latest id of the treatment libraries
                const latestId: number = getLatestPropertyValue('id', this.treatmentLibraries);
                // set the createdTreatmentLibrary.id = latestId + 1 (if present), otherwise set as 1
                createdTreatmentLibrary.id = hasValue(latestId) ? latestId + 1 : 1;
                createdTreatmentLibrary = this.setIdsForNewTreatmentLibraryRelatedData(createdTreatmentLibrary);
                // set isBusy to true, then dispatch action to create the new treatment library
                this.setIsBusyAction({isBusy: true});
                this.createTreatmentLibraryAction({createdTreatmentLibrary: createdTreatmentLibrary})
                    .then(() => {
                        this.setIsBusyAction({isBusy: false});
                        this.setSuccessMessageAction({message: 'Treatment library created successfully'});
                        this.onClearSelectedTreatmentLibrary();
                        setTimeout(() => {
                            this.treatmentLibrarySelectItemValue = createdTreatmentLibrary.id.toString();
                        });
                    });
            }
        }

        /**
         * Sets the ids for treatments and each treatment's feasibility, costs, and consequences
         */
        setIdsForNewTreatmentLibraryRelatedData(createdTreatmentLibrary: TreatmentLibrary) {
            if (hasValue(createdTreatmentLibrary.treatments)) {
                // get next treatment id
                let nextTreatmentId = hasValue(this.latestTreatmentId) ? this.latestTreatmentId + 1 : 1;
                // get next feasibility id
                let nextFeasibilityId: number = hasValue(this.latestFeasibilityId) ? this.latestFeasibilityId + 1 : 1;
                // get next cost id
                let nextCostId: number = hasValue(this.latestCostId) ? this.latestCostId + 1 : 1;
                // get next consequence id
                let nextConsequenceId: number = hasValue(this.latestConsequenceId) ? this.latestConsequenceId + 1 : 1;
                // for each created treatment library's treatments, set the new treatment's treatmentLibraryId & id,
                // then set a new id for each of the treatment's feasibilities, costs, and consequences
                createdTreatmentLibrary.treatments = sortByProperty('id', createdTreatmentLibrary.treatments)
                    .map((treatment: Treatment) => {
                        treatment.treatmentLibraryId = createdTreatmentLibrary.id;
                        treatment.id = nextTreatmentId;
                        nextTreatmentId++;
                        if (hasValue(treatment.feasibility)) {
                            // @ts-ignore
                            treatment.feasibility.id = nextFeasibilityId;
                            nextFeasibilityId++;
                        }
                        if (hasValue(treatment.costs)) {
                            treatment.costs = sortByProperty('id', treatment.costs).map((cost: Cost) => {
                                cost.id = nextCostId;
                                nextCostId++;
                                return cost;
                            });
                        }
                        if (hasValue(treatment.consequences)) {
                            treatment.consequences = sortByProperty('id', treatment.consequences).map((consequence: Consequence) => {
                                consequence.id = nextConsequenceId;
                                nextConsequenceId++;
                                return consequence;
                            });
                        }
                        return treatment;
                    });
            }
            return createdTreatmentLibrary;
        }

        /**
         * User has submitted CreateTreatmentDialog result
         * @param createdTreatment The created treatment data that was submitted
         */
        onCreateTreatment(createdTreatment: Treatment) {
            // hide the CreateTreatmentDialog component
            this.showCreateTreatmentDialog = false;
            if (!isNil(createdTreatment)) {
                // set createdTreatment.treatmentLibraryId = selectedTreatmentLibrary.id
                createdTreatment.treatmentLibraryId = this.selectedTreatmentLibrary.id;
                // set the createdTreatment.id = this.latestTreatmentId + 1 (if present), otherwise set as 1
                createdTreatment.id = hasValue(this.latestTreatmentId) ? this.latestTreatmentId + 1 : 1;
                // dispatch action to update selectedTreatmentLibrary with the created treatment
                this.updateSelectedTreatmentLibraryAction({
                    updatedSelectedTreatmentLibrary: {
                        ...this.selectedTreatmentLibrary,
                        treatments: append(createdTreatment, this.selectedTreatmentLibrary.treatments)
                    }
                }).then(() => this.treatmentSelectItemValue = createdTreatment.id.toString());
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
</style>