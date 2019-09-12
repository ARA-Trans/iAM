<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="250px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center>
                        <h3>New Equation</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column>
                        <v-text-field label="Name" v-model="createdPerformanceLibraryEquation.equationName" outline>
                        </v-text-field>
                        <v-select label="Select Attribute" :items="attributesSelectListItems"
                                  v-model="createdPerformanceLibraryEquation.attribute" outline>
                        </v-select>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)"
                               :disabled="createdPerformanceLibraryEquation.equationName === '' ||
                                          createdPerformanceLibraryEquation.attribute === ''">
                            Submit
                        </v-btn>
                        <v-btn class="ara-orange-bg white--text" @click="onSubmit(false)">Cancel</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {emptyEquation, PerformanceLibraryEquation} from '@/shared/models/iAM/performance';
    import {isEmpty, clone} from 'ramda';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {hasValue} from '@/shared/utils/has-value-util';

    @Component
    export default class CreatePerformanceLibraryDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        attributesSelectListItems: SelectItem[] = [];
        createdPerformanceLibraryEquation: PerformanceLibraryEquation = clone(emptyEquation);

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
         * Emits the createdPerformanceLibraryEquation object or a null value to the parent component and resets the
         * createdPerformanceLibraryEquation object
         * @param submit Whether or not to emit the createdPerformanceLibraryEquation object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.createdPerformanceLibraryEquation);
            } else {
                this.$emit('submit', null);
            }

            this.createdPerformanceLibraryEquation = {...emptyEquation};
        }
    }
</script>