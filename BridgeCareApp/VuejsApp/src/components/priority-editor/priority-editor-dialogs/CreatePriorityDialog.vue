<template>
    <v-dialog v-model="showDialog" persistent max-width="450px">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Priority</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Priority" v-model="newPriority.priorityLevel" outline></v-text-field>
                    <v-autocomplete :items="years" v-model="selectedYear" outline label="Select a Year"></v-autocomplete>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)" :disabled="disableSubmit()">
                        Submit
                    </v-btn>
                    <v-btn class="ara-orange-bg white--text" @click="onSubmit(false)">
                        Cancel
                    </v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {emptyPriority, Priority} from '@/shared/models/iAM/priority';
    import {hasValue} from '@/shared/utils/has-value-util';
    import moment from 'moment';
    const ObjectID = require('bson-objectid');
    import {clone} from 'ramda';

    @Component
    export default class CreatePriorityDialog extends Vue {
        @Prop() showDialog: boolean;

        newPriority: Priority = clone({...emptyPriority, id: ObjectID.generate(), year: moment().year()});
        selectedYear: string = moment().year().toString();
        years: string[] = [];

        mounted() {
            const endYear = moment().subtract(50, 'years');
            let currentYear = moment().add(50, 'years');
            while (currentYear >= endYear) {
                this.years.push(currentYear.year().toString());
                currentYear = currentYear.clone().subtract(1, 'years');
            }
        }

        @Watch('selectedYear')
        onSelectedYearChanged() {
            this.newPriority.year = parseInt(this.selectedYear);
        }


        /**
         * Whether or not to disable the 'Submit' button
         */
        disableSubmit() {
            return !hasValue(this.newPriority.priorityLevel) || !hasValue(this.newPriority.year);
        }

        /**
         * Emits the newPriority object or a null value to the parent component and resets the newPriority object
         * @param submit Whether or not to emit the newPriority object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.newPriority);
            } else {
                this.$emit('submit', null);
            }

            this.newPriority = clone({...emptyPriority, id: ObjectID.generate(), year: moment().year()});
        }
    }
</script>