<template>
    <v-layout row justify-center>
        <v-dialog v-model="showCriteriaEditor" persistent scrollable max-width="700px">
            <v-card>
                <v-card-title>
                    <v-layout column>
                        <v-flex>
                            <h3>Criteria Editor</h3>
                        </v-flex>

                        <v-flex>
                            Current Criteria Output
                            <v-textarea class="criteria-output-textarea"
                                        no-resize
                                        outline
                                        readonly
                                        full-width
                                        :value="currentCriteriaOutput">
                            </v-textarea>
                        </v-flex>
                    </v-layout>
                </v-card-title>
                <v-divider></v-divider>
                <v-card-text class="query-builder-card-text">
                    <vue-query-builder :labels="labels" :rules="rules" :maxDepth="25" :styled="true"
                                       v-model="criteria">
                    </vue-query-builder>
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions>
                    <v-btn color="blue darken-1" v-on:click="onSubmit(true)" v-bind:disabled="isNotValidCriteria()">
                        Apply
                    </v-btn>
                    <v-btn color="red darken-1" v-on:click="onSubmit(false)">Cancel</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import VueQueryBuilder from 'vue-query-builder/src/VueQueryBuilder.vue';
    import {Criteria, CriteriaEditorAttribute, emptyCriteria} from '@/shared/models/iAM/criteria';
    import {parseQueryBuilderJson} from '@/shared/utils/criteria-editor-parsers';
    import {hasValue} from '@/shared/utils/has-value';

    @Component({
        components: {VueQueryBuilder}
    })
    export default class CriteriaEditor extends Vue {
        @Prop() showCriteriaEditor: boolean;

        @State(state => state.criteriaEditor.criteriaEditorAttributes) criteriaEditorAttributes: CriteriaEditorAttribute[];
        @State(state => state.criteriaEditor.criteria) stateCriteria: Criteria;

        @Action('getCriteriaEditorAttributes') setCriteriaEditorAttributesAction: any;

        criteria: Criteria = emptyCriteria;
        rules: any[] = [];
        labels: object = {
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

        @Watch('criteriaEditorAttributes')
        onCriteriaEditorAttributesChanged(val: CriteriaEditorAttribute[]) {
            this.rules = val.map((cea: CriteriaEditorAttribute) => ({
                // TODO: implement select when we have web service that returns predetermined values
                /*type: 'select',
                label: ca.name,
                id: ca.name,
                operators: ['=', '<>', '<', '<=', '>', '>='],
                choices: ca.values.map((val: string) => ({
                    label: val,
                    value: val
                }))*/
                type: 'text',
                label: cea.name,
                id: cea.name,
                operators: ['=', '<>', '<', '<=', '>', '>=']
            }));
        }

        @Watch('stateCriteria')
        onStateCriteriaChanged(val: Criteria) {
            this.criteria = val;
            this.setCurrentCriteriaOutput();
        }

        @Watch('criteria')
        onCriteriaChanged() {
            this.setCurrentCriteriaOutput();
        }

        mounted() {
            this.setCriteriaEditorAttributesAction();
        }

        setCurrentCriteriaOutput() {
            if (this.isNotValidCriteria()) {
                this.currentCriteriaOutput = 'Could Not Parse Current Criteria';
            } else {
                this.currentCriteriaOutput = parseQueryBuilderJson(this.criteria).join('');
            }
        }

        isNotValidCriteria() {
            return !hasValue(parseQueryBuilderJson(this.criteria).join(''));
        }

        onSubmit(notCanceled: boolean) {
            if (notCanceled) {
                this.$emit('applyCriteria', parseQueryBuilderJson(this.criteria).join(''));
            } else {
                this.$emit('applyCriteria', '');
            }
        }
    }
</script>

<style>
    .criteria-output-textarea {
        height: 100px;
    }

    .query-builder-card-text {
        height: 700px;
    }
</style>