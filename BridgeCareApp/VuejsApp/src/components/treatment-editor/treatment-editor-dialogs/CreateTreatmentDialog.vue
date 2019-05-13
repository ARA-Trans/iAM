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
                        <v-btn color="info" v-on:click="onSubmit(true)" :disabled="createdTreatment.name === ''">Submit</v-btn>
                        <v-btn color="error" v-on:click="onSubmit(false)">Cancel</v-btn>
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
    import {clone} from 'ramda';

    @Component
    export default class CreateTreatmentDialog extends Vue {
        @Prop() showDialog: boolean;

        createdTreatment: Treatment = clone(emptyTreatment);

        /**
         * Emits the createdTreatment object or a null value to the parent component and resets the createdTreatment object
         * @param Whether or not to emit the createdTreatment object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.createdTreatment);
            } else {
                this.$emit('submit', null);
            }

            this.createdTreatment = clone(emptyTreatment);
        }
    }
</script>