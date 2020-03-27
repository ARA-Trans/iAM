<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn @click="onAddNewTargetLibrary" class="ara-blue-bg white--text"
                           v-show="selectedScenarioId === '0'">
                        New Library
                    </v-btn>
                    <v-select :items="targetLibrariesSelectListItems"
                              label="Select a Target Library"
                              outline v-if="!hasSelectedTargetLibrary || selectedScenarioId !== '0'" v-model="selectItemValue">
                    </v-select>
                    <v-text-field label="Library Name" v-if="hasSelectedTargetLibrary && selectedScenarioId === '0'"
                                  v-model="selectedTargetLibrary.name">
                        <template slot="append">
                            <v-btn @click="selectItemValue = null" class="ara-orange" icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedTargetLibrary && selectedScenarioId === '0'">
                        Owner: {{selectedTargetLibrary.owner ? selectedTargetLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" label="Shared"
                                v-if="hasSelectedTargetLibrary && selectedScenarioId === '0'" v-model="selectedTargetLibrary.shared"/>
                </v-flex>
            </v-layout>
            <v-flex v-show="hasSelectedTargetLibrary" xs3>
                <v-btn @click="showCreateTargetDialog = true" class="ara-blue-bg white--text">Add</v-btn>
                <v-btn :disabled="selectedTargetIds.length === 0" @click="onDeleteTargets"
                       class="ara-orange-bg white--text">
                    Delete
                </v-btn>
            </v-flex>
        </v-flex>
        <v-flex v-show="hasSelectedTargetLibrary" xs12>
            <div class="targets-data-table">
                <v-data-table :headers="targetDataTableHeaders" :items="targets" class="elevation-1 fixed-header v-table__overflow" item-key="id"
                              select-all v-model="selectedTargetRows">
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-checkbox hide-details primary v-model="props.selected"></v-checkbox>
                        </td>
                        <td v-for="header in targetDataTableHeaders">
                            <div v-if="header.value === 'attribute'">
                                <v-edit-dialog
                                        :return-value.sync="props.item.attribute"
                                        @save="onEditTargetProperty(props.item, 'attribute', props.item.attribute)" large lazy persistent>
                                    <input :value="props.item.attribute" class="output" readonly type="text"/>
                                    <template slot="input">
                                        <v-select :items="numericAttributes" label="Select an Attribute"
                                                  v-model="props.item.attribute">
                                        </v-select>
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-if="header.value !== 'attribute' && header.value !== 'criteria'">
                                <v-edit-dialog
                                        :return-value.sync="props.item[header.value]"
                                        @save="onEditTargetProperty(props.item, header.value, props.item[header.value])" large lazy persistent>
                                    <input :value="props.item[header.value]" class="output" readonly type="text"/>
                                    <template slot="input">
                                        <v-text-field label="Edit" single-line v-model="props.item[header.value]">
                                        </v-text-field>
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-if="header.value === 'criteria'">
                                <v-layout align-center row style="flex-wrap:nowrap">
                                    <v-menu bottom min-height="500px" min-width="500px">
                                        <template slot="activator">
                                            <input :value="props.item.criteria" class="output target-criteria-output"
                                                   readonly type="text"/>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea :value="props.item.criteria" full-width no-resize outline readonly
                                                            rows="5">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn @click="onEditTargetCriteria(props.item)" class="edit-icon" icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-layout>
                            </div>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>
        <v-flex v-show="hasSelectedTargetLibrary && selectedTargetLibrary.id !== stateScenarioTargetLibrary.id" xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea label="Description" no-resize outline rows="4"
                                v-model="selectedTargetLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedTargetLibrary" xs12>
            <v-layout justify-end row>
                <v-btn @click="onApplyTargetLibraryToScenario" class="ara-blue-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Save
                </v-btn>
                <v-btn @click="onUpdateTargetLibrary" class="ara-blue-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Update Library
                </v-btn>
                <v-btn :disabled="!hasSelectedTargetLibrary" @click="onAddAsNewTargetLibrary"
                       class="ara-blue-bg white--text">
                    Create as New Library
                </v-btn>
                <v-btn @click="onDeleteTargetLibrary" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Delete Library
                </v-btn>
                <v-btn @click="onDiscardTargetLibraryChanges" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse"/>

        <CreateTargetLibraryDialog :dialogData="createTargetLibraryDialogData" @submit="onCreateNewTargetLibrary"/>

        <CreateTargetDialog :showDialog="showCreateTargetDialog" @submit="onSubmitNewTarget"/>

        <TargetCriteriaEditor :dialogData="targetCriteriaEditorDialogData" @submit="onSubmitTargetCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {Action, Getter, State} from 'vuex-class';
    import {emptyTarget, emptyTargetLibrary, Target, TargetLibrary} from '@/shared/models/iAM/target';
    import {clone, contains, findIndex, isNil, prepend, propEq, update} from 'ramda';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import CreateTargetDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetDialog.vue';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {setItemPropertyValue} from '@/shared/utils/setter-utils';
    import {
        CreateTargetLibraryDialogData,
        emptyCreateTargetLibraryDialogData
    } from '@/shared/models/modals/create-target-library-dialog-data';
    import CreateTargetLibraryDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetLibraryDialog.vue';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';

    @Component({
        components: {CreateTargetLibraryDialog, CreateTargetDialog, TargetCriteriaEditor: CriteriaEditorDialog, Alert}
    })
    export default class TargetEditor extends Vue {
        @State(state => state.targetEditor.targetLibraries) stateTargetLibraries: TargetLibrary[];
        @State(state => state.targetEditor.selectedTargetLibrary) stateSelectedTargetLibrary: TargetLibrary;
        @State(state => state.targetEditor.scenarioTargetLibrary) stateScenarioTargetLibrary: TargetLibrary;
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getTargetLibraries') getTargetLibrariesAction: any;
        @Action('selectTargetLibrary') selectTargetLibraryAction: any;
        @Action('createTargetLibrary') createTargetLibraryAction: any;
        @Action('updateTargetLibrary') updateTargetLibraryAction: any;
        @Action('deleteTargetLibrary') deleteTargetLibraryAction: any;
        @Action('getScenarioTargetLibrary') getScenarioTargetLibraryAction: any;
        @Action('saveScenarioTargetLibrary') saveScenarioTargetLibraryAction: any;
        @Action('getAttributes') getAttributesAction: any;
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        @Getter('getNumericAttributes') getNumericAttributesGetter: any;

        selectedScenarioId: string = '0';
        targetLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string | null = '';
        selectedTargetLibrary: TargetLibrary = clone(emptyTargetLibrary);
        hasSelectedTargetLibrary: boolean = false;
        targets: Target[] = [];
        targetDataTableHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Name', value: 'name', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Year', value: 'year', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Target', value: 'targetMean', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '50%'}
        ];
        numericAttributes: string[] = [];
        selectedTargetRows: Target[] = [];
        selectedTargetIds: string[] = [];
        selectedTarget: Target = clone(emptyTarget);
        showCreateTargetDialog: boolean = false;
        targetCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        createTargetLibraryDialogData: CreateTargetLibraryDialogData = clone(emptyCreateTargetLibraryDialogData);
        alertBeforeDelete: AlertData = clone(emptyAlertData);
        objectIdMOngoDBForScenario: string = '';

        /**
         * Sets onload component UI properties
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/TargetEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;
                    if (vm.selectedScenarioId === '0') {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.selectItemValue = null;
                vm.getTargetLibrariesAction()
                    .then(() => {
                        if (vm.selectedScenarioId !== '0') {
                            vm.getScenarioTargetLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});
                        }
                    });
            });
        }

        /**
         * Sets the component's targetLibrariesSelectListItems array with the target libraries data in state
         */
        @Watch('stateTargetLibraries')
        onStateTargetLibrariesChanged() {
            this.targetLibrariesSelectListItems = this.stateTargetLibraries.map((targetLibrary: TargetLibrary) => ({
                text: targetLibrary.name,
                value: targetLibrary.id
            }));
        }

        /**
         * Dispatches an action to set the selected target library in state
         */
        @Watch('selectItemValue')
        onSelectItemValueChanged() {
            this.selectTargetLibraryAction({selectedLibraryId: this.selectItemValue});
        }

        /**
         * Sets the component's selectedTargetLibrary object with the selected target library data in state
         */
        @Watch('stateSelectedTargetLibrary')
        onStateSelectedTargetLibraryChanged() {
            this.selectedTargetLibrary = clone(this.stateSelectedTargetLibrary);
        }

        /**
         * Sets component UI properties based on the selected target library data
         */
        @Watch('selectedTargetLibrary')
        onSelectedTargetLibraryChanged() {
            this.setHasUnsavedChangesAction({
                value: hasUnsavedChanges(
                    'target', this.selectedTargetLibrary, this.stateSelectedTargetLibrary, this.stateScenarioTargetLibrary
                )
            });
            this.hasSelectedTargetLibrary = this.selectedTargetLibrary.id !== '0';
            this.targets = clone(this.selectedTargetLibrary.targets);
            if (this.numericAttributes.length === 0) {
                this.numericAttributes = getPropertyValues('name', this.getNumericAttributesGetter);
            }
        }

        /**
         * Sets selectedTargetIds using selectedTargetRows
         */
        @Watch('selectedTargetRows')
        onSelectedTargetRowsChanged() {
            this.selectedTargetIds = getPropertyValues('id', this.selectedTargetRows) as string[];
        }

        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            this.numericAttributes = getPropertyValues('name', this.stateNumericAttributes);
        }

        /**
         * Sets selectItemValue to '' or '0' to de-select the selected target library
         */
        onClearSelectedTargetLibrary() {
            this.selectItemValue = hasValue(this.selectItemValue) ? '' : '0';
        }

        /**
         * Enables the CreateTargetLibraryDialog
         */
        onAddNewTargetLibrary() {
            this.createTargetLibraryDialogData = {
                ...this.createTargetLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * Adds the CreateTargetDialog's result to the selected target library's targets list
         * @param newTarget CreateTargetDialog's Target object result
         */
        onSubmitNewTarget(newTarget: Target) {
            this.showCreateTargetDialog = false;

            if (!isNil(newTarget)) {
                this.selectedTargetLibrary = {
                    ...this.selectedTargetLibrary,
                    targets: prepend(newTarget, this.selectedTargetLibrary.targets)
                };
            }
        }

        /**
         * Updates the selected target's property with the value
         * @param target Selected Target object
         * @param property Selected Target object's property
         * @param value Value to set on the selected Target object's propertyd
         */
        onEditTargetProperty(target: Target, property: string, value: any) {
            this.selectedTargetLibrary = {
                ...this.selectedTargetLibrary,
                targets: update(
                    findIndex(propEq('id', target.id), this.selectedTargetLibrary.targets),
                    setItemPropertyValue(property, value, target) as Target,
                    this.selectedTargetLibrary.targets
                )
            };
        }

        /**
         * Enables the CriteriaEditorDialog and sends to it the selected target's criteria
         * @param target Selected Target object
         */
        onEditTargetCriteria(target: Target) {
            this.selectedTarget = clone(target);

            this.targetCriteriaEditorDialogData = {
                showDialog: true,
                criteria: target.criteria
            };
        }

        /**
         * Updates the selected target's criteria with the CriteriaEditorDialog's result
         * @param criteria CriteriaEditorDialog result
         */
        onSubmitTargetCriteria(criteria: string) {
            this.targetCriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectedTargetLibrary = {
                    ...this.selectedTargetLibrary,
                    targets: update(
                        findIndex(propEq('id', this.selectedTarget.id), this.selectedTargetLibrary.targets),
                        {...this.selectedTarget, criteria: criteria},
                        this.selectedTargetLibrary.targets
                    )
                };
            }

            this.selectedTarget = clone(emptyTarget);
        }

        /**
         * Enables the CreateTargetLibraryDialog and sends it the selected target library's description & targets data
         */
        onAddAsNewTargetLibrary() {
            this.createTargetLibraryDialogData = {
                showDialog: true,
                description: this.selectedTargetLibrary.description,
                targets: this.selectedTargetLibrary.targets
            };
        }

        /**
         * Dispatches an action to create a new target library in the mongo database
         */
        onCreateNewTargetLibrary(createdTargetLibrary: TargetLibrary) {
            this.createTargetLibraryDialogData = clone(emptyCreateTargetLibraryDialogData);

            if (!isNil(createdTargetLibrary)) {
                this.createTargetLibraryAction({createdTargetLibrary: createdTargetLibrary})
                    .then(() => this.selectItemValue = createdTargetLibrary.id);
            }
        }

        /**
         * Dispatches an action to update the scenario's target library data in the sql server database
         */
        onApplyTargetLibraryToScenario() {
            this.saveScenarioTargetLibraryAction({
                saveScenarioTargetLibraryData: {
                    ...this.selectedTargetLibrary,
                    id: this.stateScenarioTargetLibrary.id
                },
                objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario
            });
        }

        /**
         * Dispatches an action to clear changes made to the selected target library
         */
        onDiscardTargetLibraryChanges() {
            this.selectItemValue = null;
            setTimeout(() => this.selectTargetLibraryAction({selectedLibraryId: this.stateScenarioTargetLibrary.id}));
        }

        /**
         * Dispatches an action to remove selected targets from the selected target library
         */
        onDeleteTargets() {
            this.selectedTargetLibrary = {
                ...this.selectedTargetLibrary,
                targets: this.selectedTargetLibrary.targets
                    .filter((target: Target) => !contains(target.id, this.selectedTargetIds))
            };
        }

        /**
         * Dispatches an action to update the selected target library data in the mongo database
         */
        onUpdateTargetLibrary() {
            this.updateTargetLibraryAction({updatedTargetLibrary: this.selectedTargetLibrary});
        }

        onDeleteTargetLibrary() {
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
                this.deleteTargetLibraryAction({targetLibrary: this.selectedTargetLibrary});
            }
        }
    }
</script>

<style>
    .targets-data-table {
        height: 425px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .targets-data-table .v-menu--inline, .target-criteria-output {
        width: 100%;
    }

    .sharing label {
        padding-top: 0.5em;
    }

    .sharing {
        padding-top: 0;
        margin: 0;
    }
</style>
