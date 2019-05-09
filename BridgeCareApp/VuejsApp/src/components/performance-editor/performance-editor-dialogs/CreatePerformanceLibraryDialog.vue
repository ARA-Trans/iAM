<template>
    <v-layout>
        <v-dialog v-model="dialogData.showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Performance Library Library</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="createdPerformanceLibrary.name" outline></v-text-field>
                        <v-textarea rows="3" no-resize outline full-width
                                    :label="createdPerformanceLibrary.description === '' ? 'Description' : ''"
                                    v-model="createdPerformanceLibrary.description">
                        </v-textarea>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit"
                               :disabled="createdPerformanceLibrary.name === ''">
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
    import {emptyPerformanceLibrary, PerformanceLibrary} from '@/shared/models/iAM/performance';
    import {hasValue} from '@/shared/utils/has-value';
    import {
        CreatePerformanceLibraryDialogData
    } from '@/shared/models/dialogs/performance-editor-dialogs/create-performance-library-dialog-data';

    @Component
    export default class CreatePerformanceLibraryDialog extends Vue {
        @Prop() dialogData: CreatePerformanceLibraryDialogData;

        createdPerformanceLibrary: PerformanceLibrary = {...emptyPerformanceLibrary};

        /**
         * Watcher: CreatePerformanceLibraryDialogData
         */
        @Watch('dialogData')
        onSelectedPerformanceLibraryChanged() {
            // if dialog data has a selectedPerformanceLibraryDescription for created performance library...
            if (hasValue(this.dialogData.selectedPerformanceLibraryDescription)) {
                // set the created performance library with dialog data's selectedPerformanceLibraryDescription
                this.createdPerformanceLibrary.description = this.dialogData.selectedPerformanceLibraryDescription;
            }
            // if dialog data has selectedPerformanceLibraryEquations for created performance library...
            if (hasValue(this.dialogData.selectedPerformanceLibraryEquations)) {
                // set the created performance library with dialog data's selectedPerformanceLibraryEquations
                this.createdPerformanceLibrary.equations = this.dialogData.selectedPerformanceLibraryEquations;
            }
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            this.$emit('submit', this.createdPerformanceLibrary);
            this.createdPerformanceLibrary = {...emptyPerformanceLibrary};
        }

        /**
         * One of the 'Cancel' buttons has been clicked
         */
        onCancel() {
            this.$emit('submit', null);
            this.createdPerformanceLibrary = {...emptyPerformanceLibrary};
        }
    }
</script>