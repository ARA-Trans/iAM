<template>
    <v-container fluid grid-list-xl>
        <div class="performance-editor-container">
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
                            <v-select v-if="!hasSelectedPerformanceLibrary || selectedScenarioId > 0"
                                      :items="performanceLibrariesSelectListItems"
                                      label="Select a Performance Library" outline
                                      v-model="selectItemValue">
                            </v-select>
                            <v-text-field v-if="hasSelectedPerformanceLibrary && selectedScenarioId === 0"
                                          label="Library Name" append-icon="clear" v-model="selectedPerformanceLibrary.name"
                                          @click:append="onClearSelectedPerformanceLibrary">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedPerformanceLibrary"></v-divider>
                <v-flex xs12 v-if="hasSelectedPerformanceLibrary">
                    <v-layout justify-center fill-height>
                        <v-flex xs8>
                            <v-btn color="info" v-on:click="onAddEquation">
                                Add
                            </v-btn>
                        </v-flex>
                    </v-layout>
                    <v-layout justify-center fill-height>
                        <v-flex xs8>
                            <v-layout fill-height>
                                <div class="data-table">
                                    <v-data-table :headers="equationsGridHeaders"
                                                  :items="equationsGridData"
                                                  item-key="performanceLibraryEquationId"
                                                  class="elevation-1 fixed-header v-table__overflow" hide-actions>
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
                                                <v-menu v-if="props.item.equation !== ''" left min-width="500px"
                                                        min-height="500px">
                                                    <template slot="activator">
                                                        <v-btn flat icon>
                                                            <v-icon>visibility</v-icon>
                                                        </v-btn>
                                                    </template>
                                                    <v-card>
                                                        <v-card-text>
                                                            <v-textarea rows="5" no-resize readonly full-width outline
                                                                        :value="props.item.equation">
                                                            </v-textarea>
                                                        </v-card-text>
                                                    </v-card>
                                                </v-menu>
                                                <v-btn flat icon color="success" v-on:click="onShowEquationEditorDialog(props.item.id)">
                                                    <v-icon>edit</v-icon>
                                                </v-btn>
                                            </td>
                                            <td class="text-xs-center">
                                                <v-menu v-if="props.item.criteria !== ''" right min-width="500px"
                                                        min-height="500px">
                                                    <template slot="activator">
                                                        <v-btn flat icon>
                                                            <v-icon>visibility</v-icon>
                                                        </v-btn>
                                                    </template>
                                                    <v-card>
                                                        <v-card-text>
                                                            <v-textarea rows="5" no-resize readonly full-width outline
                                                                        :value="props.item.criteria">
                                                            </v-textarea>
                                                        </v-card-text>
                                                    </v-card>
                                                </v-menu>
                                                <v-btn flat icon color="success" v-on:click="onShowCriteriaEditorDialog(props.item.id)">
                                                    <v-icon>edit</v-icon>
                                                </v-btn>
                                            </td>
                                            <td class="text-xs-center">
                                                <v-btn flat icon color="error" v-on:click="onDeleteEquation(props.item.id)">
                                                    <v-icon>delete</v-icon>
                                                </v-btn>
                                            </td>
                                        </template>
                                    </v-data-table>
                                </div>
                            </v-layout>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedPerformanceLibrary"></v-divider>
                <v-flex xs12 v-if="hasSelectedPerformanceLibrary">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-textarea rows="4" no-resize outline full-width
                                        :label="selectedPerformanceLibrary.description === '' ? 'Description' : ''"
                                        v-model="selectedPerformanceLibrary.description">
                            </v-textarea>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn v-show="selectedScenarioId > 0" color="error lighten-1" v-on:click="onDiscardChanges"
                       :disabled="!hasSelectedPerformanceLibrary">
                    Discard Changes
                </v-btn>
                <v-btn color="info lighten-2" v-on:click="onCreateAsNewLibrary" :disabled="!hasSelectedPerformanceLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId === 0" color="info lighten-1" v-on:click="onUpdateLibrary"
                       :disabled="!hasSelectedPerformanceLibrary">
                    Update Library
                </v-btn>
                <v-btn v-show="selectedScenarioId > 0" color="info" v-on:click="onApplyToScenario"
                       :disabled="!hasSelectedPerformanceLibrary">
                    Apply
                </v-btn>
            </v-layout>
        </v-footer>

        <CreatePerformanceLibraryDialog :dialogData="createPerformanceLibraryDialogData"
                                         @submit="onCreatePerformanceLibrary" />

        <CreatePerformanceLibraryEquationDialog :showDialog="showCreatePerformanceLibraryEquationDialog"
                                                 @submit="onCreatePerformanceLibraryEquation" />

        <EquationEditor :dialogData="equationEditorDialogData" @submit="onSubmitEquationEditorDialogResult" />

        <CriteriaEditor :dialogData="criteriaEditorDialogData" @submit="onSubmitCriteriaEditorDialogResult" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Watch } from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {State, Action} from 'vuex-class';
    import CreatePerformanceLibraryDialog from './performance-editor-dialogs/CreatePerformanceLibraryDialog.vue';
    import CreatePerformanceLibraryEquationDialog from './performance-editor-dialogs/CreatePerformanceLibraryEquationDialog.vue';
    import EquationEditor from '../../shared/dialogs/EquationEditor.vue';
    import CriteriaEditor from '../../shared/dialogs/CriteriaEditor.vue';
    import {PerformanceLibraryEquation, PerformanceLibrary, emptyEquation} from '@/shared/models/iAM/performance';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {any, propEq, clone, isNil, findIndex, append, isEmpty, uniq} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value';
    import {
        CreatePerformanceLibraryDialogData,
        emptyCreatePerformanceLibraryDialogData
    } from '@/shared/models/dialogs/performance-editor-dialogs/create-performance-library-dialog-data';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/dialogs/criteria-editor-dialog/criteria-editor-dialog-data';
    import {
        emptyEquationEditorDialogData,
        EquationEditorDialogData
    } from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-data';
    import {getLatestPropertyValue} from '@/shared/utils/getter-utils';
    import {EquationEditorDialogResult} from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-result';
    import {sortByProperty} from '@/shared/utils/sorter';

    @Component({
        components: {CreatePerformanceLibraryDialog: CreatePerformanceLibraryDialog, CreatePerformanceLibraryEquationDialog, EquationEditor, CriteriaEditor}
    })
    export default class PerformanceEditor extends Vue {
        @State(state => state.performanceEditor.performanceLibraries) performanceLibraries: PerformanceLibrary[];
        @State(state => state.performanceEditor.selectedPerformanceLibrary) selectedPerformanceLibrary: PerformanceLibrary;
        @State(state => state.performanceEditor.scenarioPerformanceLibrary) scenarioPerformanceLibrary: PerformanceLibrary;
        @State(state => state.attribute.attributes) attributes: string[];
        @State(state => state.breadcrumb.navigation) navigation: any[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getPerformanceLibraries') getPerformanceLibrariesAction: any;
        @Action('selectPerformanceLibrary') selectPerformanceLibraryAction: any;
        @Action('createPerformanceLibrary') createPerformanceLibraryAction: any;
        @Action('updatePerformanceLibrary') updatePerformanceLibraryAction: any;
        @Action('updateSelectedPerformanceLibrary') updateSelectedPerformanceLibraryAction: any;
        @Action('getScenarioPerformanceLibrary') getScenarioPerformanceLibraryAction: any;
        @Action('upsertScenarioPerformanceLibrary') upsertScenarioPerformanceLibraryAction: any;
        @Action('getAttributes') getAttributesAction: any;
        @Action('setNavigation') setNavigationAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;

        selectedScenarioId: number = 0;
        scenarioName: string = '';
        hasSelectedPerformanceLibrary: boolean = false;
        performanceLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string = '';
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
        createPerformanceLibraryDialogData: CreatePerformanceLibraryDialogData = {
            ...emptyCreatePerformanceLibraryDialogData
        };
        equationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        showCreatePerformanceLibraryEquationDialog = false;
        allPerformanceLibraries: PerformanceLibrary[] = [];
        latestLibraryId: number = 0;
        latestEquationId: number = 0;

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/PerformanceEditor/FromScenario/') {
                    vm.selectedScenarioId = isNaN(parseInt(to.query.simulationId)) ? 0 : parseInt(to.query.simulationId);
                    if (vm.selectedScenarioId === 0) {
                        // set 'no selected scenario' error message, then redirect user to Scenario UI
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                    vm.setNavigationAction([
                        {
                            text: 'Scenario dashboard',
                            to: {path: '/Scenarios/', query: {}}
                        },
                        {
                            text: 'Scenario editor',
                            to: {path: '/EditScenario/', query: {simulationId: to.query.simulationId}}
                        },
                        {
                            text: 'Performance editor',
                            to: {path: '/PerformanceEditor/FromScenario/', query: {simulationId: to.query.simulationId}}
                        }
                    ]);
                } else {
                    next((vm: any) => {
                        vm.setNavigationAction([]);
                    });
                }
                vm.onClearSelectedPerformanceLibrary();
                setTimeout(() => {
                    vm.setIsBusyAction({isBusy: true});
                    vm.getPerformanceLibrariesAction()
                        .then(() => {
                            if (vm.selectedScenarioId > 0) {
                                vm.getScenarioPerformanceLibraryAction({selectedScenarioId: vm.selectedScenarioId})
                                    .then(() => vm.setIsBusyAction({isBusy: false}));
                            } else {
                                vm.setIsBusyAction({isBusy: false});
                            }
                        });
                }, 0);
            });
        }

        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === '/PerformanceEditor/Library/') {
                this.selectedScenarioId = 0;
                this.onClearSelectedPerformanceLibrary();
                next();
            }
        }

        /**
         * Sets the performanceLibrariesSelectListItems using the performanceLibraries from state
         */
        @Watch('performanceLibraries')
        onPerformanceLibrariesChanged(performanceLibraries: PerformanceLibrary[]) {
            this.performanceLibrariesSelectListItems = performanceLibraries
                .map((performanceLibrary: PerformanceLibrary) => ({
                    text: performanceLibrary.name,
                    value: performanceLibrary.id.toString()
                }));
            // set allPerformanceLibraries
            this.setAllPerformanceLibraries();
        }

        /**
         * Sets scenarioName using the scenarioPerformanceLibrary from state
         */
        @Watch('scenarioPerformanceLibrary')
        onScenarioPerformanceLibraryChanged() {
            this.scenarioName = this.scenarioPerformanceLibrary.name;
            // set allPerformanceLibraries
            this.setAllPerformanceLibraries();
        }

        /**
         * Sets the selected performance library based on selectItemValue
         */
        @Watch('selectItemValue')
        onPerformanceLibrariesSelectItemChanged() {
            const id: number = hasValue(this.selectItemValue) ? parseInt(this.selectItemValue) : 0;
            if (id !== this.selectedPerformanceLibrary.id) {
                this.selectPerformanceLibraryAction({performanceLibraryId: id});
            }
        }

        /**
         * Sets/resets the UI component properties when a performance library has been selected/unselected
         */
        @Watch('selectedPerformanceLibrary')
        onSelectedPerformanceLibraryChanged() {
            if (this.selectedPerformanceLibrary.id !== 0) {
                // set
                this.hasSelectedPerformanceLibrary = true;
                this.equationsGridData = this.selectedPerformanceLibrary.equations;
                if (isEmpty(this.attributes)) {
                    // dispatch an action to get all attributes
                    this.setIsBusyAction({isBusy: true});
                    this.getAttributesAction()
                        .then(() => this.setIsBusyAction({isBusy: false}));
                }
            } else {
                // reset
                this.hasSelectedPerformanceLibrary = false;
                this.equationsGridData = [];
            }
            // set allPerformanceLibraries
            this.setAllPerformanceLibraries();
        }

        /**
         * Sets the attributesSelectListItems using the attributes from state
         */
        @Watch('attributes')
        onAttributesChanged() {
            if (!isEmpty(this.attributes)) {
                this.attributesSelectListItems = this.attributes.map((attribute: string) => ({
                    text: attribute,
                    value: attribute
                }));
            }
        }

        @Watch('allPerformanceLibraries')
        onAllPerformanceLibrariesChanged() {
            // set latestLibraryId
            this.latestLibraryId = getLatestPropertyValue('id', this.allPerformanceLibraries);
            // set latestEquationId
            const equations: PerformanceLibraryEquation[] = [];
            this.allPerformanceLibraries.forEach((performanceLibrary) => {
                equations.push(...performanceLibrary.equations);
            });
            this.latestEquationId = getLatestPropertyValue('id', equations);
        }

        /**
         * Sets the latestLibraryId & latestEquationId properties
         */
        setAllPerformanceLibraries() {
            // add all performance libraries from state
            const libraries: PerformanceLibrary[] = [...this.performanceLibraries];
            // add scenario performance library from state if present
            if (this.scenarioPerformanceLibrary.id > 0) {
                libraries.push(this.scenarioPerformanceLibrary);
            }
            // add selected performance library from state if present
            if (this.selectedPerformanceLibrary.id > 0) {
                libraries.push(this.selectedPerformanceLibrary);
            }
            // remove duplicates and set allPerformanceLibraries
            this.allPerformanceLibraries = uniq(libraries);
        }

        /**
         * Clears the selected performance library by setting selectItemValue to an empty string or 0
         */
        onClearSelectedPerformanceLibrary() {
            this.selectItemValue = hasValue(this.selectItemValue) ? '' : '0';
        }

        /**
         * 'New Library' button has been clicked
         */
        onNewLibrary() {
            // create new CreatePerformanceLibraryDialogData object
            this.createPerformanceLibraryDialogData = {
                ...emptyCreatePerformanceLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * 'Add Equation' button has been clicked
         */
        onAddEquation() {
            // set showCreatePerformanceLibraryEquationDialog to true to show CreatePerformanceLibraryEquationDialog
            this.showCreatePerformanceLibraryEquationDialog = true;
        }

        /**
         * User has submitted a createdEquation dialog result
         */
        onCreatePerformanceLibraryEquation(createdEquation: PerformanceLibraryEquation) {
            // set showCreatePerformanceLibraryEquationDialog to false to hide CreatePerformanceLibraryEquationDialog
            this.showCreatePerformanceLibraryEquationDialog = false;
            // if there is a createdPerformanceLibraryEquation
            if (!isNil(createdEquation)) {
                // set the equation id & performanceLibraryId
                createdEquation.id = hasValue(this.latestEquationId) ? this.latestEquationId + 1 : 1;
                createdEquation.performanceLibraryId = this.selectedPerformanceLibrary.id;
                // update the selected performance library's equations with the new equation
                this.updateSelectedPerformanceLibraryAction({
                    updatedSelectedPerformanceLibrary: {
                        ...this.selectedPerformanceLibrary,
                        equations: append(createdEquation, this.selectedPerformanceLibrary.equations)
                    }
                });
            }
        }

        /**
         * A performance library equation's name/attribute property has been edited
         * @param id: Performance library equation id
         * @param property Performance library equation property
         * @param value Value to set on the performance library equation property
         */
        onEditEquationProperty(id: number, property: string, value: any) {
            if (any(propEq('id', id), this.selectedPerformanceLibrary.equations)) {
                // get the selected performance library's list of equations
                const updatedEquations: PerformanceLibraryEquation[] = this.selectedPerformanceLibrary.equations;
                // find the index of the updated equation in the list of equations
                const index = findIndex(propEq('id', id), updatedEquations);
                // update the specified property of the equation at the specified index with the given value
                // @ts-ignore
                updatedEquations[index][property] = value;
                // dispatch action to update the selected performance library's equations
                this.updateSelectedPerformanceLibraryAction({
                    updatedSelectedPerformanceLibrary: {
                        ...this.selectedPerformanceLibrary,
                        equations: updatedEquations
                    }
                });
            }
        }

        /**
         * 'Edit Equation' button has been clicked
         * @param id The id of the selected equation for edit
         */
        onShowEquationEditorDialog(id: number) {
            // get the selected equation from the selected performance library's equations list using the specified id
            this.selectedEquation = this.selectedPerformanceLibrary.equations
                .find((equation: PerformanceLibraryEquation) => equation.id === id) as PerformanceLibraryEquation;
            if (!isNil(this.selectedEquation)) {
                // create a new equationEditorDialogData object using selectedEquation data
                this.equationEditorDialogData = {
                    showDialog: true,
                    equation: this.selectedEquation.equation,
                    canBePiecewise: true,
                    isPiecewise: this.selectedEquation.piecewise,
                    isFunction: this.selectedEquation.isFunction
                };
            }
        }

        /**
         * User has submitted PerformanceEquationEditorDialog result
         * @param result The submitted dialog result
         */
        onSubmitEquationEditorDialogResult(result: EquationEditorDialogResult) {
            // reset equationEditorDialogData
            this.equationEditorDialogData = clone(emptyEquationEditorDialogData);
            // check that a result was submitted
            if (!isNil(result)) {
                // get the selected performance library's equations
                const updatedEquations: PerformanceLibraryEquation[] = this.selectedPerformanceLibrary.equations;
                // find the index of the submitted equation in the list of equations
                const index = findIndex(propEq('id', this.selectedEquation.id), updatedEquations);
                // update the updatedEquation at the index
                updatedEquations[index] = {
                    ...this.selectedEquation,
                    equation: result.equation,
                    piecewise: result.isPiecewise,
                    isFunction: result.isFunction
                };
                // reset the selectedEquation property
                this.selectedEquation = clone(emptyEquation);
                // dispatch an action to update the selected performance library's equations
                this.updateSelectedPerformanceLibraryAction({
                    updatedSelectedPerformanceLibrary: {
                        ...this.selectedPerformanceLibrary,
                        equations: updatedEquations
                    }
                });
            }
        }

        /**
         * 'Edit Criteria' button has been clicked
         * @param id The id of the selected equation for edit
         */
        onShowCriteriaEditorDialog(id: number) {
            // set selectedEquation using the found equation in the selected performance library's list of equations
            // using the specified id
            this.selectedEquation = this.selectedPerformanceLibrary.equations
                .find((equation: PerformanceLibraryEquation) => equation.id === id) as PerformanceLibraryEquation;
            if (!isNil(this.selectedEquation)) {
                // create a new criteriaEditorDialogData object and set the criteria using selectedEquation
                this.criteriaEditorDialogData = {
                    showDialog: true,
                    criteria: this.selectedEquation.criteria
                };
            }
        }

        /**
         * User has submitted CriteriaEditorDialog result
         * @param criteria The submitted criteria string
         */
        onSubmitCriteriaEditorDialogResult(criteria: string) {
            // reset criteriaEditorDialogData
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
            // check that a result submitted
            if (!isNil(criteria)) {
                // get the selected performance library's equations
                const updatedEquations: PerformanceLibraryEquation[] = this.selectedPerformanceLibrary.equations;
                // find the index of the submitted equation in the list of equations
                const index = findIndex(propEq('id', this.selectedEquation.id), updatedEquations);
                // update the updatedEquation at the index
                updatedEquations[index] = {
                    ...this.selectedEquation,
                    criteria: criteria
                };
                // reset the selectedEquation property
                this.selectedEquation = clone(emptyEquation);
                // dispatch an action to update the selected performance library's equations
                this.updateSelectedPerformanceLibraryAction({
                    updatedSelectedPerformanceLibrary: {
                        ...this.selectedPerformanceLibrary,
                        equations: updatedEquations
                    }
                });
            }
        }

        /**
         * Filters out an equation that has a matching id with the specified id
         */
        onDeleteEquation(id: number) {
            this.updateSelectedPerformanceLibraryAction({
                updatedSelectedPerformanceLibrary: {
                    ...this.selectedPerformanceLibrary,
                    equations: this.selectedPerformanceLibrary.equations
                        .filter((equation: PerformanceLibraryEquation) => equation.id !== id)
                }
            });
        }

        /**
         * Resets the component UI by discarding the user's changes and resetting the selected performance library with
         * the current scenario's performance library data
         */
        onDiscardChanges() {
            this.onClearSelectedPerformanceLibrary();
            if (this.scenarioPerformanceLibrary.id > 0) {
                setTimeout(() => {
                    this.updateSelectedPerformanceLibraryAction({
                        updatedSelectedPerformanceLibrary: this.scenarioPerformanceLibrary
                    });
                });
            } else {
                setTimeout(() => {
                    this.setIsBusyAction({isBusy: true});
                    this.getScenarioPerformanceLibraryAction({selectedScenarioId: this.selectedScenarioId})
                        .then(() => this.setIsBusyAction({isBusy: false}));
                });
            }
        }

        /**
         * 'Create as New Library' button has been clicked
         */
        onCreateAsNewLibrary() {
            // create a new CreatePerformanceLibraryDialogData object, setting the description and equations list
            // with data from the selected performance library
            this.createPerformanceLibraryDialogData = {
                showDialog: true,
                selectedPerformanceLibraryDescription: this.selectedPerformanceLibrary.description,
                selectedPerformanceLibraryEquations: this.selectedPerformanceLibrary.equations
            };
        }

        /**
         * User has submitted a createdPerformanceLibrary dialog result
         * @param createdPerformanceLibrary The created performance library to save
         */
        onCreatePerformanceLibrary(createdPerformanceLibrary: PerformanceLibrary) {
            // create a new CreatePerformanceLibraryDialogData object
            this.createPerformanceLibraryDialogData = clone(emptyCreatePerformanceLibraryDialogData);
            // if there is a createdPerformanceLibrary...
            if (!isNil(createdPerformanceLibrary)) {
                // set the performance library using latestLibraryId + 1 (if present), otherwise set as 1
                createdPerformanceLibrary.id = hasValue(this.latestLibraryId) ? this.latestLibraryId + 1 : 1;
                createdPerformanceLibrary = this.setIdsForNewPerformanceLibraryRelatedData(createdPerformanceLibrary);
                // set isBusy to true, then dispatch action to create performance library
                this.setIsBusyAction({isBusy: true});
                this.createPerformanceLibraryAction({createdPerformanceLibrary: createdPerformanceLibrary})
                    .then(() => this.setIsBusyAction({isBusy: false}));
            }
        }

        /**
         * Sets the ids for the created performance library's equations
         */
        setIdsForNewPerformanceLibraryRelatedData(createdPerformanceLibrary: PerformanceLibrary) {
            if (hasValue(createdPerformanceLibrary.equations)) {
                let nextEquationId: number = hasValue(this.latestEquationId) ? this.latestEquationId + 1 : 1;
                createdPerformanceLibrary.equations = sortByProperty('id', createdPerformanceLibrary.equations)
                    .map((equation: PerformanceLibraryEquation) => {
                        equation.performanceLibraryId = createdPerformanceLibrary.id;
                        equation.id = nextEquationId;
                        nextEquationId++;
                        return equation;
                    });
            }
            return createdPerformanceLibrary;
        }

        /**
         * 'Update Library' button has been clicked
         */
        onUpdateLibrary() {
            // dispatch the updatePerformanceLibrary action to update the selected performance library
            this.setIsBusyAction({isBusy: true});
            this.updatePerformanceLibraryAction({updatedPerformanceLibrary: this.selectedPerformanceLibrary})
                .then(() => {
                    this.setIsBusyAction({isBusy: false});
                    this.setSuccessMessageAction({message: 'Performance library updated successfully'});
                });
        }

        /**
         * Applies the current library data to the selected scenario for edit
         */
        onApplyToScenario() {
            this.setIsBusyAction({isBusy: true});
            this.upsertScenarioPerformanceLibraryAction({
                upsertedScenarioPerformanceLibrary: this.selectedPerformanceLibrary
            }).then(() => {
                this.setIsBusyAction({isBusy: false});
                this.setSuccessMessageAction({message: 'Scenario performance library updated successfully'});
            });
        }
    }
</script>

<style>
    .performance-editor-container {
        height: 730px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .data-table {
        height: 310px;
        overflow-y: auto;
    }

    .equation-name-text-field-output {
        margin-left: 10px;
    }

    .attribute-text-field-output {
        margin-left: 15px;
    }
</style>