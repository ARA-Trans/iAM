<template>
    <v-layout>
        <v-dialog v-model="dialogData.showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Performance Strategy Library</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="createdPerformanceStrategy.name" outline></v-text-field>
                        <v-textarea rows="3" no-resize outline full-width
                                    :label="createdPerformanceStrategy.description === '' ? 'Description' : ''"
                                    v-model="createdPerformanceStrategy.description">
                        </v-textarea>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit"
                               :disabled="createdPerformanceStrategy.name === ''">
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
    import {emptyPerformanceStrategy, PerformanceStrategy} from '@/shared/models/iAM/performance';
    import {hasValue} from '@/shared/utils/has-value';
    import {
        CreatePerformanceStrategyDialogData
    } from '@/shared/models/dialogs/performance-editor-dialogs/create-performance-strategy-dialog-data';

    @Component
    export default class CreatePerformanceStrategyDialog extends Vue {
        @Prop() dialogData: CreatePerformanceStrategyDialogData;

        createdPerformanceStrategy: PerformanceStrategy = {...emptyPerformanceStrategy};

        /**
         * Watcher: CreatePerformanceStrategyDialogData
         */
        @Watch('dialogData')
        onSelectedPerformanceStrategyChanged() {
            // if dialog data has a selectedPerformanceStrategyDescription for created performance strategy...
            if (hasValue(this.dialogData.selectedPerformanceStrategyDescription)) {
                // set the created performance strategy with dialog data's selectedPerformanceStrategyDescription
                this.createdPerformanceStrategy.description = this.dialogData.selectedPerformanceStrategyDescription;
            }
            // if dialog data has selectedPerformanceStrategyEquations for created performance strategy...
            if (hasValue(this.dialogData.selectedPerformanceStrategyEquations)) {
                // set the created performance strategy with dialog data's selectedPerformanceStrategyEquations
                this.createdPerformanceStrategy.equations = this.dialogData.selectedPerformanceStrategyEquations;
            }
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            this.$emit('submit', this.createdPerformanceStrategy);
            this.createdPerformanceStrategy = {...emptyPerformanceStrategy};
        }

        /**
         * One of the 'Cancel' buttons has been clicked
         */
        onCancel() {
            this.$emit('submit', null);
            this.createdPerformanceStrategy = {...emptyPerformanceStrategy};
        }
    }
</script>