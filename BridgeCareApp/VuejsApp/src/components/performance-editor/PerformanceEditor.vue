<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn v-show="selectedScenarioId === 0" class="ara-blue-bg white--text" @click="onNewLibrary">
                        New Library
                    </v-btn>
                    <v-select v-if="!hasSelectedPerformanceLibrary || selectedScenarioId > 0"
                              :items="performanceLibrariesSelectListItems"
                              label="Select a Performance Library" outline
                              v-model="selectItemValue">
                    </v-select>
                    <v-text-field v-if="hasSelectedPerformanceLibrary && selectedScenarioId === 0" label="Library Name"
                                  v-model="selectedPerformanceLibrary.name">
                        <template slot="append">
                            <v-btn class="ara-orange" icon @click="selectItemValue = null">
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedPerformanceLibrary && selectedScenarioId === 0">
                        Owner: {{selectedPerformanceLibrary.owner ? selectedPerformanceLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" v-if="hasSelectedPerformanceLibrary && selectedScenarioId === 0"
                        v-model="selectedPerformanceLibrary.shared" label="Shared"/>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedPerformanceLibrary"></v-divider>
        <v-flex xs12 v-show="hasSelectedPerformanceLibrary">
            <v-layout justify-center class="header-height">
                <v-flex xs8>
                    <v-btn class="ara-blue-bg white--text" @click="showCreatePerformanceLibraryEquationDialog = true">
                        Add
                    </v-btn>
                </v-flex>
            </v-layout>
            <v-layout justify-center class="data-table">
                <v-flex xs8>
                    <v-card>
                        <v-card-title>
                            Performance equation
                            <v-spacer></v-spacer>
                            <v-text-field v-model="searchEquation" append-icon="fas fa-search" lablel="Search" single-line
                                          hide-details>
                            </v-text-field>
                        </v-card-title>
                        <v-data-table :headers="equationsGridHeaders"
                                      :items="equationsGridData"
                                      item-key="performanceLibraryEquationId"
                                      class="elevation-1 fixed-header v-table__overflow"
                                      :search="searchEquation">
                            <template slot="items" slot-scope="props">
                                <td class="text-xs-center">
                                    <v-edit-dialog @save="onEditEquationProperty(props.item.id, 'equationName', props.item.equationName)"
                                                   :return-value.sync="props.item.equationName"
                                                   large lazy persistent>
                                        <v-text-field class="equation-name-text-field-output" readonly :value="props.item.equationName"></v-text-field>
                                        <template slot="input">
                                            <v-text-field v-model="props.item.equationName"
                                                          label="Edit" single-line>
                                            </v-text-field>
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td class="text-xs-center">
                                    <v-edit-dialog @save="onEditEquationProperty(props.item.id, 'attribute', props.item.attribute)"
                                                   :return-value.sync="props.item.attribute"
                                                   large lazy persistent>
                                        <v-text-field class="attribute-text-field-output" readonly :value="props.item.attribute"></v-text-field>
                                        <template slot="input">
                                            <v-select :items="attributesSelectListItems" v-model="props.item.attribute"
                                                      label="Edit">
                                            </v-select>
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td class="text-xs-center">
                                    <v-menu v-show="props.item.equation !== ''" left min-width="500px"
                                            min-height="500px">
                                        <template slot="activator">
                                            <v-btn icon class="ara-blue"><v-icon>fas fa-eye</v-icon></v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea rows="5" no-resize readonly full-width outline
                                                            :value="props.item.equation">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn icon class="edit-icon" @click="onShowEquationEditorDialog(props.item.id)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td class="text-xs-center">
                                    <v-menu v-show="props.item.criteria !== ''" right min-width="500px"
                                            min-height="500px">
                                        <template slot="activator">
                                            <v-btn flat icon class="ara-blue"><v-icon>fas fa-eye</v-icon></v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea rows="5" no-resize readonly full-width outline
                                                            :value="props.item.criteria">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn icon class="edit-icon" @click="onShowCriteriaEditorDialog(props.item.id)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td class="text-xs-center">
                                    <v-btn icon class="ara-orange" @click="onDeleteEquation(props.item.id)">
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
        <v-flex xs12 v-show="hasSelectedPerformanceLibrary && (stateScenarioPerformanceLibrary === null || selectedPerformanceLibrary.id !== stateScenarioPerformanceLibrary.id)">
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea rows="4" no-resize outline label="Description"
                                v-model="selectedPerformanceLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout v-show="hasSelectedPerformanceLibrary" justify-end row>
                <v-btn v-show="selectedScenarioId > 0" class="ara-blue-bg white--text" :disabled="!hasSelectedPerformanceLibrary"
                       @click="onApplyToScenario">
                    Save
                </v-btn>
                <v-btn v-show="selectedScenarioId === 0" class="ara-blue-bg white--text" :disabled="!hasSelectedPerformanceLibrary"
                       @click="onUpdateLibrary">
                    Update Library
                </v-btn>
                <v-btn class="ara-blue-bg white--text" :disabled="!hasSelectedPerformanceLibrary"
                       @click="onCreateAsNewLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId === 0" class="ara-orange-bg white--text" @click="onDeletePerformanceLibrary">
                    Delete Library
                </v-btn>
                <v-btn v-show="selectedScenarioId > 0" class="ara-orange-bg white--text" :disabled="!hasSelectedPerformanceLibrary"
                       @click="onDiscardPerformanceLibraryChanges">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse" />

        <CreatePerformanceLibraryDialog :dialogData="createPerformanceLibraryDialogData"
                                        @submit="onCreatePerformanceLibrary" />

        <CreatePerformanceLibraryEquationDialog :showDialog="showCreatePerformanceLibraryEquationDialog"
                                                @submit="onCreatePerformanceLibraryEquation" />

        <EquationEditorDialog :dialogData="equationEditorDialogData" @submit="onSubmitEquationEditorDialogResult" />

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitCriteriaEditorDialogResult" />
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Watch } from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {State, Action} from 'vuex-class';
    import CreatePerformanceLibraryDialog from './performance-editor-dialogs/CreatePerformanceLibraryDialog.vue';
    import CreatePerformanceLibraryEquationDialog from './performance-editor-dialogs/CreatePerformanceLibraryEquationDialog.vue';
    import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
    import CriteriaEditorDialog from '../../shared/modals/CriteriaEditorDialog.vue';
    import {
        PerformanceLibraryEquation,
        PerformanceLibrary,
        emptyEquation,
        emptyPerformanceLibrary
    } from '@/shared/models/iAM/performance';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {any, propEq, isNil, findIndex, append, find, update, clone} from 'ramda';
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
    const ObjectID = require('bson-objectid');

    @Component({
        components: {CreatePerformanceLibraryDialog, CreatePerformanceLibraryEquationDialog, EquationEditorDialog, CriteriaEditorDialog, Alert}
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

        searchEquation = '';
        performanceLibraries: PerformanceLibrary[] = [];
        selectedPerformanceLibrary: PerformanceLibrary = clone(emptyPerformanceLibrary);
        selectedScenarioId: number = 0;
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
                vm.selectedScenarioId = 0;

                if (to.path === '/PerformanceEditor/Scenario/') {
                    vm.selectedScenarioId = isNaN(parseInt(to.query.selectedScenarioId)) ? 0 : parseInt(to.query.selectedScenarioId);
                    vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;
                    if (vm.selectedScenarioId === 0) {
                        vm.setErrorMessageAction({message: 'No scenario has been selected'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.selectItemValue = null;
                setTimeout(() => {
                    vm.getPerformanceLibrariesAction()
                        .then(() => {
                            if (vm.selectedScenarioId > 0) {
                                vm.getScenarioPerformanceLibraryAction({selectedScenarioId: vm.selectedScenarioId});
                            }
                        });
                });
            });
        }

        /**
         * Component mounted event handler
         */
        mounted() {
            this.setAttributesSelectListItems();
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
            const selectedPerformanceLibrary: PerformanceLibrary = find(
                propEq('id', this.selectItemValue), this.statePerformanceLibraries
            ) as PerformanceLibrary;

            this.selectPerformanceLibraryAction({
                selectedPerformanceLibrary: hasValue(selectedPerformanceLibrary) ? selectedPerformanceLibrary : emptyPerformanceLibrary
            });
        }

        /**
         * Sets component UI/functional properties on a change to the stateSelectedPerformanceLibrary object
         */
        @Watch('stateSelectedPerformanceLibrary')
        onStateSelectedPerformanceLibraryChanged() {
            this.selectedPerformanceLibrary = clone(this.stateSelectedPerformanceLibrary);
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
         * Dispatches an action to append a new performance library equation to the selected performance library's
         * equations list
         */
        onCreatePerformanceLibraryEquation(createdEquation: PerformanceLibraryEquation) {
            this.showCreatePerformanceLibraryEquationDialog = false;

            if (!isNil(createdEquation)) {
                this.selectPerformanceLibraryAction({selectedPerformanceLibrary: {
                        ...this.selectedPerformanceLibrary,
                        equations: append(createdEquation, this.selectedPerformanceLibrary.equations)
                    }
                });
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

                this.selectPerformanceLibraryAction({selectedPerformanceLibrary: {
                        ...this.selectedPerformanceLibrary,
                        equations: update(
                            findIndex(propEq('id', equation.id), this.selectedPerformanceLibrary.equations),
                            setItemPropertyValue(property, value, equation) as PerformanceLibraryEquation,
                            this.selectedPerformanceLibrary.equations
                        )
                    }
                });
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
                this.selectPerformanceLibraryAction({selectedPerformanceLibrary: {
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
                    }
                }).then(() => this.selectedEquation = clone(emptyEquation));
            }
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
                this.selectPerformanceLibraryAction({selectedPerformanceLibrary: {
                        ...this.selectedPerformanceLibrary,
                        equations: update(
                            findIndex(propEq('id', this.selectedEquation.id), this.selectedPerformanceLibrary.equations),
                            setItemPropertyValue('criteria', criteria, this.selectedEquation) as PerformanceLibraryEquation,
                            this.selectedPerformanceLibrary.equations
                        )
                    }
                }).then(() => this.selectedEquation = clone(emptyEquation));
            }
        }

        /**
         * Removes a performance library equation from a performance library's equations property
         */
        onDeleteEquation(id: string) {
            this.selectPerformanceLibraryAction({selectedPerformanceLibrary: {
                    ...this.selectedPerformanceLibrary,
                    equations: this.selectedPerformanceLibrary.equations
                        .filter((equation: PerformanceLibraryEquation) => equation.id !== id)
                }
            });
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
                saveScenarioPerformanceLibraryData: {...this.selectedPerformanceLibrary, id: this.selectedScenarioId},
                objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario
            }).then(() => this.onDiscardPerformanceLibraryChanges());
        }
        /**
         * Resets the component's UI to remove any changes made to a scenario's performance library
         */
        onDiscardPerformanceLibraryChanges() {
            this.selectItemValue = null;
            setTimeout(() => {
                this.selectPerformanceLibraryAction({selectedPerformanceLibrary: this.stateScenarioPerformanceLibrary});
            });
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
                this.deletePerformanceLibraryAction({performanceLibrary: this.selectedPerformanceLibrary});
                this.onClearSelectedPerformanceLibrary();
            }
        }
    }
</script>

<style>
    .performance-editor-container {
        height: 730px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .equation-name-text-field-output {
        margin-left: 10px;
    }

    .attribute-text-field-output {
        margin-left: 15px;
    }

    .dropdown-height{
        height: 75px;
    }

    .header-height{
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