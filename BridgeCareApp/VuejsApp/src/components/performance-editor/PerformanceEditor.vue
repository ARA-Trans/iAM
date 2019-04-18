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
                        <v-flex xs6>
                            <v-layout justify-space-between fill-height>
                                <v-btn color="info" v-on:click="onAddEquation">
                                    Add
                                </v-btn>
                                <v-btn color="info lighten-1" v-on:click="onToggleShift"
                                       :disabled="selectedGridRows.length === 0">
                                    Toggle Shift
                                </v-btn>
                                <v-btn color="info lighten-2" v-on:click="onShowEquationEditorDialog"
                                       :disabled="selectedGridRows.length !== 1">
                                    Edit Equation
                                </v-btn>
                                <v-btn color="info lighten-2" v-on:click="onShowCriteriaEditorDialog"
                                       :disabled="selectedGridRows.length !== 1">
                                    Edit Criteria
                                </v-btn>
                                <v-btn color="error" v-on:click="onDeleteEquations"
                                       :disabled="selectedGridRows.length === 0">
                                    Delete
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
                                                  v-model="selectedGridRows"
                                                  select-all item-key="performanceStrategyEquationId"
                                                  class="elevation-1 fixed-header v-table__overflow" hide-actions>
                                        <template slot="items" slot-scope="props">
                                            <td>
                                                <v-checkbox v-model="props.selected" primary hide-details></v-checkbox>
                                            </td>
                                            <td class="text-xs-center">
                                                <v-edit-dialog @save="onEditEquationProperty(props.item.performanceStrategyEquationId, 'equationName', props.item.equationName)"
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
                                                <v-edit-dialog @save="onEditEquationProperty(props.item.performanceStrategyEquationId, 'attribute', props.item.attribute)"
                                                               :return-value.sync="props.item.attribute"
                                                               large lazy persistent>
                                                    <v-text-field class="attribute-text-field-output" readonly :value="props.item.attribute"></v-text-field>
                                                    <template slot="input">
                                                        <v-select :items="attributes" v-model="props.item.attribute"
                                                                  label="Edit">
                                                        </v-select>
                                                    </template>
                                                </v-edit-dialog>
                                            </td>
                                            <td class="text-xs-center">
                                                <v-menu v-if="props.item.equation !== ''" left min-width="500px"
                                                        min-height="500px">
                                                    <template slot="activator">
                                                        <v-btn flat icon >
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
                                            </td>
                                            <td class="text-xs-center">
                                                <v-menu v-if="props.item.criteria !== ''" right min-width="500px"
                                                        min-height="500px">
                                                    <template slot="activator">
                                                        <v-btn flat icon >
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
                                            </td>
                                            <td class="text-xs-center">
                                                <v-icon v-if="props.item.shift">done</v-icon>
                                                <v-icon v-if="!props.item.shift">clear</v-icon>
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

        <EquationEditor :dialogData="equationEditorDialogData" @submit="onSubmitEquationEditorDialogResult"/>

        <CriteriaEditor :dialogData="criteriaEditorDialogData" @submit="onSubmitCriteriaEditorDialogResult" />
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import CreatePerformanceStrategyDialog from './performance-editor-dialogs/CreatePerformanceStrategyDialog.vue';
    import CreatePerformanceStrategyEquationDialog from './performance-editor-dialogs/CreatePerformanceStrategyEquationDialog.vue';
    import EquationEditor from '../../shared/dialogs/EquationEditor.vue';
    import CriteriaEditor from '../../shared/dialogs/CriteriaEditor.vue';
    import {
        PerformanceStrategyEquation,
        PerformanceStrategy,
        CreatedPerformanceStrategy,
        UpdatedPerformanceStrategy, CreatedPerformanceStrategyEquation, DeletedPerformanceStrategyEquations
    } from '@/shared/models/iAM/performance';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {any, propEq, contains, clone, isNil} from 'ramda';
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
    import {EquationEditorDialogResult} from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-result';

    @Component({
        components: {CreatePerformanceStrategyDialog, CreatePerformanceStrategyEquationDialog, EquationEditor, CriteriaEditor}
    })
    export default class PerformanceEditor extends Vue {
        @State(state => state.performanceEditor.performanceStrategies) performanceStrategies: PerformanceStrategy[];
        @State(state => state.performanceEditor.selectedPerformanceStrategy) selectedPerformanceStrategy: PerformanceStrategy;
        @State(state => state.attribute.attributes) attributes: string[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getPerformanceStrategies') getPerformanceStrategiesAction: any;
        @Action('selectPerformanceStrategy') selectPerformanceStrategyAction: any;
        @Action('createPerformanceStrategy') createPerformanceStrategyAction: any;
        @Action('updatePerformanceStrategy') updatePerformanceStrategyAction: any;
        @Action('createEquation') createEquationAction: any;
        @Action('updateEquations') updateEquationsAction: any;
        @Action('deleteEquations') deleteEquationsAction: any;
        @Action('getAttributes') getAttributesAction: any;

        performanceStrategiesSelectListItems: SelectItem[] = [];
        selectItemValue: string = '';
        hasSelectedPerformanceStrategy: boolean = false;
        equationsGridHeaders: DataTableHeader[] = [
            {text: 'Name', value: 'equationName', align: 'center', sortable: true, class: '', width: ''},
            {text: 'Attribute', value: 'attribute', align: 'center', sortable: true, class: '', width: ''},
            {text: 'Equation', value: 'equation', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Shifted', value: 'shift', align: 'center', sortable: false, class: '', width: ''}
        ];
        equationsGridData: PerformanceStrategyEquation[] = [];
        selectedGridRows: PerformanceStrategyEquation[] = [];
        selectedEquationIds: number[] = [];
        createPerformanceStrategyDialogData: CreatePerformanceStrategyDialogData = {
            ...emptyCreatePerformanceStrategyDialogData
        };
        equationEditorDialogData: EquationEditorDialogData = {...emptyEquationEditorDialogData};
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
        showCreatePerformanceStrategyEquationDialog = false;

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
                this.hasSelectedPerformanceStrategy = true;
                this.equationsGridData = clone(this.selectedPerformanceStrategy.performanceStrategyEquations);
                if (!hasValue(this.selectItemValue)) {
                    this.selectItemValue = this.selectedPerformanceStrategy.id.toString();
                }
            } else {
                this.hasSelectedPerformanceStrategy = false;
                this.equationsGridData = [];
                this.selectedGridRows = [];
            }
        }

        /**
         * Watcher: selectedGridRows
         */
        @Watch('selectedGridRows')
        onSelectedGridRowsChanged() {
            // set selectedEquationIds with the ids of the selected performance equations in selectedGridRows
            this.selectedEquationIds = this.selectedGridRows.map((equation: PerformanceStrategyEquation) =>
                equation.performanceStrategyEquationId
            );
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
            // set isBusy to true, then dispatch action to get all attributes
            this.setIsBusyAction({isBusy: true});
            this.getAttributesAction()
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
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
         * 'Clear' button has been clicked
         */
        onClearPerformanceStrategySelection() {
            this.selectItemValue = '';
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
        onCreatePerformanceStrategyEquation(createdPerformanceStrategyEquation: CreatedPerformanceStrategyEquation) {
            // set showCreatePerformanceStrategyEquationDialog to false to hide CreatePerformanceStrategyEquationDialog
            this.showCreatePerformanceStrategyEquationDialog = false;
            // if there is a createdPerformanceStrategyEquation
            if (!isNil(createdPerformanceStrategyEquation)) {
                // set the createdPerformanceStrategyEquation.performanceStrategyEquationId with selected performance strategy id
                createdPerformanceStrategyEquation.performanceStrategyId = this.selectedPerformanceStrategy.id;
                // set isBusy to true, then dispatch an action to create the equation
                this.setIsBusyAction({isBusy: true});
                this.createEquationAction({createdEquation: createdPerformanceStrategyEquation})
                    .then(() => this.setIsBusyAction({isBusy: false}))
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
        }

        /**
         * 'Toggle Shift' button has been clicked
         */
        onToggleShift() {
            // get the equations to update from selectedPerformanceStrategy.performanceStrategyEquations using selectedEquationIds
            // list and negate their current shift values
            const updatedEquations: PerformanceStrategyEquation[] = this.selectedPerformanceStrategy.performanceStrategyEquations
                .filter((equation: PerformanceStrategyEquation) => contains(equation.performanceStrategyEquationId, this.selectedEquationIds))
                .map((equation: PerformanceStrategyEquation) => ({...equation, shift: !equation.shift}));
            // dispatch updateEquations action to update the performance strategy equations in state
            this.updateEquationsAction({updatedEquations: updatedEquations});
        }

        /**
         * 'Edit Equation' button has been clicked
         */
        onShowEquationEditorDialog() {
            // get the selectedEquation from selectedPerformanceStrategy.performanceStrategyEquations using selectedEquationIds
            // list, which should only contain one entry
            const selectedEquation: PerformanceStrategyEquation = this.selectedPerformanceStrategy.performanceStrategyEquations
                .find((equation: PerformanceStrategyEquation) =>
                    contains(equation.performanceStrategyEquationId, this.selectedEquationIds)
                ) as PerformanceStrategyEquation;
            if (!isNil(selectedEquation)) {
                // create a new equationEditorDialogData object using selectedEquation data
                this.equationEditorDialogData = {
                    showDialog: true,
                    equation: selectedEquation.equation,
                    isPiecewise: selectedEquation.piecewise,
                    isFunction: selectedEquation.isFunction
                };
            }
        }

        /**
         * User has submitted PerformanceEquationEditorDialog result
         * @param result The submitted equation editor dialog result
         */
        onSubmitEquationEditorDialogResult(result: EquationEditorDialogResult) {
            // reset equationEditorDialogData
            this.equationEditorDialogData = {...emptyEquationEditorDialogData};
            // check that a result was submitted
            if (!isNil(result)) {
                // get the selectedEquation from selectedPerformanceStrategy.performanceStrategyEquations using selectedEquationIds
                // list, which should only contain one entry
                const selectedEquation: PerformanceStrategyEquation = this.selectedPerformanceStrategy.performanceStrategyEquations
                    .find((equation: PerformanceStrategyEquation) =>
                        contains(equation.performanceStrategyEquationId, this.selectedEquationIds)
                    ) as PerformanceStrategyEquation;
                // update the selected equation with the equation editor dialog result
                selectedEquation.equation = result.equation;
                selectedEquation.piecewise = result.isPiecewise;
                selectedEquation.isFunction = result.isFunction;
                // dispatch an action to update the selected equation on the server
                this.updateEquationsAction({updatedEquations: [selectedEquation]});
            }
        }

        /**
         * 'Edit Criteria' button has been clicked
         */
        onShowCriteriaEditorDialog() {
            // get the selectedEquation from selectedPerformanceStrategy.performanceStrategyEquations using selectedEquationIds
            // list, which should only contain one entry
            const selectedEquation: PerformanceStrategyEquation = this.selectedPerformanceStrategy.performanceStrategyEquations
                .find((equation: PerformanceStrategyEquation) =>
                    contains(equation.performanceStrategyEquationId, this.selectedEquationIds)
                ) as PerformanceStrategyEquation;
            if (!isNil(selectedEquation)) {
                // create a new criteriaEditorDialogData object and set the criteria using selectedEquation
                this.criteriaEditorDialogData = {
                    showDialog: true,
                    criteria: selectedEquation.criteria
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
                // get the selectedEquation from selectedPerformanceStrategy.performanceStrategyEquations using selectedEquationIds
                // list, which should only contain one entry
                const selectedEquation: PerformanceStrategyEquation = this.selectedPerformanceStrategy.performanceStrategyEquations
                    .find((equation: PerformanceStrategyEquation) =>
                        contains(equation.performanceStrategyEquationId, this.selectedEquationIds)
                    ) as PerformanceStrategyEquation;
                // set the selected performance strategy equation criteria with the given criteria
                selectedEquation.criteria = criteria;
                // dispatch updateEquations action to update the performance strategy equation
                this.updateEquationsAction({updatedEquations: [selectedEquation]});
            }
        }


        /**
         * A performance strategy equation's name/attribute property has been edited
         * @param id: Performance strategy equation id
         * @param property Performance strategy equation property
         * @param value Value to set on the performance strategy equation property
         */
        onEditEquationProperty(id: number, property: string, value: any) {
            // check that the selected performance strategy has an equation with the given id
            if (any(propEq('performanceStrategyEquationId', id), this.selectedPerformanceStrategy.performanceStrategyEquations)) {
                const updatedEquation: PerformanceStrategyEquation = this.selectedPerformanceStrategy.performanceStrategyEquations
                    .find((equation: PerformanceStrategyEquation) =>
                        equation.performanceStrategyEquationId === id
                    ) as PerformanceStrategyEquation;
                // update the equation's property with the given value
                // @ts-ignore
                updatedEquation[property] = value;
                // dispatch updateEquations action to update the performance strategy equation in state
                this.updateEquationsAction({updatedEquations: [updatedEquation]});
            }
        }

        /**
         * 'Delete' button was clicked
         */
        onDeleteEquations() {
            // create a new DeletedPerformanceStrategyEquations object using the selectedPerformanceStrategy.id and
            // the selectedEquationIds list
            const deletedEquations: DeletedPerformanceStrategyEquations = {
                performanceStrategyId: this.selectedPerformanceStrategy.id,
                deletedEquationIds: this.selectedEquationIds
            };
            // dispatch the deleteEquations action to delete the selected performance strategy equations
            this.deleteEquationsAction({deletedEquations: deletedEquations});
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
                selectedPerformanceStrategyEquations: this.selectedPerformanceStrategy.performanceStrategyEquations
            };
        }

        /**
         * User has submitted a createdPerformanceStrategy dialog result
         * @param createdPerformanceStrategy The created performance strategy to save
         */
        onCreatePerformanceStrategy(createdPerformanceStrategy: CreatedPerformanceStrategy) {
            // create a new CreatePerformanceStrategyDialogData object
            this.createPerformanceStrategyDialogData = {...emptyCreatePerformanceStrategyDialogData};
            // if there is a createdPerformanceStrategy...
            if (hasValue(createdPerformanceStrategy)) {
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
            // create a new UpdatedPerformanceStrategy object and set the data using the selected performance strategy
            const updatedPerformanceStrategy: UpdatedPerformanceStrategy = {
                id: this.selectedPerformanceStrategy.id,
                name: this.selectedPerformanceStrategy.name,
                description: this.selectedPerformanceStrategy.description
            };
            // dispatch the updatePerformanceStrategy action to update the performance strategy
            this.updatePerformanceStrategyAction({updatedPerformanceStrategy: updatedPerformanceStrategy});
        }

        /**
         * 'Apply' button has been clicked
         */
        onApplyToScenario() {
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