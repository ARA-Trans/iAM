<template>
    <v-container fluid grid-list-xl>
        <v-layout row wrap>
            <v-flex xs12>
                <v-btn v-on:click="logIt">See it!</v-btn>
            </v-flex>
            <v-flex xs12>
                <vue-query-builder :labels="labels" :rules="rules" :maxDepth="25" :styled="true"
                                   v-model="criteria"></vue-query-builder>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from "vue";
    import {Component} from "vue-property-decorator";
    import VueQueryBuilder from "vue-query-builder/src/VueQueryBuilder.vue";
    import {Criteria, CriteriaAttribute, emptyCriteria} from "@/models/criteria";
    import {parseCriteriaString, parseQueryBuilderJson} from "@/shared/utils/criteria-editor-parsers";

    @Component({
        components: {VueQueryBuilder}
    })
    export default class CriteriaEditor extends Vue {
        criteriaAttributes: CriteriaAttribute[] = this.$store.getters.criteriaAttributes;
        criteria: Criteria = emptyCriteria;
        rules: any[] = [];
        labels: object = {
            "matchType": "",
            "matchTypes": [
                {"id": "AND", "label": "AND"},
                {"id": "OR", "label": "OR"}
            ],
            "addRule": "Add Rule",
            "removeRule": "&times;",
            "addGroup": "Add Group",
            "removeGroup": "&times;",
            "textInputPlaceholder": "value"
        };

        beforeCreate() {
            this.$store.dispatch({type: "getCriteriaAttributes"});
        }

        created() {
            this.rules = this.criteriaAttributes.map((ca: CriteriaAttribute) => ({
                // TODO: implement select when we have service that returns predetermined values
                /*type: 'select',
                label: ca.name,
                id: ca.name,
                operators: ['=', '<>', '<', '<=', '>', '>='],
                choices: ca.values.map((val: string) => ({
                    label: val,
                    value: val
                }))*/
                type: "text",
                label: ca.name,
                id: ca.name,
                operators: ["=", "<>", "<", "<=", ">", ">="]
            }));
        }

        logIt() {
            console.log(JSON.stringify(this.criteria));
            const clause = parseQueryBuilderJson(this.criteria).join("");
            const newCriteria: Criteria = {
                logicalOperator: "AND",
                children: []
            };
            const json = parseCriteriaString(clause, newCriteria);
            console.log(clause);
            console.log(JSON.stringify(json));
            /*console.log(
                parseCriteriaString(
                    '[ADTTOTAL]>=\'5\' AND ([ADTYEAR]>=\'4\' AND [ADTYEAR]<=\'7\' AND ([AGE]=\'4\' OR [AGE]=\'6\'))',
                    {logicalOperator: 'AND', children: []}
                )
            );*/
        }
    }
</script>