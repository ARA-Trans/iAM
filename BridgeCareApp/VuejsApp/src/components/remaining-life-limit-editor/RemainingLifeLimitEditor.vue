<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn @click="onNewLibrary" class="ara-blue-bg white--text" v-show="selectedScenarioId === '0'">
                        New Library
                    </v-btn>
                    <v-select :items="selectListItems"
                              label="Select a Remaining Life Limit Library" outline v-if="!hasSelectedRemainingLifeLimitLibrary || selectedScenarioId !== '0'"
                              v-model="selectItemValue">
                    </v-select>
                    <v-text-field label="Library Name"
                                  v-if="hasSelectedRemainingLifeLimitLibrary && selectedScenarioId === '0'"
                                  v-model="selectedRemainingLifeLimitLibrary.name">
                        <template slot="append">
                            <v-btn @click="selectItemValue = null" class="ara-orange" icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                </v-flex>
            </v-layout>
            <v-flex v-show="hasSelectedRemainingLifeLimitLibrary" xs3>
                <v-btn @click="onAddRemainingLifeLimit" class="ara-blue-bg white--text">Add</v-btn>
            </v-flex>
        </v-flex>
        <v-flex v-show="hasSelectedRemainingLifeLimitLibrary" xs12>
            <div class="remaining-life-limit-data-table">
                <v-data-table :headers="gridHeaders" :items="gridData"
                              class="elevation-1 fixed-header v-table__overflow">
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
                            <v-text-field :value="props.item.criteria" readonly>
                                <template slot="append-outer">
                                    <v-icon @click="onEditCriteria(props.item, props.item.criteria)" class="edit-icon">
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
            <v-layout justify-end row v-show="hasSelectedRemainingLifeLimitLibrary">
                <v-btn :disabled="!hasSelectedRemainingLifeLimitLibrary" @click="onApplyToScenario" class="ara-blue-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Save
                </v-btn>
                <v-btn :disabled="!hasSelectedRemainingLifeLimitLibrary" @click="onUpdateLibrary" class="ara-blue-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Update Library
                </v-btn>
                <v-btn :disabled="!hasSelectedRemainingLifeLimitLibrary" @click="onCreateAsNewLibrary"
                       class="ara-blue-bg white--text">
                    Create as New Library
                </v-btn>
                <v-btn @click="onDeleteRemainingLifeLimitLibrary" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Delete Library
                </v-btn>
                <v-btn :disabled="!hasSelectedRemainingLifeLimitLibrary" @click="onDiscardChanges" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse"/>

        <CreateRemainingLifeLimitLibraryDialog :dialogData="createRemainingLifeLimitLibraryDialogData"
                                               @submit="onCreateLibrary"/>

        <CreateRemainingLifeLimitDialog :dialogData="createRemainingLifeLimitDialogData"
                                        @submit="onCreateRemainingLifeLimit"/>

        <CriteriaEditorDialog :dialogData="remainingLifeLimitCriteriaEditorDialogData"
                              @submitCriteriaEditorDialogResult="onSubmitEditedCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Action, State} from 'vuex-class';
    import {Watch} from 'vue-property-decorator';
    import {
        emptyRemainingLifeLimit,
        emptyRemainingLifeLimitLibrary,
        RemainingLifeLimit,
        RemainingLifeLimitLibrary
    } from '@/shared/models/iAM/remaining-life-limit';
    import {append, clone, findIndex, isNil, propEq, update} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import CreateRemainingLifeLimitDialog
        from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import {
        CreateRemainingLifeLimitLibraryDialogData,
        emptyCreateRemainingLifeLimitLibraryDialogData
    } from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
    import CreateRemainingLifeLimitLibraryDialog
        from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitLibraryDialog.vue';
    import {
        CreateRemainingLifeLimitDialogData,
        emptyCreateRemainingLifeLimitDialogData
    } from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';

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
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = [];
        scenarioRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(emptyRemainingLifeLimitLibrary);
        selectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(emptyRemainingLifeLimitLibrary);
        selectedScenarioId: string = '0';
        selectItemValue: string | null = '';
        selectListItems: SelectItem[] = [];
        hasSelectedRemainingLifeLimitLibrary: boolean = false;
        gridHeaders: DataTableHeader[] = [
            {
                text: 'Remaining Life Attribute',
                value: 'attribute',
                align: 'left',
                sortable: true,
                class: '',
                width: '12.4%'
            },
            {text: 'Limit', value: 'limit', align: 'left', sortable: true, class: '', width: '12.4%'},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '75%'}
        ];
        gridData: RemainingLifeLimit[] = [];
        numericAttributesSelectListItems: SelectItem[] = [];
        createRemainingLifeLimitDialogData: CreateRemainingLifeLimitDialogData = clone(emptyCreateRemainingLifeLimitDialogData);
        selectedRemainingLifeLimit: RemainingLifeLimit = clone(emptyRemainingLifeLimit);
        remainingLifeLimitCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        createRemainingLifeLimitLibraryDialogData: CreateRemainingLifeLimitLibraryDialogData = clone(
            emptyCreateRemainingLifeLimitLibraryDialogData
        );
        alertBeforeDelete: AlertData = clone(emptyAlertData);
        objectIdMOngoDBForScenario: string = '';

        /**
         * Sets component UI properties that triggers cascading UI updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/RemainingLifeLimitEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;
                    if (vm.selectedScenarioId === '0') {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.selectItemValue = null;
                vm.getRemainingLifeLimitLibrariesAction()
                    .then(() => {
                        if (vm.selectedScenarioId !== '0') {
                            vm.getScenarioRemainingLifeLimitLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});
                        }
                    });
            });
        }

        beforeDestroy() {
            this.setHasUnsavedChangesAction({value: false});
        }

        /**
         * Sets remainingLifeLimitLibraries with a copy of stateRemainingLifeLimitLibraries
         */
        @Watch('stateRemainingLifeLimitLibraries')
        onStateRemainingLifeLimitLibrariesChanged() {
            this.selectListItems = this.stateRemainingLifeLimitLibraries
                .map((remainingLifeLimitLibrary: RemainingLifeLimitLibrary) => ({
                    text: remainingLifeLimitLibrary.name,
                    value: remainingLifeLimitLibrary.id.toString()
                }));
        }

        /**
         * Dispatches selectRemainingLifeLimitLibraryAction with selectItemValue
         */
        @Watch('selectItemValue')
        onSelectItemValueChanged() {
            this.selectRemainingLifeLimitLibraryAction({selectedLibraryId: this.selectItemValue});
        }

        /**
         * Sets selectedRemainingLifeLimitLibrary with a copy of stateSelectedRemainingLifeLimitLibrary
         */
        @Watch('stateSelectedRemainingLifeLimitLibrary')
        onStateSelectedRemainingLifeLimitLibraryChanged() {
            this.selectedRemainingLifeLimitLibrary = clone(this.stateSelectedRemainingLifeLimitLibrary);
        }

        @Watch('selectedRemainingLifeLimitLibrary')
        onSelectedRemainingLifeLimitLibraryChanged() {
            this.setHasUnsavedChangesAction({
                value: hasUnsavedChanges(
                    'remaininglifelimit', this.selectedRemainingLifeLimitLibrary, this.stateSelectedRemainingLifeLimitLibrary, this.stateScenarioRemainingLifeLimitLibrary
                )
            });
            this.hasSelectedRemainingLifeLimitLibrary = this.selectedRemainingLifeLimitLibrary.id !== '0';
            this.gridData = clone(this.selectedRemainingLifeLimitLibrary.remainingLifeLimits);
        }

        /**
         * Sets numericAttributesSelectListItems using stateNumericAttributes
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            this.setAttributesSelectListItems();
        }

        /**
         * Component mounted event handler
         */
        mounted() {
            this.setAttributesSelectListItems();
        }

        /**
         * Setter for the numericAttributesSelectListItems object
         */
        setAttributesSelectListItems() {
            if (hasValue(this.stateNumericAttributes)) {
                this.numericAttributesSelectListItems = this.stateNumericAttributes.map((attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name
                }));
            }
        }

        /**
         * Toggles the CreateRemainingLifeLimitLibraryDialog modal
         */
        onNewLibrary() {
            this.createRemainingLifeLimitLibraryDialogData = {
                ...emptyCreateRemainingLifeLimitLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * Toggles the CreateRemainingLifeLimitDialog modal
         */
        onAddRemainingLifeLimit() {
            this.createRemainingLifeLimitDialogData = {
                showDialog: true,
                numericAttributesSelectListItems: this.numericAttributesSelectListItems
            };
        }

        /**
         * Dispatches an action to append a new RemainingLifeLimit to the selectedRemainingLifeLimitLibrary
         * remainingLifeLimits property
         */
        onCreateRemainingLifeLimit(newRemainingLifeLimit: RemainingLifeLimit) {
            this.createRemainingLifeLimitDialogData = clone(emptyCreateRemainingLifeLimitDialogData);

            if (!isNil(newRemainingLifeLimit)) {
                this.selectedRemainingLifeLimitLibrary = {
                    ...this.selectedRemainingLifeLimitLibrary,
                    remainingLifeLimits: append(
                        newRemainingLifeLimit, this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
                    )
                };
            }
        }

        /**
         * Toggles the CriteriaEditorDialog modal
         */
        onEditCriteria(remainingLifeLimit: RemainingLifeLimit, criteria: string) {
            this.selectedRemainingLifeLimit = remainingLifeLimit;

            this.remainingLifeLimitCriteriaEditorDialogData = {
                showDialog: true,
                criteria: criteria
            };
        }

        /**
         * Modifies a RemainingLifeLimit criteria property with the CriteriaEditorDialog modal result
         */
        onSubmitEditedCriteria(criteria: string) {
            this.remainingLifeLimitCriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectedRemainingLifeLimitLibrary = {
                    ...this.selectedRemainingLifeLimitLibrary,
                    remainingLifeLimits: update(
                        findIndex(propEq('id', this.selectedRemainingLifeLimit.id), this.selectedRemainingLifeLimitLibrary.remainingLifeLimits),
                        {...this.selectedRemainingLifeLimit, criteria: criteria},
                        this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
                    )
                };
            }

            this.selectedRemainingLifeLimit = clone(emptyRemainingLifeLimit);
        }

        /**
         * Toggles the CreateRemainingLifeLimitLibraryDialog modal to create a new RemainingLifeLimitLibrary using
         * the selectedRemainingLifeLimitLibrary data
         */
        onCreateAsNewLibrary() {
            this.createRemainingLifeLimitLibraryDialogData = {
                showDialog: true,
                description: this.selectedRemainingLifeLimitLibrary.description,
                remainingLifeLimits: this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
            };
        }

        /**
         * Dispatches an action to create a new RemainingLifeLimitLibrary in the mongo database
         */
        onCreateLibrary(createdRemainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
            this.createRemainingLifeLimitLibraryDialogData = clone(emptyCreateRemainingLifeLimitLibraryDialogData);

            if (!isNil(createdRemainingLifeLimitLibrary)) {
                this.createRemainingLifeLimitLibraryAction({createdRemainingLifeLimitLibrary: createdRemainingLifeLimitLibrary})
                    .then(() => this.selectItemValue = createdRemainingLifeLimitLibrary.id);
            }
        }

        /**
         * Dispatches an action to modify a scenario's RemainingLifeLimitLibrary data in the sql server database
         */
        onApplyToScenario() {
            this.saveScenarioRemainingLifeLimitLibraryAction({
                saveScenarioRemainingLifeLimitLibraryData: {
                    ...this.selectedRemainingLifeLimitLibrary,
                    id: this.selectedScenarioId
                },
                objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario
            }).then(() => this.onDiscardChanges());
        }

        /**
         * Dispatches an action to reset the selectedRemainingLifeLimitLibrary with the current stateScenarioRemainingLifeLimitLibrary
         */
        onDiscardChanges() {
            this.selectItemValue = null;
            setTimeout(() => this.selectRemainingLifeLimitLibraryAction({selectedLibraryId: this.stateScenarioRemainingLifeLimitLibrary.id}));
        }

        /**
         * Dispatches an action to modify the selectedRemainingLifeLimitLibrary data in the mongo database
         */
        onUpdateLibrary() {
            this.updateRemainingLifeLimitLibraryAction({updatedRemainingLifeLimitLibrary: this.selectedRemainingLifeLimitLibrary});
        }

        onDeleteRemainingLifeLimitLibrary() {
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
                this.selectItemValue = null;
                this.deleteRemainingLifeLimitLibraryAction({remainingLifeLimitLibrary: this.selectedRemainingLifeLimitLibrary});
            }
        }
    }
</script>
