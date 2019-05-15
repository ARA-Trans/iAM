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
                        <v-btn color="info" v-on:click="onSubmit(true)"
                               :disabled="createdPerformanceLibrary.name === ''">
                            Submit
                        </v-btn>
                        <v-btn color="error" v-on:click="onSubmit(false)">Cancel</v-btn>
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
    import {clone} from 'ramda';

    @Component
    export default class CreatePerformanceLibraryDialog extends Vue {
        @Prop() dialogData: CreatePerformanceLibraryDialogData;

        createdPerformanceLibrary: PerformanceLibrary = clone(emptyPerformanceLibrary);

        /**
         * Sets the createdPerformanceLibrary's description & equation data properties if present in the dialogData object
         */
        @Watch('dialogData')
        onSelectedPerformanceLibraryChanged() {
            if (hasValue(this.dialogData.selectedPerformanceLibraryDescription)) {
                this.createdPerformanceLibrary.description = this.dialogData.selectedPerformanceLibraryDescription;
            }

            if (hasValue(this.dialogData.selectedPerformanceLibraryEquations)) {
                this.createdPerformanceLibrary.equations = this.dialogData.selectedPerformanceLibraryEquations;
            }
        }

        /**
         * Emits the createdPerformanceLibrary object or a null value to the parent component and resets the
         * createdPerformanceLibrary object
         * @param submit Whether or not to emit the createdPerformanceLibrary object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.createdPerformanceLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.createdPerformanceLibrary = {...emptyPerformanceLibrary};
        }
    }
</script>