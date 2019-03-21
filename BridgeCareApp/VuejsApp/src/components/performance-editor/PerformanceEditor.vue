<template>
    <v-container fluid grid-list-xl>
        <v-layout column>
            <v-flex xs12>
                <v-layout justify-center column>
                    <div class="performance-strategy-select-div">
                        <v-btn color="info" v-on:click="onShowCreatePerformanceStrategyDialog">
                            New Performance Strategy
                        </v-btn>
                        <v-select :items="performanceStrategiesSelectListItems"
                                  label="Select a Performance Strategy"
                                  outline
                                  v-on:change="onSelectPerformanceStrategy"
                                  v-model="performanceStrategiesSelectItem">
                        </v-select>
                    </div>
                </v-layout>
            </v-flex>
            <v-flex xs12>
                <div class="container-div">

                </div>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {
        emptyPerformanceStrategy,
        PerformanceEquation,
        PerformanceStrategy,
        SavedPerformanceStrategy
    } from '@/shared/models/iAM/performance';
    import {defaultSelectItem, SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {CreatePerformanceStrategyDialogResult} from '@/shared/models/dialogs/create-performance-strategy-dialog-result';
    import {CreatePerformanceEquationDialogResult} from '@/shared/models/dialogs/create-performance-equation-dialog-result';
    import {PerformanceEquationEditorDialogResult} from '@/shared/models/dialogs/performance-equation-editor-dialog-result';
    import {CriteriaEditorDialogResult} from '@/shared/models/dialogs/criteria-editor-dialog-result';

    @Component
    export default class PerformanceEditor extends Vue {
        @State(state => state.performanceEditor.performanceStrategies) performanceStrategies: PerformanceStrategy[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getPerformanceStrategies') getPerformanceStrategiesAction: any;
        @Action('setPerformanceEquation') setPerformanceEquationAction: any;
        @Action('setCriteria') setCriteriaAction: any;

        performanceStrategiesSelectListItems: SelectItem[] = [];
        performanceStrategiesSelectItem: SelectItem = {...defaultSelectItem};
        performanceStrategiesGridHeaders: DataTableHeader[] = [
            {text: 'Name', value: 'equationName', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Equation', value: 'equation', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''}
        ];
        performanceStrategiesGridData: PerformanceEquation[] = [];
        selectedGridRows: PerformanceEquation[] = [];
        savedPerformanceStrategy: SavedPerformanceStrategy = {
            ...emptyPerformanceStrategy,
            deletedPerformanceEquations: []
        };
        hasNoSelectedPerformanceStrategy: boolean = true;
        showCreatePerformanceStrategyDialog: boolean = false;
        showCreatePerformanceEquationDialog: boolean = false;
        showPerformanceEquationEditorDialog: boolean = false;
        showCriteriaEditorDialog: boolean = false;

        @Watch('performanceStrategies')
        onPerformanceStrategiesChanged(performanceStrategies: PerformanceStrategy[]) {
            // set the performanceStrategiesSelectListeItems list using performanceStrategies list
            this.performanceStrategiesSelectListItems = performanceStrategies
                .map((performanceStrategy: PerformanceStrategy) => ({
                    text: performanceStrategy.name,
                    value: performanceStrategy.simulationId.toString()
                }));
        }

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
         * 'New Performance Strategy' button has been clicked
         */
        onShowCreatePerformanceStrategyDialog() {
            // set showCreatePerformanceStrategyDialog = true to show CreatePerformanceStrategyDialog
            this.showCreatePerformanceStrategyDialog = true;
        }

        /**
         * User has submitted CreatePerformanceStrategyDialog result
         * @param result CreatePerformanceStrategyDialogResult object
         */
        onCreatePerformanceStrategy(result: CreatePerformanceStrategyDialogResult) {
            // set showCreatePerformanceStrategyDialog = false to hide CreatePerformanceStrategyDialog
            this.showCreatePerformanceStrategyDialog = false;
        }

        /**
         * A performance strategy select item has been selected from performanceStrategiesSelectListItems
         * @param value
         */
        onSelectPerformanceStrategy(value: string) {

        }

        /**
         * 'Add Equation' button has been clicked
         */
        onShowCreatePerformanceEquationDialog() {
            // set showCreatePerformanceEquationDialog = true to show CreatePerformanceEquationDialog
            this.showCreatePerformanceEquationDialog = true;
        }

        /**
         * User has submitted CreatePerformanceEquationDialog result
         * @param result CreatePerformanceEquationDialogResult object
         */
        onCreatePerformanceEquation(result: CreatePerformanceEquationDialogResult) {
            // set showCreatePerformanceEquationDialog = false to hide CreatePerformanceEquationDialog
            this.showCreatePerformanceEquationDialog = false;
        }

        /**
         * 'Edit Equation' button has been clicked
         */
        onShowPerformanceEquationEditorDialog() {
            // set showPerformanceEquationEditorDialog = true to show PerformanceEquationEditorDialog
            this.showPerformanceEquationEditorDialog = true;
        }

        /**
         * User has submitted PerformanceEquationEditorDialog result
         * @param result PerformanceEquationEditorDialogResult object
         */
        onSubmitPerformanceEquationEditResult(result: PerformanceEquationEditorDialogResult) {
            // set showPerformanceEquationEditorDialog = false to hide PerformanceEquationEditorDialog
            this.showPerformanceEquationEditorDialog = false
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

        /**
         * 'Apply Shift' button has been clicked
         */
        onApplyShift() {

        }

        /**
         * 'Create as New Library' button has been clicked
         */
        onCreateAsNewLibrary() {

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
    .performance-strategy-select {
        width: 50%;
    }
</style>