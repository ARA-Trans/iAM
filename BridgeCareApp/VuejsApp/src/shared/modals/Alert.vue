<template>
    <v-layout row justify-center>
        <v-dialog v-model="dialogData.showDialog" persistent max-width="290">
            <v-card>
                <v-card-title class="headline">
                    <v-layout justify-center fill-height>
                        {{dialogData.heading}}
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    {{dialogData.message}}
                </v-card-text>
                <v-card-actions>
                    <v-layout v-if="dialogData.choice" justify-space-between row fill-height>
                        <v-btn color="info" @click="onSubmit(true)">
                            Proceed
                        </v-btn>
                        <v-btn color="error" @click="onSubmit(false)">
                            Cancel
                        </v-btn>
                    </v-layout>
                    <v-layout v-if="!dialogData.choice" justify-center fill-height>
                        <v-btn color="info" @click="onSubmit(true)">
                            OK
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
