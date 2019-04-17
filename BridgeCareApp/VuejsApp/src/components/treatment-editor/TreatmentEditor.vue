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
                                      label="Select a Treatment Strategy" outline v-model="selectTreatmentStrategyItemValue">
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
                    <v-layout justify-center row fill-height>
                        <v-flex xs3>
                            <v-btn color="info" v-on:click="onAddTreatment">
                                Add Treatment
                            </v-btn>
                            <v-select :items="treatmentsSelectListItems" label="Select a Treatment" outline
                                      clearable v-model="selectTreatmentItemValue">
                            </v-select>
                        </v-flex>
                        <v-flex xs9>
                            <v-tabs v-model="activeTab">
                                <v-tab v-for="(treatmentTab, index) in treatmentTabs" :key="index" ripple>
                                    {{treatmentTab}}
                                </v-tab>
                                <v-tab-item>
                                    <div v-if="activeTab === 'feasibility'">
                                        FEASIBILITY
                                    </div>
                                    <div v-if="activeTab === 'costs'">
                                        COSTS
                                    </div>
                                    <div v-if="activeTab === 'consequences'">
                                        CONSEQUENCES
                                    </div>
                                    <div v-if="activeTab === 'budgets'">
                                        BUDGETS
                                    </div>
                                </v-tab-item>
                            </v-tabs>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedTreatmentStrategy"></v-divider>
                <v-flex xs12>
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
    import CreateTreatmentStrategyDialog
        from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentStrategyDialog.vue';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {
        CreateTreatmentStrategyDialogData,
        emptyCreateTreatmentStrategyDialogData
    } from "@/shared/models/dialogs/treatment-editor-dialogs/create-treatment-strategy-dialog-data";
    import {emptyTreatment, Treatment, TreatmentStrategy} from "@/shared/models/iAM/treatment";
    import {hasValue} from '@/shared/utils/has-value';
    import CreateTreatmentDialog
        from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentDialog.vue';
    import {isNil, append} from 'ramda';
    import {getLatestPropertyValue} from '@/shared/utils/getter-utils';

    @Component({
        components: {CreateTreatmentDialog, CreateTreatmentStrategyDialog}
    })
    export default class TreatmentEditor extends Vue {
        @State(state => state.treatmentEditor.treatmentStrategies) treatmentStrategies: TreatmentStrategy[];
        @State(state => state.treatmentEditor.selectedTreatmentStrategy) selectedTreatmentStrategy: TreatmentStrategy;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getTreatmentStrategies') getTreatmentStrategiesAction: any;
        @Action('selectTreatmentStrategy') selectTreatmentStrategyAction: any;
        @Action('updateSelectedTreatmentStrategy') updateSelectedTreatmentStrategyAction: any;
        @Action('createTreatmentStrategy') createTreatmentStrategyAction: any;
        @Action('updateTreatmentStrategy') updateTreatmentStrategyAction: any;

        hasSelectedTreatmentStrategy: boolean = false;
        treatmentStrategiesSelectListItems: SelectItem[] = [];
        selectTreatmentStrategyItemValue: string = '';
        treatmentsSelectListItems: SelectItem[] = [];
        selectTreatmentItemValue: string = '';
        selectedTreatment: Treatment = {...emptyTreatment};
        activeTab: string = 'feasibility';
        treatmentTabs: string[] = ['feasibility', 'costs', 'consequences', 'budgets'];
        createTreatmentStrategyDialogData: CreateTreatmentStrategyDialogData = {...emptyCreateTreatmentStrategyDialogData};
        showCreateTreatmentDialog: boolean = false;

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
         * Watcher: selectTreatmentStrategyItemValue
         */
        @Watch('selectTreatmentStrategyItemValue')
        onTreatmentStrategiesSelectItemChanged() {
            if (hasValue(this.selectTreatmentStrategyItemValue) && this.selectedTreatmentStrategy.id === 0) {
                // parse selected item as an integer
                const id: number = parseInt(this.selectTreatmentStrategyItemValue);
                // dispatch action to select a treatment strategy in state from the parsed value
                this.selectTreatmentStrategyAction({treatmentStrategyId: id});
            } else if (!hasValue(this.selectTreatmentStrategyItemValue) && this.selectedTreatmentStrategy.id !== 0) {
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
                if (!hasValue(this.selectTreatmentStrategyItemValue)) {
                    this.selectTreatmentStrategyItemValue = this.selectedTreatmentStrategy.id.toString();
                }
                this.hasSelectedTreatmentStrategy = true;
                this.treatmentsSelectListItems = this.selectedTreatmentStrategy.treatments
                    .map((treatment: Treatment) => ({
                        text: treatment.name,
                        value: treatment.id.toString()
                    }));
            } else {
                // reset properties for an unselected treatment strategy
                this.hasSelectedTreatmentStrategy = false;
                this.selectTreatmentStrategyItemValue = '';
                this.selectTreatmentItemValue = '';
                this.treatmentsSelectListItems = [];

            }
        }

        /**
         * Watcher: selectTreatmentItemValue
         */
        @Watch('selectTreatmentItemValue')
        onSelectedTreatmentChanged() {
            if (hasValue(this.selectTreatmentItemValue) && this.selectedTreatment.id === 0) {
                // parse selectTreatmentItemValue as integer to get the id value of the selected treatment
                const id: number = parseInt(this.selectTreatmentItemValue);
                // set selectedTreatment by finding the treatment in the selected treatment strategy's list of treatments
                // using the parsed id
                this.selectedTreatment = this.selectedTreatmentStrategy.treatments.find((treatment: Treatment) =>
                    treatment.id === id
                ) as Treatment;
            } else if (!hasValue(this.selectTreatmentItemValue) && this.selectedTreatment.id !== 0) {
                // set selectedTreatment as an emptyTreatment object
                this.selectedTreatment = {...emptyTreatment};
            }
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
                })
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
            // dispatch action to unselect the selected treatment strategy
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
         * 'Create as New Library' button has been clicked
         */
        onCreateAsNewLibrary() {
            // show the CreateTreatmentStrategyDialog component and include selectedTreatmentStrategy.description &
            // selectedTreatmentStrategy.treatments
            this.createTreatmentStrategyDialogData = {
                showDialog: true,
                selectedTreatmentStrategyDescription: this.selectedTreatmentStrategy.description,
                selectedTreatmentStrategyTreatments: this.selectedTreatmentStrategy.treatments
            }
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
                // set isBusy to true, then dispatch action to create the new treatment strategy
                this.setIsBusyAction({isBusy: true});
                this.createTreatmentStrategyAction({createdTreatmentStrategy: createdTreatmentStrategy})
                    .then(() => this.setIsBusyAction({isBusy: false}))
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
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
                });
            }
        }
    }
</script>