<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn @click="onNewLibrary" class="ara-blue-bg white--text" v-show="selectedScenarioId === '0'">
                        New Library
                    </v-btn>
                    <v-select :items="performanceLibrariesSelectListItems"
                              label="Select a Performance Library"
                              outline v-if="!hasSelectedPerformanceLibrary || selectedScenarioId !== '0'"
                              v-model="selectItemValue">
                    </v-select>
                    <v-text-field label="Library Name" v-if="hasSelectedPerformanceLibrary && selectedScenarioId === '0'"
                                  v-model="selectedPerformanceLibrary.name">
                        <template slot="append">
                            <v-btn @click="selectItemValue = null" class="ara-orange" icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedPerformanceLibrary && selectedScenarioId === '0'">
                        Owner: {{selectedPerformanceLibrary.owner ? selectedPerformanceLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" label="Shared"
                                v-if="hasSelectedPerformanceLibrary && selectedScenarioId === '0'" v-model="selectedPerformanceLibrary.shared"/>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedPerformanceLibrary"></v-divider>
        <v-flex v-show="hasSelectedPerformanceLibrary" xs12>
            <v-layout class="header-height" justify-center>
                <v-flex xs8>
                    <v-btn @click="showCreatePerformanceLibraryEquationDialog = true" class="ara-blue-bg white--text">
                        Add
                    </v-btn>
                </v-flex>
            </v-layout>
            <v-layout class="data-table" justify-center>
                <v-flex xs8>
                    <v-card>
                        <v-card-title>
                            Performance equation
                            <v-spacer></v-spacer>
                            <v-text-field append-icon="fas fa-search" hide-details lablel="Search"
                                          single-line
                                          v-model="searchEquation">
                            </v-text-field>
                        </v-card-title>
                        <v-data-table :headers="equationsGridHeaders"
                                      :items="equationsGridData"
                                      :search="searchEquation"
                                      class="elevation-1 fixed-header v-table__overflow"
                                      item-key="performanceLibraryEquationId">
                            <template slot="items" slot-scope="props">
                                <td class="text-xs-center">
                                    <v-edit-dialog
                                            :return-value.sync="props.item.equationName"
                                            @save="onEditEquationProperty(props.item.id, 'equationName', props.item.equationName)"
                                            large lazy persistent>
                                        <v-text-field :value="props.item.equationName" class="equation-name-text-field-output"
                                                      readonly></v-text-field>
                                        <template slot="input">
                                            <v-text-field label="Edit"
                                                          single-line v-model="props.item.equationName">
                                            </v-text-field>
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td class="text-xs-center">
                                    <v-edit-dialog
                                            :return-value.sync="props.item.attribute"
                                            @save="onEditEquationProperty(props.item.id, 'attribute', props.item.attribute)"
                                            large lazy persistent>
                                        <v-text-field :value="props.item.attribute" class="attribute-text-field-output"
                                                      readonly></v-text-field>
                                        <template slot="input">
                                            <v-select :items="attributesSelectListItems" label="Edit"
                                                      v-model="props.item.attribute">
                                            </v-select>
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td class="text-xs-center">
                                    <v-menu left min-height="500px" min-width="500px"
                                            v-show="props.item.equation !== ''">
                                        <template slot="activator">
                                            <v-btn class="ara-blue" icon>
                                                <v-icon>fas fa-eye</v-icon>
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea :value="props.item.equation" full-width no-resize outline readonly
                                                            rows="5">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn @click="onShowEquationEditorDialog(props.item.id)" class="edit-icon" icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td class="text-xs-center">
                                    <v-menu min-height="500px" min-width="500px" right
                                            v-show="props.item.criteria !== ''">
                                        <template slot="activator">
                                            <v-btn class="ara-blue" flat icon>
                                                <v-icon>fas fa-eye</v-icon>
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea :value="props.item.criteria" full-width no-resize outline readonly
                                                            rows="5">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn @click="onShowCriteriaEditorDialog(props.item.id)" class="edit-icon" icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td class="text-xs-center">
                                    <v-btn @click="onDeleteEquation(props.item.id)" class="ara-orange" icon>
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </td>
                            </template>
                        </v-data-table>
                    </v-card>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedPerformanceLibrary"></v-divider>
        <v-flex v-show="hasSelectedPerformanceLibrary && (stateScenarioPerformanceLibrary === null || selectedPerformanceLibrary.id !== stateScenarioPerformanceLibrary.id)"
                xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea label="Description" no-resize outline rows="4"
                                v-model="selectedPerformanceLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout justify-end row v-show="hasSelectedPerformanceLibrary">
                <v-btn :disabled="!hasSelectedPerformanceLibrary" @click="onApplyToScenario"
                       class="ara-blue-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Save
                </v-btn>
                <v-btn :disabled="!hasSelectedPerformanceLibrary" @click="onUpdateLibrary"
                       class="ara-blue-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Update Library
                </v-btn>
                <v-btn :disabled="!hasSelectedPerformanceLibrary" @click="onCreateAsNewLibrary"
                       class="ara-blue-bg white--text">
                    Create as New Library
                </v-btn>
                <v-btn @click="onDeletePerformanceLibrary" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Delete Library
                </v-btn>
                <v-btn :disabled="!hasSelectedPerformanceLibrary" @click="onDiscardChanges"
                       class="ara-orange-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse"/>

        <CreatePerformanceLibraryDialog :dialogData="createPerformanceLibraryDialogData"
                                        @submit="onCreatePerformanceLibrary"/>

        <CreatePerformanceLibraryEquationDialog :showDialog="showCreatePerformanceLibraryEquationDialog"
                                                @submit="onCreatePerformanceLibraryEquation"/>

        <EquationEditorDialog :dialogData="equationEditorDialogData" @submit="onSubmitEquationEditorDialogResult"/>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData"
                              @submitCriteriaEditorDialogResult="onSubmitCriteriaEditorDialogResult"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Watch} from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {Action, State} from 'vuex-class';
    import CreatePerformanceLibraryDialog from './performance-editor-dialogs/CreatePerformanceLibraryDialog.vue';
    import CreatePerformanceLibraryEquationDialog
        from './performance-editor-dialogs/CreatePerformanceLibraryEquationDialog.vue';
    import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
    import CriteriaEditorDialog from '../../shared/modals/CriteriaEditorDialog.vue';
    import {
        emptyEquation,
        emptyPerformanceLibrary,
        PerformanceLibrary,
        PerformanceLibraryEquation
    } from '@/shared/models/iAM/performance';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {any, append, clone, find, findIndex, isNil, propEq, update} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {
        CreatePerformanceLibraryDialogData,
        emptyCreatePerformanceLibraryDialogData
    } from '@/shared/models/modals/create-performance-library-dialog-data';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {
        emptyEquationEditorDialogData,
        EquationEditorDialogData
    } from '@/shared/models/modals/equation-editor-dialog-data';
    import {EquationEditorDialogResult} from '@/shared/models/modals/equation-editor-dialog-result';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    import {setItemPropertyValue} from '@/shared/utils/setter-utils';
    import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';

    const ObjectID = require('bson-objectid');

    @Component({
        components: {
            CreatePerformanceLibraryDialog,
            CreatePerformanceLibraryEquationDialog,
            EquationEditorDialog,
            CriteriaEditorDialog,
            Alert
        }
    })
    export default class PerformanceEditor extends Vue {
        @State(state => state.performanceEditor.performanceLibraries) statePerformanceLibraries: PerformanceLibrary[];
        @State(state => state.performanceEditor.selectedPerformanceLibrary) stateSelectedPerformanceLibrary: PerformanceLibrary;
        @State(state => state.performanceEditor.scenarioPerformanceLibrary) stateScenarioPerformanceLibrary: PerformanceLibrary;
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('getPerformanceLibraries') getPerformanceLibrariesAction: any;
        @Action('selectPerformanceLibrary') selectPerformanceLibraryAction: any;
        @Action('createPerformanceLibrary') createPerformanceLibraryAction: any;
        @Action('updatePerformanceLibrary') updatePerformanceLibraryAction: any;
        @Action('deletePerformanceLibrary') deletePerformanceLibraryAction: any;
        @Action('getScenarioPerformanceLibrary') getScenarioPerformanceLibraryAction: any;
        @Action('saveScenarioPerformanceLibrary') saveScenarioPerformanceLibraryAction: any;
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        searchEquation = '';
        performanceLibraries: PerformanceLibrary[] = [];
        selectedPerformanceLibrary: PerformanceLibrary = clone(emptyPerformanceLibrary);
        selectedScenarioId: string = '0';
        hasSelectedPerformanceLibrary: boolean = false;
        performanceLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string | null = '';
        equationsGridHeaders: DataTableHeader[] = [
            {text: 'Name', value: 'equationName', align: 'center', sortable: true, class: '', width: ''},
            {text: 'Attribute', value: 'attribute', align: 'center', sortable: true, class: '', width: ''},
            {text: 'Equation', value: 'equation', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'center', sortable: false, class: '', width: ''},
            {text: '', value: '', align: 'center', sortable: false, class: '', width: ''}
        ];
        equationsGridData: PerformanceLibraryEquation[] = [];
        attributesSelectListItems: SelectItem[] = [];
        selectedEquation: PerformanceLibraryEquation = clone(emptyEquation);
        createPerformanceLibraryDialogData: CreatePerformanceLibraryDialogData = clone(emptyCreatePerformanceLibraryDialogData);
        equationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        showCreatePerformanceLibraryEquationDialog = false;
        alertBeforeDelete: AlertData = clone(emptyAlertData);
        objectIdMOngoDBForScenario: string = '';

        /**
         * beforeRouteEnter event handler
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/PerformanceEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;
                    if (vm.selectedScenarioId === 0) {
                        vm.setErrorMessageAction({message: 'No scenario has been selected'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.selectItemValue = null;
                vm.getPerformanceLibrariesAction()
                    .then(() => {
                        if (vm.selectedScenarioId !== '0') {
                            vm.getScenarioPerformanceLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});
                        }
                    });
            });
        }

        /**
         * Component mounted event handler
         */
        mounted() {
            this.setAttributesSelectListItems();
        }

        beforeDestroy() {
            this.setHasUnsavedChangesAction({value: false});
        }

        /**
         * Setter for the performanceLibrariesSelectListItems object
         */
        @Watch('statePerformanceLibraries')
        onStatePerformanceLibrariesChanged() {
            this.performanceLibrariesSelectListItems = this.statePerformanceLibraries
                .map((performanceLibrary: PerformanceLibrary) => ({
                    text: performanceLibrary.name,
                    value: performanceLibrary.id.toString()
                }));
        }

        /**
         * Dispatches an action to set the selected performance library using the selectItemValue property
         */
        @Watch('selectItemValue')
        onPerformanceLibrariesSelectItemChanged() {
            this.selectPerformanceLibraryAction({selectedLibraryId: this.selectItemValue});
        }

        /**
         * Sets component UI/functional properties on a change to the stateSelectedPerformanceLibrary object
         */
        @Watch('stateSelectedPerformanceLibrary')
        onStateSelectedPerformanceLibraryChanged() {
            this.selectedPerformanceLibrary = clone(this.stateSelectedPerformanceLibrary);
        }

        @Watch('selectedPerformanceLibrary')
        onSelectedPerformanceLibraryChanged() {
            this.setHasUnsavedChangesAction({
                value: hasUnsavedChanges(
                    'performance', this.selectedPerformanceLibrary, this.stateSelectedPerformanceLibrary, this.stateScenarioPerformanceLibrary
                )
            });
            this.hasSelectedPerformanceLibrary = this.selectedPerformanceLibrary.id !== '0';
            this.equationsGridData = clone(this.selectedPerformanceLibrary.equations);
        }

        /**
         * Calls the setAttributesSelectListItems function on a change to the stateNumericAttributes object
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            this.setAttributesSelectListItems();
        }

        /**
         * Setter for the attributesSelectListItems object
         */
        setAttributesSelectListItems() {
            if (hasValue(this.stateNumericAttributes)) {
                this.attributesSelectListItems = this.stateNumericAttributes.map((attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name
                }));
            }
        }

        /**
         * Triggers the create performance library modal
         */
        onNewLibrary() {
            this.createPerformanceLibraryDialogData = {...emptyCreatePerformanceLibraryDialogData, showDialog: true};
        }

        /**
         * Appends new equation to selected performance library's equations list
         */
        onCreatePerformanceLibraryEquation(createdEquation: PerformanceLibraryEquation) {
            this.showCreatePerformanceLibraryEquationDialog = false;

            if (!isNil(createdEquation)) {
                this.selectedPerformanceLibrary = {
                    ...this.selectedPerformanceLibrary,
                    equations: append(createdEquation, this.selectedPerformanceLibrary.equations)
                };
            }
        }

        /**
         * Modifies a performance library equation's property with the given value
         */
        onEditEquationProperty(id: string, property: string, value: any) {
            if (any(propEq('id', id), this.selectedPerformanceLibrary.equations)) {
                const equation: PerformanceLibraryEquation = find(
                    propEq('id', id), this.selectedPerformanceLibrary.equations
                ) as PerformanceLibraryEquation;

                this.selectedPerformanceLibrary = {
                    ...this.selectedPerformanceLibrary,
                    equations: update(
                        findIndex(propEq('id', equation.id), this.selectedPerformanceLibrary.equations),
                        setItemPropertyValue(property, value, equation) as PerformanceLibraryEquation,
                        this.selectedPerformanceLibrary.equations
                    )
                };
            }
        }

        /**
         * Shows the equation editor modal for modifying a performance library equation's equation property
         */
        onShowEquationEditorDialog(id: string) {
            this.selectedEquation = find(
                propEq('id', id), this.selectedPerformanceLibrary.equations
            ) as PerformanceLibraryEquation;

            if (!isNil(this.selectedEquation)) {
                this.equationEditorDialogData = {
                    showDialog: true,
                    equation: this.selectedEquation.equation,
                    canBePiecewise: true,
                    isPiecewise: this.selectedEquation.piecewise
                };
            }
        }

        /**
         * Modifies a performance library equation's equation property with the equation editor modal result
         */
        onSubmitEquationEditorDialogResult(result: EquationEditorDialogResult) {
            this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

            if (!isNil(result)) {
                this.selectedPerformanceLibrary = {
                    ...this.selectedPerformanceLibrary,
                    equations: update(
                        findIndex(propEq('id', this.selectedEquation.id), this.selectedPerformanceLibrary.equations),
                        {
                            ...this.selectedEquation,
                            equation: result.equation,
                            piecewise: result.isPiecewise,
                            isFunction: result.isFunction
                        },
                        this.selectedPerformanceLibrary.equations
                    )
                };
            }

            this.selectedEquation = clone(emptyEquation);
        }

        /**
         * Shows the criteria editor modal for modifying a performance library equation's criteria property
         */
        onShowCriteriaEditorDialog(id: string) {
            this.selectedEquation = find(
                propEq('id', id), this.selectedPerformanceLibrary.equations
            ) as PerformanceLibraryEquation;

            if (!isNil(this.selectedEquation)) {
                this.criteriaEditorDialogData = {
                    showDialog: true,
                    criteria: this.selectedEquation.criteria
                };
            }
        }

        /**
         * Modifies a performance library equation's criteria property with the criteria editor modal result
         */
        onSubmitCriteriaEditorDialogResult(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectedPerformanceLibrary = {
                    ...this.selectedPerformanceLibrary,
                    equations: update(
                        findIndex(propEq('id', this.selectedEquation.id), this.selectedPerformanceLibrary.equations),
                        {...this.selectedEquation, criteria: criteria},
                        this.selectedPerformanceLibrary.equations
                    )
                };
            }

            this.selectedEquation = clone(emptyEquation);
        }

        /**
         * Removes a performance library equation from a performance library's equations property
         */
        onDeleteEquation(id: string) {
            this.selectedPerformanceLibrary = {
                ...this.selectedPerformanceLibrary,
                equations: this.selectedPerformanceLibrary.equations
                    .filter((equation: PerformanceLibraryEquation) => equation.id !== id)
            };
        }

        /**
         * Triggers the create performance library modal
         */
        onCreateAsNewLibrary() {
            this.createPerformanceLibraryDialogData = {
                showDialog: true,
                description: this.selectedPerformanceLibrary.description,
                equations: this.selectedPerformanceLibrary.equations
            };
        }

        /**
         * Dispatches an action to create a new performance library
         */
        onCreatePerformanceLibrary(createdPerformanceLibrary: PerformanceLibrary) {
            this.createPerformanceLibraryDialogData = clone(emptyCreatePerformanceLibraryDialogData);

            if (!isNil(createdPerformanceLibrary)) {
                this.createPerformanceLibraryAction({createdPerformanceLibrary: createdPerformanceLibrary})
                    .then(() => this.selectItemValue = createdPerformanceLibrary.id);
            }
        }

        /**
         * Dispatches an action to modify a selected performance library
         */
        onUpdateLibrary() {
            this.updatePerformanceLibraryAction({updatedPerformanceLibrary: this.selectedPerformanceLibrary});
        }

        /**
         * Dispatches an action to modify a scenario's performance library
         */
        onApplyToScenario() {
            this.saveScenarioPerformanceLibraryAction({
                saveScenarioPerformanceLibraryData: {
                    ...this.selectedPerformanceLibrary,
                    id: this.selectedScenarioId
                },
                objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario
            }).then(() => this.onDiscardChanges());
        }

        /**
         * Resets the component's UI to remove any changes made to a scenario's performance library
         */
        onDiscardChanges() {
            this.selectItemValue = null;
            setTimeout(() => this.selectPerformanceLibraryAction({selectedLibraryId: this.stateScenarioPerformanceLibrary.id}));
        }

        onDeletePerformanceLibrary() {
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
                this.deletePerformanceLibraryAction({performanceLibrary: this.selectedPerformanceLibrary});
            }
        }
    }
</script>

<style>
    .equation-name-text-field-output {
        margin-left: 10px;
    }

    .attribute-text-field-output {
        margin-left: 15px;
    }

    .header-height {
        height: 45px;
    }

    .sharing label {
        padding-top: 0.5em;
    }

    .sharing {
        padding-top: 0;
        margin: 0;
    }
</style>
