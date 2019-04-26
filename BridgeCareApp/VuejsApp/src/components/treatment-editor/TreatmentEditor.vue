<template>
    <v-container fluid grid-list-xl>
        <div class="treatment-editor-container">
            <v-layout column>
                <v-flex xs12>
                    <v-layout justify-center fill-height>
                        <v-flex xs3>
                            <v-btn color="info" v-on:click="onNewLibrary">
                                New Library
                            </v-btn>
                            <v-select v-if="!hasSelectedTreatmentLibrary" :items="treatmentLibrariesSelectListItems"
                                      label="Select a Treatment Library" outline v-model="treatmentLibrarySelectItemValue">
                            </v-select>
                            <v-text-field v-if="hasSelectedTreatmentLibrary" label="Treatment Name" append-icon="clear"
                                          v-model="selectedTreatmentLibrary.name"
                                          @click:append="onClearTreatmentLibrarySelection">
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
                            <v-textarea no-resize outline full-width
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
                <v-btn color="info lighten-2" v-on:click="onCreateAsNewLibrary" :disabled="!hasSelectedTreatmentLibrary">
                    Create as New Library
                </v-btn>
                <v-btn color="info lighten-1" v-on:click="onUpdateLibrary" :disabled="!hasSelectedTreatmentLibrary">
                    Update Library
                </v-btn>
                <v-tooltip top>
                    <template slot="activator">
                        <v-btn color="info" v-on:click="onApplyToScenario" :disabled="true">
                            Apply
                        </v-btn>
                    </template>
                    <span>Feature not ready</span>
                </v-tooltip>
            </v-layout>
        </v-footer>

        <CreateTreatmentLibraryDialog :dialogData="createTreatmentLibraryDialogData"
                                      @submit="onCreateTreatmentLibrary" />

        <CreateTreatmentDialog :showDialog="showCreateTreatmentDialog" @submit="onCreateTreatment" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
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
    import {isNil, append, any, propEq} from 'ramda';
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
        // @State(state => state.treatmentEditor.selectedTreatment) selectedTreatment: Treatment;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getTreatmentLibraries') getTreatmentLibrariesAction: any;
        @Action('selectTreatmentLibrary') selectTreatmentLibraryAction: any;
        @Action('updateSelectedTreatmentLibrary') updateSelectedTreatmentLibraryAction: any;
        @Action('selectTreatment') selectTreatmentAction: any;
        @Action('updateSelectedTreatment') updateSelectedTreatmentAction: any;
        @Action('createTreatmentLibrary') createTreatmentLibraryAction: any;
        @Action('updateTreatmentLibrary') updateTreatmentLibraryAction: any;

        hasSelectedTreatmentLibrary: boolean = false;
        treatmentLibrariesSelectListItems: SelectItem[] = [];
        treatmentLibrarySelectItemValue: string = '';
        treatmentsSelectListItems: SelectItem[] = [];
        treatmentSelectItemValue: string = '';
        selectedTreatment: Treatment = {...emptyTreatment};
        activeTab: number = 0;
        treatmentTabs: string[] = ['feasibility', 'costs', 'consequences', 'budgets'];
        createTreatmentLibraryDialogData: CreateTreatmentLibraryDialogData = {...emptyCreateTreatmentLibraryDialogData};
        showCreateTreatmentDialog: boolean = false;
        tabData: TabData = {...emptyTabData};

        /**
         * Watcher: treatmentLibraries
         */
        @Watch('treatmentLibraries')
        onTreatmentLibrariesChanged() {
            // set treatmentLibrariesSelectListItems' list of SelectItems using treatmentLibraries
            this.treatmentLibrariesSelectListItems = this.treatmentLibraries
                .map((treatmentLibrary: TreatmentLibrary) => ({
                    text: treatmentLibrary.name,
                    value: treatmentLibrary.id.toString()
                }));

        }

        /**
         * Watcher: treatmentLibrarySelectItemValue
         */
        @Watch('treatmentLibrarySelectItemValue')
        onTreatmentLibrarySelectItemValueChanged() {
            if (hasValue(this.treatmentLibrarySelectItemValue) && this.selectedTreatmentLibrary.id === 0) {
                // parse treatmentLibrarySelectItemValue as an int, then dispatch an action with the value using
                // selectTreatmentLibraryAction
                const id: number = parseInt(this.treatmentLibrarySelectItemValue);
                this.selectTreatmentLibraryAction({treatmentLibraryId: id});
            } else if (!hasValue(this.treatmentLibrarySelectItemValue) && this.selectedTreatmentLibrary.id !== 0) {
                // dispatch action to unselect the selected treatment library
                this.selectTreatmentLibraryAction({treatmentLibraryId: null});
            }
        }

        /**
         * Watcher: selectedTreatmentLibrary
         */
        @Watch('selectedTreatmentLibrary')
        onSelectedTreatmentLibraryChanged() {
            if (this.selectedTreatmentLibrary.id !== 0) {
                // set properties for a selected treatment library
                if (!hasValue(this.treatmentLibrarySelectItemValue)) {
                    this.treatmentLibrarySelectItemValue = this.selectedTreatmentLibrary.id.toString();
                }
                this.hasSelectedTreatmentLibrary = true;
                this.treatmentsSelectListItems = this.selectedTreatmentLibrary.treatments
                    .map((treatment: Treatment) => ({
                        text: treatment.name,
                        value: treatment.id.toString()
                    }));
            } else {
                // reset properties for an unselected treatment library
                this.treatmentLibrarySelectItemValue = '';
                this.hasSelectedTreatmentLibrary = false;
                this.treatmentSelectItemValue = '';
                this.treatmentsSelectListItems = [];
            }
        }

        /**
         * Watcher: treatmentSelectItemValue => parses treatmentSelectItemValue as an int (if present), then calls the
         * setSelectedTreatment function while passing in the appropriate id parameter
         */
        @Watch('treatmentSelectItemValue')
        onTreatmentSelectItemValueChanged() {
            if (hasValue(this.treatmentSelectItemValue) &&
                (this.selectedTreatment.id === 0 || this.treatmentSelectItemValue !== this.selectedTreatment.id.toString())) {
                // parse treatmentSelectItemValue as an int, then call setSelectedTreatment passing in the parsed value
                this.setSelectedTreatment(parseInt(this.treatmentSelectItemValue));
            } else if (!hasValue(this.treatmentSelectItemValue) && this.selectedTreatment.id !== 0) {
                // dispatch action to unselect the selected treatment library
                this.setSelectedTreatment(0);
            }
            /*const id: number = hasValue(this.treatmentSelectItemValue) ? parseInt(this.treatmentSelectItemValue) : 0;
            this.setSelectedTreatment(id);*/
        }

        /**
         * Sets selectedTreatment property based on the value of the id parameter
         * @param id The id of the selected treatment
         */
        setSelectedTreatment(id: number) {
            if (any(propEq('id', id), this.selectedTreatmentLibrary.treatments as Treatment[])) {
                this.selectedTreatment = {
                    ...this.selectedTreatmentLibrary.treatments.find((t: Treatment) => t.id === id) as Treatment
                };
                if (!hasValue(this.treatmentSelectItemValue) || this.treatmentSelectItemValue !== id.toString()) {
                    this.treatmentSelectItemValue = id.toString();
                }
            } else {
                this.selectedTreatment = {...emptyTreatment};
                if (hasValue(this.treatmentSelectItemValue)) {
                    this.treatmentSelectItemValue = '';
                }
            }
            this.setTabData();
        }

        /**
         * Sets data for each of the child component tabs
         */
        setTabData() {
            this.tabData = {
                tabTreatmentLibraries: hasValue(this.treatmentLibraries) ? [...this.treatmentLibraries] : [],
                tabSelectedTreatmentLibrary: {...this.selectedTreatmentLibrary},
                tabSelectedTreatment: {...this.selectedTreatment}
            };
        }

        /**
         * Component has been mounted
         */
        mounted() {
            // set isBusy to true, then dispatch action to get all treatment libraries
            this.setIsBusyAction({isBusy: true});
            this.getTreatmentLibrariesAction()
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }

        /**
         * Component is about to be destroyed
         */
        beforeDestroy() {
            // clear the selected treatment library
            this.onClearTreatmentLibrarySelection();
        }

        /**
         * 'New Library' button has been clicked
         */
        onNewLibrary() {
            // show the CreateTreatmentLibraryDialog component
            this.createTreatmentLibraryDialogData = {
                ...emptyCreateTreatmentLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * Dispatch an action to clear the selected treatment library when a user clicks the 'Clear' button on the
         * selected treatment library name text field or the component is about to be destroyed
         */
        onClearTreatmentLibrarySelection() {
            this.selectTreatmentLibraryAction({treatmentLibraryId: null});
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
                this.setSelectedTreatment(this.selectedTreatment.id);
            });
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
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }

        /**
         * 'Apply' button has been clicked
         */
        onApplyToScenario() {
            // TODO: add scenario pathway after defined
        }

        onCreateTreatmentLibrary(createdTreatmentLibrary: TreatmentLibrary) {
            // hide the CreateTreatmentLibraryDialog component
            this.createTreatmentLibraryDialogData =  {...emptyCreateTreatmentLibraryDialogData};
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
                        this.setSelectedTreatment(0);
                    })
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
        }

        /**
         * Sets the ids for treatments and each treatment's feasibility, costs, and consequences
         */
        setIdsForNewTreatmentLibraryRelatedData(createdTreatmentLibrary: TreatmentLibrary) {
            if (hasValue(createdTreatmentLibrary.treatments)) {
                // create list for all treatments, feasibilities, costs, and consequences
                const allTreatments: Treatment[] = [];
                const allFeasibilities: Feasibility[] = [];
                const allCosts: Cost[] = [];
                const allConsequences: Consequence[] = [];
                // get all treatment libraries' treatments
                this.treatmentLibraries.forEach((treatmentLibrary: TreatmentLibrary) => {
                    allTreatments.push(...treatmentLibrary.treatments);
                    // get all a treatment library's treatments' feasibilities, costs, and consequences
                    treatmentLibrary.treatments.forEach((treatment: Treatment) => {
                        if (hasValue(treatment.feasibility)) {
                            allFeasibilities.push(treatment.feasibility as Feasibility);
                        }
                        if (hasValue(treatment.costs)) {
                            allCosts.push(...treatment.costs);
                        }
                        if (hasValue(treatment.consequences)) {
                            allConsequences.push(...treatment.consequences);
                        }
                    });
                });
                // get next treatment id
                const latestTreatmentId: number = hasValue(allTreatments)
                    ? getLatestPropertyValue('id', allTreatments) : 0;
                let nextTreatmentId = hasValue(latestTreatmentId) ? latestTreatmentId + 1 : 1;
                // get next feasibility id
                const latestFeasibilityId: number = hasValue(allFeasibilities)
                    ? getLatestPropertyValue('id', allFeasibilities) : 0;
                let nextFeasibilityId: number = hasValue(latestFeasibilityId) ? latestFeasibilityId + 1 : 1;
                // get next cost id
                const latestCostId: number = hasValue(allCosts) ? getLatestPropertyValue('id', allCosts) : 0;
                let nextCostId: number = hasValue(latestCostId) ? latestCostId + 1 : 1;
                // get next consequence id
                const latestConsequenceId: number = hasValue(allConsequences)
                    ? getLatestPropertyValue('id', allConsequences) : 0;
                let nextConsequenceId: number = hasValue(latestConsequenceId) ? latestConsequenceId + 1 : 1;
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
                // get the latest id of the treatments for the selected treatment library
                const latestId: number = getLatestPropertyValue('id', this.selectedTreatmentLibrary.treatments);
                // set the createdTreatment.id = latestId + 1 (if present), otherwise set as 1
                createdTreatment.id = hasValue(latestId) ? latestId + 1 : 1;
                // dispatch action to update selectedTreatmentLibrary with the created treatment
                this.updateSelectedTreatmentLibraryAction({
                    updatedSelectedTreatmentLibrary: {
                        ...this.selectedTreatmentLibrary,
                        treatments: append(createdTreatment, this.selectedTreatmentLibrary.treatments)
                    }
                }).then(() => this.setSelectedTreatment(createdTreatment.id));
            }
        }
    }
</script>

<style>
    .treatment-editor-container {
        height: 750px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .treatments-div {
        height: 390px;
    }

    .card-tab-content {
        height: 375px;
        overflow-x: hidden;
        overflow-y: auto;
    }
</style>