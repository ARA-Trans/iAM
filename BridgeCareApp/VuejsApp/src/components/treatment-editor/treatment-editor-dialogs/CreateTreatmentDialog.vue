<template>
    <v-layout>
        <v-dialog max-width="250px" persistent v-model="showDialog">
            <v-card>
                <v-card-title>
                    <v-layout justify-center>
                        <h3>New Treatment</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout>
                        <v-text-field label="Name" outline v-model="createdTreatment.name"></v-text-field>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn :disabled="createdTreatment.name === ''" @click="onSubmit(true)" color="info">Save
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
    import {Component, Prop} from 'vue-property-decorator';
    import {emptyTreatment, Treatment} from '@/shared/models/iAM/treatment';
    import {clone} from 'ramda';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateTreatmentDialog extends Vue {
        @Prop() showDialog: boolean;

        createdTreatment: Treatment = clone({...emptyTreatment, id: ObjectID.generate()});

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

            this.createdTreatment = clone({...emptyTreatment, id: ObjectID.generate()});
        }
    }
</script>
