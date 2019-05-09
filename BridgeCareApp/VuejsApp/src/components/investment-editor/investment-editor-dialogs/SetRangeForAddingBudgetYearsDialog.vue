<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="200px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>Set Range</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-text-field v-model="range" label="Edit" single-line :mask="'####'"></v-text-field>
                </v-card-text>
                <v-card-actions>
                    <v-btn color="info" v-on:click="onSubmit" :disabled="range === ''">Submit</v-btn>
                    <v-btn color="error" v-on:click="onCancel">Cancel</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';

    @Component
    export default class SetRangeForAddingBudgetYearsDialog extends Vue {
        @Prop() showDialog: boolean;

        range: number = 0;

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            // submit range result
            this.$emit('submit', this.range);
            // reset range
            this.range = 0;
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {
            // submit range result
            this.$emit('submit', 0);
            // reset range
            this.range = 0;
        }
    }
</script>