<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="250px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Treatment</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout fill-height>
                        <v-text-field label="Name" v-model="createdTreatment.name" outline></v-text-field>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit" :disabled="createdTreatment.name === ''">Submit</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {emptyTreatment, Treatment} from '@/shared/models/iAM/treatment';

    @Component
    export default class CreateTreatmentDialog extends Vue {
        @Prop() showDialog: boolean;

        createdTreatment: Treatment = {...emptyTreatment};

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            // submit created treatment result
            this.$emit('submit', this.createdTreatment);
            // reset createdTreatment property
            this.createdTreatment = {...emptyTreatment};
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {
            // submit null result
            this.$emit('submit', null);
            // reset createdTreatment property
            this.createdTreatment = {...emptyTreatment};
        }
    }
</script>