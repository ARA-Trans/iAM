<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="250px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Equation</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="createdPerformanceLibraryEquation.equationName" outline>
                        </v-text-field>
                        <v-select label="Select Attribute" :items="attributesSelectListItems"
                                  v-model="createdPerformanceLibraryEquation.attribute" outline>
                        </v-select>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn color="info" v-on:click="onSubmit(true)"
                               :disabled="createdPerformanceLibraryEquation.equationName === '' ||
                                          createdPerformanceLibraryEquation.attribute === ''">
                            Submit
                        </v-btn>
                        <v-btn color="error" v-on:click="onSubmit(false)">Cancel</v-btn>
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

    @Component
    export default class CreatePerformanceLibraryDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.attributes) attributes: string[];

        @Action('getAttributes') getAttributesAction: any;

        attributesSelectListItems: SelectItem[] = [];
        createdPerformanceLibraryEquation: PerformanceLibraryEquation = clone(emptyEquation);

        /**
         * Dispatches an action to get the attributes data from the server if the attributes array is empty
         */
        @Watch('showDialog')
        onShowDialogChanged() {
            if (this.showDialog && isEmpty(this.attributes)) {
                this.getAttributesAction();
            }
        }

        /**
         * Sets the attributesSelectListItems object using the attributes object
         */
        @Watch('attributes')
        onAttributesChanged() {
            if (!isEmpty(this.attributes)) {
                this.attributesSelectListItems = this.attributes.map((attribute: string) => ({
                    text: attribute,
                    value: attribute
                }));
            }
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