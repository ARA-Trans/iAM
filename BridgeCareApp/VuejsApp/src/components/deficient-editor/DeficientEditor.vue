<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn @click="onAddNewDeficientLibrary" class="ara-blue-bg white--text"
                           v-show="selectedScenarioId === '0'">
                        New Library
                    </v-btn>
                    <v-select :items="deficientLibrariesSelectListItems"
                              label="Select a Deficient Library"
                              outline v-if="!hasSelectedDeficientLibrary || selectedScenarioId !== '0'" v-model="selectItemValue">
                    </v-select>
                    <v-text-field label="Library Name" v-if="hasSelectedDeficientLibrary && selectedScenarioId === '0'"
                                  v-model="selectedDeficientLibrary.name">
                        <template slot="append">
                            <v-btn @click="selectItemValue = null" class="ara-orange" icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedDeficientLibrary && selectedScenarioId === '0'">
                        Owner: {{selectedDeficientLibrary.owner ? selectedDeficientLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" label="Shared"
                                v-if="hasSelectedDeficientLibrary && selectedScenarioId === '0'" v-model="selectedDeficientLibrary.shared"/>
                </v-flex>
            </v-layout>
            <v-flex v-show="hasSelectedDeficientLibrary" xs3>
                <v-btn @click="showCreateDeficientDialog = true" class="ara-blue-bg white--text">Add</v-btn>
                <v-btn :disabled="selectedDeficientIds.length === 0" @click="onDeleteDeficients"
                       class="ara-orange-bg white--text">
                    Delete
                </v-btn>
            </v-flex>
        </v-flex>
        <v-flex xs12>
            <div class="deficients-data-table">
                <v-data-table :headers="deficientDataTableHeaders" :items="deficients" class="elevation-1 fixed-header v-table__overflow"
                              item-key="id" select-all v-model="selectedDeficientRows">
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-checkbox hide-details primary v-model="props.selected"></v-checkbox>
                        </td>
                        <td v-for="header in deficientDataTableHeaders">
                            <div v-if="header.value === 'attribute'">
                                <v-edit-dialog
                                        :return-value.sync="props.item.attribute"
                                        @save="onEditDeficientProperty(props.item, 'attribute', props.item.attribute)" large lazy persistent>
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
                                        @save="onEditDeficientProperty(props.item, header.value, props.item[header.value])" large lazy persistent>
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
                                            <input :value="props.item.criteria" class="output deficient-criteria-output"
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
                                    <v-btn @click="onEditDeficientCriteria(props.item)" class="edit-icon" icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-layout>
                            </div>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>
        <v-flex v-show="hasSelectedDeficientLibrary && selectedDeficientLibrary.id !== stateScenarioDeficientLibrary.id"
                xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea label="Description" no-resize outline rows="4"
                                v-model="selectedDeficientLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedDeficientLibrary" xs12>
            <v-layout justify-end row>
                <v-btn @click="onApplyToScenario" class="ara-blue-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Save
                </v-btn>
                <v-btn @click="onUpdateDeficientLibrary" class="ara-blue-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Update Library
                </v-btn>
                <v-btn @click="onAddAsNewDeficientLibrary" class="ara-blue-bg white--text">
                    Create as New Library
                </v-btn>
                <v-btn @click="onDeleteDeficientLibrary" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Delete Library
                </v-btn>
                <v-btn @click="onDiscardChanges" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse"/>

        <CreateDeficientLibraryDialog :dialogData="createDeficientLibraryDialogData"
                                      @submit="onCreateNewDeficientLibrary"/>

        <CreateDeficientDialog :showDialog="showCreateDeficientDialog" @submit="onSubmitNewDeficient"/>

        <DeficientCriteriaEditor :dialogData="deficientCriteriaEditorDialogData"
                                 @submitCriteriaEditorDialogResult="onSubmitDeficientCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {Action, Getter, State} from 'vuex-class';
    import {Deficient, DeficientLibrary, emptyDeficient, emptyDeficientLibrary} from '@/shared/models/iAM/deficient';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {clone, contains, findIndex, isNil, prepend, propEq, update} from 'ramda';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import CreateDeficientDialog from '@/components/deficient-editor/deficient-editor-dialogs/CreateDeficientDialog.vue';
    import {
        CreateDeficientLibraryDialogData,
        emptyCreateDeficientLibraryDialogData
    } from '@/shared/models/modals/create-deficient-library-dialog-data';
    import {setItemPropertyValue} from '@/shared/utils/setter-utils';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import CreateDeficientLibraryDialog from '@/components/deficient-editor/deficient-editor-dialogs/CreateDeficientLibraryDialog.vue';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';

    @Component({
        components: {
            CreateDeficientLibraryDialog,
            CreateDeficientDialog,
            DeficientCriteriaEditor: CriteriaEditorDialog,
            Alert
        }
    })
    export default class DeficientEditor extends Vue {
        @State(state => state.deficientEditor.deficientLibraries) stateDeficientLibraries: DeficientLibrary[];
        @State(state => state.deficientEditor.selectedDeficientLibrary) stateSelectedDeficientLibrary: DeficientLibrary;
        @State(state => state.deficientEditor.scenarioDeficientLibrary) stateScenarioDeficientLibrary: DeficientLibrary;
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getDeficientLibraries') getDeficientLibrariesAction: any;
        @Action('selectDeficientLibrary') selectDeficientLibraryAction: any;
        @Action('createDeficientLibrary') createDeficientLibraryAction: any;
        @Action('updateDeficientLibrary') updateDeficientLibraryAction: any;
        @Action('deleteDeficientLibrary') deleteDeficientLibraryAction: any;
        @Action('getScenarioDeficientLibrary') getScenarioDeficientLibraryAction: any;
        @Action('saveScenarioDeficientLibrary') saveScenarioDeficientLibraryAction: any;
        @Action('getAttributes') getAttributesAction: any;
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        @Getter('getNumericAttributes') getNumericAttributesGetter: any;

        selectedScenarioId: string = '0';
        deficientLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string | null = null;
        selectedDeficientLibrary: DeficientLibrary = clone(emptyDeficientLibrary);
        hasSelectedDeficientLibrary: boolean = false;
        deficients: Deficient[] = [];
        deficientDataTableHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Name', value: 'name', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Deficient Level', value: 'deficient', align: 'left', sortable: false, class: '', width: ''},
            {
                text: 'Allowed Deficient(%)',
                value: 'percentDeficient',
                align: 'left',
                sortable: false,
                class: '',
                width: ''
            },
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '50%'}
        ];
        numericAttributes: string[] = [];
        selectedDeficientRows: Deficient[] = [];
        selectedDeficientIds: string[] = [];
        selectedDeficient: Deficient = clone(emptyDeficient);
        showCreateDeficientDialog: boolean = false;
        deficientCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        createDeficientLibraryDialogData: CreateDeficientLibraryDialogData = clone(emptyCreateDeficientLibraryDialogData);
        alertBeforeDelete: AlertData = clone(emptyAlertData);
        objectIdMOngoDBForScenario: string = '';

        /**
         * Sets onload component UI properties
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/DeficientEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;
                    if (vm.selectedScenarioId === '0') {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.selectItemValue = null;
                vm.getDeficientLibrariesAction()
                    .then(() => {
                        if (vm.selectedScenarioId !== '0') {
                            vm.getScenarioDeficientLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});
                        }
                    });
            });
        }

        beforeDestroy() {
            this.setHasUnsavedChangesAction({value: false});
        }

        /**
         * Sets the component's deficientLibrariesSelectListItems array using the deficient libraries data in state
         */
        @Watch('stateDeficientLibraries')
        onStateDeficientLibrariesChanged() {
            this.deficientLibrariesSelectListItems = this.stateDeficientLibraries.map((deficientLibrary: DeficientLibrary) => ({
                text: deficientLibrary.name,
                value: deficientLibrary.id
            }));
        }

        /**
         * Dispatches an action to set the selected deficient library in state
         */
        @Watch('selectItemValue')
        onSelectItemValueChanged() {
            this.selectDeficientLibraryAction({selectedLibraryId: this.selectItemValue});
        }

        /**
         * Sets the component's selectedDeficientLibrary object with the selected deficient library data in state
         */
        @Watch('stateSelectedDeficientLibrary')
        onStateSelectedDeficientLibraryChanged() {
            this.selectedDeficientLibrary = clone(this.stateSelectedDeficientLibrary);
        }

        /**
         * Sets component UI properties based on the selected deficient library data
         */
        @Watch('selectedDeficientLibrary')
        onSelectedDeficientLibraryChanged() {
            this.setHasUnsavedChangesAction({
                value: hasUnsavedChanges(
                    'deficient', this.selectedDeficientLibrary, this.stateSelectedDeficientLibrary, this.stateScenarioDeficientLibrary)
            });
            this.hasSelectedDeficientLibrary = this.selectedDeficientLibrary.id !== '0';
            this.deficients = clone(this.selectedDeficientLibrary.deficients);
            if (this.numericAttributes.length === 0) {
                this.numericAttributes = getPropertyValues('name', this.getNumericAttributesGetter);
            }
        }

        /**
         * Sets selectedTargetIds using selectedDeficientRows
         */
        @Watch('selectedDeficientRows')
        onSelectedDeficientRowsChanged() {
            this.selectedDeficientIds = getPropertyValues('id', this.selectedDeficientRows) as string[];
        }

        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            this.numericAttributes = getPropertyValues('name', this.stateNumericAttributes);
        }

        /**
         * Enables the CreateDeficientLibraryDialog
         */
        onAddNewDeficientLibrary() {
            this.createDeficientLibraryDialogData = {
                ...this.createDeficientLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * Adds the CreateDeficientDialog's result to the selected deficient library's deficients list
         * @param newDeficient CreateDeficientDialog's Deficient object result
         */
        onSubmitNewDeficient(newDeficient: Deficient) {
            this.showCreateDeficientDialog = false;

            if (!isNil(newDeficient)) {
                this.selectedDeficientLibrary = {
                    ...this.selectedDeficientLibrary,
                    deficients: prepend(newDeficient, this.selectedDeficientLibrary.deficients)
                };
            }
        }

        /**
         * Updates the selected deficient's property with the value
         * @param deficient Selected Deficient object
         * @param property Selected Deficient object's property
         * @param value Value to set on the selected Deficient object's property
         */
        onEditDeficientProperty(deficient: Deficient, property: string, value: any) {
            this.selectedDeficientLibrary = {
                ...this.selectedDeficientLibrary,
                deficients: update(
                    findIndex(propEq('id', deficient.id), this.selectedDeficientLibrary.deficients),
                    setItemPropertyValue(property, value, deficient) as Deficient,
                    this.selectedDeficientLibrary.deficients
                )
            };
        }

        /**
         * Enables the CriteriaEditorDialog and sends to it the selected deficient's criteria
         * @param deficient Selected Deficient object
         */
        onEditDeficientCriteria(deficient: Deficient) {
            this.selectedDeficient = clone(deficient);

            this.deficientCriteriaEditorDialogData = {
                showDialog: true,
                criteria: deficient.criteria
            };
        }

        /**
         * Updates the selected deficient's criteria with the CriteriaEditorDialog's result
         * @param criteria CriteriaEditorDialog result
         */
        onSubmitDeficientCriteria(criteria: string) {
            this.deficientCriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectedDeficientLibrary = {
                    ...this.selectedDeficientLibrary,
                    deficients: update(
                        findIndex(propEq('id', this.selectedDeficient.id), this.selectedDeficientLibrary.deficients),
                        {...this.selectedDeficient, criteria: criteria},
                        this.selectedDeficientLibrary.deficients
                    )
                };
            }

            this.selectedDeficient = clone(emptyDeficient);
        }

        /**
         * Enables the CreateDeficientLibraryDialog and sends to it the selected deficient library's description &
         * deficients data
         */
        onAddAsNewDeficientLibrary() {
            this.createDeficientLibraryDialogData = {
                showDialog: true,
                description: this.selectedDeficientLibrary.description,
                deficients: this.selectedDeficientLibrary.deficients
            };
        }

        /**
         * Dispatches an action to create a new deficient library in the mongo database
         * @param createdDeficientLibrary New DeficientLibrary object
         */
        onCreateNewDeficientLibrary(createdDeficientLibrary: DeficientLibrary) {
            this.createDeficientLibraryDialogData = clone(emptyCreateDeficientLibraryDialogData);

            if (!isNil(createdDeficientLibrary)) {
                this.createDeficientLibraryAction({createdDeficientLibrary: createdDeficientLibrary})
                    .then(() => this.selectItemValue = createdDeficientLibrary.id);
            }
        }

        /**
         * Dispatches an action to update the scenario's deficient library data in the sql server database
         */
        onApplyToScenario() {
            this.saveScenarioDeficientLibraryAction({
                saveScenarioDeficientLibraryData: {
                    ...this.selectedDeficientLibrary,
                    id: this.selectedScenarioId
                },
                objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario
            }).then(() => this.onDiscardChanges());
        }

        /**
         * Dispatches an action to clear changes made to the selected deficient library
         */
        onDiscardChanges() {
            this.selectItemValue = null;
            setTimeout(() => this.selectDeficientLibraryAction({selectedLibraryId: this.stateScenarioDeficientLibrary.id}));
        }

        /**
         * Dispatches an action to remove selected deficients from the selected deficient library
         */
        onDeleteDeficients() {
            this.selectedDeficientLibrary = {
                ...this.selectedDeficientLibrary,
                deficients: this.selectedDeficientLibrary.deficients
                    .filter((deficient: Deficient) => !contains(deficient.id, this.selectedDeficientIds))
            };
        }

        /**
         * Dispatches an action to update the selected deficient library data in the mongo database
         */
        onUpdateDeficientLibrary() {
            this.updateDeficientLibraryAction({updatedDeficientLibrary: this.selectedDeficientLibrary});
        }

        onDeleteDeficientLibrary() {
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
                this.deleteDeficientLibraryAction({deficientLibrary: this.selectedDeficientLibrary});
            }
        }
    }
</script>

<style>
    .deficients-data-table {
        height: 425px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .deficients-data-table .v-menu--inline, .deficient-criteria-output {
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
