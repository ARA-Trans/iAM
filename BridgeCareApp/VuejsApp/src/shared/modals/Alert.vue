<template>
    <v-dialog max-width="290" persistent v-model="dialogData.showDialog">
        <v-card>
            <v-card-title class="headline">
                <v-layout justify-center>
                    {{dialogData.heading}}
                </v-layout>
            </v-card-title>
            <v-card-text>
                {{dialogData.message}}
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row v-if="dialogData.choice">
                    <v-btn @click="onSubmit(true)" class="ara-blue-bg white--text">
                        Proceed
                    </v-btn>
                    <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">
                        Cancel
                    </v-btn>
                </v-layout>
                <v-layout justify-center v-if="!dialogData.choice">
                    <v-btn @click="onSubmit(true)" class="ara-blue-bg white--text">
                        OK
                    </v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {AlertData} from '../models/modals/alert-data';

    @Component
    export default class Alert extends Vue {
        @Prop() dialogData: AlertData;

        /**
         * Emits a boolean result to the parent component
         * @param submit
         */
        onSubmit(submit: boolean) {
            this.$emit('submit', submit);
        }
    }
</script>
