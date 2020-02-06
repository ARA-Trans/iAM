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
                                    <div v-for="(clause, index) in clauses">
                                        <v-layout column>
                                            <v-textarea class="clause-textarea" rows="3" no-resize box :class="{'textarea-focused': index === selectedClauseIndex}"
                                                         readonly full-width :value="clause" @click="onClickClauseTextarea(clause, index)">
                                                <template slot="append">
                                                    <v-btn class="ara-orange" icon @click="onRemoveSubCriteria(index)">
                                                        <v-icon>fas fa-times</v-icon>
                                                    </v-btn>
                                                </template>
                                            </v-textarea>
                                            <v-layout justify-center v-if="clauses.length > 1 && index !== clauses.length - 1">
                                                <p>{{selectedConjunction}}</p>
                                            </v-layout>
                                        </v-layout>
                                    </div>
                                </v-card-text>
                                <v-card-actions>
                                    <v-btn class="ara-blue-bg white--text" @click="onCheckCriteria">Check</v-btn>
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
                                    <v-tabs v-if="selectedClause !== null" centered>
                                        <v-tab ripple @click="onParseRawCriteriaStringToCriteriaJson">
                                            Tree View
                                        </v-tab>
                                        <v-tab ripple @click="onParseCriteriaJsonToRawCriteriaString">
                                            Raw Criteria
                                        </v-tab>
                                        <v-tab-item>
                                            <vue-query-builder v-if="editorRules.length > 0" :labels="queryBuilderLabels"
                                                               :rules="editorRules" :maxDepth="25" :styled="true" v-model="selectedClause">
                                            </vue-query-builder>
                                        </v-tab-item>
                                        <v-tab-item>
                                            <v-textarea rows="20" no-resize outline v-model="selectedClauseRaw"></v-textarea>
                                        </v-tab-item>
                                    </v-tabs>
                                </v-card-text>
                                <v-card-actions>
                                    <v-btn class="ara-blue-bg white--text" @click="onCheckSubCriteria">Check</v-btn>
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
    import {isEmpty, clone, update, remove, findIndex} from 'ramda';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import CriteriaEditorService from '@/services/criteria-editor.service';
    import {AxiosResponse} from 'axios';
    import {CriteriaValidation} from '@/shared/models/iAM/criteria-validation';
    import {Mutation} from 'vuex';
    import {SelectItem} from '@/shared/models/vue/select-item';

    @Component({
        components: {VueQueryBuilder}
    })
    export default class CriteriaEditorDialog extends Vue {
        @Prop() dialogData: CriteriaEditorDialogData;

        @State(state => state.attribute.attributes) stateAttributes: Attribute[];

        @Action('getAttributes') getAttributesAction: any;

        criteria: Criteria = {...emptyCriteria};
        editorRules: any[] = [];
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
        clauses: string[] = [];
        selectedClause: Criteria | null = null;
        selectedClauseRaw: string = '';
        selectedClauseIndex: number = -1;
        activeTab = 'tree-view';

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateAttributes)) {
                this.setEditorRules();
            }
        }

        /**
         * Setter for the criteria property using the dialogData.criteria property
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.criteria = parseCriteriaString(this.dialogData.criteria) as Criteria;
            if (this.criteria && isEmpty(this.criteria.logicalOperator)) {
                this.criteria.logicalOperator = 'OR';
            }
        }

        /**
         * Calls the editorRules property setter if stateAttributes has a value
         */
        @Watch('stateAttributes')
        onStateAttributesChanged() {
            if (hasValue(this.stateAttributes)) {
                this.setEditorRules();
            }
        }

        /**
         * Setter for the editorRules property using the attributes from state
         */
        setEditorRules() {
            this.editorRules = this.stateAttributes.map((attribute: Attribute) => ({
                type: 'text',
                label: attribute.name,
                id: attribute.name,
                operators: ['=', '<>', '<', '<=', '>', '>=']
            }));
        }

        /**
         * Calls the setter for the clauses property and sets related component UI properties
         */
        @Watch('criteria')
        onCriteriaChanged() {
            this.setSubCriteria();
            // reset criteria validation properties
            this.validCriteriaMessage = null;
            this.invalidCriteriaMessage = null;
            this.cannotSubmit = true;
        }

        /**
         * Setter for the sub-criteria messages triggered on a selectedClause change
         */
        @Watch('selectedClause')
        onSelectedClauseChanged() {
            this.validSubCriteriaMessage = null;
            this.invalidSubCriteriaMessage = null;
        }

        /**
         * Setter for the sub-criteria messages triggered on a selectedClauseRaw change
         */
        @Watch('selectedClauseRaw')
        onSelectedClauseRawChanged() {
            this.validSubCriteriaMessage = null;
            this.invalidSubCriteriaMessage = null;
        }

        /**
         * Setter for the clauses property & selectedConjunction property
         */
        setSubCriteria() {
            this.clauses = [];
            if (hasValue(this.criteria) && hasValue(this.criteria.children)) {
                this.selectedConjunction = hasValue(this.criteria.logicalOperator) ? this.criteria.logicalOperator : 'OR';
                if (hasValue(this.criteria.children)) {
                    this.criteria.children!.forEach((criteriaType: CriteriaType) => {
                        const clause: string = parseCriteriaTypeJson(criteriaType);
                        if (hasValue(clause)) {
                            this.clauses.push(clause);
                        }
                    });
                }
            }
        }

        /**
         * Adds a new clause to the clauses property
         */
        onAddSubCriteria() {
            this.clauses.push('');
            this.selectedClauseIndex = this.clauses.length - 1;
            this.selectedClause = clone(emptyCriteria);
        }

        /**
         * Sets the selectedClause & selectedClauseIndex property with the specified clause & clauseIndex parameters;
         * sets an invalid sub-criteria message if the clause cannot be parsed
         */
        onClickClauseTextarea(clause: string, clauseIndex: number) {
            this.invalidSubCriteriaMessage = null;
            this.selectedClauseIndex = clauseIndex;
            this.selectedClause = parseCriteriaString(clause);
            if (this.selectedClause) {
                if (!hasValue(this.selectedClause.logicalOperator)) {
                    this.selectedClause.logicalOperator = 'OR';
                }
            } else {
                this.invalidSubCriteriaMessage = 'Unable to parse selected criteria';
            }
        }

        /**
         * Removes a sub-criteria from the clauses list
         */
        onRemoveSubCriteria(clauseIndex: number) {
            this.clauses = remove(clauseIndex, 1, this.clauses);

            if (this.selectedClauseIndex === clauseIndex) {
                this.selectedClause = null;
                this.selectedClauseIndex = -1;
            } else {
                this.selectedClauseIndex = findIndex((clause: string) => {
                    const parsedClause = this.activeTab === 'tree-view'
                        ? parseCriteriaJson(this.selectedClause as Criteria).join('')
                        : this.selectedClauseRaw;
                    return clause === parsedClause;
                }, this.clauses);
            }
        }

        /**
         * Parses the raw criteria for the tree view if valid; otherwise sets an invalid sub-criteria message
         */
        onParseRawCriteriaStringToCriteriaJson() {
            this.activeTab = 'tree-view';
            this.invalidSubCriteriaMessage = null;
            const parsedRawCriteriaString = parseCriteriaString(this.selectedClauseRaw);
            if (parsedRawCriteriaString) {
                this.selectedClause = parsedRawCriteriaString;
                if (!hasValue(this.selectedClause.logicalOperator)) {
                    this.selectedClause.logicalOperator = 'OR';
                }
            } else {
                this.invalidSubCriteriaMessage = 'The raw criteria string is invalid';
            }
        }

        /**
         * Parses the tree view criteria json for the raw criteria view
         */
        onParseCriteriaJsonToRawCriteriaString() {
            this.activeTab = 'raw-criteria';
            this.invalidSubCriteriaMessage = null;
            const parsedCriteriaJson = parseCriteriaJson(this.selectedClause as Criteria);
            if (parsedCriteriaJson) {
                this.selectedClauseRaw = parsedCriteriaJson.join('');
            } else {
                this.invalidSubCriteriaMessage = 'The criteria json is invalid';
            }
        }

        /**
         * Checks criteria editor validity and if valid modifies the main criteria with the editor changes; otherwise
         * sets an invalid criteria message
         */
        onCheckCriteria() {
            this.selectedClause = null;
            this.selectedClauseIndex = -1;

            const rebuiltCriteria: Criteria = {
                logicalOperator: this.selectedConjunction,
                children: this.clauses.map((clause: string) => {
                    const parsedClause: Criteria = parseCriteriaString(clause) as Criteria;
                    if (parsedClause.children!.length === 1) {
                        return parsedClause.children![0];
                    }
                    return {
                        type: 'query-builder-group',
                        query: {
                            logicalOperator: parsedClause.logicalOperator,
                            children: parsedClause.children
                        }
                    };
                })
            };

            const criteriaValidation: CriteriaValidation = {
                criteria: parseCriteriaJson(rebuiltCriteria).join('')
            };

            CriteriaEditorService.checkCriteriaValidity(criteriaValidation)
                .then((response: AxiosResponse<string>) => {
                    if (response.data.indexOf('results match query') !== -1) {
                        this.criteria = clone(rebuiltCriteria);
                        setTimeout(() => {
                            this.validCriteriaMessage = response.data;
                            this.invalidCriteriaMessage = null;
                            this.cannotSubmit = false;
                        });
                    } else {
                        this.invalidCriteriaMessage = response.data;
                        this.validCriteriaMessage = null;
                        this.cannotSubmit = true;
                    }
                });
        }

        /**
         * Checks the currently selected sub-criteria validity and if valid updates the clauses list at the index of the
         * selected sub-criteria; otherwise sets an invalid sub-criteria message
         */
        onCheckSubCriteria() {
            const criteria = this.activeTab === 'tree-view'
                ? parseCriteriaJson(this.selectedClause as Criteria).join('')
                : this.selectedClauseRaw;

            const criteriaValidation: CriteriaValidation = {
                criteria: criteria
            };

            CriteriaEditorService.checkCriteriaValidity(criteriaValidation)
                .then((response: AxiosResponse<string>) => {
                    if (response.data.indexOf('results match query') !== -1) {
                        this.validSubCriteriaMessage = response.data;
                        this.invalidSubCriteriaMessage = null;
                        this.clauses = update(
                            this.selectedClauseIndex,
                            criteria,
                            this.clauses
                        );
                    } else {
                        this.invalidSubCriteriaMessage = response.data;
                        this.validSubCriteriaMessage = null;
                    }
                });
        }

        /**
         * Emits the parsed criteria object's data to the calling parent component, or null if the user clicked the
         * 'Cancel' button
         */
        onSubmit(submit: boolean) {
            // reset component's calculated properties
            this.resetComponentCalculatedProperties();

            if (submit) {
                this.$emit('submit', parseCriteriaJson(this.criteria).join(''));
            } else {
                this.$emit('submit', null);
            }
        }

        /**
         * Resets component's calculated properties
         */
        resetComponentCalculatedProperties() {
            this.selectedConjunction = 'OR';
            this.selectedClause = null;
            this.selectedClauseIndex = -1;
            this.validCriteriaMessage = null;
            this.invalidCriteriaMessage = null;
            this.validSubCriteriaMessage = null;
            this.invalidSubCriteriaMessage = null;
            this.cannotSubmit = true;
        }
    }
</script>

<style>
    .criteria-editor-card-text {
        height: 700px;
        overflow: hidden !important;
    }

    .validation-message-div {
        height: 21px;
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