<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Scenario</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="createScenarioData.name" outline></v-text-field>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn color="info" v-on:click="onSubmit(true)" :disabled="createScenarioData.name === ''">
                            Submit
                        </v-btn>
                        <v-btn v-on:click="onSubmit(false)">Cancel</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {
        CreateScenarioData, emptyCreateScenarioData
    } from '@/shared/models/modals/scenario-creation-data';
    import {clone} from 'ramda';

    @Component
    export default class CreateScenarioDialog extends Vue {
        @Prop() showDialog: boolean;

        createScenarioData: CreateScenarioData = clone(emptyCreateScenarioData);

        /**
         * 'Submit' button has been clicked
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.createScenarioData);
            } else {
                this.$emit('submit', null);
            }

            this.createScenarioData = clone(emptyCreateScenarioData);
        }
    }
</script>