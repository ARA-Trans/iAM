<template>
    <v-dialog max-width="450px" persistent v-model="showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Priority</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Priority" outline v-model="newPriority.priorityLevel"
                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsNumeric]"/>
                    <v-autocomplete :items="years" label="Select a Year" outline v-model="selectedYear"
                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="disableSubmit()" @click="onSubmit(true)" class="ara-blue-bg white--text">
                        Save
                    </v-btn>
                    <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">
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
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreatePriorityDialog extends Vue {
        @Prop() showDialog: boolean;

        newPriority: Priority = {...emptyPriority, id: ObjectID.generate(), year: moment().year()};
        selectedYear: string = moment().year().toString();
        years: string[] = [];
        rules: InputValidationRules = {...rules};

        mounted() {
            const endYear = moment().subtract(50, 'years');
            let currentYear = moment().add(50, 'years');
            this.years.push('');
            while (currentYear >= endYear) {
                this.years.push(currentYear.year().toString());
                currentYear = currentYear.clone().subtract(1, 'years');
            }
        }

        @Watch('selectedYear')
        onSelectedYearChanged() {
            if (!hasValue(this.selectedYear)) {
                this.newPriority.year = null;
                return;
            }
            this.newPriority.year = parseInt(this.selectedYear);
        }


        /**
         * Whether or not to disable the 'Submit' button
         */
        disableSubmit() {
            return !hasValue(this.newPriority.priorityLevel);
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

            this.newPriority = {...emptyPriority, id: ObjectID.generate(), year: moment().year()};
        }
    }
</script>
