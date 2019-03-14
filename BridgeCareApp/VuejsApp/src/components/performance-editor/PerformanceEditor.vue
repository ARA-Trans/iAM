<template>
    <v-container fluid grid-list-xl>

    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import AppSpinner from '../../shared/AppSpinner.vue';
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
            this.performanceStrategiesSelectListItems
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
    }
</script>