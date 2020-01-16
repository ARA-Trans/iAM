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
                                <v-card-text class="clauses-container">
                                    <div class="sub-text">
                                        <v-layout justify-start>
                                            <p class="invalid-message" v-if="showInvalidMessage">{{invalidMessage}}</p>
                                            <p class="valid-message" v-if="showValidMessage">{{validMessage}}</p>
                                        </v-layout>
                                    </div>
                                    <div v-for="(clause, index) in clauses">
                                        <v-layout column>
                                            <v-textarea  class="clause-textarea" rows="3" no-resize outline
                                                         readonly full-width :value="clause" @click="onClickClauseTextarea(clause)">
                                            </v-textarea>
                                            <v-layout justify-center v-if="clauses.length > 1 && index !== clauses.length - 1">
                                                <p>{{clauseConjunction}}</p>
                                            </v-layout>
                                        </v-layout>
                                    </div>
                                </v-card-text>
                            </v-card>
                        </v-flex>
                        <v-flex xs7>
                            <v-card>
                                <v-card-title>
                                    <v-layout justify-center><h3>Criteria Editor</h3></v-layout>
                                </v-card-title>
                                <v-card-text class="criteria-editor-container">
                                    <div class="sub-text"></div>
                                    <vue-query-builder v-if="editorRules.length > 0" id="vqb" :labels="queryBuilderLabels"
                                                       :rules="editorRules" :maxDepth="25" :styled="true" v-model="criteria">
                                    </vue-query-builder>
                                </v-card-text>
                            </v-card>
                        </v-flex>
                    </v-layout>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-flex xs2>
                        <v-layout justify-space-around row>
                            <v-btn class="ara-blue-bg white--text" @click="onCheckCriteria">Check</v-btn>
                            <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)" :disabled="cannotSubmit">
                                Save
                            </v-btn>
                        </v-layout>
                    </v-flex>

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
    import {isEmpty, clone} from 'ramda';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import CriteriaEditorService from '@/services/criteria-editor.service';
    import {AxiosResponse} from 'axios';
    import {CriteriaValidation} from '@/shared/models/iAM/criteria-validation';
    import {Mutation} from 'vuex';

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
        currentCriteriaOutput = '';
        showInvalidMessage: boolean = false;
        showValidMessage: boolean = false;
        cannotSubmit: boolean = false;
        validMessage: string = '';
        invalidMessage: string = '';
        clauseConjunction: string = 'OR';
        clauses: string[] = [];
        vqb: HTMLElement;
        mutationObserver: MutationObserver;

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateAttributes)) {
                this.setEditorRules();
            }
            this.mutationObserver = new MutationObserver((mutations) => {
                mutations.forEach((mutation) => {
                    console.log(`mutation.type = ${mutation.type}`);
                    mutation.addedNodes.forEach((addedNode) => {
                        console.log(`added node = ${addedNode}`);
                    })
                })
            });
        }

        updated() {
            if (!hasValue(this.vqb)) {
                this.vqb = document.getElementById('vqb') as HTMLElement;
                this.mutationObserver.observe(this.vqb, {childList: true});
            }
        }

        /**
         * Sets the criteria object with the dialogData object's parsed criteria string, then dispatches an action to
         * get the attributes from the server if the attributes array is empty
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.criteria = parseCriteriaString(this.dialogData.criteria);
            if (isEmpty(this.criteria.logicalOperator)) {
                this.criteria.logicalOperator = 'AND';
            }
        }

        /**
         * Calls the setEditorRules function if a change to stateAttributes causes it to have a value
         */
        @Watch('stateAttributes')
        onStateAttributesChanged() {
            if (hasValue(this.stateAttributes)) {
                this.setEditorRules();
            }
        }

        /**
         * Sets editorRules property using the attributes from state
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
         * Calls the setCurrentCriteriaOutput function when the criteria object has been modified
         */
        @Watch('criteria')
        onCriteriaChanged() {
            this.setCurrentCriteriaOutput();
            // reset criteria validation properties
            this.showInvalidMessage = false;
            this.showValidMessage = false;
            this.cannotSubmit = !isEmpty(parseCriteriaJson(this.criteria).join('')) ||
                                this.dialogData.criteria === parseCriteriaJson(this.criteria).join('');
        }

        /**
         * Sets the currentCriteriaOutput string based on if the current criteria is valid or not
         */
        setCurrentCriteriaOutput() {
            this.clauses = [];
            if (hasValue(this.criteria) && hasValue(this.criteria.children)) {
                this.clauseConjunction = hasValue(this.criteria.logicalOperator) ? this.criteria.logicalOperator : 'OR';
                this.criteria.children!.forEach((criteriaType: CriteriaType) => {
                    const clause: string = parseCriteriaTypeJson(criteriaType);
                    if (hasValue(clause)) {
                        this.clauses.push(clause);
                    }
                });
            }
            /*if (this.isNotValidCriteria()) {
                this.currentCriteriaOutput = 'Could Not Parse Current Criteria';
            } else {
                this.currentCriteriaOutput = parseCriteriaJson(this.criteria).join('');
            }*/
        }

        /**
         * Determines if the current criteria data is valid or not
         */
        /*isNotValidCriteria() {
            return !hasValue(parseCriteriaJson(this.criteria).join(''));
        }*/

        onClickClauseTextarea(clause: string) {
            const vqbGroupBody = null;
        }

        onCheckCriteria() {
            const criteriaValidation: CriteriaValidation = {
                criteria: parseCriteriaJson(this.criteria).join('')
            };

            CriteriaEditorService.checkCriteriaValidity(criteriaValidation)
                .then((response: AxiosResponse<string>) => {
                    if (response.data.indexOf('results match query') !== -1) {
                        this.validMessage = response.data;
                        // if result is true then set showValidMessage = true, cannotSubmit = false, & showInvalidMessage = false
                        this.showValidMessage = true;
                        this.cannotSubmit = false;
                        this.showInvalidMessage = false;
                    } else {
                        this.invalidMessage = response.data;
                        // if result is false then set showInvalidMessage = true, cannotSubmit = true, & showValidMessage = false
                        this.invalidMessage = response.data;
                        this.showInvalidMessage = true;
                        this.cannotSubmit = true;
                        this.showValidMessage = false;
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
            this.showInvalidMessage = false;
            this.showValidMessage = false;
            this.cannotSubmit = false;
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

    .clauses-container, .criteria-editor-container {
        height: 600px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .clause-textarea {
        font-size: 12px !important;
    }

    .sub-text {
        height: 35px;
    }
</style>