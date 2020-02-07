<template>
    <v-dialog v-model="dialogData.showDialog" persistent scrollable max-width="1000px">
        <v-card>
            <v-card-text class="criteria-editor-card-text">
                <v-layout column>
                    <v-layout justify-space-between row>
                        <v-flex xs5>
                            <v-card>
                                <v-card-title>
                                    <v-layout justify-center><h3>Output</h3></v-layout>
                                </v-card-title>
                                <div class="conjunction-and-messages-container">
                                    <div class="sub-text">
                                        <v-layout justify-start>
                                            <p class="invalid-message" v-if="invalidCriteriaMessage !== null">{{invalidCriteriaMessage}}</p>
                                            <p class="valid-message" v-if="validCriteriaMessage !== null">{{validCriteriaMessage}}</p>
                                        </v-layout>
                                    </div>
                                    <v-layout justify-space-between>
                                        <div class="conjunction-select-list-container">
                                            <v-select class="conjunction-select-list" :items="conjunctionSelectListItems"
                                                      v-model="selectedConjunction" lablel="Select a Conjunction">
                                            </v-select>
                                        </div>
                                        <v-btn class="ara-blue-bg white--text" @click="onAddSubCriteria">Add Criteria</v-btn>
                                    </v-layout>
                                </div>
                                <v-card-text class="clauses-card">
                                    <div v-for="(clause, index) in subCriteriaClauses">
                                        <v-layout column>
                                            <v-textarea class="clause-textarea" rows="3" no-resize box :class="{'textarea-focused': index === selectedSubCriteriaClauseIndex}"
                                                         readonly full-width :value="clause" @click="onClickSubCriteriaClauseTextarea(clause, index)">
                                                <template slot="append">
                                                    <v-btn class="ara-orange" icon @click="onRemoveSubCriteria(index)">
                                                        <v-icon>fas fa-times</v-icon>
                                                    </v-btn>
                                                </template>
                                            </v-textarea>
                                            <v-layout justify-center v-if="subCriteriaClauses.length > 1 && index !== subCriteriaClauses.length - 1">
                                                <p>{{selectedConjunction}}</p>
                                            </v-layout>
                                        </v-layout>
                                    </div>
                                </v-card-text>
                                <v-card-actions>
                                    <v-btn class="ara-blue-bg white--text" @click="onCheckCriteria" :disabled="onDisableCriteriaCheck()">
                                        Check
                                    </v-btn>
                                </v-card-actions>
                            </v-card>
                        </v-flex>
                        <v-flex xs7>
                            <v-card>
                                <v-card-title>
                                    <v-layout justify-center><h3>Criteria Editor</h3></v-layout>
                                </v-card-title>
                                <div class="sub-criteria-messages-container">
                                    <div class="sub-text">
                                        <v-layout justify-start>
                                            <p class="invalid-message" v-if="invalidSubCriteriaMessage !== null">{{invalidSubCriteriaMessage}}</p>
                                            <p class="valid-message" v-if="validSubCriteriaMessage !== null">{{validSubCriteriaMessage}}</p>
                                        </v-layout>
                                    </div>
                                </div>
                                <v-card-text class="criteria-editor-container">
                                    <v-tabs v-if="selectedSubCriteriaClauseIndex !== -1" centered>
                                        <v-tab ripple @click="onParseRawSubCriteria">
                                            Tree View
                                        </v-tab>
                                        <v-tab ripple @click="onParseSubCriteriaJson">
                                            Raw Criteria
                                        </v-tab>
                                        <v-tab-item>
                                            <vue-query-builder v-if="queryBuilderRules.length > 0" :labels="queryBuilderLabels"
                                                               :rules="queryBuilderRules" :maxDepth="25" :styled="true"
                                                               v-model="selectedSubCriteriaClause">
                                            </vue-query-builder>
                                        </v-tab-item>
                                        <v-tab-item>
                                            <v-textarea rows="20" no-resize outline v-model="selectedRawSubCriteriaClause"></v-textarea>
                                        </v-tab-item>
                                    </v-tabs>
                                </v-card-text>
                                <v-card-actions>
                                    <v-btn class="ara-blue-bg white--text" @click="onCheckSubCriteria" :disabled="onDisableSubCriteriaCheck()">
                                        Check
                                    </v-btn>
                                </v-card-actions>
                            </v-card>
                        </v-flex>
                    </v-layout>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)" :disabled="cannotSubmit">
                        Save
                    </v-btn>
                    <v-btn class="ara-orange-bg white--text" @click="onSubmit(false)">Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import VueQueryBuilder from 'vue-query-builder/src/VueQueryBuilder.vue';
    import {Criteria, CriteriaType, emptyCriteria} from '../models/iAM/criteria';
    import {parseCriteriaString, parseCriteriaJson, parseCriteriaTypeJson} from '../utils/criteria-editor-parsers';
    import {hasValue} from '../utils/has-value-util';
    import {CriteriaEditorDialogData} from '../models/modals/criteria-editor-dialog-data';
    import {isEmpty, clone, update, remove, findIndex, isNil, equals} from 'ramda';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import CriteriaEditorService from '@/services/criteria-editor.service';
    import {AxiosResponse} from 'axios';
    import {SelectItem} from '@/shared/models/vue/select-item';

    @Component({
        components: {VueQueryBuilder}
    })
    export default class CriteriaEditorDialog extends Vue {
        @Prop() dialogData: CriteriaEditorDialogData;

        @State(state => state.attribute.attributes) stateAttributes: Attribute[];

        @Action('getAttributes') getAttributesAction: any;

        mainCriteria: Criteria = clone(emptyCriteria);
        queryBuilderRules: any[] = [];
        queryBuilderLabels: object = {
            'matchType': '',
            'matchTypes': [
                {'id': 'AND', 'label': 'AND'},
                {'id': 'OR', 'label': 'OR'}
            ],
            'addRule': 'Add Rule',
            'removeRule': '&times;',
            'addGroup': 'Add Group',
            'removeGroup': '&times;',
            'textInputPlaceholder': 'value'
        };
        cannotSubmit: boolean = true;
        validCriteriaMessage: string | null = null;
        invalidCriteriaMessage: string | null = null;
        validSubCriteriaMessage: string | null = null;
        invalidSubCriteriaMessage: string | null = null;
        conjunctionSelectListItems: SelectItem[] = [
            {text: 'OR', value: 'OR'},
            {text: 'AND', value: 'AND'}
        ];
        selectedConjunction: string = 'OR';
        subCriteriaClauses: string[] = [];
        selectedSubCriteriaClauseIndex: number = -1;
        selectedSubCriteriaClause: Criteria | null = null;
        selectedRawSubCriteriaClause: string = '';
        activeTab = 'tree-view';

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateAttributes)) {
                this.setQueryBuilderRules();
            }
        }

        /**
         * Setter: mainCriteria property
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.mainCriteria = parseCriteriaString(this.dialogData.criteria) as Criteria;
            if (this.mainCriteria && isEmpty(this.mainCriteria.logicalOperator)) {
                this.mainCriteria.logicalOperator = 'OR';
            }
        }

        /**
         * Calls setQueryBuilderRules func. if stateAttributes property has value
         */
        @Watch('stateAttributes')
        onStateAttributesChanged() {
            if (hasValue(this.stateAttributes)) {
                this.setQueryBuilderRules();
            }
        }

        /**
         * Calls the setter for the subCriteriaClauses property and sets related component UI properties
         */
        @Watch('mainCriteria')
        onCriteriaChanged() {
            this.resetComponentCriteriaUIProperties();
            this.setSubCriteriaClauses();
        }

        /**
         * Setter: selectedConjunction property
         */
        @Watch('subCriteriaClauses')
        onSubCriteriaClausesChanged() {
            if (!hasValue(this.selectedConjunction) && hasValue(this.mainCriteria)) {
                this.selectedConjunction = hasValue(this.mainCriteria.logicalOperator)
                    ? this.mainCriteria.logicalOperator : 'OR';
            }
        }

        /**
         * Calls the resetSubCriteriaValidationMessageProperties func. on selectedSubCriteriaClause property change
         */
        @Watch('selectedSubCriteriaClause')
        onSelectedClauseChanged() {
            console.log(this.selectedSubCriteriaClause);
            this.resetSubCriteriaValidationMessageProperties();
        }

        /**
         * Calls the resetSubCriteriaValidationMessageProperties func. on selectedRawSubCriteriaClause property change
         */
        @Watch('selectedRawSubCriteriaClause')
        onSelectedClauseRawChanged() {
            this.resetSubCriteriaValidationMessageProperties();
        }

        /**
         * Setter: queryBuilderRules property
         */
        setQueryBuilderRules() {
            this.queryBuilderRules = this.stateAttributes.map((attribute: Attribute) => ({
                type: 'text',
                label: attribute.name,
                id: attribute.name,
                operators: ['=', '<>', '<', '<=', '>', '>=']
            }));
        }

        /**
         * Setter: subCriteriaClauses property
         */
        setSubCriteriaClauses() {
            this.subCriteriaClauses = [];
            if (hasValue(this.mainCriteria) && hasValue(this.mainCriteria.children)) {
                this.mainCriteria.children!.forEach((criteriaType: CriteriaType) => {
                    const clause: string = parseCriteriaTypeJson(criteriaType);
                    if (hasValue(clause)) {
                        this.subCriteriaClauses.push(clause);
                    }
                });
            }
        }

        /**
         * Resets component criteria UI related properties to their default values
         */
        resetComponentCriteriaUIProperties() {
            this.validCriteriaMessage = null;
            this.invalidCriteriaMessage = null;
            this.cannotSubmit = true;
        }

        /**
         * Resets component sub-criteria UI related properties to their default values
         */
        resetSubCriteriaValidationMessageProperties() {
            this.validSubCriteriaMessage = null;
            this.invalidSubCriteriaMessage = null;
        }

        /**
         * Resets component sub-criteria 'selected' named properties to their default values
         */
        resetSubCriteriaSelectedProperties() {
            this.selectedSubCriteriaClauseIndex = -1;
            this.selectedSubCriteriaClause = null;
            this.selectedRawSubCriteriaClause = '';
        }

        /**
         * Functionality for adding a new clause
         */
        onAddSubCriteria() {
            this.resetSubCriteriaSelectedProperties();
            setTimeout(() => {
                this.subCriteriaClauses.push('');
                this.selectedSubCriteriaClauseIndex = this.subCriteriaClauses.length - 1;
                this.selectedSubCriteriaClause = clone(emptyCriteria);
            });
        }

        /**
         * Sets the selectedSubCriteriaClause & selectedSubCriteriaClauseIndex property with the specified clause & clauseIndex parameters;
         * sets an invalid sub-criteria message if the clause cannot be parsed
         */
        onClickSubCriteriaClauseTextarea(subCriteriaClause: string, subCriteriaClauseIndex: number) {
            this.resetSubCriteriaSelectedProperties();
            this.selectedSubCriteriaClauseIndex = subCriteriaClauseIndex;
            this.selectedSubCriteriaClause = parseCriteriaString(subCriteriaClause);
            if (this.selectedSubCriteriaClause) {
                if (!hasValue(this.selectedSubCriteriaClause.logicalOperator)) {
                    this.selectedSubCriteriaClause.logicalOperator = 'OR';
                }
            } else {
                this.invalidSubCriteriaMessage = 'Unable to parse selected criteria';
            }
        }

        /**
         * Removes a sub-criteria from the subCriteriaClauses property
         */
        onRemoveSubCriteria(subCriteriaClauseIndex: number) {
            this.subCriteriaClauses = remove(subCriteriaClauseIndex, 1, this.subCriteriaClauses);

            if (this.selectedSubCriteriaClauseIndex === subCriteriaClauseIndex) {
                this.resetSubCriteriaSelectedProperties();
            } else {
                this.selectedSubCriteriaClauseIndex = findIndex((subCriteriaClause: string) => {
                    const parsedCriteriaJson = parseCriteriaJson(this.selectedSubCriteriaClause as Criteria);
                    if (parsedCriteriaJson) {
                        return subCriteriaClause === parsedCriteriaJson.join('');
                    }
                    return subCriteriaClause === this.selectedRawSubCriteriaClause;
                }, this.subCriteriaClauses);
            }
        }

        /**
         * Parses the raw criteria for the tree view if valid; otherwise sets an invalid sub-criteria message
         */
        onParseRawSubCriteria() {
            this.activeTab = 'tree-view';
            this.resetSubCriteriaValidationMessageProperties();
            const parsedRawSubCriteria = parseCriteriaString(this.selectedRawSubCriteriaClause);
            if (parsedRawSubCriteria) {
                this.selectedSubCriteriaClause = parsedRawSubCriteria;
                if (!hasValue(this.selectedSubCriteriaClause.logicalOperator)) {
                    this.selectedSubCriteriaClause.logicalOperator = 'OR';
                }
            } else {
                this.invalidSubCriteriaMessage = 'The raw criteria string is invalid';
            }
        }

        /**
         * Parses the tree view criteria json for the raw criteria view
         */
        onParseSubCriteriaJson() {
            this.activeTab = 'raw-criteria';
            this.resetSubCriteriaValidationMessageProperties();
            const parsedSubCriteria = parseCriteriaJson(this.selectedSubCriteriaClause as Criteria);
            if (parsedSubCriteria) {
                this.selectedRawSubCriteriaClause = parsedSubCriteria.join('');
            } else {
                this.invalidSubCriteriaMessage = 'The criteria json is invalid';
            }
        }

        /**
         * Checks criteria editor validity and if valid modifies the main criteria with the editor changes; otherwise
         * sets an invalid criteria message
         */
        onCheckCriteria() {
            this.resetSubCriteriaSelectedProperties();

            const criteria: Criteria = {
                logicalOperator: this.selectedConjunction,
                children: this.subCriteriaClauses
                    .filter((subCriteriaClause: string) => !isEmpty(subCriteriaClause))
                    .map((subCriteriaClause: string) => {
                        const parsedSubCriteriaClause: Criteria = parseCriteriaString(subCriteriaClause) as Criteria;
                        if (parsedSubCriteriaClause.children!.length === 1) {
                            return parsedSubCriteriaClause.children![0];
                        }
                        return {
                            type: 'query-builder-group',
                            query: {
                                logicalOperator: parsedSubCriteriaClause.logicalOperator,
                                children: parsedSubCriteriaClause.children
                            }
                        };
                    })
            };

            const parsedCriteria = parseCriteriaJson(criteria);
            if (parsedCriteria) {
                CriteriaEditorService.checkCriteriaValidity({criteria: parsedCriteria.join('')})
                    .then((response: AxiosResponse<string>) => {
                        if (response.data.indexOf('results match query') !== -1) {
                            this.mainCriteria = clone(criteria);
                            setTimeout(() => {
                                this.validCriteriaMessage = response.data;
                                this.cannotSubmit = false;
                            });
                        } else {
                            this.resetComponentCriteriaUIProperties();
                            setTimeout(() => this.invalidCriteriaMessage = response.data);
                        }
                    });
            } else {
                this.invalidCriteriaMessage = 'Unable to parse criteria';
            }
        }

        /**
         * Checks the validity of the currently selected sub-criteria and sets an appropriate validation message based
         * on the validity
         */
        onCheckSubCriteria() {
            const criteria = this.getSubCriteriaValueToCheck();

            if (isNil(criteria)) {
                this.invalidSubCriteriaMessage = 'Unable to parse criteria';
                return;
            }
            if (isEmpty(criteria)) {
                this.invalidSubCriteriaMessage = 'No criteria to evaluate';
                return;
            }

            CriteriaEditorService.checkCriteriaValidity({criteria: criteria})
                .then((response: AxiosResponse<string>) => {
                    this.resetSubCriteriaValidationMessageProperties();

                    if (response.data.indexOf('results match query') !== -1) {
                        this.resetComponentCriteriaUIProperties();
                        this.validSubCriteriaMessage = response.data;
                        this.subCriteriaClauses = update(
                            this.selectedSubCriteriaClauseIndex,
                            criteria,
                            this.subCriteriaClauses
                        );
                    } else {
                        this.invalidSubCriteriaMessage = response.data;
                    }
                });
        }

        /**
         * Getter: the sub-criteria value to check
         */
        getSubCriteriaValueToCheck() {
            if (this.activeTab === 'tree-view') {
                const parsedCriteriaJson = parseCriteriaJson(this.selectedSubCriteriaClause as Criteria);
                if (parsedCriteriaJson) {
                    return parsedCriteriaJson.join('');
                }
                return null;
            }
            return this.selectedRawSubCriteriaClause;
        }

        /**
         * Emits the parsed criteria object's data to the calling parent component or null if the user clicked the
         * 'Cancel' button
         */
        onSubmit(submit: boolean) {
            this.resetSubCriteriaSelectedProperties();
            this.resetComponentCriteriaUIProperties();

            if (submit) {
                const parsedCriteria = parseCriteriaJson(this.mainCriteria);
                if (parsedCriteria) {
                    this.selectedConjunction = 'OR';
                    this.$emit('submit', parsedCriteria.join(''));
                } else {
                    this.invalidCriteriaMessage = 'Unable to parse the criteria';
                }
            } else {
                this.selectedConjunction = 'OR';
                this.$emit('submit', null);
            }
        }

        /**
         * Determines whether or not the main criteria 'Check' button should be disabled
         */
        onDisableCriteriaCheck() {
            const subCriteriaClausesAreEmpty = this.subCriteriaClauses
                .every((subCriteriaClause: string) => isEmpty(subCriteriaClause));
            return (equals(this.mainCriteria, emptyCriteria) && subCriteriaClausesAreEmpty) ||
                   (!hasValue(this.mainCriteria.children) && subCriteriaClausesAreEmpty);
        }

        /**
         * Determines whether or not the sub-criteria 'Check' button should be disabled
         */
        onDisableSubCriteriaCheck() {
            const parsedSelectedSubCriteriaClause = parseCriteriaJson(this.selectedSubCriteriaClause as Criteria);
            const parsedSelectedRawSubCriteriaClause = parseCriteriaJson(parseCriteriaString(this.selectedRawSubCriteriaClause) as Criteria);
            return this.selectedSubCriteriaClauseIndex === -1 ||
                   (
                       this.activeTab === 'tree-view' &&
                       (
                           parsedSelectedSubCriteriaClause === null ||
                           equals(this.selectedSubCriteriaClause, emptyCriteria)) ||
                           (parsedSelectedSubCriteriaClause && isEmpty(parsedSelectedSubCriteriaClause.join(''))
                       )
                   ) ||
                   (
                       this.activeTab === 'raw-criteria' &&
                       (
                           isEmpty(this.selectedRawSubCriteriaClause) ||
                           parsedSelectedRawSubCriteriaClause === null ||
                           (parsedSelectedRawSubCriteriaClause && isEmpty(parsedSelectedRawSubCriteriaClause.join('')))
                       )
                   );
        }
    }
</script>

<style>
    .criteria-editor-card-text {
        height: 700px;
        overflow: hidden !important;
    }

    .invalid-message {
        color: red;
    }

    .valid-message {
        color: green;
    }

    .clauses-card, .criteria-editor-container {
        overflow-y: auto;
        overflow-x: hidden;
    }

    .clauses-card {
        height: 445px;
    }

    .criteria-editor-container {
        height: 513px;
    }

    .clause-textarea {
        font-size: 12px !important;
    }

    .clause-textarea .v-input__slot {
        background-color: transparent !important;
    }

    .clause-textarea textarea {
        border: 1px solid;
    }

    .sub-text {
        height: 35px;
    }

    .conjunction-and-messages-container, .sub-criteria-messages-container {
        padding-left: 20px;
    }

    .conjunction-select-list {
        width: 100px;
    }

    .textarea-focused textarea {
        background: lightblue;
    }
</style>