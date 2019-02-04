<template>
    <v-layout row justify-center>
        <v-dialog v-model="loading" persistent content content-class="centered-dialog">
            <v-container fill-height>
                <v-layout column justify-center align-center>
                    <v-progress-circular indeterminate :size="70" :width="7" color="purple"></v-progress-circular>
                    <h1>Please wait</h1>
                </v-layout>
            </v-container>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';

    @Component
    export default class ShowProgress extends Vue {
        loading: boolean = true

    }
</script>

<style>
    .dialog.centered-dialog,
    .v-dialog.centered-dialog {
        background: grey;
        box-shadow: none;
        border-radius: 6px;
        width: auto;
        color: whitesmoke;
    }
</style>