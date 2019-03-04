<template>
    <v-container fluid grid-list-xl>
        <v-layout row wrap>
            <v-flex xs12>
                <v-btn v-on:click="onLaunchCriteriaEditor()">Launch Criteria Editor</v-btn>
            </v-flex>
            <v-flex xs12>
                <div>Before Parsing: {{beforeParsing}}</div>
            </v-flex>
            <v-flex>
                <div v-show="afterParsing !== ''">After Parsing: {{afterParsing}}</div>
            </v-flex>
            <v-flex xs12>
                <CriteriaEditor :showCriteriaEditor="showCriteriaEditor" @applyCriteria="onApplyCriteria"/>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from "vue";
    import {Component} from "vue-property-decorator";
    import {Action} from "vuex-class";
    import CriteriaEditor from "../shared/CriteriaEditor.vue";

    @Component({
        components: {CriteriaEditor}
    })
    export default class Criteria extends Vue {
        @Action("setCriteria") setCriteriaAction: any;

        beforeParsing: string = "[ADTTOTAL]='50' OR [ADTYEAR]='1999' AND [AGE]>'30' OR [APPRALIGN]>='7' OR [APPRALIGN]<='5'";
        afterParsing: string = "";
        showCriteriaEditor: boolean = false;

        onLaunchCriteriaEditor() {
            this.afterParsing = "";
            this.setCriteriaAction({clause: this.beforeParsing});
            this.showCriteriaEditor = true;
        }

        onApplyCriteria(clause: string) {
            this.afterParsing = clause;
            this.setCriteriaAction({clause: ""});
            this.showCriteriaEditor = false;
        }
    }
</script>