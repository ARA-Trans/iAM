<template>
    <v-layout>
        <v-dialog v-model="dialogData.showDialog" persistent max-width="250px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Treatment Strategy</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="createdTreatmentStrategy.name" outline></v-text-field>
                        <v-textarea rows="3" no-resize outline full-width
                                    :label="createdTreatmentStrategy.description === '' ? 'Description' : ''"
                                    v-model="createdTreatmentStrategy.description">
                        </v-textarea>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit"
                               :disabled="createdTreatmentStrategy.name === '' ||
                                          createdTreatmentStrategy.description === ''">
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

        createdTreatmentStrategy: TreatmentLibrary = {...emptyTreatmentLibrary};

        /**
         * Watcher: dialogData
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            if (hasValue(this.dialogData.selectedTreatmentLibraryDescription)) {
                // set createdTreatmentStrategy.description property with description data from dialogData
                this.createdTreatmentStrategy.description = this.dialogData.selectedTreatmentLibraryDescription;
            }
            if (hasValue(this.dialogData.selectedTreatmentLibraryTreatments)) {
                // set createdTreatmentStrategy.description property with treatments data from dialogData
                this.createdTreatmentStrategy.treatments = this.dialogData.selectedTreatmentLibraryTreatments;
            }
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            // submit created treatment strategy result
            this.$emit('submit', this.createdTreatmentStrategy);
            // reset createdTreatmentStrategy property
            this.createdTreatmentStrategy = {...emptyTreatmentLibrary};
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {
            // submit null result
            this.$emit('submit', null);
            // reset createdTreatmentStrategy property
            this.createdTreatmentStrategy = {...emptyTreatmentLibrary};
        }
    }
</script>