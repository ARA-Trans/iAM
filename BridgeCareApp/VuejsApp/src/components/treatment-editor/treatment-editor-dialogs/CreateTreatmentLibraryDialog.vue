<template>
    <v-layout>
        <v-dialog v-model="dialogData.showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center>
                        <h3>New Treatment Library</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column>
                        <v-text-field label="Name" v-model="createdTreatmentLibrary.name" outline></v-text-field>
                        <v-textarea rows="3" no-resize outline label="Description"
                                    v-model="createdTreatmentLibrary.description">
                        </v-textarea>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn color="info" @click="onSubmit(true)"
                               :disabled="createdTreatmentLibrary.name === ''">
                            Save
                        </v-btn>
                        <v-btn color="error" @click="onSubmit(false)">Cancel</v-btn>
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
    } from '@/shared/models/modals/create-treatment-library-dialog-data';
    import {emptyTreatmentLibrary, TreatmentLibrary} from '@/shared/models/iAM/treatment';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {clone} from 'ramda';
    import {getUserName} from '@/shared/utils/get-user-info';

    @Component
    export default class CreateTreatmentLibraryDialog extends Vue {
        @Prop() dialogData: CreateTreatmentLibraryDialogData;

        createdTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);

        /**
         * Sets the createdTreatmentLibrary's description & treatments data properties if present in the dialogData object
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            if (hasValue(this.dialogData.selectedTreatmentLibraryDescription)) {
                this.createdTreatmentLibrary.description = this.dialogData.selectedTreatmentLibraryDescription;
            }

            if (hasValue(this.dialogData.selectedTreatmentLibraryTreatments)) {
                this.createdTreatmentLibrary.treatments = this.dialogData.selectedTreatmentLibraryTreatments;
            }
        }

        /**
         * Emits the createdTreatmentLibrary object or a null value to the parent component and resets the
         * createdTreatmentLibrary object
         * @param submit Whether or not to emit the createdTreatmentLibrary object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.createdTreatmentLibrary.owner = getUserName();
                this.$emit('submit', this.createdTreatmentLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.createdTreatmentLibrary = clone(emptyTreatmentLibrary);
        }
    }
</script>