<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onNewLibrary">
                        New Library
                    </v-btn>
                    <v-select v-if="!hasSelectedCashFlowLibrary || selectedScenarioId !== '0'"
                              :items="cashFlowLibrariesSelectListItems" label="Select a Cash Flow Library" outline
                              v-model="cashFlowLibrarySelectItemValue">
                    </v-select>
                    <v-text-field v-if="hasSelectedCashFlowLibrary && selectedScenarioId === '0'" label="Library Name"
                                  v-model="selectedCashFlowLibrary.name">
                        <template slot="append">
                            <v-btn class="ara-orange" icon @click="cashFlowLibrarySelectItemValue = null">
                                <v-icon>fas fa-times</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedCashFlowLibrary">
            <div class="cash-flow-library-tables">
                <v-layout justify-center row>
                    <v-flex xs5>
                        <v-btn icon class="plus-icon" @click="onAddParameter"><v-icon>fas fa-plus</v-icon></v-btn>
                        <v-data-table :headers="parametersTableHeaders" :items="parametersTableData"
                                      class="elevation-1 v-table__overflow">
                            <template slot="items" slot-scope="props">
                                <td>
                                    <v-edit-dialog :return-value.sync="props.item.parameter" large lazy persistent
                                                   @save="onEditSelectedLibraryListData(props.item, 'parameters')">
                                        <input class="output" type="text" :value="props.item.parameter" readonly />
                                        <template slot="input">
                                            <v-select :items="parametersSelectListItems" label="Select a Parameter"
                                                      v-model="props.item.parameter">
                                            </v-select>
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td>
                                    <v-menu bottom min-width="500px" min-height="500px">
                                        <template slot="activator">
                                            <input class="output" type="text" :value="props.item.criteria" readonly />
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea rows="5" no-resize readonly full-width outline
                                                            :value="props.item.criteria">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>

                                    <v-btn icon class="edit-icon" @click="onEditCriteria(props.item)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td>
                                    <v-btn icon class="ara-orange" @click="onDeleteParameter(props.item)">
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </td>
                            </template>
                        </v-data-table>
                    </v-flex>

                    <v-flex xs5>
                        <v-btn icon class="plus-icon" @click="onAddDuration"><v-icon>fas fa-plus</v-icon></v-btn>
                        <v-data-table :headers="durationsTableHeaders" :items="durationsTableData"
                                      class="elevation-1 v-table__overflow">
                            <template slot="items" slot-scope="props">
                                <td>
                                    <v-edit-dialog :return-value.sync="props.item.duration" large lazy persistent full-width
                                                   @save="onEditSelectedLibraryListData(props.item, 'durations')">
                                        <input class="output" type="text" :value="props.item.duration" readonly />
                                        <template slot="input">
                                            <v-text-field v-model="props.item.duration" label="Edit" single-line></v-text-field>
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td>
                                    <v-edit-dialog :return-value.sync="props.item.maxTreatmentCost" large lazy persistent
                                                   @save="onEditSelectedLibraryListData(props.item, 'durations')">
                                        <CashFlowEditorMaxTreatmentCostOutput :maxTreatmentCost="props.item.maxTreatmentCost"
                                                                              :durationId="props.item.id"/>
                                        <input class="output" type="text" :value="formatAsCurrency(props.item.maxTreatmentCost)" readonly />
                                        <template slot="input">
                                            <v-text-field v-model="props.item.maxTreatmentCost" label="Edit" single-line></v-text-field>
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td>
                                    <v-btn icon class="ara-orange" @click="onDeleteDuration(props.item)">
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </td>
                            </template>
                        </v-data-table>
                    </v-flex>
                </v-layout>
            </div>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedCashFlowLibrary && selectedScenarioId === '0'">
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea rows="4" no-resize outline label="Description" v-model="selectedCashFlowLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout v-show="hasSelectedCashFlowLibrary" justify-end row>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-blue-bg white--text" @click="onApplyToScenario">
                    Apply
                </v-btn>
                <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onUpdateLibrary">
                    Update Library
                </v-btn>
                <v-btn class="ara-blue-bg white--text" @click="onCreateAsNewLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-orange-bg white--text" @click="onDiscardChanges">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <CreateCashFlowLibraryDialog :dialogData="createCashFlowLibraryDialogData" @submit="onCreateCashFlowLibrary" />

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitCriteria" />
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {clone, any, propEq, find, update, findIndex, isNil, prepend} from 'ramda';
    import {
        CashFlowDuration,
        CashFlowLibrary,
        CashFlowParameter, emptyCashFlowDuration,
        emptyCashFlowLibrary,
        emptyCashFlowParameter, Parameter, parameterDummyData
    } from '@/shared/models/iAM/cash-flow';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {
        CreateCashFlowLibraryDialogData,
        emptyCreateCashFlowLibraryDialogData
    } from '@/shared/models/modals/create-cash-flow-library-dialog-data';
    import CreateCashFlowLibraryDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/CreateCashFlowLibraryDialog.vue';
    import {formatAsCurrency} from "@/shared/utils/currency-formatter";
    const ObjectID = require('bson-objectid');

    @Component({
        components: {CreateCashFlowLibraryDialog, CriteriaEditorDialog}
    })
    export default class CashFlowEditor extends Vue {
        @State(state => state.cashFlow.cashFlowLibraries) stateCashFlowLibraries: CashFlowLibrary[];
        @State(state => state.cashFlow.selectedCashFlowLibrary) stateSelectedCashFlowLibrary: CashFlowLibrary;
        @State(state => state.cashFlow.scenarioCashFlowLibrary) stateScenarioCashFlowLibrary: CashFlowLibrary;

        @Action('getCashFlowLibraries') getCashFlowLibrariesAction: any;
        @Action('selectCashFlowLibrary') selectCashFlowLibraryAction: any;
        @Action('createCashFlowLibrary') createCashFlowLibraryAction: any;
        @Action('updateCashFlowLibrary') updateCashFlowLibraryAction: any;
        @Action('getScenarioCashFlowLibrary') getScenarioCashFlowLibraryAction: any;
        @Action('saveScenarioCashFlowLibrary') saveScenarioCashFlowLibraryAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        hasSelectedCashFlowLibrary: boolean = false;
        selectedScenarioId: string = '0';
        cashFlowLibrariesSelectListItems: SelectItem[] = [];
        cashFlowLibrarySelectItemValue: any = null;
        selectedCashFlowLibrary: CashFlowLibrary = clone(emptyCashFlowLibrary);
        parametersTableHeaders: DataTableHeader[] = [
            {text: 'Parameter', value: '', align: 'left', sortable: true, class: '', width: '20%'},
            {text: 'Criteria', value: '', align: 'left', sortable: false, class: '', width: '75%'},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '5%'}
        ];
        parametersTableData: CashFlowParameter[] = [];
        parametersSelectListItems: SelectItem[] = parameterDummyData.map((data: Parameter) => ({
            text: data.name,
            value: data.name
        }));
        durationsTableHeaders: DataTableHeader[] = [
            {text: 'Duration (Years)', value: 'duration', align: 'left', sortable: true, class: '', width: '20%'},
            {text: 'Maximum Treatment Cost', value: 'maxTreatmentCost', align: 'left', sortable: true, class: '', width: '75%'},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '5%'}
        ];
        durationsTableData: CashFlowDuration[] = [];
        selectedCashFlowParameter: CashFlowParameter = clone(emptyCashFlowParameter);
        createCashFlowLibraryDialogData: CreateCashFlowLibraryDialogData = clone(emptyCreateCashFlowLibraryDialogData);
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/CashFlowEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    if (vm.selectedScenarioId === '0') {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.cashFlowLibrarySelectItemValue = null;
                vm.getCashFlowLibrariesAction().then(() => {
                    if (vm.selectedScenarioId !== '0') {
                        vm.getScenarioCashFlowLibraryAction({selectedScenarioId: vm.selectedScenarioId});
                    }
                });
            });
        }

        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === '/CashFlowEditor/Library/') {
                this.selectedScenarioId = '0';
                this.cashFlowLibrarySelectItemValue = null;
                next();
            }
        }

        @Watch('stateCashFlowLibraries')
        onStateCashFlowLibrariesChanged() {
            this.cashFlowLibrariesSelectListItems = this.stateCashFlowLibraries.map((cashFlowLibrary: CashFlowLibrary) => ({
                text: cashFlowLibrary.name,
                value: cashFlowLibrary.id
            }));
        }

        @Watch('cashFlowLibrarySelectItemValue')
        onCashFlowLibrarySelectItemChanged() {
            if (any(propEq('id', this.cashFlowLibrarySelectItemValue), this.stateCashFlowLibraries)) {
                this.selectCashFlowLibraryAction({
                    selectedCashFlowLibrary: find(
                        propEq('id', this.cashFlowLibrarySelectItemValue), this.stateCashFlowLibraries)
                });
            } else {
                this.selectCashFlowLibraryAction({selectedCashFlowLibrary: emptyCashFlowLibrary});
            }
        }

        @Watch('stateSelectedCashFlowLibrary')
        onStateSelectedCashFlowLibraryChanged() {
            this.selectedCashFlowLibrary = clone(this.stateSelectedCashFlowLibrary);
            if (this.selectedCashFlowLibrary.id !== '0') {
                this.hasSelectedCashFlowLibrary = true;
                this.parametersTableData = clone(this.selectedCashFlowLibrary.parameters);
                this.durationsTableData = clone(this.selectedCashFlowLibrary.durations);
            } else {
                this.hasSelectedCashFlowLibrary = false;
                this.parametersTableData = [];
                this.durationsTableData = [];
            }
        }

        /*onClearSelectedCashFlowLibrary() {
            this.cashFlowLibrarySelectItemValue = null;
        }*/

        onNewLibrary() {
            this.createCashFlowLibraryDialogData = clone({
                ...emptyCreateCashFlowLibraryDialogData,
                showDialog: true
            });
        }

        onCreateAsNewLibrary() {
            this.createCashFlowLibraryDialogData = {
                showDialog: true,
                parameters: this.selectedCashFlowLibrary.parameters,
                durations: this.selectedCashFlowLibrary.durations
            };
        }

        onCreateCashFlowLibrary(createdCashFlowLibrary: CashFlowLibrary) {
            this.createCashFlowLibraryDialogData = clone(emptyCreateCashFlowLibraryDialogData);

            if (!isNil(createdCashFlowLibrary)) {
                this.createCashFlowLibraryAction({createdCashFlowLibrary: createdCashFlowLibrary});
            }
        }

        onAddParameter() {
            const newParameter: CashFlowParameter = clone({
                ...emptyCashFlowParameter,
                id: ObjectID.generate()
            });
            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                    ...this.selectedCashFlowLibrary,
                    parameters: prepend(newParameter, this.selectedCashFlowLibrary.parameters)
                })
            });
        }

        onDeleteParameter(cashFlowParameter: CashFlowParameter) {
            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                    ...this.selectedCashFlowLibrary,
                    parameters: this.selectedCashFlowLibrary.parameters
                        .filter((parameter: CashFlowParameter) => parameter.id !== cashFlowParameter.id)
                })
            });
        }

        onAddDuration() {
            const newDuration: CashFlowDuration = clone({
                ...emptyCashFlowDuration,
                id: ObjectID.generate()
            });
            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                    ...this.selectedCashFlowLibrary,
                    durations: prepend(newDuration, this.selectedCashFlowLibrary.durations)
                })
            });
        }

        onDeleteDuration(cashFlowDuration: CashFlowDuration) {
            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                    ...this.selectedCashFlowLibrary,
                    durations: this.selectedCashFlowLibrary.durations
                        .filter((duration: CashFlowDuration) => duration.id !== cashFlowDuration.id)
                })
            });
        }

        onEditCriteria(cashFlowParameter: CashFlowParameter) {
            this.selectedCashFlowParameter = clone(cashFlowParameter);

            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.selectedCashFlowParameter.criteria
            };
        }

        onSubmitCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                        ...this.selectedCashFlowLibrary,
                        parameters: update(
                            findIndex(propEq('id', this.selectedCashFlowParameter.id), this.selectedCashFlowLibrary.parameters),
                            clone({...this.selectedCashFlowParameter, criteria: criteria}),
                            this.selectedCashFlowLibrary.parameters
                        )
                    })
                });
            }
        }

        onEditSelectedLibraryListData(data: any, property: string) {
            const cashFlowLibrary: CashFlowLibrary = clone(this.selectedCashFlowLibrary);

            switch(property) {
                case 'parameters':
                    cashFlowLibrary.parameters = update(
                        findIndex(propEq('id', data.id), cashFlowLibrary.parameters),
                        data as CashFlowParameter,
                        cashFlowLibrary.parameters
                    );
                        break;
                case 'durations':
                    cashFlowLibrary.durations = update(
                        findIndex(propEq('id', data.id), cashFlowLibrary.durations),
                        data as CashFlowDuration,
                        cashFlowLibrary.durations
                    );
            }

            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: cashFlowLibrary});
        }

        onApplyToScenario() {
            this.saveScenarioCashFlowLibraryAction({scenarioCashFlowLibrary: clone({
                    ...this.selectedCashFlowLibrary,
                    id: this.stateScenarioCashFlowLibrary.id
                })
            });
        }

        onUpdateLibrary() {
            this.updateCashFlowLibraryAction({updatedCashFlowLibrary: this.selectedCashFlowLibrary});
        }

        onDiscardChanges() {
            this.cashFlowLibrarySelectItemValue = null;

            if (this.stateScenarioCashFlowLibrary.id !== '0') {
                this.selectCashFlowLibraryAction({selectedCashFlowLibrary: this.stateScenarioCashFlowLibrary});
            } else {
                this.getScenarioCashFlowLibraryAction({selectedScenarioId: this.selectedScenarioId});
            }
        }

        /**
         * Formats a value as currency by calling the formatAsCurrency utility function
         * @param value Value to format
         */
        formatAsCurrency(value: any) {
            return formatAsCurrency(value);
        }
    }
</script>

<style>
    .cash-flow-library-tables {
        height: 425px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .cash-flow-library-tables .v-menu--inline {
        width: 85%;
    }

    .cash-flow-library-tables .v-menu__activator a, .cash-flow-library-tables .v-menu--inline input {
        width: 100%;
    }

    .output {
        border-bottom: 1px solid;
    }
</style>