<template>
    <v-container fluid grid-list-xl>
        <div class="">
            <v-layout>

            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn color="info" @click="onSavePriorities">Save</v-btn>
                <v-btn color="error" @click="onCancelChangesToPriorities">Cancel</v-btn>
            </v-layout>
        </v-footer>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {Priority} from "@/shared/models/iAM/priority";

    @Component
    export default class PrioritiesTab extends Vue {
        @State(state => state.priority.priorities) statePriorities: Priority[];

        @Action('savePriorities') savePrioritiesAction: any;

        @Watch('statePriorities')
        onStatePrioritiesChanged() {

        }

        onSavePriorities() {

        }

        onCancelChangesToPriorities() {

        }
    }
</script>