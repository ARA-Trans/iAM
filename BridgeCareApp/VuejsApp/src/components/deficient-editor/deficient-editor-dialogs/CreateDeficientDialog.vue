<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center>
                        <h3>New Deficient</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text class="new-deficient-card-text">
                    <v-layout column>
                        <v-flex>
                            <v-text-field label="Name" v-model="newDeficient.name" outline></v-text-field>
                        </v-flex>
                        <v-flex>
                            <v-select label="Select Attribute" :items="numericAttributes" v-model="newDeficient.attribute"
                                      outline>
                            </v-select>
                        </v-flex>
                        <v-flex>
                            <v-text-field label="Deficient Level" v-model="newDeficient.deficient" outline></v-text-field>
                        </v-flex>
                        <v-flex>
                            <v-text-field label="Allowed Deficient(%)" v-model="newDeficient.percentDeficient" outline>
                            </v-text-field>
                        </v-flex>
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
    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateDeficientDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        newDeficient: Deficient = clone(emptyDeficient);
        numericAttributes: string[] = [];

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateNumericAttributes)) {
                this.numericAttributes = getPropertyValues('name', this.stateNumericAttributes);
            }
        }

        /**
         * Sets newTarget.scenarioId property with dialogData.scenarioId property value
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            if (this.dialogData.showDialog) {
                this.newDeficient.scenarioId = this.dialogData.scenarioId;
                this.newDeficient.id = ObjectID.generate();
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
            return !hasValue(this.newDeficient.name) || !hasValue(this.newDeficient.attribute) ||
                !hasValue(this.newDeficient.deficient) || !hasValue(this.newDeficient.percentDeficient);
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

            this.newDeficient = clone(emptyDeficient);
        }
    }
</script>
