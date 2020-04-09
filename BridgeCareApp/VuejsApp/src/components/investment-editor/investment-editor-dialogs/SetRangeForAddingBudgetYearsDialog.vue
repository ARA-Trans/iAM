<template>
    <v-dialog max-width="200px" persistent v-model="showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>Set Range</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-text-field :mask="'####'" label="Edit" single-line v-model="range"></v-text-field>
            </v-card-text>
            <v-card-actions>
                <v-btn :disabled="range === ''" @click="onSubmit(true)" class="ara-blue-bg white--text">Save</v-btn>
                <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';

    @Component
    export default class SetRangeForAddingBudgetYearsDialog extends Vue {
        @Prop() showDialog: boolean;

        range: number = 0;

        /**
         * Emits the range value or a null value to the parent component then resets the range value to 0
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.range);
            } else {
                this.$emit('submit', 0);
            }

            this.range = 0;
        }
    }
</script>
