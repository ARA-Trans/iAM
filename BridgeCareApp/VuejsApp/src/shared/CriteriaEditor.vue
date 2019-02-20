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
    import {Criteria, emptyCriteria} from "@/models/criteria";
    import {parseQueryBuilderJson} from "@/shared/utils/criteria-editor-parsers";
    import {attributes} from "@/shared/utils/mock-data";

    @Component({
        components: {VueQueryBuilder}
    })
    export default class CriteriaEditor extends Vue {
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

        created() {
            attributes.forEach((attr: string) => {
                this.rules.push({
                    type: "text",
                    label: attr,
                    id: attr,
                    operators: ["=", "<>", "<", "<=", ">", ">="]
                });
            });
        }

        logIt() {
            console.log(JSON.stringify(this.criteria));
            console.log(parseQueryBuilderJson(this.criteria).join(""));
        }
    }
</script>