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

                    <v-menu v-model="showDatePicker" :close-on-content-click="false" :nudge-right="40" lazy
                            transition="scale-transition" offset-y full-width min-width="290px">
                        <template slot="activator">
                            <v-text-field label="Year" v-model="newPriority.year" outline append-icon="fas fa-calendar-day">
                            </v-text-field>
                        </template>
                        <v-date-picker v-model="year" ref="picker" min="1950" reactive no-title @input="onSetYear">
                        </v-date-picker>
                    </v-menu>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)" :disabled="disableSubmit()">
                        Save
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

        newPriority: Priority = {...clone(emptyPriority), id: ObjectID.generate(), year: moment().year()};
        showDatePicker: boolean = false;
        year: string = moment().year().toString();

        /**
         * Sets the date picker to show year options on nextTrick when showDatePicker has changed
         */
        @Watch('showDatePicker')
        onShowDatePickerChanged() {
            if (this.showDatePicker) {
                // @ts-ignore
                this.$nextTick(() => this.$refs.picker.activePicker = 'YEAR');
            }
        }

        /**
         * Sets the newPriority.year property with the currently selected year
         * @param year Currently selected year as a string
         */
        onSetYear(year: string) {
            this.year = year.substr(0, 4);
            this.newPriority.year = parseInt(this.year);
            this.showDatePicker = false;
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

            this.newPriority = {...clone(emptyPriority), id: ObjectID.generate(), year: moment().year()};
        }
    }
</script>