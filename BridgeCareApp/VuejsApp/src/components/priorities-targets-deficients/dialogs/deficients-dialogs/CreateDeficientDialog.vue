<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Deficient</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="newDeficient.name" outline></v-text-field>

                        <v-select label="Select Attribute" :items="numericAttributes" v-model="newDeficient.attribute"
                                  outline>
                        </v-select>

                        <v-text-field label="Deficient Level" v-model="newDeficient.deficient" outline></v-text-field>

                        <v-text-field label="Allowed Deficient(%)" v-model="newDeficient.percentDeficient" outline>
                        </v-text-field>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn color="info" @click="onSubmit(true)" :disabled="disableSubmit()">
                            Submit
                        </v-btn>
                        <v-btn color="error" @click="onSubmit(false)">
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