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
                            <v-btn class="ara-orange" icon @click="onClearSelectedCashFlowLibrary">
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedCashFlowLibrary && selectedScenarioId === '0'">
                        Owner: {{selectedCashFlowLibrary.owner ? selectedCashFlowLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" v-if="hasSelectedCashFlowLibrary && selectedScenarioId === '0'" 
                        v-model="selectedCashFlowLibrary.shared" label="Shared"/>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedCashFlowLibrary">
            <div class="cash-flow-library-tables">
                <v-layout justify-center row>
                    <v-flex xs8>
                        <v-card>
                            <v-card-title>
                                <v-btn @click="onAddSplitTreatment">
                                    <v-icon left class="plus-icon">fas fa-plus</v-icon>Add Cash Flow Rule
                                </v-btn>
                            </v-card-title>
                            <v-card-text class="cash-flow-library-card">
                                <v-data-table :headers="splitTreatmentTableHeaders" :items="splitTreatmentTableData" item-key="id"
                                              class="elevation-1 v-table__overflow">
                                    <template slot="items" slot-scope="props">
                                        <td>
                                            <v-radio-group class="cash-flow-radio-group" v-model="splitTreatmentRadioValue" :mandatory="false">
                                                <v-radio :value="props.item.id"></v-radio>
                                            </v-radio-group>
                                        </td>
                                        <td>
                                            <v-edit-dialog :return-value.sync="props.item.description" large lazy persistent
                                                           @save="onEditSelectedLibraryListData(props.item, 'description')">
                                                <input class="output" type="text" :value="props.item.description" readonly />
                                                <template slot="input">
                                                    <v-textarea rows="5" no-resize outline label="Description"
                                                                v-model="props.item.description">
                                                    </v-textarea>
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
                                            <v-btn icon class="ara-orange" @click="onDeleteSplitTreatment(props.item)">
                                                <v-icon>fas fa-trash</v-icon>
                                            </v-btn>
                                        </td>
                                    </template>
                                </v-data-table>
                            </v-card-text>
                        </v-card>
                    </v-flex>
                    <v-flex xs4 v-if="selectedSplitTreatment.id !== '0'">
                        <v-card>
                            <v-card-title>
                                <v-btn @click="onAddSplitTreatmentLimit">
                                    <v-icon left class="plus-icon">fas fa-plus</v-icon>Add Distribution Rule
                                </v-btn>
                            </v-card-title>
                            <v-card-text class="cash-flow-library-card">
                                <v-data-table :headers="splitTreatmentLimitTableHeaders" :items="splitTreatmentLimitTableData"
                                              class="elevation-1 v-table__overflow">
                                    <template slot="items" slot-scope="props">
                                        <td>
                                            <v-edit-dialog :return-value.sync="props.item.rank" large lazy persistent full-width
                                                           @save="onEditSelectedLibraryListData(props.item, 'rank')">
                                                <input class="output" type="text" readonly :value="props.item.rank"
                                                       :class="{'invalid-input':splitTreatmentLimitRankNotLessThanOrEqualToPreviousRank(props.item) !== true}" />
                                                <template slot="input">
                                                    <v-text-field v-model.number="props.item.rank" label="Edit" single-line
                                                                  :rules="[splitTreatmentLimitRankNotLessThanOrEqualToPreviousRank(props.item)]">
                                                    </v-text-field>
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-edit-dialog :return-value.sync="props.item.amount" large lazy persistent full-width
                                                           @save="onEditSelectedLibraryListData(props.item, 'amount')">
                                                <input v-if="props.item.amount === null || props.item.amount === undefined || props.item.amount === ''"
                                                       class="output" type="text" readonly :value="props.item.amount" />
                                                <input v-else class="output" type="text" readonly :value="formatAsCurrency(props.item.amount)"
                                                       :class="{'invalid-input':splitTreatmentLimitAmountNotLessThanPreviousAmount(props.item) !== true}" />
                                                <template slot="input">
                                                    <v-text-field v-model.number="props.item.amount" label="Edit" single-line
                                                                  :rules="[splitTreatmentLimitAmountNotLessThanPreviousAmount(props.item)]">
                                                    </v-text-field>
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-edit-dialog :return-value.sync="props.item.percentage" large lazy persistent full-width
                                                           @save="onEditSelectedLibraryListData(props.item, 'percentage')">
                                                <input class="output" type="text" readonly :value="props.item.percentage"
                                                       :class="{'invalid-input':sumOfPercentsEqualsOneHundred(props.item.percentage) !== true}" />
                                                <template slot="input">
                                                    <v-text-field v-model="props.item.percentage" label="Edit" single-line
                                                                  :rules="[sumOfPercentsEqualsOneHundred]">
                                                    </v-text-field>
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-btn icon class="ara-orange" @click="onDeleteSplitTreatmentLimit(props.item)">
                                                <v-icon>fas fa-trash</v-icon>
                                            </v-btn>
                                        </td>
                                    </template>
                                </v-data-table>
                            </v-card-text>
                        </v-card>
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
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-blue-bg white--text" @click="onApplyToScenario"
                       :disabled="disableSubmitButtons()">
                    Save
                </v-btn>
                <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onUpdateLibrary"
                       :disabled="disableSubmitButtons()">
                    Update Library
                </v-btn>
                <v-btn class="ara-blue-bg white--text" @click="onCreateAsNewLibrary" :disabled="disableSubmitButtons()">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId === '0'" class="ara-orange-bg white--text" @click="onDeleteCashFlowLibrary">
                    Delete Library
                </v-btn>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-orange-bg white--text" @click="onDiscardChanges">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse" />

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
    import {clone, any, propEq, find, update, findIndex, isNil, prepend, append} from 'ramda';
    import {
        SplitTreatmentLimit,
        CashFlowLibrary,
        emptyCashFlowLibrary,
        emptySplitTreatment,
        SplitTreatment, emptySplitTreatmentLimit
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
    import {formatAsCurrency} from '@/shared/utils/currency-formatter';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {getLatestPropertyValue, getPropertyValuesNonUniq} from '@/shared/utils/getter-utils';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    const ObjectID = require('bson-objectid');

    @Component({
        components: {CreateCashFlowLibraryDialog, CriteriaEditorDialog, Alert}
    })
    export default class CashFlowEditor extends Vue {
        @State(state => state.cashFlow.cashFlowLibraries) stateCashFlowLibraries: CashFlowLibrary[];
        @State(state => state.cashFlow.selectedCashFlowLibrary) stateSelectedCashFlowLibrary: CashFlowLibrary;
        @State(state => state.cashFlow.scenarioCashFlowLibrary) stateScenarioCashFlowLibrary: CashFlowLibrary;

        @Action('getCashFlowLibraries') getCashFlowLibrariesAction: any;
        @Action('selectCashFlowLibrary') selectCashFlowLibraryAction: any;
        @Action('createCashFlowLibrary') createCashFlowLibraryAction: any;
        @Action('updateCashFlowLibrary') updateCashFlowLibraryAction: any;
        @Action('deleteCashFlowLibrary') deleteCashFlowLibraryAction: any;
        @Action('getScenarioCashFlowLibrary') getScenarioCashFlowLibraryAction: any;
        @Action('saveScenarioCashFlowLibrary') saveScenarioCashFlowLibraryAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        hasSelectedCashFlowLibrary: boolean = false;
        selectedScenarioId: string = '0';
        cashFlowLibrariesSelectListItems: SelectItem[] = [];
        cashFlowLibrarySelectItemValue: any = null;
        selectedCashFlowLibrary: CashFlowLibrary = clone(emptyCashFlowLibrary);
        splitTreatmentTableHeaders: DataTableHeader[] = [
            {text: 'Select', value: '', align: 'left', sortable: false, class: '', width: '5%'},
            {text: 'Rule Name', value: 'description', align: 'left', sortable: false, class: '', width: '25%'},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '65%'},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '5%'}
        ];
        splitTreatmentTableData: SplitTreatment[] = [];
        splitTreatmentRadioValue: string = '';
        selectedSplitTreatment: SplitTreatment = clone(emptySplitTreatment);
        selectedSplitTreatmentForCriteriaEdit: SplitTreatment = clone(emptySplitTreatment);
        splitTreatmentLimitTableHeaders: DataTableHeader[] = [
            {text: 'Duration (yr)', value: 'rank', align: 'left', sortable: false, class: '', width: '31.6%'},
            {text: 'Up to Amount', value: 'amount', align: 'left', sortable: false, class: '', width: '31.6%'},
            {text: 'Distribution (%)', value: 'percentage', align: 'left', sortable: false, class: '', width: '31.6%'},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '4.2%'}
        ];
        splitTreatmentLimitTableData: SplitTreatmentLimit[] = [];
        createCashFlowLibraryDialogData: CreateCashFlowLibraryDialogData = clone(emptyCreateCashFlowLibraryDialogData);
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        alertBeforeDelete: AlertData = clone(emptyAlertData);

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
                this.splitTreatmentTableData = clone(this.selectedCashFlowLibrary.splitTreatments);
            } else {
                this.hasSelectedCashFlowLibrary = false;
                this.splitTreatmentTableData = [];
            }
        }

        @Watch('splitTreatmentTableData')
        onSplitTreatmentTableDataChanged() {
            this.onSelectSplitTreatment();
        }

        @Watch('splitTreatmentRadioValue')
        onSplitTreatmentRadioValueChanged() {
            this.onSelectSplitTreatment();
        }

        @Watch('selectedSplitTreatment')
        onSelectedSplitTreatmentIdChanged() {
            if (this.selectedSplitTreatment.id === '0') {
                this.splitTreatmentLimitTableData = [];
            } else {
                if (hasValue(this.selectedSplitTreatment)) {
                    this.splitTreatmentLimitTableData = clone(this.selectedSplitTreatment.splitTreatmentLimits);
                }
            }
        }

        onSelectSplitTreatment() {
            const splitTreatment: SplitTreatment = find(
                propEq('id', this.splitTreatmentRadioValue), this.selectedCashFlowLibrary.splitTreatments
            ) as SplitTreatment;

            if (hasValue(splitTreatment)) {
                this.selectedSplitTreatment = clone(splitTreatment);
            } else {
                this.selectedSplitTreatment = clone(emptySplitTreatment);
            }
        }

        onClearSelectedCashFlowLibrary() {
            this.cashFlowLibrarySelectItemValue = null;
            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: emptyCashFlowLibrary});
        }

        onNewLibrary() {
            this.createCashFlowLibraryDialogData = clone({
                ...emptyCreateCashFlowLibraryDialogData,
                showDialog: true
            });
        }

        onCreateAsNewLibrary() {
            this.createCashFlowLibraryDialogData = {
                showDialog: true,
                splitTreatments: this.selectedCashFlowLibrary.splitTreatments
            };
        }

        onCreateCashFlowLibrary(createdCashFlowLibrary: CashFlowLibrary) {
            this.createCashFlowLibraryDialogData = clone(emptyCreateCashFlowLibraryDialogData);

            if (!isNil(createdCashFlowLibrary)) {
                this.createCashFlowLibraryAction({createdCashFlowLibrary: createdCashFlowLibrary});
            }
        }

        onAddSplitTreatment() {
            const newSplitTreatment: SplitTreatment = clone({
                ...emptySplitTreatment,
                id: ObjectID.generate()
            });
            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                    ...this.selectedCashFlowLibrary,
                    splitTreatments: prepend(newSplitTreatment, this.selectedCashFlowLibrary.splitTreatments)
                })
            });
        }

        onDeleteSplitTreatment(deletedSplitTreatment: SplitTreatment) {
            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                    ...this.selectedCashFlowLibrary,
                    splitTreatments: this.selectedCashFlowLibrary.splitTreatments
                        .filter((splitTreatment: SplitTreatment) => splitTreatment.id !== deletedSplitTreatment.id)
                })
            });
        }

        onAddSplitTreatmentLimit() {
            const newSplitTreatmentLimit: SplitTreatmentLimit = this.modifyNewSplitTreatmentLimitDefaultValues();

            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                    ...this.selectedCashFlowLibrary,
                    splitTreatments: update(
                        findIndex(propEq('id', this.selectedSplitTreatment.id), this.selectedCashFlowLibrary.splitTreatments),
                        clone({
                            ...this.selectedSplitTreatment,
                            splitTreatmentLimits: append(newSplitTreatmentLimit, this.selectedSplitTreatment.splitTreatmentLimits)
                        }),
                        this.selectedCashFlowLibrary.splitTreatments
                    )
                })
            });
        }

        modifyNewSplitTreatmentLimitDefaultValues() {
            const newSplitTreatmentLimit: SplitTreatmentLimit = clone({
                ...emptySplitTreatmentLimit,
                id: ObjectID.generate()
            });

            if (this.selectedSplitTreatment.splitTreatmentLimits.length === 0) {
                return newSplitTreatmentLimit;
            } else {
                const newRank: number = getLatestPropertyValue('rank', this.selectedSplitTreatment.splitTreatmentLimits) + 1;
                const newAmount: number = getLatestPropertyValue('amount', this.selectedSplitTreatment.splitTreatmentLimits);
                const newPercentages = this.getNewSplitTreatmentLimitPercentages(newRank);

                return clone({
                    ...newSplitTreatmentLimit,
                    rank: newRank,
                    amount: newSplitTreatmentLimit.amount! < newAmount ? newAmount : newSplitTreatmentLimit.amount,
                    percentage: newPercentages
                });
            }
        }

        getNewSplitTreatmentLimitPercentages(rank: number) {
            const  percentages: number[] = [];
            let percentage = 100 / rank;

            if (100 % rank !== 0) {
                percentage = Math.floor(percentage);

                for (let i = 0; i < rank; i++) {
                    if (i === rank - 1) {
                        const sumCurrentPercentages: number = percentages.reduce((x, y) => x + y);
                        percentages.push(100 - sumCurrentPercentages);
                    } else {
                        percentages.push(percentage);
                    }
                }
            } else {
                for (let i = 0; i < rank; i++) {
                    percentages.push(percentage);
                }
            }

            return percentages.join('/');
        }

        onDeleteSplitTreatmentLimit(deletedSplitTreatmentLimit: SplitTreatmentLimit) {
            this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                    ...this.selectedCashFlowLibrary,
                    splitTreatments: update(
                        findIndex(propEq('id', this.selectedSplitTreatment), this.selectedCashFlowLibrary.splitTreatments),
                        clone({
                            ...this.selectedSplitTreatment,
                            splitTreatmentLimits: this.selectedSplitTreatment.splitTreatmentLimits
                                .filter((splitTreatmentLimit: SplitTreatmentLimit) => splitTreatmentLimit.id !== deletedSplitTreatmentLimit.id)
                        }),
                        this.selectedCashFlowLibrary.splitTreatments
                    )
                })
            });
        }

        onEditCriteria(splitTreatment: SplitTreatment) {
            this.selectedSplitTreatmentForCriteriaEdit = clone(splitTreatment);

            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.selectedSplitTreatmentForCriteriaEdit.criteria
            };
        }

        onSubmitCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectCashFlowLibraryAction({selectedCashFlowLibrary: clone({
                        ...this.selectedCashFlowLibrary,
                        splitTreatments: update(
                            findIndex(propEq('id', this.selectedSplitTreatmentForCriteriaEdit.id), this.selectedCashFlowLibrary.splitTreatments),
                            clone({...this.selectedSplitTreatmentForCriteriaEdit, criteria: criteria}),
                            this.selectedCashFlowLibrary.splitTreatments
                        )
                    })
                });

                this.selectedSplitTreatmentForCriteriaEdit = clone(emptySplitTreatment);
            }
        }

        onEditSelectedLibraryListData(data: any, property: string) {
            const cashFlowLibrary: CashFlowLibrary = clone(this.selectedCashFlowLibrary);

            switch(property) {
                case 'description':
                    cashFlowLibrary.splitTreatments = update(
                        findIndex(propEq('id', data.id), cashFlowLibrary.splitTreatments),
                        data as SplitTreatment,
                        cashFlowLibrary.splitTreatments
                    );
                        break;
                case 'rank':
                case 'amount':
                case 'percentage':
                    cashFlowLibrary.splitTreatments = update(
                        findIndex(propEq('id', this.selectedSplitTreatment.id), cashFlowLibrary.splitTreatments),
                        clone({
                            ...this.selectedSplitTreatment,
                            splitTreatmentLimits: update(
                                findIndex(propEq('id', data.id), this.selectedSplitTreatment.splitTreatmentLimits),
                                {...data, amount: hasValue(data.amount) ? data.amount : null} as SplitTreatmentLimit,
                                this.selectedSplitTreatment.splitTreatmentLimits
                            )
                        }),
                        cashFlowLibrary.splitTreatments
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

            setTimeout(() => {
                if (this.stateScenarioCashFlowLibrary.id !== '0') {
                    this.selectCashFlowLibraryAction({selectedCashFlowLibrary: this.stateScenarioCashFlowLibrary});
                } else {
                    this.getScenarioCashFlowLibraryAction({selectedScenarioId: this.selectedScenarioId});
                }
            });
        }

        /**
         * Formats a value as currency by calling the formatAsCurrency utility function
         * @param value Value to format
         */
        formatAsCurrency(value: any) {
            return formatAsCurrency(value);
        }

        disableSubmitButtons() {
            let disabled: boolean = false;

            if (this.selectedSplitTreatment.id !== '0' && hasValue(this.selectedSplitTreatment.splitTreatmentLimits)) {
                if (this.selectedSplitTreatment.splitTreatmentLimits.length > 1) {
                    let index = 1;
                    while (!disabled && index !== this.selectedSplitTreatment.splitTreatmentLimits.length) {
                        disabled = this.splitTreatmentLimitRankNotLessThanOrEqualToPreviousRank(
                            this.selectedSplitTreatment.splitTreatmentLimits[index]) !== true;

                        if (!disabled) {
                            disabled = this.splitTreatmentLimitAmountNotLessThanPreviousAmount(
                                this.selectedSplitTreatment.splitTreatmentLimits[index]) !== true;
                        }

                        index++;
                    }
                }

                if (!disabled) {
                    const percentages: string[] = getPropertyValuesNonUniq(
                        'percentage', this.selectedSplitTreatment.splitTreatmentLimits);

                    if (percentages.length > 0) {
                        let index = 0;
                        while (!disabled && index !== percentages.length) {
                            disabled = this.sumOfPercentsEqualsOneHundred(percentages[index]) !== true;
                            index++;
                        }
                    }
                }
            }

            return disabled;
        }

        splitTreatmentLimitRankNotLessThanOrEqualToPreviousRank(splitTreatmentLimit: SplitTreatmentLimit) {
            const index: number = findIndex(propEq('id', splitTreatmentLimit.id), this.selectedSplitTreatment.splitTreatmentLimits);
            if (index > 0) {
                return this.selectedSplitTreatment.splitTreatmentLimits[index - 1].rank < splitTreatmentLimit.rank ||
                    'This split treatment limit year must be > than previous year';
            }

            return true;
        }

        splitTreatmentLimitAmountNotLessThanPreviousAmount(splitTreatmentLimit: SplitTreatmentLimit) {
            const index: number = findIndex(propEq('id', splitTreatmentLimit.id), this.selectedSplitTreatment.splitTreatmentLimits);
            if (index > 0) {
                return !hasValue(splitTreatmentLimit.amount) ||
                    (hasValue(splitTreatmentLimit.amount) && !hasValue(this.selectedSplitTreatment.splitTreatmentLimits[index - 1].amount)) ||
                    (hasValue(splitTreatmentLimit.amount) && hasValue(this.selectedSplitTreatment.splitTreatmentLimits[index - 1].amount) &&
                        this.selectedSplitTreatment.splitTreatmentLimits[index - 1].amount! <= splitTreatmentLimit.amount!) ||
                    'This split treatment limit amount must be >= to previous amount';
            }

            return true;
        }

        sumOfPercentsEqualsOneHundred(value: string) {
            if (value.indexOf('/')) {
                const percents: string[] = value.split('/');
                value = percents.reduce((x, y) => (parseInt(x) + parseInt(y)).toString());
            }

            return parseInt(value) === 100 || 'Percents must add up to 100';
        }

        onDeleteCashFlowLibrary() {
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
                this.deleteCashFlowLibraryAction({cashFlowLibrary: this.selectedCashFlowLibrary});
                this.onClearSelectedCashFlowLibrary();
            }
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

    .cash-flow-radio-group .v-input--radio-group__input {
        padding-top: 25px;
    }

    .output {
        border-bottom: 1px solid;
    }

    .cash-flow-library-card {
        height: 330px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .invalid-input {
        color: red;
    }

    .sharing label {
        padding-top: 0.5em;
    }

    .sharing {
        padding-top: 0;
        margin: 0;
    }
</style>