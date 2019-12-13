<template>
    <v-dialog v-model="dialogData.showDialog" persistent scrollable max-width="700px">
        <v-card>
            <v-card-title>
                <v-layout column>
                    <v-flex>
                        <v-layout justify-center>
                            <h3>Criteria Editor</h3>
                        </v-layout>
                    </v-flex>

                    <v-flex>
                        Current Criteria Output
                        <v-textarea rows="5" no-resize outline readonly full-width :value="currentCriteriaOutput">
                        </v-textarea>
                        <div class="validation-message-div">
                            <v-layout justify-end>
                                <p class="invalid-message" v-if="showInvalidMessage">{{invalidMessage}}</p>
                                <p class="valid-message" v-if="showValidMessage">{{validMessage}}</p>
                            </v-layout>
                        </div>
                    </v-flex>
                </v-layout>
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text class="query-builder-card-text">
                <div v-if="editorRules.length > 0">
                    <vue-query-builder :labels="queryBuilderLabels" :rules="editorRules" :maxDepth="25" :styled="true"
                                       v-model="criteria">
                    </vue-query-builder>
                </div>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-flex xs2>
                        <v-layout>
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
    import {Criteria, emptyCriteria} from '../models/iAM/criteria';
    import {parseCriteriaString, parseQueryBuilderJson} from '../utils/criteria-editor-parsers';
    import {hasValue} from '../utils/has-value-util';
    import {CriteriaEditorDialogData} from '../models/modals/criteria-editor-dialog-data';
    import {isEmpty} from 'ramda';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import CriteriaEditorService from '@/services/criteria-editor.service';
    import {AxiosResponse} from 'axios';
    import {CriteriaValidation} from '@/shared/models/iAM/criteria-validation';

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

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateAttributes)) {
                this.setEditorRules();
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
            this.cannotSubmit = !isEmpty(parseQueryBuilderJson(this.criteria).join('')) ||
                                this.dialogData.criteria === parseQueryBuilderJson(this.criteria).join('');
        }

        /**
         * Sets the currentCriteriaOutput string based on if the current criteria is valid or not
         */
        setCurrentCriteriaOutput() {
            if (this.isNotValidCriteria()) {
                this.currentCriteriaOutput = 'Could Not Parse Current Criteria';
            } else {
                this.currentCriteriaOutput = parseQueryBuilderJson(this.criteria).join('');
            }
        }

        /**
         * Determines if the current criteria data is valid or not
         */
        isNotValidCriteria() {
            return !hasValue(parseQueryBuilderJson(this.criteria).join(''));
        }

        onCheckCriteria() {
            const criteriaValidation: CriteriaValidation = {
                criteria: parseQueryBuilderJson(this.criteria).join('')
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
                this.$emit('submit', parseQueryBuilderJson(this.criteria).join(''));
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
    .query-builder-card-text {
        height: 700px;
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
</style>