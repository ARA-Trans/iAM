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
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit">
                            Apply
                        </v-btn>
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
    import {hasValue} from '../utils/has-value';
    import {CriteriaEditorDialogData} from '../models/dialogs/criteria-editor-dialog/criteria-editor-dialog-data';
    import {isEmpty} from 'ramda';

    @Component({
        components: {VueQueryBuilder}
    })
    export default class CriteriaEditor extends Vue {
        @Prop() dialogData: CriteriaEditorDialogData;

        @State(state => state.attribute.attributes) attributes: string[];

        @Action('setIsBusy') setIsBusyAction: any;
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

        @Watch('dialogData')
        onDialogDataChanged() {
            // set the criteria string
            this.criteria = parseCriteriaString(this.dialogData.criteria);
            if (this.dialogData.showDialog && isEmpty(this.attributes)) {
                // set isBusy to true, then dispatch action to get attributes
                this.setIsBusyAction({isBusy: true});
                this.getAttributesAction()
                    .then(() => this.setIsBusyAction({isBusy: false}))
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
        }

        /**
         * Watcher: attributes
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
         * Criteria in dialog has changed
         */
        @Watch('criteria')
        onCriteriaChanged() {
            this.setCurrentCriteriaOutput();
        }

        /**
         * Sets the currentCriteriaOutput based on if the current criteria is valid or not
         */
        setCurrentCriteriaOutput() {
            if (this.isNotValidCriteria()) {
                this.currentCriteriaOutput = 'Could Not Parse Current Criteria';
            } else {
                this.currentCriteriaOutput = parseQueryBuilderJson(this.criteria).join('');
            }
        }

        /**
         * Whether or not the current criteria is valid
         */
        isNotValidCriteria() {
            return !hasValue(parseQueryBuilderJson(this.criteria).join(''));
        }

        /**
         * 'Apply' button was clicked
         */
        onSubmit() {
            // emit dialog result
            this.$emit('submit', parseQueryBuilderJson(this.criteria).join(''));
        }

        /**
         * 'Cancel' button was clicked
         */
        onCancel() {
            // emit null result
            this.$emit('submit', null);
        }
    }
</script>

<style>
    .query-builder-card-text {
        height: 700px;
    }
</style>