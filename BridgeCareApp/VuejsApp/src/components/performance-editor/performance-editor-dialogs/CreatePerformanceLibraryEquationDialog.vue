<template>
    <v-layout>
        <v-dialog max-width="250px" persistent v-model="showDialog">
            <v-card>
                <v-card-title>
                    <v-layout justify-center>
                        <h3>New Equation</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column>
                        <v-text-field label="Name" outline v-model="newPerformanceLibraryEquation.equationName"
                                      :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                        <v-select :items="attributesSelectListItems" label="Select Attribute"
                                  outline v-model="newPerformanceLibraryEquation.attribute"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn :disabled="newPerformanceLibraryEquation.equationName === '' ||
                                          newPerformanceLibraryEquation.attribute === ''"
                               @click="onSubmit(true)" class="ara-blue-bg white--text">
                            Save
                        </v-btn>
                        <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State} from 'vuex-class';
    import {emptyEquation, PerformanceLibraryEquation} from '@/shared/models/iAM/performance';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';
    import {clone} from 'ramda';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreatePerformanceLibraryEquationDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        attributesSelectListItems: SelectItem[] = [];
        newPerformanceLibraryEquation: PerformanceLibraryEquation = {...emptyEquation, id: ObjectID.generate()};
        rules: InputValidationRules = clone(rules);

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateNumericAttributes)) {
                this.setAttributesSelectListItems();
            }
        }

        /**
         * Calls the setAttributesSelectListItems function if a change to stateNumericAttributes causes it to have a value
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            if (hasValue(this.stateNumericAttributes)) {
                this.setAttributesSelectListItems();
            }
        }

        /**
         * Sets the attribute select items using numeric attributes from state
         */
        setAttributesSelectListItems() {
            this.attributesSelectListItems = this.stateNumericAttributes.map((attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name
            }));
        }

        /**
         * Emits the newPerformanceLibraryEquation object or a null value to the parent component and resets the
         * newPerformanceLibraryEquation object
         * @param submit Whether or not to emit the newPerformanceLibraryEquation object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.newPerformanceLibraryEquation);
            } else {
                this.$emit('submit', null);
            }

            this.newPerformanceLibraryEquation = {...emptyEquation, id: ObjectID.generate()};
        }
    }
</script>
