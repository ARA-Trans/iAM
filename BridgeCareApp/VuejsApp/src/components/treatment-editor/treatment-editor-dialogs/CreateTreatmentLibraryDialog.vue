<template>
    <v-layout>
        <v-dialog v-model="dialogData.showDialog" persistent max-width="250px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Treatment Library</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="createdTreatmentLibrary.name" outline></v-text-field>
                        <v-textarea rows="3" no-resize outline full-width
                                    :label="createdTreatmentLibrary.description === '' ? 'Description' : ''"
                                    v-model="createdTreatmentLibrary.description">
                        </v-textarea>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit"
                               :disabled="createdTreatmentLibrary.name === '' ||
                                          createdTreatmentLibrary.description === ''">
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
    import {
        CreateTreatmentLibraryDialogData
    } from '@/shared/models/dialogs/treatment-editor-dialogs/create-treatment-library-dialog-data';
    import {emptyTreatmentLibrary, TreatmentLibrary} from '@/shared/models/iAM/treatment';
    import {hasValue} from '@/shared/utils/has-value';

    @Component
    export default class CreateTreatmentLibraryDialog extends Vue {
        @Prop() dialogData: CreateTreatmentLibraryDialogData;

        createdTreatmentLibrary: TreatmentLibrary = {...emptyTreatmentLibrary};

        /**
         * Watcher: dialogData
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            if (hasValue(this.dialogData.selectedTreatmentLibraryDescription)) {
                // set createdTreatmentLibrary.description property with description data from dialogData
                this.createdTreatmentLibrary.description = this.dialogData.selectedTreatmentLibraryDescription;
            }
            if (hasValue(this.dialogData.selectedTreatmentLibraryTreatments)) {
                // set createdTreatmentLibrary.description property with treatments data from dialogData
                this.createdTreatmentLibrary.treatments = this.dialogData.selectedTreatmentLibraryTreatments;
            }
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            // submit created treatment library result
            this.$emit('submit', this.createdTreatmentLibrary);
            // reset createdTreatmentLibrary property
            this.createdTreatmentLibrary = {...emptyTreatmentLibrary};
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {
            // submit null result
            this.$emit('submit', null);
            // reset createdTreatmentLibrary property
            this.createdTreatmentLibrary = {...emptyTreatmentLibrary};
        }
    }
</script>