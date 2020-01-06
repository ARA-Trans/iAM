<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onNewLibrary">
                        New Library
                    </v-btn>
                    <v-select v-if="!hasSelectedRemainingLifeLimitLibrary || selectedScenarioId !== '0'"
                              :items="librarySelectListItems" label="Select a Remaining Life Limit Library" outline
                              v-model="librarySelectItemValue">
                    </v-select>
                    <v-text-field v-if="hasSelectedRemainingLifeLimitLibrary && selectedScenarioId === '0'" label="Library Name"
                                  v-model="selectedRemainingLifeLimitLibrary.name">
                        <template slot="append">
                            <v-btn class="ara-orange" icon @click="librarySelectItemValue = '0'">
                                <v-icon>fas fa-times</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                </v-flex>
            </v-layout>
            <v-flex xs3 v-show="hasSelectedRemainingLifeLimitLibrary">
                <v-btn class="ara-blue-bg white--text" @click="onAddRemainingLifeLimit">Add</v-btn>
            </v-flex>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedRemainingLifeLimitLibrary">
            <div class="remaining-life-limit-data-table">
                <v-data-table :headers="gridHeaders" :items="gridData" class="elevation-1 fixed-header v-table__overflow">
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-edit-dialog :return-value.sync="props.item.attribute" large lazy persistent>
                                {{props.item.attribute}}
                                <template slot="input">
                                    <v-select :items="numericAttributesSelectListItems" label="Select an Attribute"
                                              outline v-model="props.item.attribute">
                                    </v-select>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-edit-dialog :return-value.sync="props.item.limit" large lazy persistent>
                                {{props.item.limit}}
                                <template slot="input">
                                    <v-text-field label="Edit" single-line v-model="props.item.limit"></v-text-field>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-text-field readonly :value="props.item.criteria">
                                <template slot="append-outer">
                                    <v-icon class="edit-icon" @click="onEditCriteria(props.item.id, props.item.criteria)">
                                        fas fa-edit
                                    </v-icon>
                                </template>
                            </v-text-field>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>
        <v-flex xs12>
            <v-layout v-show="hasSelectedRemainingLifeLimitLibrary" justify-end row>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-blue-bg white--text" @click="onApplyToScenario"
                       :disabled="!hasSelectedRemainingLifeLimitLibrary">
                    Save
                </v-btn>
                <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onUpdateLibrary"
                       :disabled="!hasSelectedRemainingLifeLimitLibrary">
                    Update Library
                </v-btn>
                <v-btn class="ara-blue-bg white--text" @click="onCreateAsNewLibrary"
                       :disabled="!hasSelectedRemainingLifeLimitLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId === '0'" class="ara-orange-bg white--text" @click="onDeleteRemainingLifeLimitLibrary">
                    Delete Library
                </v-btn>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-orange-bg white--text" @click="onDiscardChanges"
                       :disabled="!hasSelectedRemainingLifeLimitLibrary">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse" />

        <CreateRemainingLifeLimitLibraryDialog :dialogData="createRemainingLifeLimitLibraryDialogData"
                                               @submit="onCreateRemainingLifeLimitLibrary" />

        <CreateRemainingLifeLimitDialog :dialogData="createRemainingLifeLimitDialogData"
                                        @submit="onCreateRemainingLifeLimit" />

        <CriteriaEditorDialog :dialogData="remainingLifeLimitCriteriaEditorDialogData"
                              @submit="onSubmitEditedCriteria" />
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {State, Action} from 'vuex-class';
    import {Watch} from 'vue-property-decorator';
    import {
        emptyRemainingLifeLimitLibrary,
        RemainingLifeLimit,
        RemainingLifeLimitLibrary
    } from '@/shared/models/iAM/remaining-life-limit';
    import {clone, isNil, append} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import CreateRemainingLifeLimitDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import {
        CreateRemainingLifeLimitLibraryDialogData,
        emptyCreateRemainingLifeLimitLibraryDialogData
    } from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
    import CreateRemainingLifeLimitLibraryDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitLibraryDialog.vue';
    import {
        CreateRemainingLifeLimitDialogData,
        emptyCreateRemainingLifeLimitDialogData
    } from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    const ObjectID = require('bson-objectid');
    @Component({
        components: {CreateRemainingLifeLimitLibraryDialog, CreateRemainingLifeLimitDialog, CriteriaEditorDialog, Alert}
    })
    export default class RemainingLifeLimitEditor extends Vue {
        @State(state => state.remainingLifeLimitEditor.remainingLifeLimitLibraries) stateRemainingLifeLimitLibraries: RemainingLifeLimitLibrary[];
        @State(state => state.remainingLifeLimitEditor.scenarioRemainingLifeLimitLibrary) stateScenarioRemainingLifeLimitLibrary: RemainingLifeLimitLibrary;
        @State(state => state.remainingLifeLimitEditor.selectedRemainingLifeLimitLibrary) stateSelectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary;
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('getRemainingLifeLimitLibraries') getRemainingLifeLimitLibrariesAction: any;
        @Action('createRemainingLifeLimitLibrary') createRemainingLifeLimitLibraryAction: any;
        @Action('updateRemainingLifeLimitLibrary') updateRemainingLifeLimitLibraryAction: any;
        @Action('deleteRemainingLifeLimitLibrary') deleteRemainingLifeLimitLibraryAction: any;
        @Action('getScenarioRemainingLifeLimitLibrary') getScenarioRemainingLifeLimitLibraryAction: any;
        @Action('saveScenarioRemainingLifeLimitLibrary') saveScenarioRemainingLifeLimitLibraryAction: any;
        @Action('selectRemainingLifeLimitLibrary') selectRemainingLifeLimitLibraryAction: any;
        @Action('updateSelectedRemainingLifeLimitLibrary') updateSelectedRemainingLifeLimitLibraryAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = [];
        scenarioRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(emptyRemainingLifeLimitLibrary);
        selectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(emptyRemainingLifeLimitLibrary);
        selectedScenarioId: string = '0';
        librarySelectItemValue: string = '';
        librarySelectListItems: SelectItem[] = [];
        hasSelectedRemainingLifeLimitLibrary: boolean = false;
        gridHeaders: DataTableHeader[] = [
            {text: 'Remaining Life Attribute', value: 'attribute', align: 'left', sortable: true, class: '', width: '12.4%'},
            {text: 'Limit', value: 'limit', align: 'left', sortable: true, class: '', width: '12.4%'},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '75%'}
        ];
        gridData: RemainingLifeLimit[] = [];
        numericAttributesSelectListItems: SelectItem[] = [];
        selectedGridRows: any[] = [];
        createRemainingLifeLimitDialogData: CreateRemainingLifeLimitDialogData = clone(emptyCreateRemainingLifeLimitDialogData);
        selectedRemainingLifeLimitForCriteriaEditIndex: number = -1;
        remainingLifeLimitCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        createRemainingLifeLimitLibraryDialogData: CreateRemainingLifeLimitLibraryDialogData = clone(
            emptyCreateRemainingLifeLimitLibraryDialogData
        );
        alertBeforeDelete: AlertData = clone(emptyAlertData);

        /**
         * Sets component UI properties that triggers cascading UI updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/RemainingLifeLimitEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    if (vm.selectedScenarioId === '0') {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.librarySelectItemValue = '0';
                setTimeout(() => {
                    vm.getRemainingLifeLimitLibrariesAction()
                        .then(() => {
                            if (vm.selectedScenarioId !== '0') {
                                vm.getScenarioRemainingLifeLimitLibraryAction({
                                    selectedScenarioId: parseInt(vm.selectedScenarioId)
                                });
                            }
                        });
                });
            });
        }

        /**
         * Resets component UI properties that triggers cascading UI updates
         */
        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === 'RemainingLifeLimitEditor/Library/') {
                this.selectedScenarioId = '0';
                this.librarySelectItemValue = '0';
                next();
            }
        }

        /**
         * Sets remainingLifeLimitLibraries with a copy of stateRemainingLifeLimitLibraries
         */
        @Watch('stateRemainingLifeLimitLibraries')
        onStateRemainingLifeLimitLibrariesChanged() {
            this.remainingLifeLimitLibraries = clone(this.stateRemainingLifeLimitLibraries);
        }

        /**
         * Sets scenarioRemainingLifeLimitLibrary with a copy of stateScenarioRemainingLifeLimitLibrary
         */
        @Watch('stateScenarioRemainingLifeLimitLibrary')
        onStateScenarioRemainingLifeLimitLibraryChanged() {
            this.scenarioRemainingLifeLimitLibrary = clone(this.stateScenarioRemainingLifeLimitLibrary);
        }

        /**
         * Sets selectedRemainingLifeLimitLibrary with a copy of stateSelectedRemainingLifeLimitLibrary
         */
        @Watch('stateSelectedRemainingLifeLimitLibrary')
        onStateSelectedRemainingLifeLimitLibraryChanged() {
            this.selectedRemainingLifeLimitLibrary = clone(this.stateSelectedRemainingLifeLimitLibrary);
        }

        /**
         * Sets numericAttributesSelectListItems using stateNumericAttributes
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            if (hasValue(this.stateNumericAttributes)) {
                this.setAttributesSelectListItems();
            }
        }

        /**
         * Sets the librarySelectListItems using the remainingLifeLimitLibraries
         */
        @Watch('remainingLifeLimitLibraries')
        onRemainingLifeLimitLibrariesChanged() {
            this.librarySelectListItems = this.remainingLifeLimitLibraries.map((remainingLifeLimitLibrary: RemainingLifeLimitLibrary) => ({
                text: remainingLifeLimitLibrary.name,
                value: remainingLifeLimitLibrary.id.toString()
            }));
        }

        /**
         * Dispatches selectRemainingLifeLimitLibraryAction with librarySelectItemValue
         */
        @Watch('librarySelectItemValue')
        onSelectItemValueChanged() {
            this.selectRemainingLifeLimitLibraryAction({remainingLifeLimitLibraryId: this.librarySelectItemValue});
        }

        /**
         *
         */
        @Watch('selectedRemainingLifeLimitLibrary')
        onSelectedRemainingLifeLimitLibraryChanged() {
            if (hasValue(this.selectedRemainingLifeLimitLibrary) && this.selectedRemainingLifeLimitLibrary.id !== '0') {
                this.hasSelectedRemainingLifeLimitLibrary = true;
                this.gridData = this.selectedRemainingLifeLimitLibrary.remainingLifeLimits;
            } else {
                this.hasSelectedRemainingLifeLimitLibrary = false;
                this.gridData = [];
            }
        }

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateNumericAttributes)) {
                this.setAttributesSelectListItems();
            }
        }

        /**
         * Sets the attributes select items using the numeric attributes from state
         */
        setAttributesSelectListItems() {
            this.numericAttributesSelectListItems = this.stateNumericAttributes.map((attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name
            }));
        }

        /**
         * Instantiates a new createRemainingLifeLimitLibraryDialogData object with showDialog set to 'true'
         */
        onNewLibrary() {
            this.createRemainingLifeLimitLibraryDialogData = {
                ...emptyCreateRemainingLifeLimitLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * Instantiates a new createRemainingLifeLimitDialogData object with showDialog set to true and
         * numericAttributesSelectListItems set with this component's numericAttributesSelectListItems
         */
        onAddRemainingLifeLimit() {
            this.createRemainingLifeLimitDialogData = {
                showDialog: true,
                numericAttributesSelectListItems: this.numericAttributesSelectListItems
            };
        }

        /**
         * Dispatches an updateSelectedRemainingLifeLimitLibrary action to update the selectedRemainingLifeLimitLibrary
         * with newRemainingLifeLimit appended to the selectedRemainingLifeLimitLibrary.remainingLifeLimits list
         * @param newRemainingLifeLimit Remaining life limit to append
         */
        onCreateRemainingLifeLimit(newRemainingLifeLimit: RemainingLifeLimit) {
            this.createRemainingLifeLimitDialogData = clone(emptyCreateRemainingLifeLimitDialogData);

            if (!isNil(newRemainingLifeLimit)) {
                this.updateSelectedRemainingLifeLimitLibraryAction({
                    updatedSelectedRemainingLifeLimitLibrary: {
                        ...this.selectedRemainingLifeLimitLibrary,
                        remainingLifeLimits: append(
                            newRemainingLifeLimit, this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
                        )
                    }
                });
            }
        }

        /**
         * Sets the index of the remaining life limit in the selectedRemainingLifeLimitLibrary.remainingLifeLimits list,
         * then instantiates a new remainingLifeLimitCriteriaEditorDialogData object
         * @param remainingLifeLimitId Remaining life limit id
         * @param criteria Remaining life limit criteria
         */
        onEditCriteria(remainingLifeLimitId: any, criteria: string) {
            this.selectedRemainingLifeLimitForCriteriaEditIndex = this.selectedRemainingLifeLimitLibrary
                .remainingLifeLimits.findIndex((r: RemainingLifeLimit) => r.id === remainingLifeLimitId);

            this.remainingLifeLimitCriteriaEditorDialogData = {
                showDialog: true,
                criteria: criteria
            };
        }

        /**
         * Updates the criteria of the remaining life limit at the specified index in the
         * selectedRemainingLifeLimitLibrary.remainingLifeLimits list with the submitted CriteriaEditorDialog criteria,
         * if present, then dispatches an updateSelectedRemainingLifeLimitLibraryAction to update the
         * selectedRemainingLifeLimitLibrary in state
         * @param criteria
         */
        onSubmitEditedCriteria(criteria: string) {
            this.remainingLifeLimitCriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                const remainingLifeLimits = clone(this.selectedRemainingLifeLimitLibrary.remainingLifeLimits);
                remainingLifeLimits[this.selectedRemainingLifeLimitForCriteriaEditIndex].criteria = criteria;

                this.updateSelectedRemainingLifeLimitLibraryAction({
                    updatedSelectedRemainingLifeLimitLibrary: {
                        ...this.selectedRemainingLifeLimitLibrary,
                        remainingLifeLimits: remainingLifeLimits
                    }
                });
            }
        }

        /**
         * Instantiates a new createRemainingLifeLimitLibraryDialogData object with the description and remainingLifeLimits
         * data of the current selectedRemainingLifeLimitLibrary
         */
        onCreateAsNewLibrary() {
            this.createRemainingLifeLimitLibraryDialogData = {
                showDialog: true,
                description: this.selectedRemainingLifeLimitLibrary.description,
                remainingLifeLimits: this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
            };
        }

        /**
         * Dispatches a createRemainingLifeLimitLibraryAction with the createdRemainingLifeLimitLibrary to insert
         * into the Mongo DB
         * @param createdRemainingLifeLimitLibrary Remaining life limit library
         */
        onCreateRemainingLifeLimitLibrary(createdRemainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
            this.createRemainingLifeLimitLibraryDialogData = clone(emptyCreateRemainingLifeLimitLibraryDialogData);

            if (!isNil(createdRemainingLifeLimitLibrary)) {
                createdRemainingLifeLimitLibrary = this.setIdsForNewRemainingLifeLimitLibraryData(createdRemainingLifeLimitLibrary);

                this.createRemainingLifeLimitLibraryAction({createdRemainingLifeLimitLibrary: createdRemainingLifeLimitLibrary})
                    .then(() => {
                        this.librarySelectItemValue = createdRemainingLifeLimitLibrary.id;
                    });
            }
        }

        /**
         * Generates new ids for the createdRemainingLifeLimitLibrary's remaining life limits if it has any
         * @param createdRemainingLifeLimitLibrary Remaining life limit library
         */
        setIdsForNewRemainingLifeLimitLibraryData(createdRemainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
            if (hasValue(createdRemainingLifeLimitLibrary.remainingLifeLimits)) {
                createdRemainingLifeLimitLibrary.remainingLifeLimits.map((remainingLifeLimit: RemainingLifeLimit) => ({
                    ...remainingLifeLimit,
                    id: ObjectID.generate()
                }));
            }

            return createdRemainingLifeLimitLibrary;
        }

        /**
         * Dispatches a saveScenarioRemainingLifeLimitLibraryAction to upsert the scenario's remaining life limit library
         * with the selectedRemainingLifeLimitLibrary
         */
        onApplyToScenario() {
            this.saveScenarioRemainingLifeLimitLibraryAction({saveScenarioRemainingLifeLimitLibraryData: {
                    ...this.selectedRemainingLifeLimitLibrary,
                    id: this.selectedScenarioId.toString(),
                    name: this.scenarioRemainingLifeLimitLibrary.name
                }
            }).then(() => {
                this.librarySelectItemValue = '0';
                setTimeout(() => {
                    this.updateSelectedRemainingLifeLimitLibraryAction({
                        updatedSelectedRemainingLifeLimitLibrary: this.scenarioRemainingLifeLimitLibrary
                    });
                });
            });
        }

        /**
         * Dispatches an updateRemainingLifeLimitLibraryAction to update the selectedRemainingLifeLimitLibrary in the
         * Mongo DB
         */
        onUpdateLibrary() {
            this.updateRemainingLifeLimitLibraryAction({updatedRemainingLifeLimitLibrary: this.selectedRemainingLifeLimitLibrary});
        }

        /**
         * Clears the selected remaining life limit library, then dispatches an updateSelectedRemainingLifeLimitLibraryAction
         * to set the selectedRemainingLifeLimitLibrary with the scenarioRemainingLifeLimitLibrary, if present, otherwise
         * a getScenarioRemainingLifeLimitLibraryAction is dispatched to get the scenarios remaining life limit library
         */
        onDiscardChanges() {
            this.librarySelectItemValue = '0';

            if (this.scenarioRemainingLifeLimitLibrary.id !== '0') {
                setTimeout(() => {
                    this.updateSelectedRemainingLifeLimitLibraryAction({
                        updatedSelectedRemainingLifeLimitLibrary: this.scenarioRemainingLifeLimitLibrary
                    });
                });
            } else {
                setTimeout(() => {
                    this.getScenarioRemainingLifeLimitLibraryAction({selectedScenarioId: this.selectedScenarioId});
                });
            }
        }

        onDeleteRemainingLifeLimitLibrary() {
            this.alertBeforeDelete = {
                showDialog: true,
                heading: 'Warning',
                choice: true,
                message: 'Are you sure you want to delete?'
            };
        }

        /**
         * Clears the selected remaining life limit library and resets the active tab to first tab
         */
        onClearSelectedRemainingLifeLimitLibrary() {
            this.librarySelectItemValue = hasValue(this.librarySelectItemValue) ? '' : '0';
        }

        onSubmitDeleteResponse(response: boolean) {
            this.alertBeforeDelete = clone(emptyAlertData);
            
            if (response) {
                this.deleteRemainingLifeLimitLibraryAction({remainingLifeLimitLibrary: this.selectedRemainingLifeLimitLibrary});
                this.onClearSelectedRemainingLifeLimitLibrary();
            }
        }
    }
</script>