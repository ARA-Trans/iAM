<template>
    <v-dialog v-model="dialogData.showDialog" persistent max-width="450px">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Remaining Limit Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" outline v-model="newRemainingLifeLimitLibrary.name"></v-text-field>
                    <v-textarea rows="3" label="Description" no-resize outline
                                v-model="newRemainingLifeLimitLibrary.description">
                    </v-textarea>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)"
                           :disabled="newRemainingLifeLimitLibrary.name === ''">
                        Save
                    </v-btn>
                    <v-btn class="ara-orange-bg white--text" @click="onSubmit(false)">Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {CreateRemainingLifeLimitLibraryDialogData} from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
    import {emptyRemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';
    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateRemainingLifeLimitLibraryDialog extends Vue {
        @Prop() dialogData: CreateRemainingLifeLimitLibraryDialogData;

        newRemainingLifeLimitLibrary = {...emptyRemainingLifeLimitLibrary, id: ObjectID.generate()};

        /**
         * Instantiates a newRemainingLifeLimitLibrary object with the description and remainingLifeLimits data in
         * dialogData
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.newRemainingLifeLimitLibrary = {
                ...this.newRemainingLifeLimitLibrary,
                description: this.dialogData.description,
                remainingLifeLimits: this.dialogData.remainingLifeLimits
            };
        }

        /**
         * Emits the dialog result then re-instantiates the newRemainingLifeLimitLibrary object
         * @param submit
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.newRemainingLifeLimitLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.newRemainingLifeLimitLibrary = {...emptyRemainingLifeLimitLibrary, id: ObjectID.generate()};
        }
    }
</script>