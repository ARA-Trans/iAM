<template>
    <v-dialog v-model="showDialog" persistent max-width="200px">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>Set Range</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-text-field v-model="range" label="Edit" single-line :mask="'####'"></v-text-field>
            </v-card-text>
            <v-card-actions>
                <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)" :disabled="range === ''">Save</v-btn>
                <v-btn class="ara-orange-bg white--text" @click="onSubmit(false)">Cancel</v-btn>
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