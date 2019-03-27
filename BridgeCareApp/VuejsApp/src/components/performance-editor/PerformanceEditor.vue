<template>
    <v-container fluid grid-list-xl>
        <div class="performance-editor-container">
            <v-layout column>
                <v-flex xs12>
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-btn color="info" v-on:click="showCreatePerformanceStrategyDialog = true">
                                New Library
                            </v-btn>
                            <v-select :items="performanceStrategiesSelectListItems"
                                      label="Select a Performance Strategy"
                                      outline
                                      v-model="performanceStrategiesSelectItem">
                            </v-select>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedPerformanceStrategy"></v-divider>
                <v-flex xs12 v-if="hasSelectedPerformanceStrategy">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-layout column fill-height>
                                <v-flex>
                                    <v-layout justify-space-between fill-height>
                                        <v-btn color="info" v-on:click="onShowAddEquationDialog">Add</v-btn>
                                        <v-btn color="info lighten-1" v-on:click="onToggleShift"
                                               :disabled="selectedGridRows.length === 0">
                                            Toggle Shift
                                        </v-btn>
                                        <v-btn color="info lighten-2" :disabled="selectedGridRows.length !== 1">
                                            Edit Equation
                                        </v-btn>
                                        <v-btn color="info lighten-2" :disabled="selectedGridRows.length !== 1">
                                            Edit Criteria
                                        </v-btn>
                                        <v-btn color="error" v-on:click="onDeleteEquations"
                                               :disabled="selectedGridRows.length === 0">
                                            Delete
                                        </v-btn>
                                    </v-layout>
                                </v-flex>
                                <v-flex>
                                    <div class="data-table">
                                        <v-data-table :headers="equationsGridHeaders"
                                                      :items="equationsGridData"
                                                      v-model="selectedGridRows"
                                                      select-all item-key="performanceStrategyEquationId"
                                                      class="elevation-1" hide-actions>
                                            <template slot="items" slot-scope="props">
                                                <td>
                                                    <v-checkbox v-model="props.selected" primary hide-details></v-checkbox>
                                                </td>
                                                <td v-for="header in equationsGridHeaders">
                                                    <div class="text-align-center"
                                                         v-if="header.value !== 'equation' && header.value !== 'criteria' && header.value !== 'shift'">
                                                        {{props.item[header.value]}}
                                                        <v-edit-dialog large lazy persistent
                                                                       :return-value.sync="props.item[header.value]"
                                                                       @save="onEditEquationProperty(props.item.performanceStrategyEquationId, header.value, props.item[header.value])">
                                                            <template slot="input">
                                                                <v-text-field v-model="props.item[header.value]"
                                                                              label="Edit" single-line>
                                                                </v-text-field>
                                                            </template>
                                                        </v-edit-dialog>
                                                    </div>
                                                    <div class="text-align-center" v-if="header.value === 'equation'|| header.value === 'criteria'">
                                                        <p v-if="props.item[header.value].length <= 20">
                                                            {{props.item[header.value]}}
                                                        </p>
                                                        <v-layout v-if="props.item[header.value].length > 20"
                                                                  align-end row fill-height>
                                                            <p>{{props.item[header.value].slice(0, 17) + '...'}}</p>
                                                            <v-btn flat icon
                                                                   v-on:click="onShowEquationDetailDialog(header.text, props.item[header.value])">
                                                                <v-icon>visibility</v-icon>
                                                            </v-btn>
                                                        </v-layout>
                                                    </div>
                                                    <div class="text-align-center" v-if="header.value === 'shift'">
                                                        <v-icon v-if="props.item[header.value]">done</v-icon>
                                                        <v-icon v-if="!props.item[header.value]">clear</v-icon>
                                                    </div>
                                                </td>
                                            </template>
                                        </v-data-table>
                                    </div>
                                </v-flex>
                            </v-layout>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-divider v-if="hasSelectedPerformanceStrategy"></v-divider>
                <v-flex xs12 v-if="hasSelectedPerformanceStrategy">
                    <v-layout justify-center fill-height>
                        <v-flex xs6>
                            <v-textarea class="text-area" no-resize outline full-width
                                        v-model="selectedPerformanceStrategy.description">
                            </v-textarea>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>

            <v-layout>
                <v-dialog v-model="showCreatePerformanceStrategyDialog" persistent max-width="450px">
                    <v-card>
                        <v-card-title>
                            <v-layout align-end justify-space-between row fill-height>
                                <h3>New Performance Strategy Library</h3>
                                <v-btn flat icon v-on:click="showCreatePerformanceStrategyDialog = false">
                                    <v-icon>clear</v-icon>
                                </v-btn>
                            </v-layout>
                        </v-card-title>
                        <v-card-text>
                            <v-layout column fill-height>
                                <v-text-field label="Name"
                                              v-model="createdPerformanceStrategy.name"
                                              outline>
                                </v-text-field>
                                <v-textarea class="text-area" no-resize outline full-width
                                            :label="createdPerformanceStrategy.description === ''
                                                    ? 'Description' : ''"
                                            v-model="createdPerformanceStrategy.description">
                                </v-textarea>
                            </v-layout>
                        </v-card-text>
                        <v-card-actions>
                            <v-layout justify-space-between row fill-height>
                                <v-btn color="info"
                                       v-on:click="onCreatePerformanceStrategy(true)"
                                       :disabled="createdPerformanceStrategy.name === ''">
                                    Submit
                                </v-btn>
                                <v-btn v-on:click="showCreatePerformanceStrategyDialog = false">
                                    Cancel
                                </v-btn>
                            </v-layout>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-layout>

            <v-layout>
                <v-dialog v-model="showAddEquationDialog" persistent max-width="250px">
                    <v-card>
                        <v-card-title>
                            <v-layout align-end justify-space-between row fill-height>
                                <h3>New Equation</h3>
                                <v-btn flat icon v-on:click="showAddEquationDialog = false">
                                    <v-icon>clear</v-icon>
                                </v-btn>
                            </v-layout>
                        </v-card-title>
                        <v-card-text>
                            <v-layout column fill-height>
                                <v-text-field label="Name"
                                              v-model="createdEquation.equationName"
                                              outline>
                                </v-text-field>
                                <v-select label="Select Attribute"
                                          :items="attributesSelectListItems"
                                          v-model="createdEquation.attribute"
                                          outline>
                                </v-select>
                            </v-layout>
                        </v-card-text>
                        <v-card-actions>
                            <v-layout justify-space-between row fill-height>
                                <v-btn color="info"
                                       v-on:click="onAddEquation(true)"
                                       :disabled="createdEquation.equationName === '' || createdEquation.attribute === ''">
                                    Submit
                                </v-btn>
                                <v-btn v-on:click="showAddEquationDialog = false">
                                    Cancel
                                </v-btn>
                            </v-layout>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-layout>

            <v-layout>
                <v-dialog v-model="showEquationDetailDialog" persistent scrollable max-width="300px">
                    <v-card>
                        <v-card-title>
                            <v-layout align-end justify-space-between row fill-height>
                                <h3>{{equationDetailTitle}} Detail</h3>
                                <v-btn flat icon v-on:click="showEquationDetailDialog = false">
                                    <v-icon>clear</v-icon>
                                </v-btn>
                            </v-layout>
                        </v-card-title>
                        <v-card-text class="equation-detail-card-text">
                            <v-textarea class="equation-detail-text-area" rows="12" no-resize readonly full-width
                                        :value="equationDetail">
                            </v-textarea>
                        </v-card-text>
                    </v-card>
                </v-dialog>
            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn v-on:click="resetComponentProperties">Cancel</v-btn>
                <v-btn color="info lighten-2" v-on:click="onCreateAsNewLibrary">
                    Create as New Library
                </v-btn>
                <v-btn color="info lighten-1" v-on:click="onUpdateLibrary">Update Library</v-btn>
                <v-btn color="info" v-on:click="onApplyToScenario">Apply</v-btn>
            </v-layout>
        </v-footer>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {
        emptyPerformanceStrategyEquation,
        emptyPerformanceStrategy,
        PerformanceStrategyEquation,
        PerformanceStrategy,
        emptyCreatedPerformanceStrategy,
        CreatedPerformanceStrategy,
        UpdatedPerformanceStrategy
    } from '@/shared/models/iAM/performance';
    import {defaultSelectItem, SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {PerformanceEquationEditorDialogResult} from '@/shared/models/dialogs/performance-equation-editor-dialog-result';
    import {CriteriaEditorDialogResult} from '@/shared/models/dialogs/criteria-editor-dialog-result';
    import {isEmpty, any, propEq, sortBy, prop, last, findIndex, contains, clone, append, sort, uniq} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value';

    @Component
    export default class PerformanceEditor extends Vue {
        @State(state => state.performanceEditor.performanceStrategies) performanceStrategies: PerformanceStrategy[];
        @State(state => state.attribute.attributes) attributes: string[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getPerformanceStrategies') getPerformanceStrategiesAction: any;
        @Action('createPerformanceStrategy') createPerformanceStrategyAction: any;
        @Action('getAttributes') getAttributesAction: any;
        @Action('setPerformanceEquation') setPerformanceEquationAction: any;
        @Action('setCriteria') setCriteriaAction: any;

        performanceStrategiesSelectListItems: SelectItem[] = [];
        performanceStrategiesSelectItem: SelectItem = {...defaultSelectItem};
        selectedPerformanceStrategy: PerformanceStrategy = {...emptyPerformanceStrategy};
        attributesSelectListItems: SelectItem[] = [];
        createdPerformanceStrategy: CreatedPerformanceStrategy = {...emptyCreatedPerformanceStrategy};
        createdEquation: PerformanceStrategyEquation = {...emptyPerformanceStrategyEquation};
        equationsGridHeaders: DataTableHeader[] = [
            {text: 'Name', value: 'equationName', align: 'center', sortable: true, class: '', width: ''},
            {text: 'Attribute', value: 'attribute', align: 'center', sortable: true, class: '', width: ''},
            {text: 'Equation', value: 'equation', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Shifted', value: 'shift', align: 'center', sortable: false, class: '', width: ''}
        ];
        equationsGridData: PerformanceStrategyEquation[] = [];
        selectedGridRows: PerformanceStrategyEquation[] = [];
        updatedPerformanceStrategy: UpdatedPerformanceStrategy = {
            ...emptyPerformanceStrategy,
            deletedPerformanceStrategyEquations: []
        };
        equationDetailTitle: string = '';
        equationDetail: string = '';
        hasSelectedPerformanceStrategy: boolean = false;
        showCreatePerformanceStrategyDialog: boolean = false;
        showAddEquationDialog: boolean = false;
        showEquationEditorDialog: boolean = false;
        showCriteriaEditorDialog: boolean = false;
        showEquationDetailDialog: boolean = false;

        /**
         * Watcher: performanceStrategies
         */
        @Watch('performanceStrategies')
        onPerformanceStrategiesChanged(performanceStrategies: PerformanceStrategy[]) {
            // set the performanceStrategiesSelectListeItems list using performanceStrategies list
            this.performanceStrategiesSelectListItems = performanceStrategies
                .map((performanceStrategy: PerformanceStrategy) => ({
                    text: performanceStrategy.name,
                    value: performanceStrategy.id.toString()
                }));
        }

        /**
         * Watcher: performanceStrategiesSelectItem
         */
        @Watch('performanceStrategiesSelectItem')
        onPerformanceStrategiesSelectItemChanged(value: string) {
            // parse value as an integer
            const id = parseInt(value);
            // check if the performanceStrategies list has a performance strategy with a matching id
            if (any(propEq('id', id), this.performanceStrategies)) {
                // set selectedPerformanceStrategy with found performance strategy in performanceStrategies
                this.selectedPerformanceStrategy = this.performanceStrategies
                    .find((performanceStrategy: PerformanceStrategy) => performanceStrategy.id === id) as PerformanceStrategy;
                // set hasSelectedPerformanceStrategy = true to show the rest of the ui
                this.hasSelectedPerformanceStrategy = true;
                // set the grid data
                this.setGridData();
            }
        }

        /**
         * Watcher: attributes
         */
        @Watch('attributes')
        onAttributesChanged(attributes: string[]) {
            // set the attributesSelectListItems using attributes list from state
            this.attributesSelectListItems = attributes.map((attribute: string) => ({
                text: attribute,
                value: attribute
            }));
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
         * Sets the grid data when a performance strategy has been selected or a change is made to the selected
         * performance strategy data
         */
        setGridData() {
            this.equationsGridData = clone(this.selectedPerformanceStrategy.performanceStrategyEquations);
        }

        /**
         * 'Add Equation' button has been clicked
         */
        onShowAddEquationDialog() {
            // set the performanceStrategyEquationId
            this.createdEquation.performanceStrategyEquationId = this.selectedPerformanceStrategy.id;
            // created a sortBy function to sort performance strategy equations by their performanceStrategyEquationId
            const sortById = sortBy(prop('performanceStrategyEquationId'));
            // get a sorted list of the selected performance strategy's
            const sortedPerformanceStrategyEquations = sortById(this.selectedPerformanceStrategy.performanceStrategyEquations);
            const lastPerformanceStrategyEquationInList = last(sortedPerformanceStrategyEquations) as PerformanceStrategyEquation;
            this.createdEquation.performanceStrategyEquationId = hasValue(lastPerformanceStrategyEquationInList)
                ? lastPerformanceStrategyEquationInList.performanceStrategyEquationId + 1
                : 1;
            // get attributes if empty
            if (isEmpty(this.attributes)) {
                this.setIsBusyAction({isBusy: true});
                this.getAttributesAction()
                    .then(() =>
                        this.setIsBusyAction({isBusy: false})
                    )
                    .catch((error: any) => console.log(error));
            }
            // set showAddEquationDialog = true to show CreatePerformanceEquationDialog
            this.showAddEquationDialog = true;
        }

        /**
         * User has submitted a createdEquation dialog result
         */
        onAddEquation() {
            // set showAddEquationDialog = false to hide CreatePerformanceEquationDialog
            this.showAddEquationDialog = false;
            // append the added equation to the selected performance strategy's list of equations
            this.selectedPerformanceStrategy.performanceStrategyEquations = append(
                this.createdEquation, this.selectedPerformanceStrategy.performanceStrategyEquations
            );
            // update the list of performance strategies
            this.performanceStrategies = this.performanceStrategies.map((performanceStrategy: PerformanceStrategy) => {
                if (performanceStrategy.id === this.selectedPerformanceStrategy.id) {
                    return this.selectedPerformanceStrategy;
                }
                return performanceStrategy;
            });
            // reset the grid data
            this.setGridData();
        }

        /**
         * 'Toggle Shift' button has been clicked
         */
        onToggleShift() {
            // get the selected grid row equation ids
            const selectedGridRowIds = this.selectedGridRows
                .map((gridRow: PerformanceStrategyEquation) => gridRow.performanceStrategyEquationId) as number[];
            // update the selected performance strategy's selected equations' shift value
            this.selectedPerformanceStrategy.performanceStrategyEquations = this.selectedPerformanceStrategy
                .performanceStrategyEquations.map((equation: PerformanceStrategyEquation) => {
                    if (contains(equation.performanceStrategyEquationId, selectedGridRowIds)) {
                        return {...equation, shift: !equation.shift};
                    }
                    return equation;
            });
            // reset the grid data
            this.setGridData();
        }

        /**
         * 'Edit Equation' button has been clicked
         */
        onShowEquationEditorDialog() {
            // set showEquationEditorDialog = true to show PerformanceEquationEditorDialog
            this.showEquationEditorDialog = true;
        }

        /**
         * User has submitted PerformanceEquationEditorDialog result
         * @param result PerformanceEquationEditorDialogResult object
         */
        onSubmitEquationEditResult(result: PerformanceEquationEditorDialogResult) {
            // set showEquationEditorDialog = false to hide PerformanceEquationEditorDialog
            this.showEquationEditorDialog = false;
        }

        /**
         * 'Edit Critieria' button has been clicked
         */
        onShowCriteriaEditorDialog() {
            // set showCriteriaEditorDialog = true to show CriteriaEditorDialog
            this.showCriteriaEditorDialog = true;
        }

        /**
         * User has submitted CriteriaEditorDialog result
         * @param result CriteriaEditorDialogResult object
         */
        onSubmitCriteriaEditorDialogResult(result: CriteriaEditorDialogResult) {
            // set showCriteriaEditorDialog = false to hide CriteriaEditorDialog
            this.showCriteriaEditorDialog = false;
        }


        onEditEquationProperty(id: number, property: string, value: any) {
            if (any(propEq('performanceStrategyEquationId', id), this.selectedPerformanceStrategy.performanceStrategyEquations)) {
                const equationIndex = findIndex(
                    propEq('performanceStrategyEquationId', id),
                    this.selectedPerformanceStrategy.performanceStrategyEquations) as number;
                // @ts-ignore
                this.selectedPerformanceStrategy.performanceStrategyEquations[equationIndex][property] = value;
            }
        }

        /**
         * 'Delete' button was clicked
         */
        onDeleteEquations() {
            const selectedEquationIds = this.selectedGridRows
                .map((gridRow: PerformanceStrategyEquation) => gridRow.performanceStrategyEquationId) as number[];
            // filter the deleted equations from the selected performance equation's list of equations
            this.selectedPerformanceStrategy.performanceStrategyEquations = this.selectedPerformanceStrategy
                .performanceStrategyEquations.filter((equation: PerformanceStrategyEquation) =>
                    !contains(equation.performanceStrategyEquationId, selectedEquationIds)
                );
            // combine the updatedPerformanceStrategy.deletedPerformanceStrategyEquations & selectedEquationIds lists,
            // and remove duplicates
            const deletedEquationIds = uniq([
                ...this.updatedPerformanceStrategy.deletedPerformanceStrategyEquations,
                ...selectedEquationIds
            ]) as number[];
            // set updatedPerformanceStrategy.deletedPerformanceStrategyEquations with the sorted deletedEquationIds list
            this.updatedPerformanceStrategy.deletedPerformanceStrategyEquations = sort(
                (a: number, b: number) => a - b, deletedEquationIds
            ) as number[];
        }

        onShowEquationDetailDialog(title: string, detail: string) {
            this.equationDetailTitle = title;
            // set equationDetail with incoming detail value
            this.equationDetail = detail;
            // set showEquationDetailDialog = true to show EquationDetailDialog
            this.showEquationDetailDialog = true;
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {

        }

        /**
         * 'Create as New Library' button has been clicked
         */
        onCreateAsNewLibrary() {
            // set showCreatePerformanceStrategyDialog = true to show CreatePerformanceStrategyDialog
            this.showCreatePerformanceStrategyDialog = true;
            // set createdPerformanceStrategy description = selectedPerformanceStrategy description
            this.createdPerformanceStrategy = {
                name: '',
                description: this.selectedPerformanceStrategy.description,
                performanceStrategyEquations: this.selectedPerformanceStrategy.performanceStrategyEquations
            };
        }

        /**
         * User has submitted a createdPerformanceStrategy dialog result
         * @param notCanceled Whether or not the user canceled to create a new performance strategy
         */
        onCreatePerformanceStrategy(notCanceled: boolean) {
            // set showCreatePerformanceStrategyDialog = false to hide CreatePerformanceStrategyDialog
            this.showCreatePerformanceStrategyDialog = false;
            if (notCanceled) {
                // set isBusy to true, then dispatch action to create performance strategy
                this.setIsBusyAction({isBusy: true});
                this.createPerformanceStrategyAction({createdPerformanceStrategy: this.createdPerformanceStrategy})
                    .then(() => this.setIsBusyAction({isBusy: false}))
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
            this.createdPerformanceStrategy = {...emptyCreatedPerformanceStrategy};
        }

        /**
         * 'Update Library' button has been clicked
         */
        onUpdateLibrary() {
            this.updatedPerformanceStrategy = {
                ...this.selectedPerformanceStrategy,
                deletedPerformanceStrategyEquations: this.updatedPerformanceStrategy.deletedPerformanceStrategyEquations
            };
        }

        /**
         * 'Apply' button has been clicked
         */
        onApplyToScenario() {

        }

        /**
         * Resets the PerformanceEditor component properties
         */
        resetComponentProperties() {

        }
    }
</script>

<style>
    .performance-editor-container {
        height: 850px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .text-area {
        height: 100px;
    }

    .data-table {
        height: 420px;
        overflow-y: auto;
    }

    .equation-detail-card-text {
        height: 300px;
    }

    .equation-detail-text-area {
        max-height: 235px;
    }

    .text-align-center {
        text-align: center;
    }
</style>