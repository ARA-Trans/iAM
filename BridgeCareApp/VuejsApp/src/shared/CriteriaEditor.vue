<template>
    <v-layout row justify-center>
        <v-dialog v-model="showCriteriaEditor" persistent scrollable max-width="700px">
            <v-card>
                <v-card-title>Criteria Editor</v-card-title>
                <v-divider></v-divider>
                <v-card-text style="height: 700px;">
                    <vue-query-builder :labels="queryBuildLabels" :rules="rules" :maxDepth="25" :styled="true"
                                       v-model="criteria"></vue-query-builder>
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
    import {CriteriaEditorDialogResult} from '@/shared/models/dialogs/criteria-editor-dialog-result';

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
        queryBuildLabels: object = {
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

        /**
         * CriteriaEditorDialog attributes list has changed
         * @param criteriaEditorAttributes CriteriaEditorAttribute object list
         */
        @Watch('criteriaEditorAttributes')
        onCriteriaEditorAttributesChanged(criteriaEditorAttributes: CriteriaEditorAttribute[]) {
            this.rules = criteriaEditorAttributes.map((cea: CriteriaEditorAttribute) => ({
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

        /**
         * Criteria in state has changed
         * @criteria Criteria object
         */
        @Watch('stateCriteria')
        onStateCriteriaChanged(criteria: Criteria) {
            this.criteria = criteria;
        }

        /**
         * Component has been mounted
         */
        mounted() {
            this.setCriteriaEditorAttributesAction();
        }

        /**
         * Whether or not the current criteria is valid
         */
        isNotValidCriteria() {
            return !hasValue(parseQueryBuilderJson(this.criteria).join(''));
        }

        onSubmit(isCanceled: boolean) {
            // create dialog result
            const result: CriteriaEditorDialogResult = {
                canceled: isCanceled,
                criteria: parseQueryBuilderJson(this.criteria).join('')
            };
            // emit dialog result
            this.$emit('result', result);
        }
    }
</script>