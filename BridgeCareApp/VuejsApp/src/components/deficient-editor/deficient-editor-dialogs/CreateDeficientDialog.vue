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
                                          v-model.number="newDeficient.deficient" :mask="'##########'"
                                          :rules="[rules['generalRules'].valueIsNotEmpty]"></v-text-field>
                        </v-flex>
                        <v-flex>
                            <v-text-field label="Allowed Deficient(%)" outline v-model.number="newDeficient.percentDeficient"
                                          :mask="'###'"
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
        @Prop() numberOfDeficients: number;

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

                if (this.showDialog) {
                    this.setNewDeficientDefaultValues();
                }
            }
        }

        @Watch('showDialog')
        onShowDialogChanged() {
            if (this.showDialog) {
                this.setNewDeficientDefaultValues();
            }
        }

        @Watch('numberOfDeficients')
        onNumberOfDeficientsChanged() {
            if (this.showDialog) {
                this.setNewDeficientDefaultValues();
            }
        }

        setNewDeficientDefaultValues() {
            this.newDeficient = {
                ...this.newDeficient,
                attribute: hasValue(this.numericAttributes) ? this.numericAttributes[0] : '',
                name: `Unnamed Deficient ${this.numberOfDeficients + 1}`,
                deficient: this.numberOfDeficients > 0 ? this.numberOfDeficients + 1 : 1
            };
        }

        /**
         * Disables the submit button if the new deficient is missing required properties' data
         */
        disableSubmit() {
            return !(this.rules['generalRules'].valueIsNotEmpty(this.newDeficient.name) === true &&
                this.rules['generalRules'].valueIsNotEmpty(this.newDeficient.attribute) &&
                this.rules['generalRules'].valueIsNotEmpty(this.newDeficient.deficient) &&
                this.rules['generalRules'].valueIsNotEmpty(this.newDeficient.percentDeficient) &&
                this.rules['generalRules'].valueIsWithinRange(this.newDeficient.percentDeficient, [0, 100]));
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

            this.newDeficient = {...emptyDeficient, id: ObjectID.generate()};
        }
    }
</script>
