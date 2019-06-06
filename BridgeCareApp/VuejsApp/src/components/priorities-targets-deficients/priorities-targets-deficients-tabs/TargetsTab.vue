<template>
    <v-container fluid grid-list-xl>
        <div class="">
            <v-layout>

            </v-layout>
        </div>

        <v-footer>
            <v-layout justify-end row fill-height>
                <v-btn color="info" @click="onSaveTargets">Save</v-btn>
                <v-btn color="error" @click="onCancelChangesToTargets">Cancel</v-btn>
            </v-layout>
        </v-footer>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {Target} from '@/shared/models/iAM/target';

    @Component
    export default class TargetsTab extends Vue {
        @State(state => state.target.targets) stateTargets: Target[];

        @Action('saveTargets') saveTargetsAction: any;

        @Watch('stateTargets')
        onStateTargetsChanged() {

        }

        onSaveTargets() {

        }

        onCancelChangesToTargets() {

        }
    }
</script>