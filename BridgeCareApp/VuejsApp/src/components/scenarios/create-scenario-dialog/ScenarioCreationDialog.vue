<template>
    <v-layout>
        <v-dialog v-model="scenarioDialog.showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Scenario</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="createdScenario.name" outline></v-text-field>
                        <v-textarea rows="3" no-resize outline full-width
                                    :label="createdScenario.description === '' ? 'Description' : ''"
                                    v-model="createdScenario.description">
                        </v-textarea>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit"
                               :disabled="createdScenario.name === ''">
                            Submit
                        </v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {hasValue} from '@/shared/utils/has-value';
    import { CreateScenario, emptyScenario, CreateScenarioDialogData } from '@/shared/models/dialogs/create-scenario-dialog/scenario-creation-data';

    @Component
    export default class ScenarioCreationDialog extends Vue {
        @Prop() scenarioDialog: CreateScenarioDialogData;

        createdScenario: CreateScenario = { ...emptyScenario };

        /**
         * Watcher: dialogData
         */
        @Watch('scenarioDialog')
        onScenarioDialogChanged() {
            /*if a user has selected an investment strategy to create a new library from, then set the new investment
            strategy's inflation/discount rates, description, budget order, and budget years with the selected investment
            strategy's*/
            this.createdScenario = {
                ...this.createdScenario,
                description: hasValue(this.scenarioDialog.description) ? this.scenarioDialog.description : ''
            };
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            this.$emit('submit', this.createdScenario);
            this.createdScenario = { ...emptyScenario};
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {
            this.$emit('submit', null);
            this.createdScenario = { ...emptyScenario};
        }
    }
</script>