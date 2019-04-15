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
                        <v-text-field label="Name" v-model="createdPerformanceStrategyEquation.equationName" outline>
                        </v-text-field>
                        <v-select label="Select Attribute" :items="attributesSelectListItems"
                                  v-model="createdPerformanceStrategyEquation.attribute" outline>
                        </v-select>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit"
                               :disabled="createdPerformanceStrategyEquation.equationName === '' ||
                                          createdPerformanceStrategyEquation.attribute === ''">
                            Submit
                        </v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {emptyEquation, PerformanceStrategyEquation} from '@/shared/models/iAM/performance';
    import {attributes} from '@/shared/utils/attributes';

    @Component
    export default class CreatePerformanceStrategyDialog extends Vue {
        @Prop() showDialog: boolean;

        attributesSelectListItems = attributes.map((attribute: string) => ({
            text: attribute,
            value: attribute
        }));
        createdPerformanceStrategyEquation: PerformanceStrategyEquation = {...emptyEquation};

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            this.$emit('submit', this.createdPerformanceStrategyEquation);
            this.createdPerformanceStrategyEquation = {...emptyEquation};
        }

        /**
         * One of the 'Cancel' buttons has been clicked
         */
        onCancel() {
            this.$emit('submit', null);
            this.createdPerformanceStrategyEquation = {...emptyEquation};
        }
    }
</script>