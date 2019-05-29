<template>
    <v-layout row justify-center>
        <v-dialog v-model="dialogData.showDialog" persistent scrollable max-width="700px">
            <v-card>
                <v-card-title>
                    <v-layout column fill-height>
                        <v-flex>
                            <v-layout justify-center fill-height>
                                <h3>Criteria Editor</h3>
                            </v-layout>
                        </v-flex>

                        <v-flex>
                            Current Criteria Output
                            <v-textarea rows="5" no-resize outline readonly full-width :value="currentCriteriaOutput">
                            </v-textarea>
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
                    <v-layout justify-space-between row fill-height>
                        <v-btn color="info" v-on:click="onSubmit(true)">
                            Apply
                        </v-btn>
                        <v-btn color="error" v-on:click="onSubmit(false)">Cancel</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
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

    @Component({
        components: {VueQueryBuilder}
    })
    export default class CriteriaEditorDialog extends Vue {
        @Prop() dialogData: CriteriaEditorDialogData;

        @State(state => state.attribute.attributes) attributes: string[];

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

            if (this.dialogData.showDialog && isEmpty(this.attributes)) {
                this.getAttributesAction();
            }
        }

        /**
         * Sets the editorRules array using the attributes array data
         */
        @Watch('attributes')
        onAttributesChanged() {
            if (!isEmpty(this.attributes)) {
                // set the rules property using the list of attributes
                this.editorRules = this.attributes.map((attribute: string) => ({
                    type: 'text',
                    label: attribute,
                    id: attribute,
                    operators: ['=', '<>', '<', '<=', '>', '>=']
                }));
            }
        }

        /**
         * Calls the setCurrentCriteriaOutput function when the criteria object has been modified
         */
        @Watch('criteria')
        onCriteriaChanged() {
            this.setCurrentCriteriaOutput();
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

        /**
         * Emits the parsed criteria object's data to the calling parent component, or null if the user clicked the
         * 'Cancel' button
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', parseQueryBuilderJson(this.criteria).join(''));
            } else {
                this.$emit('submit', null);
            }

        }
    }
</script>

<style>
    .query-builder-card-text {
        height: 700px;
    }
</style>