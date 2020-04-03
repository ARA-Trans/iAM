<template>
    <v-layout>
        <v-dialog content content-class="centered-dialog" persistent v-model="loading">
            <v-container>
                <v-layout align-center column justify-center>
                    <v-progress-circular :size="70" :width="7" class="ara-blue-pantone-281"
                                         indeterminate></v-progress-circular>
                    <h1>Please wait</h1>
                </v-layout>
            </v-container>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';
    import {State} from 'vuex-class';

    @Component
    export default class Spinner extends Vue {
        @State(store => store.busy.isBusy) loading: boolean;
    }
</script>

<style>
    .dialog.centered-dialog, .v-dialog.centered-dialog {
        background: grey;
        box-shadow: none;
        border-radius: 6px;
        width: auto;
        color: whitesmoke;
    }
</style>
