<template>
    <v-layout justify-center column class="criteria-editor-card-text">
        <div>
            <v-layout justify-space-between row>
                <v-flex xs5>
                    <v-card>
                        <v-card-title>
                            <v-layout justify-center><h3>Output</h3></v-layout>
                        </v-card-title>
                        <div class="conjunction-and-messages-container">
                            <v-layout
                                    :class="{'justify-space-between': !criteriaEditorData.isLibraryContext, 'justify-space-around': criteriaEditorData.isLibraryContext}">
                                <div class="conjunction-select-list-container">
                                    <v-select :items="conjunctionSelectListItems" class="conjunction-select-list"
                                              lablel="Select a Conjunction" v-model="selectedConjunction">
                                    </v-select>
                                </div>
                                <v-btn @click="onAddSubCriteria" class="ara-blue-bg white--text">Add Criteria
                                </v-btn>
                            </v-layout>
                        </div>
                        <v-card-text
                                :class="{'clauses-card-dialog': !criteriaEditorData.isLibraryContext, 'clauses-card-library': criteriaEditorData.isLibraryContext}">
                            <div v-for="(clause, index) in subCriteriaClauses">
                                <v-textarea :class="{'textarea-focused': index === selectedSubCriteriaClauseIndex}"
                                            :value="clause" @click="onClickSubCriteriaClauseTextarea(clause, index)"
                                            box
                                            class="clause-textarea"
                                            full-width no-resize readonly
                                            rows="3">
                                    <template slot="append">
                                        <v-btn @click="onRemoveSubCriteria(index)" class="ara-orange" icon>
                                            <v-icon>fas fa-times</v-icon>
                                        </v-btn>
                                    </template>
                                </v-textarea>
                            </div>
                        </v-card-text>
                        <v-card-actions :class="{'validation-actions': criteriaEditorData.isLibraryContext}">
                            <v-layout row>
                                <div class="validation-check-btn-container">
                                    <v-btn :disabled="onDisableCriteriaCheck()" @click="onCheckCriteria"
                                           class="ara-blue-bg white--text">
                                        Check Output
                                    </v-btn>
                                </div>
                                <div class="validation-messages-container">
                                    <p class="invalid-message" v-if="invalidCriteriaMessage !== null">
                                        <strong>{{invalidCriteriaMessage}}</strong>
                                    </p>
                                    <p class="valid-message" v-if="validCriteriaMessage !== null">
                                        {{validCriteriaMessage}}
                                    </p>
                                    <p v-if="checkOutput">Please click here to check entire rule</p>
                                </div>
                            </v-layout>
                        </v-card-actions>
                    </v-card>
                </v-flex>
                <v-flex xs7>
                    <v-card>
                        <v-card-title>
                            <v-layout justify-center><h3>Criteria Editor</h3></v-layout>
                        </v-card-title>
                        <v-card-text
                                :class="{'criteria-editor-card-dialog': !criteriaEditorData.isLibraryContext, 'criteria-editor-card-library': criteriaEditorData.isLibraryContext}">
                            <v-tabs centered v-if="selectedSubCriteriaClauseIndex !== -1">
                                <v-tab @click="onParseRawSubCriteria" ripple>
                                    Tree View
                                </v-tab>
                                <v-tab @click="onParseSubCriteriaJson" ripple>
                                    Raw Criteria
                                </v-tab>
                                <v-tab-item>
                                    <vue-query-builder :labels="queryBuilderLabels"
                                                       :maxDepth="25"
                                                       :rules="queryBuilderRules" :styled="true"
                                                       v-if="queryBuilderRules.length > 0"
                                                       v-model="selectedSubCriteriaClause">
                                    </vue-query-builder>
                                </v-tab-item>
                                <v-tab-item>
                                    <v-textarea no-resize outline rows="23"
                                                v-model="selectedRawSubCriteriaClause"></v-textarea>
                                </v-tab-item>
                            </v-tabs>
                        </v-card-text>
                        <v-card-actions :class="{'validation-actions': criteriaEditorData.isLibraryContext}">
                            <v-layout row>
                                <div class="validation-check-btn-container">
                                    <v-btn :disabled="onDisableSubCriteriaCheck()" @click="onCheckSubCriteria"
                                           class="ara-blue-bg white--text">
                                        Check Criteria
                                    </v-btn>
                                </div>
                                <div class="validation-messages-container">
                                    <p class="invalid-message" v-if="invalidSubCriteriaMessage !== null">
                                        <strong>{{invalidSubCriteriaMessage}}</strong>
                                    </p>
                                    <p class="valid-message" v-if="validSubCriteriaMessage !== null">
                                        {{validSubCriteriaMessage}}
                                    </p>
                                </div>
                            </v-layout>
                        </v-card-actions>
                    </v-card>
                </v-flex>
            </v-layout>
        </div>
        <div class="main-criteria-check-output-container">
            <v-flex v-show="!criteriaEditorData.isLibraryContext" class="save-cancel-flex">
                <v-layout justify-center row wrap>
                    <v-btn :disabled="cannotSubmit" @click="onSubmitCriteriaEditorResult(true)"
                           class="ara-blue-bg white--text">
                        Save
                    </v-btn>
                    <v-btn @click="onSubmitCriteriaEditorResult(false)" class="ara-orange-bg white--text">Cancel</v-btn>
                </v-layout>
            </v-flex>
        </div>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import VueQueryBuilder from 'vue-query-builder/src/VueQueryBuilder.vue';
    import {
        Criteria,
        CriteriaEditorData, CriteriaRule,
        CriteriaType,
        CriteriaValidationResult,
        emptyCriteria
    } from '../models/iAM/criteria';
    import {parseCriteriaJson, parseCriteriaString, parseCriteriaTypeJson} from '../utils/criteria-editor-parsers';
    import {hasValue} from '../utils/has-value-util';
    import {clone, equals, findIndex, isEmpty, isNil, remove, update, propEq, append, any} from 'ramda';
    import {Attribute, AttributeSelectValues} from '@/shared/models/iAM/attribute';
    import CriteriaEditorService from '@/services/criteria-editor.service';
    import {AxiosResponse} from 'axios';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {Network} from '@/shared/models/iAM/network';

    @Component({
        components: {VueQueryBuilder}
    })
    export default class CriteriaEditor extends Vue {
        @Prop() criteriaEditorData: CriteriaEditorData;

        @State(state => state.attribute.attributes) stateAttributes: Attribute[];
        @State(state => state.attribute.attributesSelectValues) stateAttributesSelectValues: AttributeSelectValues[];
        @State(state => state.network.networks) stateNetworks: Network[];

        @Action('getAttributes') getAttributesAction: any;
        @Action('getAttributeSelectValues') getAttributeSelectValuesAction: any;

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
        checkOutput: boolean = false;

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateAttributes)) {
                this.setQueryBuilderRules();
            }
        }

        /**
         * Event handler: criteriaEditorData
         */
        @Watch('criteriaEditorData')
        onCriteriaEditorDataChanged() {
            const mainCriteria: Criteria = parseCriteriaString(this.criteriaEditorData.mainCriteriaString) as Criteria;

            const parsedSubCriteria: string[] | null = parseCriteriaJson(this.getMainCriteria());
            const mainCriteriaString: string | null = hasValue(parsedSubCriteria) ? parsedSubCriteria!.join('') : null;

            if (!equals(this.criteriaEditorData.mainCriteriaString, mainCriteriaString) && mainCriteria) {
                if (!hasValue(mainCriteria.logicalOperator)) {
                    mainCriteria.logicalOperator = 'OR';
                }

                this.selectedConjunction = mainCriteria.logicalOperator;

                this.setSubCriteriaClauses(mainCriteria);
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
         * Setter: selectedConjunction property
         */
        @Watch('subCriteriaClauses')
        onSubCriteriaClausesChanged() {
            this.resetComponentCriteriaUIProperties();
        }

        /**
         * Calls the resetSubCriteriaValidationMessageProperties func. on selectedSubCriteriaClause property change
         */
        @Watch('selectedSubCriteriaClause')
        onSelectedClauseChanged() {
            this.resetSubCriteriaValidationMessageProperties();

            if (hasValue(this.selectedSubCriteriaClause) && hasValue(this.selectedSubCriteriaClause!.children)) {
                let missingAttributes: string[] = [];

                for (let index = 0; index < this.selectedSubCriteriaClause!.children!.length; index++) {
                    missingAttributes = this.getMissingAttribute(this.selectedSubCriteriaClause!.children![index].query, missingAttributes);
                }

                if (hasValue(missingAttributes)) {
                    this.getAttributeSelectValuesAction({networkAttribute: {
                            networkId: this.stateNetworks[0].networkId,
                            attributes: missingAttributes
                        }
                    });
                }
            }
        }

        /**
         * Calls the resetSubCriteriaValidationMessageProperties func. on selectedRawSubCriteriaClause property change
         */
        @Watch('selectedRawSubCriteriaClause')
        onSelectedClauseRawChanged() {
            this.resetSubCriteriaValidationMessageProperties();
        }

        @Watch('stateAttributesSelectValues')
        onStateAttributesSelectValuesChanged() {
            if (hasValue(this.queryBuilderRules) && hasValue(this.stateAttributesSelectValues)) {
                const filteredAttributesSelectValues: AttributeSelectValues[] = this.stateAttributesSelectValues
                    .filter((asv: AttributeSelectValues) => hasValue(asv.values));
                if (hasValue(filteredAttributesSelectValues)) {
                    filteredAttributesSelectValues.forEach((asv: AttributeSelectValues) => {
                        this.queryBuilderRules = update(
                            findIndex(propEq('id', asv.attribute), this.queryBuilderRules),
                            {
                                type: 'select',
                                id: asv.attribute,
                                label: asv.attribute,
                                operators: ['=', '<>', '<', '<=', '>', '>='],
                                choices: asv.values.map((value: string) => ({label: value, value: value}))
                            },
                            this.queryBuilderRules
                        );
                    });
                }
            }
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
        setSubCriteriaClauses(mainCriteria: Criteria) {
            this.subCriteriaClauses = [];
            if (hasValue(mainCriteria) && hasValue(mainCriteria.children)) {
                mainCriteria.children!.forEach((criteriaType: CriteriaType) => {
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
            this.cannotSubmit = !isEmpty(parseCriteriaJson(this.getMainCriteria()));
        }

        /**
         * Resets component sub-criteria UI related properties to their default values
         */
        resetSubCriteriaValidationMessageProperties() {
            this.validSubCriteriaMessage = null;
            this.invalidSubCriteriaMessage = null;
            this.checkOutput = false;
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
                this.resetComponentCriteriaUIProperties();
            });
        }

        /**
         * Sets the selectedSubCriteriaClause & selectedSubCriteriaClauseIndex property with the specified clause & clauseIndex parameters;
         * sets an invalid sub-criteria message if the clause cannot be parsed
         */
        onClickSubCriteriaClauseTextarea(subCriteriaClause: string, subCriteriaClauseIndex: number) {
            this.resetSubCriteriaSelectedProperties();
            setTimeout(() => {
                this.selectedSubCriteriaClauseIndex = subCriteriaClauseIndex;
                this.selectedSubCriteriaClause = parseCriteriaString(subCriteriaClause);
                if (this.selectedSubCriteriaClause) {
                    if (!hasValue(this.selectedSubCriteriaClause.logicalOperator)) {
                        this.selectedSubCriteriaClause.logicalOperator = 'OR';
                    }
                } else {
                    this.invalidSubCriteriaMessage = 'Unable to parse selected criteria';
                }
            });
        }

        /**
         * Removes a sub-criteria from the subCriteriaClauses property
         */
        onRemoveSubCriteria(subCriteriaClauseIndex: number) {
            const subCriteriaClause: string = this.subCriteriaClauses[subCriteriaClauseIndex];

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

            this.resetComponentCriteriaUIProperties();


            if (this.criteriaEditorData.isLibraryContext) {
                if (!hasValue(this.subCriteriaClauses)) {
                    this.$emit('submitCriteriaEditorResult', {validated: true, criteria: ''});
                } else if (hasValue(subCriteriaClause)) {
                    this.$emit('submitCriteriaEditorResult', {validated: false, criteria: null});
                }
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
            this.checkOutput = false;
            this.resetSubCriteriaSelectedProperties();

            const parsedCriteria = parseCriteriaJson(this.getMainCriteria());

            if (parsedCriteria) {
                CriteriaEditorService.checkCriteriaValidity({criteria: parsedCriteria.join('')})
                    .then((response: AxiosResponse<CriteriaValidationResult>) => {
                        if (hasValue(response, 'data')) {
                            const validationResult: CriteriaValidationResult = response.data;
                            const message = `${validationResult.numberOfResults} result(s) returned`;
                            if (validationResult.isValid) {
                                this.validCriteriaMessage = message;
                                this.cannotSubmit = false;

                                if (this.criteriaEditorData.isLibraryContext) {
                                    const parsedCriteria = parseCriteriaJson(this.getMainCriteria());
                                    if (parsedCriteria) {
                                        this.$emit('submitCriteriaEditorResult', {
                                            validated: true,
                                            criteria: parsedCriteria.join('')
                                        });
                                    } else {
                                        this.invalidCriteriaMessage = 'Unable to parse the criteria';
                                    }
                                }
                            } else {
                                this.resetComponentCriteriaUIProperties();
                                setTimeout(() => {
                                    if (validationResult.numberOfResults === 0) {
                                        this.invalidCriteriaMessage = message;
                                        this.cannotSubmit = false;
                                    } else {
                                        this.invalidCriteriaMessage = validationResult.message;
                                    }
                                });

                                if (this.criteriaEditorData.isLibraryContext) {
                                    this.$emit('submitCriteriaEditorResult', {validated: false, criteria: null});
                                }
                            }
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
                .then((response: AxiosResponse<CriteriaValidationResult>) => {
                    this.resetSubCriteriaValidationMessageProperties();

                    if (hasValue(response, 'data')) {
                        const validationResult: CriteriaValidationResult = response.data;
                        const message = `${validationResult.numberOfResults} result(s) returned`;
                        if (validationResult.isValid) {
                            this.validSubCriteriaMessage = message;
                            this.subCriteriaClauses = update(
                                this.selectedSubCriteriaClauseIndex,
                                criteria,
                                this.subCriteriaClauses
                            );
                            this.resetComponentCriteriaUIProperties();
                            this.checkOutput = true;

                            if (this.criteriaEditorData.isLibraryContext) {
                                this.$emit('submitCriteriaEditorResult', {validated: false, criteria: null});
                            }
                        } else {
                            if (validationResult.numberOfResults === 0) {
                                this.invalidSubCriteriaMessage = message;
                                this.subCriteriaClauses = update(
                                    this.selectedSubCriteriaClauseIndex,
                                    criteria,
                                    this.subCriteriaClauses
                                );
                                this.resetComponentCriteriaUIProperties();
                                this.checkOutput = true;
                            } else {
                                this.invalidSubCriteriaMessage = validationResult.message;
                            }
                        }
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
        onSubmitCriteriaEditorResult(submit: boolean) {
            this.resetSubCriteriaSelectedProperties();
            this.resetComponentCriteriaUIProperties();

            if (submit) {
                const parsedCriteria = parseCriteriaJson(this.getMainCriteria());
                if (parsedCriteria) {
                    this.selectedConjunction = 'OR';
                    this.$emit('submitCriteriaEditorResult', parsedCriteria.join(''));
                } else {
                    this.invalidCriteriaMessage = 'Unable to parse the criteria';
                }
            } else {
                this.selectedConjunction = 'OR';
                this.$emit('submitCriteriaEditorResult', null);
            }
        }

        /**
         * Determines whether or not the main criteria 'Check' button should be disabled
         */
        onDisableCriteriaCheck() {
            const mainCriteria: Criteria = this.getMainCriteria();
            const subCriteriaClausesAreEmpty = this.subCriteriaClauses
                .every((subCriteriaClause: string) => isEmpty(subCriteriaClause));

            return !mainCriteria || (equals(mainCriteria, emptyCriteria) && subCriteriaClausesAreEmpty) ||
                (!hasValue(mainCriteria.children) && subCriteriaClausesAreEmpty) ||
                isEmpty(parseCriteriaJson(mainCriteria));
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

        /**
         * Returns the main criteria object parsed from the sub-criteria clauses
         */
        getMainCriteria() {
            const filteredSubCriteria: string[] = this.subCriteriaClauses
                .filter((subCriteriaClause: string) => !isEmpty(subCriteriaClause));

            if (hasValue(filteredSubCriteria)) {
                return {
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
            }

            return clone(emptyCriteria);
        }

        getMissingAttribute(query: any, missingAttributes: string[]) {
            if (query.hasOwnProperty('children')) {
                const criteria: Criteria = query as Criteria;
                if (hasValue(criteria.children)) {
                    criteria.children!.forEach((child: CriteriaType) => {
                        missingAttributes = this.getMissingAttribute(child.query, missingAttributes);
                    });
                }
            } else {
                const criteriaRule: CriteriaRule = query as CriteriaRule;
                if (!any(propEq('attribute', criteriaRule.rule), this.stateAttributesSelectValues) &&
                    missingAttributes.indexOf(criteriaRule.rule) === -1) {
                    missingAttributes.push(criteriaRule.rule);
                }
            }

            return missingAttributes;
        }
    }
</script>

<style>
    .invalid-message {
        color: red;
    }

    .valid-message {
        color: green;
    }

    .clauses-card-dialog {
        height: 500px;
        max-height: calc(100vh - 400px);
        overflow-y: auto;
    }

    .clauses-card-library {
        height: 537px;
        overflow-y: auto;
    }

    .criteria-editor-card-dialog {
        height: 568px;
        max-height: calc(100vh - 332px);
        overflow-y: auto;
    }

    .criteria-editor-card-library {
        height: 581px;
        overflow-y: auto;
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

    .conjunction-and-messages-container {
        padding-left: 20px;
    }

    .conjunction-select-list {
        width: 100px;
    }

    .textarea-focused textarea {
        background: lightblue;
    }

    .save-cancel-flex {
        margin-top: 20px;
    }

    .validation-actions {
        height: 75px;
    }

    .validation-check-btn-container {
        margin-left: 10px;
    }

    .validation-messages-container {
        margin-left: 5px;
    }
</style>
