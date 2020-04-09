<template>
    <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Remaining Life Limit</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-select :items="dialogData.numericAttributesSelectListItems" label="Select an Attribute"
                              outline v-model="newRemainingLifeLimit.attribute">

                    </v-select>
                    <v-text-field label="Limit" outline v-model="newRemainingLifeLimit.limit"></v-text-field>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="disableSubmit()" @click="onSubmit(true)" class="ara-blue-bg white--text">
                        Save
                    </v-btn>
                    <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {emptyRemainingLifeLimit, RemainingLifeLimit} from '@/shared/models/iAM/remaining-life-limit';
    import {CreateRemainingLifeLimitDialogData} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
    import {hasValue} from '@/shared/utils/has-value-util';

    var ObjectID = require('bson-objectid');

    @Component
    export default class CreateRemainingLifeLimitDialog extends Vue {
        @Prop() dialogData: CreateRemainingLifeLimitDialogData;

        newRemainingLifeLimit: RemainingLifeLimit = {...emptyRemainingLifeLimit, id: ObjectID.generate()};

        /**
         * Whether or not the 'Submit' button should be enabled
         */
        disableSubmit() {
            return !hasValue(this.newRemainingLifeLimit.attribute);
        }

        /**
         * Emits the dialog result then re-instantiates the newRemainingLifeLimit object
         * @param submit
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.newRemainingLifeLimit);
            } else {
                this.$emit('submit', null);
            }

            this.newRemainingLifeLimit = {...emptyRemainingLifeLimit, id: ObjectID.generate()};
        }
    }
</script>
