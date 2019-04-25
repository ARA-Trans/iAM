<template>
    <v-container fluid grid-list-xl>
        <div class="performance-editor-container">
            <v-layout column>
                <v-flex xs12>
                    <v-layout justify-center fill-height>
                        <v-flex xs3>
                            <v-btn color="info" v-on:click="onNewLibrary">
                                New Library
                            </v-btn>
                            <v-select v-if="!hasSelectedPerformanceStrategy"
                                      :items="performanceStrategiesSelectListItems"
                                      label="Select a Performance Strategy" outline
                                      v-model="selectItemValue">
                            </v-select>
                            <v-text-field v-if="hasSelectedPerformanceStrategy" label="Strategy Name" append-icon="clear"
                                          v-model="selectedPerformanceStrategy.name"
                                          @click:append="onClearPerformanceStrategySelection">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedPerformanceStrategy"></v-divider>
                <v-flex xs12 v-if="hasSelectedPerformanceStrategy">
                    <v-layout justify-center fill-height>
                        <v-flex xs3>
                            <v-layout justify-space-between fill-height>
                                <v-btn color="info" v-on:click="onAddEquation">
                                    Add
                                </v-btn>
                            </v-layout>
                        </v-flex>
                    </v-layout>
                    <v-layout justify-center fill-height>
                        <v-flex xs8>
                            <v-layout fill-height>
                                <div class="data-table">
                                    <v-data-table :headers="equationsGridHeaders"
                                                  :items="equationsGridData"
                                                  item-key="performanceStrategyEquationId"
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
                <v-divider v-if="hasSelectedPerformanceStrategy"></v-divider>
                <v-flex xs12 v-if="hasSelectedPerformanceStrategy">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-textarea no-resize outline full-width
                                        :label="selectedPerformanceStrategy.description === '' ? 'Description' : ''"
                                        v-model="selectedPerformanceStrategy.description">
                            </v-textarea>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn color="info lighten-2" v-on:click="onCreateAsNewLibrary" :disabled="!hasSelectedPerformanceStrategy">
                    Create as New Library
                </v-btn>
                <v-btn color="info lighten-1" v-on:click="onUpdateLibrary" :disabled="!hasSelectedPerformanceStrategy">
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

        <CreatePerformanceStrategyDialog :dialogData="createPerformanceStrategyDialogData"
                                         @submit="onCreatePerformanceStrategy" />

        <CreatePerformanceStrategyEquationDialog :showDialog="showCreatePerformanceStrategyEquationDialog"
                                                 @submit="onCreatePerformanceStrategyEquation" />

        <EquationEditor :dialogData="equationEditorDialogData" @submit="onSubmitEquationEditorDialogResult" />

        <CriteriaEditor :dialogData="criteriaEditorDialogData" @submit="onSubmitCriteriaEditorDialogResult" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Watch } from 'vue-property-decorator';
    import Component from 'vue-class-component';
    import {State, Action} from 'vuex-class';
    import CreatePerformanceStrategyDialog from './performance-editor-dialogs/CreatePerformanceStrategyDialog.vue';
    import CreatePerformanceStrategyEquationDialog from './performance-editor-dialogs/CreatePerformanceStrategyEquationDialog.vue';
    import EquationEditor from '../../shared/dialogs/EquationEditor.vue';
    import CriteriaEditor from '../../shared/dialogs/CriteriaEditor.vue';
    import {
        PerformanceStrategyEquation,
        PerformanceStrategy, emptyEquation
    } from '@/shared/models/iAM/performance';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {any, propEq, clone, isNil, findIndex, append, contains, isEmpty} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value';
    import {
        CreatePerformanceStrategyDialogData,
        emptyCreatePerformanceStrategyDialogData
    } from '@/shared/models/dialogs/performance-editor-dialogs/create-performance-strategy-dialog-data';
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

    @Component({
        components: {CreatePerformanceStrategyDialog, CreatePerformanceStrategyEquationDialog, EquationEditor, CriteriaEditor}
    })
    export default class PerformanceEditor extends Vue {
        @State(state => state.performanceEditor.performanceStrategies) performanceStrategies: PerformanceStrategy[];
        @State(state => state.performanceEditor.selectedPerformanceStrategy) selectedPerformanceStrategy: PerformanceStrategy;
        @State(state => state.attribute.attributes) attributes: string[];
        @State(state => state.breadcrumb.navigation) navigation: any[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getPerformanceStrategies') getPerformanceStrategiesAction: any;
        @Action('selectPerformanceStrategy') selectPerformanceStrategyAction: any;
        @Action('createPerformanceStrategy') createPerformanceStrategyAction: any;
        @Action('updatePerformanceStrategy') updatePerformanceStrategyAction: any;
        @Action('updateSelectedPerformanceStrategy') updateSelectedPerformanceStrategyAction: any;
        @Action('getAttributes') getAttributesAction: any;
        @Action('setNavigation') setNavigationAction: any;

        performanceStrategiesSelectListItems: SelectItem[] = [];
        selectItemValue: string = '';
        hasSelectedPerformanceStrategy: boolean = false;
        equationsGridHeaders: DataTableHeader[] = [
            {text: 'Name', value: 'equationName', align: 'center', sortable: true, class: '', width: ''},
            {text: 'Attribute', value: 'attribute', align: 'center', sortable: true, class: '', width: ''},
            {text: 'Equation', value: 'equation', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'center', sortable: false, class: '', width: ''},
            {text: '', value: '', align: 'center', sortable: false, class: '', width: ''}
        ];
        equationsGridData: PerformanceStrategyEquation[] = [];
        attributesSelectListItems: SelectItem[] = [];
        selectedEquation: PerformanceStrategyEquation = {...emptyEquation};
        createPerformanceStrategyDialogData: CreatePerformanceStrategyDialogData = {
            ...emptyCreatePerformanceStrategyDialogData
        };
        equationEditorDialogData: EquationEditorDialogData = {...emptyEquationEditorDialogData};
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
        showCreatePerformanceStrategyEquationDialog = false;

        beforeRouteEnter(to: any, from: any, next: any) {
            if (to.path === '/PerformanceEditor/FromScenario/') {
                next((vm: any) => {
                    vm.setNavigationAction([
                        {
                            text: 'Scenario dashboard',
                            to: '/Scenarios/'
                        },
                        {
                            text: 'Scenario editor',
                            to: {
                                path: '/EditScenario/', query: {
                                    networkId: to.query.networkId,
                                    simulationId: to.query.simulationId,
                                    networkName: to.query.networkName,
                                    simulationName: to.query.simulationName
                                }
                            }
                        },
                        {
                            text: 'Performance editor',
                            to: {
                                path: '/PerformanceEditor/FromScenario/', query: {
                                    networkId: to.query.networkId,
                                    simulationId: to.query.simulationId,
                                    networkName: to.query.networkName,
                                    simulationName: to.query.simulationName
                                }
                            }
                        }
                    ]);
                });
            }
            else {
                next((vm: any) => {
                    vm.setNavigationAction([]);
                });
            }
        }

        beforeRouteUpdate(to: any, from: any, next: any) {
            console.log('Router rerendered');
            next();
            // called when the route that renders this component has changed,
            // but this component is reused in the new route.
            // For example, for a route with dynamic params `/foo/:id`, when we
            // navigate between `/foo/1` and `/foo/2`, the same `Foo` component instance
            // will be reused, and this hook will be called when that happens.
            // has access to `this` component instance.
        }

        /**
         * Watcher: performanceStrategies
         */
        @Watch('performanceStrategies')
        onPerformanceStrategiesChanged(performanceStrategies: PerformanceStrategy[]) {
            // set the performanceStrategiesSelectListItems list using performanceStrategies list
            this.performanceStrategiesSelectListItems = performanceStrategies
                .map((performanceStrategy: PerformanceStrategy) => ({
                    text: performanceStrategy.name,
                    value: performanceStrategy.id.toString()
                }));
        }

        /**
         * Watcher: performanceStrategiesSelectItem
         */
        @Watch('selectItemValue')
        onPerformanceStrategiesSelectItemChanged() {
            if (hasValue(this.selectItemValue) && this.selectedPerformanceStrategy.id === 0) {
                // parse selectItemValue as an integer
                const id: number = parseInt(this.selectItemValue);
                // dispatch selectPerformanceStrategyAction with id
                this.selectPerformanceStrategyAction({performanceStrategyId: id});
            } else if (!hasValue(this.selectItemValue) && this.selectedPerformanceStrategy.id !== 0) {
                // dispatch selectPerformanceStrategyAction with null
                this.selectPerformanceStrategyAction({performanceStrategyId: null});
            }
        }

        /**
         * Watcher: selectedPerformanceStrategy
         */
        @Watch('selectedPerformanceStrategy')
        onSelectedPerformanceStrategyChanged() {
            if (this.selectedPerformanceStrategy.id !== 0) {
                // set ui specific properties
                if (!hasValue(this.selectItemValue)) {
                    this.selectItemValue = this.selectedPerformanceStrategy.id.toString();
                }
                this.hasSelectedPerformanceStrategy = true;
                this.equationsGridData = clone(this.selectedPerformanceStrategy.equations);
                if (isEmpty(this.attributes)) {
                    // set isBusy to true, then dispatch action to get attributes
                    this.setIsBusyAction({isBusy: true});
                    this.getAttributesAction()
                        .then(() => this.setIsBusyAction({isBusy: false}))
                        .catch((error: any) => {
                            this.setIsBusyAction({isBusy: false});
                            console.log(error);
                        });
                }
            } else {
                // reset ui specific properties
                this.selectItemValue = '';
                this.hasSelectedPerformanceStrategy = false;
                this.equationsGridData = [];
            }
        }

        /**
         * Watcher: attributes
         */
        @Watch('attributes')
        onAttributesChanged() {
            if (!isEmpty(this.attributes)) {
                // set the attributesSelectListItems property using the list of attributes
                this.attributesSelectListItems = this.attributes.map((attribute: string) => ({
                    text: attribute,
                    value: attribute
                }));
            }
        }

        /**
         * Component has been mounted
         */
        mounted() {
            // set isBusy to true, then dispatch action to get all performance strategies
            this.setIsBusyAction({isBusy: true});
            this.getPerformanceStrategiesAction()
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
            // clear the selected performance strategy
            this.onClearPerformanceStrategySelection();
        }

        /**
         * 'New Library' button has been clicked
         */
        onNewLibrary() {
            // create new CreatePerformanceStrategyDialogData object
            this.createPerformanceStrategyDialogData = {
                ...emptyCreatePerformanceStrategyDialogData,
                showDialog: true
            };
        }

        /**
         * 'Add Equation' button has been clicked
         */
        onAddEquation() {
            // set showCreatePerformanceStrategyEquationDialog to true to show CreatePerformanceStrategyEquationDialog
            this.showCreatePerformanceStrategyEquationDialog = true;
        }

        /**
         * User has submitted a createdEquation dialog result
         */
        onCreatePerformanceStrategyEquation(createdEquation: PerformanceStrategyEquation) {
            // set showCreatePerformanceStrategyEquationDialog to false to hide CreatePerformanceStrategyEquationDialog
            this.showCreatePerformanceStrategyEquationDialog = false;
            // if there is a createdPerformanceStrategyEquation
            if (!isNil(createdEquation)) {
                // get the latest id from the list of selected performance strategy equations
                const latestId: number = getLatestPropertyValue('id', this.selectedPerformanceStrategy.equations);
                // set the created equation's id by adding 1 to latestId (if present), otherwise use 1
                createdEquation.id = hasValue(latestId) ? latestId + 1 : 1;
                // set the performanceStrategyId on the created equation with the selected performance strategy's id
                createdEquation.performanceStrategyId = this.selectedPerformanceStrategy.id;
                // update the selected performance strategy's equations with the new equation
                this.updateSelectedPerformanceStrategyAction({
                    updatedSelectedPerformanceStrategy: {
                        ...this.selectedPerformanceStrategy,
                        equations: append(createdEquation, this.selectedPerformanceStrategy.equations)
                    }
                });
            }
        }

        /**
         * 'Edit Equation' button has been clicked
         * @param id The id of the selected equation for edit
         */
        onShowEquationEditorDialog(id: number) {
            // get the selected equation from the selected performance strategy's equations list using the specified id
            this.selectedEquation = this.selectedPerformanceStrategy.equations
                .find((equation: PerformanceStrategyEquation) => equation.id === id) as PerformanceStrategyEquation;
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
            this.equationEditorDialogData = {...emptyEquationEditorDialogData};
            // check that a result was submitted
            if (!isNil(result)) {
                // get the selected performance strategy's equations
                const updatedEquations: PerformanceStrategyEquation[] = this.selectedPerformanceStrategy.equations;
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
                this.selectedEquation = {...emptyEquation};
                // dispatch an action to update the selected performance strategy's equations
                this.updateSelectedPerformanceStrategyAction({
                    updatedSelectedPerformanceStrategy: {
                        ...this.selectedPerformanceStrategy,
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
            // set selectedEquation using the found equation in the selected performance strategy's list of equations
            // using the specified id
            this.selectedEquation = this.selectedPerformanceStrategy.equations
                .find((equation: PerformanceStrategyEquation) => equation.id === id) as PerformanceStrategyEquation;
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
            this.criteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
            // check that a result submitted
            if (!isNil(criteria)) {
                // get the selected performance strategy's equations
                const updatedEquations: PerformanceStrategyEquation[] = this.selectedPerformanceStrategy.equations;
                // find the index of the submitted equation in the list of equations
                const index = findIndex(propEq('id', this.selectedEquation.id), updatedEquations);
                // update the updatedEquation at the index
                updatedEquations[index] = {
                    ...this.selectedEquation,
                    criteria: criteria
                };
                // reset the selectedEquation property
                this.selectedEquation = {...emptyEquation};
                // dispatch an action to update the selected performance strategy's equations
                this.updateSelectedPerformanceStrategyAction({
                    updatedSelectedPerformanceStrategy: {
                        ...this.selectedPerformanceStrategy,
                        equations: updatedEquations
                    }
                });
            }
        }


        /**
         * A performance strategy equation's name/attribute property has been edited
         * @param id: Performance strategy equation id
         * @param property Performance strategy equation property
         * @param value Value to set on the performance strategy equation property
         */
        onEditEquationProperty(id: number, property: string, value: any) {
            if (any(propEq('id', id), this.selectedPerformanceStrategy.equations)) {
                // get the selected performance strategy's list of equations
                const updatedEquations: PerformanceStrategyEquation[] = this.selectedPerformanceStrategy.equations;
                // find the index of the updated equation in the list of equations
                const index = findIndex(propEq('id', id), updatedEquations);
                // update the specified property of the equation at the specified index with the given value
                // @ts-ignore
                updatedEquations[index][property] = value;
                // dispatch action to update the selected performance strategy's equations
                this.updateSelectedPerformanceStrategyAction({
                    updatedSelectedPerformanceStrategy: {
                        ...this.selectedPerformanceStrategy,
                        equations: updatedEquations
                    }
                });
            }
        }

        /**
         * 'Delete' button was clicked
         */
        onDeleteEquation(id: number) {
            // combine the selected equations' ids in a list with the selected performance strategy's list of deleted
            // equation ids
            const editedDeletedEquationIds: number[] = [
                ...this.selectedPerformanceStrategy.deletedEquationIds,
                id
            ];
            // filter the list of equations by removing all equations that don't have an id in the deleted equation ids list
            const editedEquations = this.selectedPerformanceStrategy.equations
                .filter((equation: PerformanceStrategyEquation) => !contains(equation.id, editedDeletedEquationIds));
            // dispatch action to update the selected performance strategy's deleted equation ids
            this.updateSelectedPerformanceStrategyAction({
                updatedSelectedPerformanceStrategy: {
                    ...this.selectedPerformanceStrategy,
                    equations: editedEquations,
                    deletedEquationIds: editedDeletedEquationIds
                }
            });
        }

        /**
         * 'Create as New Library' button has been clicked
         */
        onCreateAsNewLibrary() {
            // create a new CreatePerformanceStrategyDialogData object, setting the description and equations list
            // with data from the selected performance strategy
            this.createPerformanceStrategyDialogData = {
                showDialog: true,
                selectedPerformanceStrategyDescription: this.selectedPerformanceStrategy.description,
                selectedPerformanceStrategyEquations: this.selectedPerformanceStrategy.equations
            };
        }

        /**
         * User has submitted a createdPerformanceStrategy dialog result
         * @param createdPerformanceStrategy The created performance strategy to save
         */
        onCreatePerformanceStrategy(createdPerformanceStrategy: PerformanceStrategy) {
            // create a new CreatePerformanceStrategyDialogData object
            this.createPerformanceStrategyDialogData = {...emptyCreatePerformanceStrategyDialogData};
            // if there is a createdPerformanceStrategy...
            if (!isNil(createdPerformanceStrategy)) {
                 // get the latest id from the list of performance strategies
                const latestId: number = getLatestPropertyValue('id', this.performanceStrategies);
                // set the created performance strategy's id by adding 1 to latestId (if present), otherwise use 1
                createdPerformanceStrategy.id = hasValue(latestId) ? latestId + 1 : 1;
                // set isBusy to true, then dispatch action to create performance strategy
                this.setIsBusyAction({isBusy: true});
                this.createPerformanceStrategyAction({createdPerformanceStrategy: createdPerformanceStrategy})
                    .then(() => this.setIsBusyAction({isBusy: false}))
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
        }

        /**
         * 'Update Library' button has been clicked
         */
        onUpdateLibrary() {
            // dispatch the updatePerformanceStrategy action to update the selected performance strategy
            this.updatePerformanceStrategyAction({updatedPerformanceStrategy: this.selectedPerformanceStrategy});
        }

        /**
         * 'Apply' button has been clicked
         */
        onApplyToScenario() {
            // TODO: add library application to scenario functionality
        }

        /**
         * 'Clear' button has been clicked
         */
        onClearPerformanceStrategySelection() {
            this.selectPerformanceStrategyAction({performanceStrategyId: null});
        }
    }
</script>

<style>
    .performance-editor-container {
        height: 785px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .data-table {
        height: 370px;
        overflow-y: auto;
    }

    .equation-name-text-field-output {
        margin-left: 10px;
    }

    .attribute-text-field-output {
        margin-left: 15px;
    }
</style>