<template>
    <v-layout>
        <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
            <v-card>
                <v-card-title>
                    <v-layout justify-center>
                        <h3>New Treatment Library</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column>
                        <v-text-field label="Name" outline v-model="newTreatmentLibrary.name"></v-text-field>
                        <v-textarea label="Description" no-resize outline rows="3"
                                    v-model="newTreatmentLibrary.description">
                        </v-textarea>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn :disabled="newTreatmentLibrary.name === ''" @click="onSubmit(true)"
                               color="info">
                            Save
                        </v-btn>
                        <v-btn @click="onSubmit(false)" color="error">Cancel</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {CreateTreatmentLibraryDialogData} from '@/shared/models/modals/create-treatment-library-dialog-data';
    import {
        Consequence,
        Cost,
        emptyFeasibility,
        emptyTreatmentLibrary,
        Treatment,
        TreatmentLibrary
    } from '@/shared/models/iAM/treatment';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {clone} from 'ramda';
    import {getUserName} from '@/shared/utils/get-user-info';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateTreatmentLibraryDialog extends Vue {
        @Prop() dialogData: CreateTreatmentLibraryDialogData;

        newTreatmentLibrary: TreatmentLibrary = clone({...emptyTreatmentLibrary, id: ObjectID.generate()});

        /**
         * Sets the newTreatmentLibrary object's description & treatments data properties if present in the dialogData object
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.newTreatmentLibrary = {
                ...this.newTreatmentLibrary,
                description: this.dialogData.selectedTreatmentLibraryDescription,
                treatments: this.dialogData.selectedTreatmentLibraryTreatments
            };
        }

        /**
         * Emits the newTreatmentLibrary object or a null value to the parent component and resets the
         * newTreatmentLibrary object
         * @param submit Whether or not to emit the newTreatmentLibrary object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.setIdsForNewTreatmentLibraryRelatedData();
                this.newTreatmentLibrary.owner = getUserName();
                this.$emit('submit', this.newTreatmentLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.newTreatmentLibrary = clone({...emptyTreatmentLibrary, id: ObjectID.generate()});
        }

        /**
         * Sets the ids for the newTreatmentLibrary object's sub-data
         */
        setIdsForNewTreatmentLibraryRelatedData() {
            if (hasValue(this.newTreatmentLibrary.treatments)) {
                this.newTreatmentLibrary.treatments = this.newTreatmentLibrary.treatments
                    .map((treatment: Treatment) => ({
                        ...treatment,
                        id: ObjectID.generate(),
                        feasibility: hasValue(treatment.feasibility)
                            ? {...treatment.feasibility, id: ObjectID.generate()}
                            : {...emptyFeasibility, id: ObjectID.generate()},
                        costs: treatment.costs
                            .map((cost: Cost) => ({...cost, id: ObjectID.generate()})),
                        consequences: treatment.consequences
                            .map((consequence: Consequence) => ({...consequence, id: ObjectID.generate()}))
                    }));
            }
        }
    }
</script>
