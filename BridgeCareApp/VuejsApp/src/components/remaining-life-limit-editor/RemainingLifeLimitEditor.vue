<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-btn v-show="selectedScenarioId === 0" class="ara-blue-bg white--text" @click="onNewLibrary">
                    New Library
                </v-btn>
                <v-select v-if="!hasSelectedRemainingLifeLimitLibrary || selectedScenarioId > 0"
                          :items="librarySelectListItems" label="Select a Remaining Life Limit Library" outline
                          v-model="librarySelectItemValue">
                </v-select>
                <v-text-field v-if="hasSelectedRemainingLifeLimitLibrary && selectedScenarioId === 0" label="Library Name"
                              v-model="selectedRemainingLifeLimitLibrary.name">
                    <template slot="append">
                        <v-btn class="ara-orange" icon @click="onClearSelectedRemainingLifeLimitLibrary">
                            <v-icon>fas fa-times</v-icon>
                        </v-btn>
                    </template>
                </v-text-field>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedRemainingLifeLimitLibrary"></v-divider>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {State, Action} from 'vuex-class';
    import {Watch} from 'vue-property-decorator';
    import {
        emptyRemainingLifeLimitLibrary,
        RemainingLifeLimit,
        RemainingLifeLimitLibrary
    } from '@/shared/models/iAM/remaining-life-limit';
    import {clone} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {Attribute} from '@/shared/models/iAM/attribute';

    @Component
    export default class RemainingLifeLimitEditor extends Vue {
        @State(state => state.remainingLifeLimitEditor.remainingLifeLimitLibraries) stateRemainingLifeLimitLibraries: RemainingLifeLimitLibrary[];
        @State(state => state.remainingLifeLimitEditor.scenarioRemainingLifeLimitLibrary) stateScenarioRemainingLifeLimitLibrary: RemainingLifeLimitLibrary;
        @State(state => state.remainingLifeLimitEditor.selectedRemainingLifeLimitLibrary) stateSelectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary;
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('getRemainingLifeLimitLibraries') getRemainingLifeLimitLibrariesAction: any;
        @Action('createRemainingLifeLimitLibrary') createRemainingLifeLimitLibraryAction: any;
        @Action('updateRemainingLifeLimitLibrary') updateRemainingLifeLimitLibraryAction: any;
        @Action('getScenarioRemainingLifeLimitLibrary') getScenarioRemainingLifeLimitLibraryAction: any;
        @Action('saveScenarioRemainingLifeLimitLibrary') saveScenarioRemainingLifeLimitLibraryAction: any;
        @Action('selectRemainingLifeLimitLibrary') selectRemainingLifeLimitLibraryAction: any;
        @Action('updateSelectedRemainingLifeLimitLibrary') updateSelectedRemainingLifeLimitLibraryAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = [];
        scenarioRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(emptyRemainingLifeLimitLibrary);
        selectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(emptyRemainingLifeLimitLibrary);
        selectedScenarioId: number = 0;
        librarySelectItemValue: string = '';
        librarySelectListItems: SelectItem[] = [];
        hasSelectedRemainingLifeLimitLibrary: boolean = false;
        gridHeaders: DataTableHeader[] = [
            {text: 'Remaining Life Attribute', value: 'attribute', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Limit', value: 'limit', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''}
        ];
        gridData: RemainingLifeLimit[] = [];
        numericAttributesSelectListItems: SelectItem[] = [];
        selectedGridRows: any[] = [];

        /**
         * Sets component UI properties that triggers cascading UI updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/RemainingLifeLimitEditor/Scenario/') {
                    vm.selectedScenarioId = isNaN(parseInt(to.query.selectedScenarioId)) ? 0 : parseInt(to.query.selectedScenarioId);
                    if (vm.selectedScenarioId === 0) {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.onClearSelectedRemainingLifeLimitLibrary();
                setTimeout(() => {
                    vm.getRemainingLifeLimitLibraries()
                        .then(() => {
                            if (vm.selectedScenarioId > 0) {
                                vm.getScenarioRemainingLifeLimitLibrary({selectedScenarioId: vm.selectedScenarioId});
                            }
                        });
                });
            });
        }

        /**
         * Resets component UI properties that triggers cascading UI updates
         */
        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === 'RemainingLifeLimitEditor/Library/') {
                this.selectedScenarioId = 0;
                this.onClearSelectedRemainingLifeLimitLibrary();
                next();
            }
        }

        /**
         * Sets remainingLifeLimitLibraries with a copy of stateRemainingLifeLimitLibraries
         */
        @Watch('stateRemainingLifeLimitLibraries')
        onStateRemainingLifeLimitLibrariesChanged() {
            this.remainingLifeLimitLibraries = clone(this.stateRemainingLifeLimitLibraries);
        }

        /**
         * Sets scenarioRemainingLifeLimitLibrary with a copy of stateScenarioRemainingLifeLimitLibrary
         */
        @Watch('stateScenarioRemainingLifeLimitLibrary')
        onStateScenarioRemainingLifeLimitLibraryChanged() {
            this.scenarioRemainingLifeLimitLibrary = clone(this.stateScenarioRemainingLifeLimitLibrary);
        }

        /**
         * Sets selectedRemainingLifeLimitLibrary with a copy of stateSelectedRemainingLifeLimitLibrary
         */
        @Watch('stateSelectedRemainingLifeLimitLibrary')
        onStateSelectedRemainingLifeLimitLibraryChanged() {
            this.selectedRemainingLifeLimitLibrary = clone(this.stateSelectedRemainingLifeLimitLibrary);
        }

        /**
         * Sets numericAttributesSelectListItems using stateNumericAttributes
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            this.numericAttributesSelectListItems = this.stateNumericAttributes.map((numericAttribute: Attribute) => ({
                text: numericAttribute.name,
                value: numericAttribute.name
            }));
        }

        /**
         * Sets the librarySelectListItems using the remainingLifeLimitLibraries
         */
        @Watch('remainingLifeLimitLibraries')
        onRemainingLifeLimitLibrariesChanged() {
            this.librarySelectListItems = this.remainingLifeLimitLibraries.map((remainingLifeLimitLibrary: RemainingLifeLimitLibrary) => ({
                text: remainingLifeLimitLibrary.name,
                value: remainingLifeLimitLibrary.id.toString()
            }));
        }

        /**
         * Dispatches selectRemainingLifeLimitLibraryAction with librarySelectItemValue
         */
        @Watch('librarySelectItemValue')
        onSelectItemValueChanged() {
            this.selectRemainingLifeLimitLibraryAction({remainingLifeLimitLibraryId: this.librarySelectItemValue});
        }

        /**
         *
         */
        @Watch('selectedRemainingLifeLimitLibrary')
        onSelectedRemainingLifeLimitLibraryChanged() {
            if (hasValue(this.selectedRemainingLifeLimitLibrary) && this.selectedRemainingLifeLimitLibrary.id !== 0) {
                this.hasSelectedRemainingLifeLimitLibrary = true;
                this.gridData = this.selectedRemainingLifeLimitLibrary.remainingLifeLimits;
            } else {
                this.hasSelectedRemainingLifeLimitLibrary = false;
                this.gridData = [];
            }
        }

        /**
         * Clears the selected remaining life limit library by setting selectItemValue to an empty value or 0
         */
        onClearSelectedRemainingLifeLimitLibrary() {
            this.librarySelectItemValue = hasValue(this.librarySelectItemValue) ? '' : '0';
        }
    }
</script>