<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn @click="onNewLibrary" class="ara-blue-bg white--text" v-show="selectedScenarioId === '0'">
                        New Library
                    </v-btn>
                    <v-select :items="cashFlowLibrariesSelectListItems"
                              label="Select a Cash Flow Library" outline
                              v-if="!hasSelectedCashFlowLibrary || selectedScenarioId !== '0'"
                              v-model="selectItemValue">
                    </v-select>
                    <v-text-field label="Library Name" v-if="hasSelectedCashFlowLibrary && selectedScenarioId === '0'"
                                  v-model="selectedCashFlowLibrary.name"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]">
                        <template slot="append">
                            <v-btn @click="selectItemValue = null" class="ara-orange" icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedCashFlowLibrary && selectedScenarioId === '0'">
                        Owner: {{selectedCashFlowLibrary.owner ? selectedCashFlowLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" label="Shared"
                                v-if="hasSelectedCashFlowLibrary && selectedScenarioId === '0'"
                                v-model="selectedCashFlowLibrary.shared"/>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedCashFlowLibrary" xs12>
            <div class="cash-flow-library-tables">
                <v-layout justify-center row>
                    <v-flex xs8>
                        <v-card>
                            <v-card-title>
                                <v-btn @click="onAddSplitTreatment">
                                    <v-icon class="plus-icon" left>fas fa-plus</v-icon>
                                    Add Cash Flow Rule
                                </v-btn>
                            </v-card-title>
                            <v-card-text class="cash-flow-library-card">
                                <v-data-table :headers="splitTreatmentTableHeaders" :items="splitTreatmentTableData"
                                              class="elevation-1 v-table__overflow"
                                              item-key="id">
                                    <template slot="items" slot-scope="props">
                                        <td>
                                            <v-radio-group :mandatory="false"
                                                           class="cash-flow-radio-group"
                                                           v-model="splitTreatmentRadioValue">
                                                <v-radio :value="props.item.id"></v-radio>
                                            </v-radio-group>
                                        </td>
                                        <td>
                                            <v-edit-dialog :return-value.sync="props.item.description" large lazy
                                                           persistent
                                                           @save="onEditSelectedLibraryListData(props.item, 'description')">
                                                <v-text-field readonly single-line class="sm-txt"
                                                              :value="props.item.description"
                                                              :rules="[rules.generalRules.valueIsNotEmpty]"/>
                                                <template slot="input">
                                                    <v-textarea label="Description" no-resize outline rows="5"
                                                                :rules="[rules.generalRules.valueIsNotEmpty]"
                                                                v-model="props.item.description"/>
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-menu bottom min-height="500px" min-width="500px">
                                                <template slot="activator">
                                                    <v-text-field readonly single-line class="sm-txt"
                                                                  :value="props.item.criteria"/>
                                                </template>
                                                <v-card>
                                                    <v-card-text>
                                                        <v-textarea :value="props.item.criteria" full-width no-resize
                                                                    outline readonly rows="5"/>
                                                    </v-card-text>
                                                </v-card>
                                            </v-menu>

                                            <v-btn @click="onEditCriteria(props.item)" class="edit-icon" icon>
                                                <v-icon>fas fa-edit</v-icon>
                                            </v-btn>
                                        </td>
                                        <td>
                                            <v-btn @click="onDeleteSplitTreatment(props.item)" class="ara-orange" icon>
                                                <v-icon>fas fa-trash</v-icon>
                                            </v-btn>
                                        </td>
                                    </template>
                                </v-data-table>
                            </v-card-text>
                        </v-card>
                    </v-flex>
                    <v-flex v-if="selectedSplitTreatment.id !== '0'" xs4>
                        <v-card>
                            <v-card-title>
                                <v-btn @click="onAddSplitTreatmentLimit">
                                    <v-icon class="plus-icon" left>fas fa-plus</v-icon>
                                    Add Distribution Rule
                                </v-btn>
                            </v-card-title>
                            <v-card-text class="cash-flow-library-card">
                                <v-data-table :headers="splitTreatmentLimitTableHeaders"
                                              :items="splitTreatmentLimitTableData"
                                              class="elevation-1 v-table__overflow">
                                    <template slot="items" slot-scope="props">
                                        <td>
                                            <v-edit-dialog :return-value.sync="props.item.rank"
                                                           @save="onEditSelectedLibraryListData(props.item, 'rank')"
                                                           full-width large
                                                           lazy
                                                           persistent>
                                                <v-text-field readonly single-line class="sm-txt"
                                                              :value="props.item.rank"
                                                              :rules="[rules['generalRules'].valueIsNotEmpty, rules['cashFlowRules'].isRankGreaterThanPreviousRank(props.item, selectedSplitTreatment)]"/>
                                                <template slot="input">
                                                    <v-text-field label="Edit" single-line
                                                                  v-model.number="props.item.rank"
                                                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['cashFlowRules'].isRankGreaterThanPreviousRank(props.item, selectedSplitTreatment)]"/>
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-edit-dialog :return-value.sync="props.item.amount" large lazy persistent
                                                           full-width
                                                           @save="onEditSelectedLibraryListData(props.item, 'amount')">
                                                <v-text-field readonly single-line class="sm-txt"
                                                              :value="formatAsCurrency(props.item.amount)"
                                                              :rules="[rules['generalRules'].valueIsNotEmpty, rules['cashFlowRules'].isAmountGreaterThanOrEqualToPreviousAmount(props.item, selectedSplitTreatment)]"/>
                                                <template slot="input">
                                                    <v-text-field label="Edit" single-line
                                                                  v-model.number="props.item.amount"
                                                                  v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: false}"
                                                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['cashFlowRules'].isAmountGreaterThanOrEqualToPreviousAmount(props.item, selectedSplitTreatment)]"/>
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-edit-dialog :return-value.sync="props.item.percentage"
                                                           @save="onEditSelectedLibraryListData(props.item, 'percentage')"
                                                           full-width
                                                           large lazy
                                                           persistent>
                                                <v-text-field readonly single-line class="sm-txt"
                                                              :value="props.item.percentage"
                                                              :rules="[rules['generalRules'].valueIsNotEmpty, rules['cashFlowRules'].doesTotalOfPercentsEqualOneHundred]"/>
                                                <template slot="input">
                                                    <v-text-field label="Edit" single-line
                                                                  v-model="props.item.percentage"
                                                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['cashFlowRules'].doesTotalOfPercentsEqualOneHundred]"/>
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-btn @click="onDeleteSplitTreatmentLimit(props.item)" class="ara-orange"
                                                   icon>
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
        <v-flex v-show="hasSelectedCashFlowLibrary && selectedScenarioId === '0'" xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea label="Description" no-resize outline rows="4"
                                v-model="selectedCashFlowLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout justify-end row v-show="hasSelectedCashFlowLibrary">
                <v-btn :disabled="disableSubmitAction()" @click="onApplyToScenario" class="ara-blue-bg white--text"
                       v-show="selectedScenarioId !== '0'">
                    Save
                </v-btn>
                <v-btn :disabled="disableSubmitAction()" @click="onUpdateLibrary" class="ara-blue-bg white--text"
                       v-show="selectedScenarioId === '0'">
                    Update Library
                </v-btn>
                <v-btn :disabled="disableSubmitAction()" @click="onCreateAsNewLibrary" class="ara-blue-bg white--text">
                    Create as New Library
                </v-btn>
                <v-btn @click="onDeleteCashFlowLibrary" class="ara-orange-bg white--text"
                       v-show="selectedScenarioId === '0'" :disabled="!hasSelectedCashFlowLibrary">
                    Delete Library
                </v-btn>
                <v-btn @click="onDiscardChanges" class="ara-orange-bg white--text" v-show="selectedScenarioId !== '0'"
                       :disabled="!hasSelectedCashFlowLibrary">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse"/>

        <CreateCashFlowLibraryDialog :dialogData="createCashFlowLibraryDialogData" @submit="onCreateCashFlowLibrary"/>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData"
                              @submitCriteriaEditorDialogResult="onSubmitCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {append, clone, find, findIndex, isNil, prepend, propEq, update} from 'ramda';
    import {
        CashFlowLibrary,
        emptyCashFlowLibrary,
        emptySplitTreatment,
        emptySplitTreatmentLimit,
        SplitTreatment,
        SplitTreatmentLimit
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
    import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';

    const ObjectID = require('bson-objectid');

    @Component({
        components: {CreateCashFlowLibraryDialog, CriteriaEditorDialog, Alert}
    })
    export default class CashFlowEditor extends Vue {
        @State(state => state.cashFlowEditor.cashFlowLibraries) stateCashFlowLibraries: CashFlowLibrary[];
        @State(state => state.cashFlowEditor.selectedCashFlowLibrary) stateSelectedCashFlowLibrary: CashFlowLibrary;
        @State(state => state.cashFlowEditor.scenarioCashFlowLibrary) stateScenarioCashFlowLibrary: CashFlowLibrary;

        @Action('getCashFlowLibraries') getCashFlowLibrariesAction: any;
        @Action('selectCashFlowLibrary') selectCashFlowLibraryAction: any;
        @Action('createCashFlowLibrary') createCashFlowLibraryAction: any;
        @Action('updateCashFlowLibrary') updateCashFlowLibraryAction: any;
        @Action('deleteCashFlowLibrary') deleteCashFlowLibraryAction: any;
        @Action('getScenarioCashFlowLibrary') getScenarioCashFlowLibraryAction: any;
        @Action('saveScenarioCashFlowLibrary') saveScenarioCashFlowLibraryAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        hasSelectedCashFlowLibrary: boolean = false;
        selectedScenarioId: string = '0';
        cashFlowLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string | null = null;
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
        objectIdMOngoDBForScenario: string = '';
        rules: InputValidationRules = clone(rules);

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/CashFlowEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;
                    vm.objectIdMOngoDBForScenario = to.query.objectIdMOngoDBForScenario;
                    if (vm.selectedScenarioId === '0') {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.selectItemValue = null;
                vm.getCashFlowLibrariesAction().then(() => {
                    if (vm.selectedScenarioId !== '0') {
                        vm.getScenarioCashFlowLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});
                    }
                });
            });
        }

        beforeDestroy() {
            this.setHasUnsavedChangesAction({value: false});
        }

        @Watch('stateCashFlowLibraries')
        onStateCashFlowLibrariesChanged() {
            this.cashFlowLibrariesSelectListItems = this.stateCashFlowLibraries.map((cashFlowLibrary: CashFlowLibrary) => ({
                text: cashFlowLibrary.name,
                value: cashFlowLibrary.id
            }));
        }

        @Watch('selectItemValue')
        onCashFlowLibrarySelectItemChanged() {
            this.selectCashFlowLibraryAction({selectedLibraryId: this.selectItemValue});
        }

        @Watch('stateSelectedCashFlowLibrary')
        onStateSelectedCashFlowLibraryChanged() {
            this.selectedCashFlowLibrary = clone(this.stateSelectedCashFlowLibrary);
        }

        @Watch('selectedCashFlowLibrary')
        onSelectedCashFlowLibraryChanged() {
            this.setHasUnsavedChangesAction({
                value: hasUnsavedChanges(
                    'cashflow', this.selectedCashFlowLibrary, this.stateSelectedCashFlowLibrary, this.stateScenarioCashFlowLibrary
                )
            });
            this.hasSelectedCashFlowLibrary = this.selectedCashFlowLibrary.id !== '0';
            this.splitTreatmentTableData = clone(this.selectedCashFlowLibrary.splitTreatments);
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

        onNewLibrary() {
            this.createCashFlowLibraryDialogData = {
                ...emptyCreateCashFlowLibraryDialogData,
                showDialog: true
            };
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
            const newSplitTreatment: SplitTreatment = {
                ...emptySplitTreatment,
                description: `Unnamed Rule ${this.selectedCashFlowLibrary.splitTreatments.length + 1}`,
                id: ObjectID.generate()
            };

            this.selectedCashFlowLibrary = {
                ...this.selectedCashFlowLibrary,
                splitTreatments: prepend(newSplitTreatment, this.selectedCashFlowLibrary.splitTreatments)
            };
        }

        onDeleteSplitTreatment(deletedSplitTreatment: SplitTreatment) {
            this.selectedCashFlowLibrary = {
                ...this.selectedCashFlowLibrary,
                splitTreatments: this.selectedCashFlowLibrary.splitTreatments
                    .filter((splitTreatment: SplitTreatment) => splitTreatment.id !== deletedSplitTreatment.id)
            };
        }

        onAddSplitTreatmentLimit() {
            const newSplitTreatmentLimit: SplitTreatmentLimit = this.modifyNewSplitTreatmentLimitDefaultValues();

            this.selectedCashFlowLibrary = {
                ...this.selectedCashFlowLibrary,
                splitTreatments: update(
                    findIndex(propEq('id', this.selectedSplitTreatment.id), this.selectedCashFlowLibrary.splitTreatments),
                    {
                        ...this.selectedSplitTreatment,
                        splitTreatmentLimits: append(newSplitTreatmentLimit, this.selectedSplitTreatment.splitTreatmentLimits)
                    },
                    this.selectedCashFlowLibrary.splitTreatments
                )
            };
        }

        modifyNewSplitTreatmentLimitDefaultValues() {
            const newSplitTreatmentLimit: SplitTreatmentLimit = {
                ...emptySplitTreatmentLimit,
                id: ObjectID.generate()
            };

            if (this.selectedSplitTreatment.splitTreatmentLimits.length === 0) {
                return newSplitTreatmentLimit;
            } else {
                const newRank: number = getLatestPropertyValue('rank', this.selectedSplitTreatment.splitTreatmentLimits) + 1;
                const newAmount: number = getLatestPropertyValue('amount', this.selectedSplitTreatment.splitTreatmentLimits);
                const newPercentages = this.getNewSplitTreatmentLimitPercentages(newRank);

                return {
                    ...newSplitTreatmentLimit,
                    rank: newRank,
                    amount: newSplitTreatmentLimit.amount! < newAmount ? newAmount : newSplitTreatmentLimit.amount,
                    percentage: newPercentages
                };
            }
        }

        getNewSplitTreatmentLimitPercentages(rank: number) {
            const percentages: number[] = [];
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
            this.selectedCashFlowLibrary = {
                ...this.selectedCashFlowLibrary,
                splitTreatments: update(
                    findIndex(propEq('id', this.selectedSplitTreatment), this.selectedCashFlowLibrary.splitTreatments),
                    {
                        ...this.selectedSplitTreatment,
                        splitTreatmentLimits: this.selectedSplitTreatment.splitTreatmentLimits
                            .filter((splitTreatmentLimit: SplitTreatmentLimit) => splitTreatmentLimit.id !== deletedSplitTreatmentLimit.id)
                    },
                    this.selectedCashFlowLibrary.splitTreatments
                )
            };
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
                const updatedSplitTreatments = update(
                    findIndex(propEq('id', this.selectedSplitTreatmentForCriteriaEdit.id), this.selectedCashFlowLibrary.splitTreatments),
                    {...this.selectedSplitTreatmentForCriteriaEdit, criteria: criteria},
                    this.selectedCashFlowLibrary.splitTreatments
                );

                this.selectedCashFlowLibrary = {
                    ...this.selectedCashFlowLibrary,
                    splitTreatments: updatedSplitTreatments
                };

                this.selectedSplitTreatmentForCriteriaEdit = clone(emptySplitTreatment);
            }
        }

        onEditSelectedLibraryListData(data: any, property: string) {
            let updatedSplitTreatments: SplitTreatment[] = clone(this.selectedCashFlowLibrary.splitTreatments);

            switch (property) {
                case 'description':
                    updatedSplitTreatments = update(
                        findIndex(propEq('id', data.id), updatedSplitTreatments),
                        data as SplitTreatment,
                        updatedSplitTreatments
                    );
                    break;
                case 'rank':
                case 'amount':
                case 'percentage':
                    updatedSplitTreatments = update(
                        findIndex(propEq('id', this.selectedSplitTreatment.id), updatedSplitTreatments),
                        {
                            ...this.selectedSplitTreatment,
                            splitTreatmentLimits: update(
                                findIndex(propEq('id', data.id), this.selectedSplitTreatment.splitTreatmentLimits),
                                {
                                    ...data,
                                    amount: hasValue(data.amount)
                                        ? parseFloat(data.amount.toString()
                                            .replace(/(\$*)(\,*)/g, ''))
                                        : null
                                } as SplitTreatmentLimit,
                                this.selectedSplitTreatment.splitTreatmentLimits
                            )
                        },
                        updatedSplitTreatments
                    );
            }

            this.selectedCashFlowLibrary = {
                ...this.selectedCashFlowLibrary,
                splitTreatments: updatedSplitTreatments
            };
        }

        onApplyToScenario() {
            this.saveScenarioCashFlowLibraryAction({
                scenarioCashFlowLibrary: {
                    ...this.selectedCashFlowLibrary,
                    id: this.selectedScenarioId
                },
                objectIdMOngoDBForScenario: this.objectIdMOngoDBForScenario
            }).then(() => this.onDiscardChanges());
        }

        onDiscardChanges() {
            this.selectItemValue = null;
            setTimeout(() => this.selectCashFlowLibraryAction({selectedLibraryId: this.stateScenarioCashFlowLibrary.id}));
        }

        onUpdateLibrary() {
            this.updateCashFlowLibraryAction({updatedCashFlowLibrary: this.selectedCashFlowLibrary});
        }

        /**
         * Formats a value as currency by calling the formatAsCurrency utility function
         * @param value Value to format
         */
        formatAsCurrency(value: any) {
            if (hasValue(value)) {
                return formatAsCurrency(value);
            }
            return null;
        }

        disableSubmitAction() {
            if (this.hasSelectedCashFlowLibrary) {
                const allDataIsValid = this.selectedCashFlowLibrary.splitTreatments.every((st: SplitTreatment) => {
                    const allSubDataIsValid = st.splitTreatmentLimits.every((stl: SplitTreatmentLimit, index: number) => {
                        let isValid: boolean = this.rules['generalRules'].valueIsNotEmpty(stl.rank) === true &&
                            this.rules['generalRules'].valueIsNotEmpty(stl.amount) === true &&
                            this.rules['generalRules'].valueIsNotEmpty(stl.percentage) === true &&
                            this.rules['cashFlowRules'].doesTotalOfPercentsEqualOneHundred(stl.percentage) === true;

                        if (index !== 0) {
                            isValid = isValid && this.rules['cashFlowRules'].isRankGreaterThanPreviousRank(stl, st) === true &&
                                this.rules['cashFlowRules'].isAmountGreaterThanOrEqualToPreviousAmount(stl, st) === true;
                        }

                        return isValid;
                    });

                    return this.rules['generalRules'].valueIsNotEmpty(st.description) === true && allSubDataIsValid;
                });

                if (this.selectedScenarioId !== '0') {
                    return !allDataIsValid;
                } else {
                    return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedCashFlowLibrary.name) === true &&
                        allDataIsValid);
                }
            }

            return true;
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
                this.selectItemValue = null;
                this.deleteCashFlowLibraryAction({cashFlowLibrary: this.selectedCashFlowLibrary});
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

    .amount-div {
        width: 208px;
    }

    .split-treatment-limit-currency-input {
        border: 1px solid;
        width: 100%;
    }

    .split-treatment-limit-amount-rule-span {
        font-size: 0.8em;
    }

    .sharing label {
        padding-top: 0.5em;
    }

    .sharing {
        padding-top: 0;
        margin: 0;
    }
</style>
