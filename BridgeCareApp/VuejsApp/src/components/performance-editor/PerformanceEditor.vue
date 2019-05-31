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
                                <v-icon left>label</v-icon>Scenario name: {{scenarioPerformanceLibrary.name}}
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
                <v-btn v-show="selectedScenarioId > 0" color="info" v-on:click="onApplyToScenario"
                       :disabled="!hasSelectedPerformanceLibrary">
                    Apply
                </v-btn>
                <v-btn v-show="selectedScenarioId === 0" color="info" v-on:click="onUpdateLibrary"
                       :disabled="!hasSelectedPerformanceLibrary">
                    Update Library
                </v-btn>
                <v-btn color="info lighten-1" v-on:click="onCreateAsNewLibrary" :disabled="!hasSelectedPerformanceLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId > 0" color="error lighten-1" v-on:click="onDiscardChanges"
                       :disabled="!hasSelectedPerformanceLibrary">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-footer>

        <CreatePerformanceLibraryDialog :dialogData="createPerformanceLibraryDialogData"
                                         @submit="onCreatePerformanceLibrary" />

        <CreatePerformanceLibraryEquationDialog :showDialog="showCreatePerformanceLibraryEquationDialog"
                                                 @submit="onCreatePerformanceLibraryEquation" />

        <EquationEditorDialog :dialogData="equationEditorDialogData" @submit="onSubmitEquationEditorDialogResult" />

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitCriteriaEditorDialogResult" />
    </v-container>
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
    import {any, propEq, clone, isNil, findIndex, append, isEmpty, uniq} from 'ramda';
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
    import {getLatestPropertyValue} from '@/shared/utils/getter-utils';
    import {EquationEditorDialogResult} from '@/shared/models/modals/equation-editor-dialog-result';
    import {sortByProperty} from '@/shared/utils/sorter-utils';

    @Component({
        components: {CreatePerformanceLibraryDialog, CreatePerformanceLibraryEquationDialog, EquationEditorDialog, CriteriaEditorDialog}
    })
    export default class PerformanceEditor extends Vue {
        @State(state => state.performanceEditor.performanceLibraries) statePerformanceLibraries: PerformanceLibrary[];
        @State(state => state.performanceEditor.selectedPerformanceLibrary) stateSelectedPerformanceLibrary: PerformanceLibrary;
        @State(state => state.performanceEditor.scenarioPerformanceLibrary) stateScenarioPerformanceLibrary: PerformanceLibrary;
        @State(state => state.attribute.attributes) stateAttributes: string[];

        @Action('getPerformanceLibraries') getPerformanceLibrariesAction: any;
        @Action('selectPerformanceLibrary') selectPerformanceLibraryAction: any;
        @Action('createPerformanceLibrary') createPerformanceLibraryAction: any;
        @Action('updatePerformanceLibrary') updatePerformanceLibraryAction: any;
        @Action('updateSelectedPerformanceLibrary') updateSelectedPerformanceLibraryAction: any;
        @Action('getScenarioPerformanceLibrary') getScenarioPerformanceLibraryAction: any;
        @Action('saveScenarioPerformanceLibrary') saveScenarioPerformanceLibraryAction: any;
        @Action('getAttributes') getAttributesAction: any;
        @Action('setNavigation') setNavigationAction: any;

        performanceLibraries: PerformanceLibrary[] = [];
        selectedPerformanceLibrary: PerformanceLibrary = clone(emptyPerformanceLibrary);
        scenarioPerformanceLibrary: PerformanceLibrary = clone(emptyPerformanceLibrary);
        attributes: string[] = [];

        selectedScenarioId: number = 0;
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
                    vm.selectedScenarioId = isNaN(parseInt(to.query.selectedScenarioId)) ? 0 : parseInt(to.query.selectedScenarioId);
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
                            to: {path: '/EditScenario/', query: {selectedScenarioId: to.query.selectedScenarioId}}
                        },
                        {
                            text: 'Performance editor',
                            to: {path: '/PerformanceEditor/FromScenario/', query: {selectedScenarioId: to.query.selectedScenarioId}}
                        }
                    ]);
                } else {
                    next((vm: any) => {
                        vm.setNavigationAction([]);
                    });
                }
                vm.onClearSelectedPerformanceLibrary();
                setTimeout(() => {
                    vm.getPerformanceLibrariesAction()
                        .then(() => {
                            if (vm.selectedScenarioId > 0) {
                                vm.getScenarioPerformanceLibraryAction({selectedScenarioId: vm.selectedScenarioId});
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
         * Sets performanceLibraries array = copy of statePerformanceLibraries array
         */
        @Watch('statePerformanceLibraries')
        onStatePerformanceLibrariesChanged() {
            this.performanceLibraries = clone(this.statePerformanceLibraries);
        }

        /**
         * Sets selectedPerformanceLibrary object = copy of stateSelectedPerformanceLibrary object
         */
        @Watch('stateSelectedPerformanceLibrary')
        onStateSelectedPerformanceLibraryChanged() {
            this.selectedPerformanceLibrary = clone(this.stateSelectedPerformanceLibrary);
        }

        /**
         * Sets scenarioPerformanceLibrary object = copy of stateScenarioPerformanceLibrary object
         */
        @Watch('stateScenarioPerformanceLibrary')
        onStateScenarioPerformanceLibraryChanged() {
            this.scenarioPerformanceLibrary = clone(this.stateScenarioPerformanceLibrary);
        }

        /**
         * Sets attributes array = copy of stateAttributes array
         */
        @Watch('stateAttributes')
        onStateAttributesChanged() {
            this.attributes = clone(this.stateAttributes);
        }

        /**
         * Sets the performanceLibrariesSelectListItems using the performanceLibraries array and calls the
         * setAllPerformanceLibraries function
         */
        @Watch('performanceLibraries')
        onPerformanceLibrariesChanged() {
            this.performanceLibrariesSelectListItems = this.performanceLibraries
                .map((performanceLibrary: PerformanceLibrary) => ({
                    text: performanceLibrary.name,
                    value: performanceLibrary.id.toString()
                }));

            this.setAllPerformanceLibraries();
        }

        /**
         * Calls the setAllPerformanceLibraries function
         */
        @Watch('scenarioPerformanceLibrary')
        onScenarioPerformanceLibraryChanged() {
            if (this.scenarioPerformanceLibrary.id > 0) {
                this.setAllPerformanceLibraries();
            }
        }

        /**
         * Dispatches an action to set stateSelectedPerformanceLibrary with the parsed selectItemValue
         */
        @Watch('selectItemValue')
        onPerformanceLibrariesSelectItemChanged() {
            const id: number = hasValue(this.selectItemValue) ? parseInt(this.selectItemValue) : 0;
            if (id !== this.selectedPerformanceLibrary.id) {
                this.selectPerformanceLibraryAction({performanceLibraryId: id});
            }
        }

        /**
         * Sets/resets the UI component properties when a performance library has been selected/unselected and calls
         * the setAllPerformanceLibraries function
         */
        @Watch('selectedPerformanceLibrary')
        onSelectedPerformanceLibraryChanged() {
            if (this.selectedPerformanceLibrary.id !== 0) {
                this.hasSelectedPerformanceLibrary = true;

                this.equationsGridData = hasValue(this.selectedPerformanceLibrary.equations)
                    ? this.selectedPerformanceLibrary.equations
                    : [];

                if (isEmpty(this.attributes)) {
                    this.getAttributesAction();
                }
            } else {
                this.hasSelectedPerformanceLibrary = false;
                this.equationsGridData = [];
            }

            this.setAllPerformanceLibraries();
        }

        /**
         * Sets the attributesSelectListItems array using the attributes array
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

        /**
         * Sets the latestLibraryId & laatestEquationId values using the allPerformanceLibraries array
         */
        @Watch('allPerformanceLibraries')
        onAllPerformanceLibrariesChanged() {
            this.latestLibraryId = getLatestPropertyValue('id', this.allPerformanceLibraries);

            const equations: PerformanceLibraryEquation[] = [];
            this.allPerformanceLibraries.forEach((performanceLibrary) => {
                if (hasValue(performanceLibrary.equations)) {
                    equations.push(...performanceLibrary.equations);
                }
            });

            this.latestEquationId = getLatestPropertyValue('id', equations);
        }

        /**
         * Pushes all PerformanceLibrary objects to the allPerformanceLibraries array
         */
        setAllPerformanceLibraries() {
            const libraries: PerformanceLibrary[] = clone(this.performanceLibraries);

            if (this.scenarioPerformanceLibrary.id > 0) {
                libraries.push(this.scenarioPerformanceLibrary);
            }

            if (this.selectedPerformanceLibrary.id > 0) {
                libraries.push(this.selectedPerformanceLibrary);
            }

            this.allPerformanceLibraries = uniq(libraries);
        }

        /**
         * Clears the stateSelectedPerformanceLibrary by setting selectItemValue to an empty string or '0'
         */
        onClearSelectedPerformanceLibrary() {
            this.selectItemValue = hasValue(this.selectItemValue) ? '' : '0';
        }

        /**
         * Shows the CreatePerformanceLibraryDialog to allow a user to create a new performance library
         */
        onNewLibrary() {
            this.createPerformanceLibraryDialogData = {
                ...emptyCreatePerformanceLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * Shows the CreatePerformanceLibraryEquation to allow a user to create a new performance library equation
         */
        onAddEquation() {
            this.showCreatePerformanceLibraryEquationDialog = true;
        }

        /**
         * Adds a new PerformanceLibraryEquation object to the selectedPerformanceLibrary object's equations data property
         * and dispatches an action to update the stateSelectedPerformanceLibrary
         */
        onCreatePerformanceLibraryEquation(createdEquation: PerformanceLibraryEquation) {
            this.showCreatePerformanceLibraryEquationDialog = false;

            if (!isNil(createdEquation)) {
                createdEquation.id = hasValue(this.latestEquationId) ? this.latestEquationId + 1 : 1;
                createdEquation.performanceLibraryId = this.selectedPerformanceLibrary.id;

                this.selectedPerformanceLibrary.equations = append(
                    createdEquation, this.selectedPerformanceLibrary.equations
                );

                this.updateSelectedPerformanceLibraryAction({
                    updatedSelectedPerformanceLibrary: this.selectedPerformanceLibrary
                });
            }
        }

        /**
         * Modifies a PerformanceLibraryEquation object's property with the specified value
         * @param id PerformanceLibraryEquation id
         * @param property PerformanceLibraryEquation property
         * @param value Value to set PerformanceLibraryEquation property equal to
         */
        onEditEquationProperty(id: number, property: string, value: any) {
            if (any(propEq('id', id), this.selectedPerformanceLibrary.equations)) {
                const index = findIndex(propEq('id', id), this.selectedPerformanceLibrary.equations);
                // @ts-ignore
                this.selectedPerformanceLibrary.equations[index][property] = value;

                this.updateSelectedPerformanceLibraryAction({
                    updatedSelectedPerformanceLibrary: this.selectedPerformanceLibrary
                });
            }
        }

        /**
         * Sets the selectedEquation object using the specified id to find the performance library equation in the
         * selectedPerformanceLibrary object's equations array, then shows the EquationEditorDialog and passes in the
         * selectedEquation object's equation, piecewise, & isFunction data
         * @param id PerformanceLibraryEquation id
         */
        onShowEquationEditorDialog(id: number) {
            this.selectedEquation = this.selectedPerformanceLibrary.equations
                .find((equation: PerformanceLibraryEquation) => equation.id === id) as PerformanceLibraryEquation;

            if (!isNil(this.selectedEquation)) {
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
         * Modifies the selected equation with the results from the EquationEditorDialog, then modifies the selected performance
         * library with the selected equation modifications, and lastly clears the selectedEquation object and dispatches
         * an action to update the selected performance library in state
         * @param result EquationEditorDialogResult
         */
        onSubmitEquationEditorDialogResult(result: EquationEditorDialogResult) {
            this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

            if (!isNil(result)) {
                const index = findIndex(propEq('id', this.selectedEquation.id), this.selectedPerformanceLibrary.equations);
                this.selectedPerformanceLibrary.equations[index] = {
                    ...this.selectedEquation,
                    equation: result.equation,
                    piecewise: result.isPiecewise,
                    isFunction: result.isFunction
                };

                this.selectedEquation = clone(emptyEquation);

                this.updateSelectedPerformanceLibraryAction({updatedSelectedPerformanceLibrary: this.selectedPerformanceLibrary});
            }
        }

        /**
         * Sets the selectedEquation object using the specified id to find the performance library equation in the
         * selectedPerformanceLibrary object's equations array, then shows the CriteriaEditorDialog and passes in the
         * selectedEquation object's criteria data
         * @param id PerformanceLibraryEquation id
         */
        onShowCriteriaEditorDialog(id: number) {
            this.selectedEquation = this.selectedPerformanceLibrary.equations
                .find((equation: PerformanceLibraryEquation) => equation.id === id) as PerformanceLibraryEquation;

            if (!isNil(this.selectedEquation)) {
                this.criteriaEditorDialogData = {
                    showDialog: true,
                    criteria: this.selectedEquation.criteria
                };
            }
        }

        /**
         * Modifies the selected equation with the results from the CriteriaEditorDialog, then modifies the selected performance
         * library with the selected equation modifications, and lastly clears the selectedEquation object and dispatches
         * an action to update the selected performance library in state
         * @param criteria The submitted criteria string
         */
        onSubmitCriteriaEditorDialogResult(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                const index = findIndex(propEq('id', this.selectedEquation.id), this.selectedPerformanceLibrary.equations);
                this.selectedPerformanceLibrary.equations[index] = {
                    ...this.selectedEquation,
                    criteria: criteria
                };

                this.selectedEquation = clone(emptyEquation);

                this.updateSelectedPerformanceLibraryAction({updatedSelectedPerformanceLibrary: this.selectedPerformanceLibrary});
            }
        }

        /**
         * Removes an equation from the selectedPerformanceLibrary object's equations array using the specified id
         * parameter to find the specific equation to remove from the list, then dispatches an action to update the
         * selected performance library in state
         */
        onDeleteEquation(id: number) {
            this.selectedPerformanceLibrary.equations = this.selectedPerformanceLibrary.equations
                .filter((equation: PerformanceLibraryEquation) => equation.id !== id);

            this.updateSelectedPerformanceLibraryAction({updatedSelectedPerformanceLibrary: this.selectedPerformanceLibrary});
        }

        /**
         * Shows the CreatePerformanceLibraryDialog and passes in the selected performance library's description &
         * equations data to allow a user to create a new performance library with the given data
         */
        onCreateAsNewLibrary() {
            this.createPerformanceLibraryDialogData = {
                showDialog: true,
                selectedPerformanceLibraryDescription: this.selectedPerformanceLibrary.description,
                selectedPerformanceLibraryEquations: this.selectedPerformanceLibrary.equations
            };
        }

        /**
         * Dispatches an action with a user's submitted CreatePerformanceLibraryDialog result in order to create a new
         * performance library on the server
         * @param createdPerformanceLibrary PerformanceLibrary object data
         */
        onCreatePerformanceLibrary(createdPerformanceLibrary: PerformanceLibrary) {
            this.createPerformanceLibraryDialogData = clone(emptyCreatePerformanceLibraryDialogData);

            if (!isNil(createdPerformanceLibrary)) {
                createdPerformanceLibrary.id = hasValue(this.latestLibraryId) ? this.latestLibraryId + 1 : 1;
                createdPerformanceLibrary = this.setIdsForNewPerformanceLibraryRelatedData(createdPerformanceLibrary);

                this.createPerformanceLibraryAction({createdPerformanceLibrary: createdPerformanceLibrary});
            }
        }

        /**
         * Sets the ids for the createdPerformanceLibrary object's equations
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
         * Dispatches an action with the selected performance library data in order to update the selected performance
         * library on the server
         */
        onUpdateLibrary() {
            this.updatePerformanceLibraryAction({updatedPerformanceLibrary: this.selectedPerformanceLibrary});
        }

        /**
         * Dispatches an action with the selected performance library data in order to update the selected scenario's
         * performance library data on the server
         */
        onApplyToScenario() {
            const appliedPerformanceLibrary: PerformanceLibrary = clone(this.selectedPerformanceLibrary);
            appliedPerformanceLibrary.id = this.selectedScenarioId;
            if (hasValue(appliedPerformanceLibrary.equations)) {
                appliedPerformanceLibrary.equations.map((equation: PerformanceLibraryEquation) => ({
                    ...equation,
                    performanceLibraryId: this.selectedScenarioId
                }));
            }
            this.saveScenarioPerformanceLibraryAction({saveScenarioPerformanceLibraryData: this.selectedPerformanceLibrary});
        }

        /**
         * Clears the selected performance library and dispatches an action to update the selected performance in state
         * with the scenario performance library (if present), otherwise an action is dispatched to get the scenario
         * performance library from the server to update the selected performance library in state
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
                    this.getScenarioPerformanceLibraryAction({selectedScenarioId: this.selectedScenarioId});
                });
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