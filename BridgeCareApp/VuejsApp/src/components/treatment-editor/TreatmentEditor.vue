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
                            <v-select v-if="!hasSelectedTreatmentStrategy" :items="treatmentStrategiesSelectListItems"
                                      label="Select a Treatment Strategy" outline v-model="treatmentStrategySelectItemValue">
                            </v-select>
                            <v-text-field v-if="hasSelectedTreatmentStrategy" label="Treatment Name" append-icon="clear"
                                          v-model="selectedTreatmentStrategy.name"
                                          @click:append="onClearTreatmentStrategySelection">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedTreatmentStrategy"></v-divider>
                <v-flex xs12 v-if="hasSelectedTreatmentStrategy">
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
                                                        <FeasibilityTab :feasibilityTabData="tabData" @submit="updateSelectedTreatmentStrategy" />
                                                    </v-card-text>
                                                </v-card>
                                            </v-tab-item>
                                            <v-tab-item>
                                                <v-card>
                                                    <v-card-text class="card-tab-content">
                                                        <CostsTab :costsTabData="tabData" @submit="updateSelectedTreatmentStrategy" />
                                                    </v-card-text>
                                                </v-card>
                                            </v-tab-item>
                                            <v-tab-item>
                                                <v-card>
                                                    <v-card-text class="card-tab-content">
                                                        <ConsequencesTab :consequencesTabData="tabData" @submit="updateSelectedTreatmentStrategy" />
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
                <v-divider v-if="hasSelectedTreatmentStrategy"></v-divider>
                <v-flex xs12 v-if="hasSelectedTreatmentStrategy">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-textarea no-resize outline full-width
                                        :label="selectedTreatmentStrategy.description === '' ? 'Description' : ''"
                                        v-model="selectedTreatmentStrategy.description">
                            </v-textarea>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn color="info lighten-2" v-on:click="onCreateAsNewLibrary" :disabled="!hasSelectedTreatmentStrategy">
                    Create as New Library
                </v-btn>
                <v-btn color="info lighten-1" v-on:click="onUpdateLibrary" :disabled="!hasSelectedTreatmentStrategy">
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

        <CreateTreatmentStrategyDialog :dialogData="createTreatmentStrategyDialogData"
                                       @submit="onCreateTreatmentStrategy" />

        <CreateTreatmentDialog :showDialog="showCreateTreatmentDialog" @submit="onCreateTreatment" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import CreateTreatmentStrategyDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentStrategyDialog.vue';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {
        CreateTreatmentStrategyDialogData,
        emptyCreateTreatmentStrategyDialogData
    } from '@/shared/models/dialogs/treatment-editor-dialogs/create-treatment-strategy-dialog-data';
    import {
        Consequence,
        Cost,
        emptyTreatment,
        Feasibility,
        Treatment,
        TreatmentStrategy
    } from '@/shared/models/iAM/treatment';
    import {hasValue} from '@/shared/utils/has-value';
    import CreateTreatmentDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentDialog.vue';
    import {isNil, append, any, propEq} from 'ramda';
    import {getLatestPropertyValue, getPropertyValues} from '@/shared/utils/getter-utils';
    import FeasibilityTab from '@/components/treatment-editor/treatment-editor-tabs/FeasibilityTab.vue';
    import CostsTab from '@/components/treatment-editor/treatment-editor-tabs/CostsTab.vue';
    import {TabData, emptyTabData} from '@/shared/models/child-components/treatment-editor/tab-data';
    import ConsequencesTab from '@/components/treatment-editor/treatment-editor-tabs/ConsequencesTab.vue';
    import UnderConstruction from '@/components/UnderConstruction.vue';
    import {sortByProperty, sorter} from '@/shared/utils/sorter';

    @Component({
        components: {
            UnderConstruction,
            ConsequencesTab, CostsTab, FeasibilityTab, CreateTreatmentDialog, CreateTreatmentStrategyDialog}
    })
    export default class TreatmentEditor extends Vue {
        @State(state => state.treatmentEditor.treatmentStrategies) treatmentStrategies: TreatmentStrategy[];
        @State(state => state.treatmentEditor.selectedTreatmentStrategy) selectedTreatmentStrategy: TreatmentStrategy;
        // @State(state => state.treatmentEditor.selectedTreatment) selectedTreatment: Treatment;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getTreatmentStrategies') getTreatmentStrategiesAction: any;
        @Action('selectTreatmentStrategy') selectTreatmentStrategyAction: any;
        @Action('updateSelectedTreatmentStrategy') updateSelectedTreatmentStrategyAction: any;
        @Action('selectTreatment') selectTreatmentAction: any;
        @Action('updateSelectedTreatment') updateSelectedTreatmentAction: any;
        @Action('createTreatmentStrategy') createTreatmentStrategyAction: any;
        @Action('updateTreatmentStrategy') updateTreatmentStrategyAction: any;

        hasSelectedTreatmentStrategy: boolean = false;
        treatmentStrategiesSelectListItems: SelectItem[] = [];
        treatmentStrategySelectItemValue: string = '';
        treatmentsSelectListItems: SelectItem[] = [];
        treatmentSelectItemValue: string = '';
        selectedTreatment: Treatment = {...emptyTreatment};
        activeTab: number = 0;
        treatmentTabs: string[] = ['feasibility', 'costs', 'consequences', 'budgets'];
        createTreatmentStrategyDialogData: CreateTreatmentStrategyDialogData = {...emptyCreateTreatmentStrategyDialogData};
        showCreateTreatmentDialog: boolean = false;
        tabData: TabData = {...emptyTabData};

        /**
         * Watcher: treatmentStrategies
         */
        @Watch('treatmentStrategies')
        onTreatmentStrategiesChanged() {
            // set treatmentStrategiesSelectListItems' list of SelectItems using treatmentStrategies
            this.treatmentStrategiesSelectListItems = this.treatmentStrategies
                .map((treatmentStrategy: TreatmentStrategy) => ({
                    text: treatmentStrategy.name,
                    value: treatmentStrategy.id.toString()
                }));

        }

        /**
         * Watcher: treatmentStrategySelectItemValue
         */
        @Watch('treatmentStrategySelectItemValue')
        onTreatmentStrategySelectItemValueChanged() {
            if (hasValue(this.treatmentStrategySelectItemValue) && this.selectedTreatmentStrategy.id === 0) {
                // parse treatmentStrategySelectItemValue as an int, then dispatch an action with the value using
                // selectTreatmentStrategyAction
                const id: number = parseInt(this.treatmentStrategySelectItemValue);
                this.selectTreatmentStrategyAction({treatmentStrategyId: id});
            } else if (!hasValue(this.treatmentStrategySelectItemValue) && this.selectedTreatmentStrategy.id !== 0) {
                // dispatch action to unselect the selected treatment strategy
                this.selectTreatmentStrategyAction({treatmentStrategyId: null});
            }
        }

        /**
         * Watcher: selectedTreatmentStrategy
         */
        @Watch('selectedTreatmentStrategy')
        onSelectedTreatmentStrategyChanged() {
            if (this.selectedTreatmentStrategy.id !== 0) {
                // set properties for a selected treatment strategy
                if (!hasValue(this.treatmentStrategySelectItemValue)) {
                    this.treatmentStrategySelectItemValue = this.selectedTreatmentStrategy.id.toString();
                }
                this.hasSelectedTreatmentStrategy = true;
                this.treatmentsSelectListItems = this.selectedTreatmentStrategy.treatments
                    .map((treatment: Treatment) => ({
                        text: treatment.name,
                        value: treatment.id.toString()
                    }));
            } else {
                // reset properties for an unselected treatment strategy
                this.treatmentStrategySelectItemValue = '';
                this.hasSelectedTreatmentStrategy = false;
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
                // dispatch action to unselect the selected treatment strategy
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
            if (any(propEq('id', id), this.selectedTreatmentStrategy.treatments as Treatment[])) {
                this.selectedTreatment = {
                    ...this.selectedTreatmentStrategy.treatments.find((t: Treatment) => t.id === id) as Treatment
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
                tabTreatmentStrategies: hasValue(this.treatmentStrategies) ? [...this.treatmentStrategies] : [],
                tabSelectedTreatmentStrategy: {...this.selectedTreatmentStrategy},
                tabSelectedTreatment: {...this.selectedTreatment}
            };
        }

        /**
         * Component has been mounted
         */
        mounted() {
            // set isBusy to true, then dispatch action to get all treatment strategies
            this.setIsBusyAction({isBusy: true});
            this.getTreatmentStrategiesAction()
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
            // clear the selected treatment strategy
            this.onClearTreatmentStrategySelection();
        }

        /**
         * 'New Library' button has been clicked
         */
        onNewLibrary() {
            // show the CreateTreatmentStrategyDialog component
            this.createTreatmentStrategyDialogData = {
                ...emptyCreateTreatmentStrategyDialogData,
                showDialog: true
            };
        }

        /**
         * Dispatch an action to clear the selected treatment strategy when a user clicks the 'Clear' button on the
         * selected treatment strategy name text field or the component is about to be destroyed
         */
        onClearTreatmentStrategySelection() {
            this.selectTreatmentStrategyAction({treatmentStrategyId: null});
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
         * Updates a selected treatment strategy by dispatching the updateSelectedTreatmentStrategyAction with the given
         * treatment strategy parameter
         * @param updatedSelectedTreatmentStrategy The selected treatment data to use for updating
         */
        updateSelectedTreatmentStrategy(updatedSelectedTreatmentStrategy: TreatmentStrategy) {
            this.updateSelectedTreatmentStrategyAction({
                updatedSelectedTreatmentStrategy: updatedSelectedTreatmentStrategy
            }).then(() => {
                this.setSelectedTreatment(this.selectedTreatment.id);
            });
        }

        /**
         * 'Create as New Library' button has been clicked
         */
        onCreateAsNewLibrary() {
            // show the CreateTreatmentStrategyDialog component and include selectedTreatmentStrategy.description &
            // selectedTreatmentStrategy.treatments
            this.createTreatmentStrategyDialogData = {
                showDialog: true,
                selectedTreatmentStrategyDescription: this.selectedTreatmentStrategy.description,
                selectedTreatmentStrategyTreatments: this.selectedTreatmentStrategy.treatments
            };
        }

        /**
         * 'Update Library' button has been clicked
         */
        onUpdateLibrary() {
            // set isBusy to true, then dispatch action to update selected treatment strategy on the server
            this.setIsBusyAction({isBusy: true});
            this.updateTreatmentStrategyAction({updatedTreatmentStrategy: this.selectedTreatmentStrategy})
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

        onCreateTreatmentStrategy(createdTreatmentStrategy: TreatmentStrategy) {
            // hide the CreateTreatmentStrategyDialog component
            this.createTreatmentStrategyDialogData =  {...emptyCreateTreatmentStrategyDialogData};
            if (!isNil(createdTreatmentStrategy)) {
                // get the latest id of the treatment strategies
                const latestId: number = getLatestPropertyValue('id', this.treatmentStrategies);
                // set the createdTreatmentStrategy.id = latestId + 1 (if present), otherwise set as 1
                createdTreatmentStrategy.id = hasValue(latestId) ? latestId + 1 : 1;
                createdTreatmentStrategy = this.setIdsForNewTreatmentStrategyRelatedData(createdTreatmentStrategy);
                // set isBusy to true, then dispatch action to create the new treatment strategy
                this.setIsBusyAction({isBusy: true});
                this.createTreatmentStrategyAction({createdTreatmentStrategy: createdTreatmentStrategy})
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
        setIdsForNewTreatmentStrategyRelatedData(createdTreatmentStrategy: TreatmentStrategy) {
            if (hasValue(createdTreatmentStrategy.treatments)) {
                // create list for all treatments, feasibilities, costs, and consequences
                const allTreatments: Treatment[] = [];
                const allFeasibilities: Feasibility[] = [];
                const allCosts: Cost[] = [];
                const allConsequences: Consequence[] = [];
                // get all treatment strategies' treatments
                this.treatmentStrategies.forEach((treatmentStrategy: TreatmentStrategy) => {
                    allTreatments.push(...treatmentStrategy.treatments);
                    // get all a treatment strategy's treatments' feasibilities, costs, and consequences
                    treatmentStrategy.treatments.forEach((treatment: Treatment) => {
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
                // for each created treatment strategy's treatments, set the new treatment's treatmentStrategyId & id,
                // then set a new id for each of the treatment's feasibilities, costs, and consequences
                createdTreatmentStrategy.treatments = sortByProperty('id', createdTreatmentStrategy.treatments)
                    .map((treatment: Treatment) => {
                        treatment.treatmentStrategyId = createdTreatmentStrategy.id;
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
            return createdTreatmentStrategy;
        }

        /**
         * User has submitted CreateTreatmentDialog result
         * @param createdTreatment The created treatment data that was submitted
         */
        onCreateTreatment(createdTreatment: Treatment) {
            // hide the CreateTreatmentDialog component
            this.showCreateTreatmentDialog = false;
            if (!isNil(createdTreatment)) {
                // set createdTreatment.treatmentStrategyId = selectedTreatmentStrategy.id
                createdTreatment.treatmentStrategyId = this.selectedTreatmentStrategy.id;
                // get the latest id of the treatments for the selected treatment strategy
                const latestId: number = getLatestPropertyValue('id', this.selectedTreatmentStrategy.treatments);
                // set the createdTreatment.id = latestId + 1 (if present), otherwise set as 1
                createdTreatment.id = hasValue(latestId) ? latestId + 1 : 1;
                // dispatch action to update selectedTreatmentStrategy with the created treatment
                this.updateSelectedTreatmentStrategyAction({
                    updatedSelectedTreatmentStrategy: {
                        ...this.selectedTreatmentStrategy,
                        treatments: append(createdTreatment, this.selectedTreatmentStrategy.treatments)
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