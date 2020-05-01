<template>
    <v-dialog max-width="450px" persistent v-model="showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Target</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" outline v-model="newTarget.name"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                    <v-select :items="numericAttributes" label="Select Attribute"
                              outline v-model="newTarget.attribute" :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                    <v-text-field :mask="'####'" label="Year" outline v-model.number="newTarget.year"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                    <v-text-field label="Target" outline :mask="'##########'" v-model.number="newTarget.targetMean"
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
    import {State} from 'vuex-class';
    import {emptyTarget, Target} from '@/shared/models/iAM/target';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {hasValue} from '@/shared/utils/has-value-util';
    import moment from 'moment';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';

    const ObjectID = require('bson-objectid');
    @Component
    export default class CreateTargetDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        newTarget: Target = {...emptyTarget, id: ObjectID.generate(), year: moment().year()};
        numericAttributes: string[] = [];
        showDatePicker: boolean = false;
        year: string = moment().year().toString();
        rules: InputValidationRules = {...rules};

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateNumericAttributes)) {
                this.numericAttributes = getPropertyValues('name', this.stateNumericAttributes);
            }
        }

        /**
         * Sets the numericAttributes list property with a copy of the stateNumericAttributes list property
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            if (hasValue(this.stateNumericAttributes)) {
                this.numericAttributes = getPropertyValues('name', this.stateNumericAttributes);
            }
        }

        /**
         * Whether or not to disable the 'Submit' button
         */
        disableSubmit() {
            return !hasValue(this.newTarget.name) || !hasValue(this.newTarget.attribute) ||
                !hasValue(this.newTarget.year) || !hasValue(this.newTarget.targetMean);
        }

        /**
         * Emits the newTarget object or a null value to the parent component and resets the newTarget object
         * @param submit Whether or not to emit the newTarget object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.newTarget);
            } else {
                this.$emit('submit', null);
            }

            this.newTarget = {...emptyTarget, id: ObjectID.generate(), year: moment().year()};
        }
    }
</script>
