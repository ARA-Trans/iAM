<template>
    <v-layout>
        <v-dialog max-width="450px" persistent v-model="showDialog">
            <v-card>
                <v-card-title>
                    <v-layout justify-center>
                        <h3>New Deficient</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text class="new-deficient-card-text">
                    <v-layout column>
                        <v-flex>
                            <v-text-field label="Name" outline v-model="newDeficient.name"
                                          :rules="[rules['generalRules'].valueIsNotEmpty]"></v-text-field>
                        </v-flex>
                        <v-flex>
                            <v-select :items="numericAttributes" label="Select Attribute"
                                      outline
                                      v-model="newDeficient.attribute" :rules="[rules['generalRules'].valueIsNotEmpty]">
                            </v-select>
                        </v-flex>
                        <v-flex>
                            <v-text-field label="Deficient Level" outline
                                          v-model="newDeficient.deficient"
                                          :rules="[rules['generalRules'].valueIsNotEmpty]"></v-text-field>
                        </v-flex>
                        <v-flex>
                            <v-text-field label="Allowed Deficient(%)" outline v-model="newDeficient.percentDeficient"
                                          :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(newDeficient.percentDeficient, [0, 100])]">
                            </v-text-field>
                        </v-flex>
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
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State} from 'vuex-class';
    import {Deficient, emptyDeficient} from '@/shared/models/iAM/deficient';
    import {clone} from 'ramda';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateDeficientDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        newDeficient: Deficient = clone({...emptyDeficient, id: ObjectID.generate()});
        numericAttributes: string[] = [];
        rules: InputValidationRules = clone(rules);

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
         * Disables the submit button if the new deficient is missing required properties' data
         */
        disableSubmit() {
            return !hasValue(this.newDeficient.name) || !hasValue(this.newDeficient.attribute) ||
                !hasValue(this.newDeficient.deficient) || !hasValue(this.newDeficient.percentDeficient) ||
                !this.rules['generalRules'].valueIsWithinRange(this.newDeficient.percentDeficient, [0, 100]);
        }

        /**
         * Emits the newDeficient object or a null value to the parent component and resets the newDeficient object
         * @param submit Whether or not to emit the newDeficient object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.newDeficient);
            } else {
                this.$emit('submit', null);
            }

            this.newDeficient = clone({...emptyDeficient, id: ObjectID.generate()});
        }
    }
</script>
