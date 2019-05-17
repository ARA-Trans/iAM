<template>
    <v-layout>
        <v-dialog v-model="dialogData.showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Scenario</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="createdScenario.name" outline></v-text-field>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn color="info" v-on:click="onSubmit(true)" :disabled="createdScenario.name === ''">
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
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {CreateScenario, emptyScenario, CreateScenarioDialogData} from '@/shared/models/modals/scenario-creation-data';
    import {clone} from 'ramda';

    @Component
    export default class CreateScenarioDialog extends Vue {
        @Prop() dialogData: CreateScenarioDialogData;

        createdScenario: CreateScenario = clone(emptyScenario);

        /**
         * Watcher: dialogData
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.createdScenario = {
                ...this.createdScenario,
                description: this.dialogData.description
            };
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.createdScenario);
            } else {
                this.$emit('submit', null);
            }

            this.createdScenario = clone(emptyScenario);
        }
    }
</script>