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
                              outline v-model="newRemainingLifeLimit.attribute"
                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                    <v-text-field label="Limit" outline :mask="'##########'"
                                  v-model.number="newRemainingLifeLimit.limit"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="disableSubmitAction()" @click="onSubmit(true)" class="ara-blue-bg white--text">
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
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {emptyRemainingLifeLimit, RemainingLifeLimit} from '@/shared/models/iAM/remaining-life-limit';
    import {CreateRemainingLifeLimitDialogData} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';
    import {hasValue} from '@/shared/utils/has-value-util';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateRemainingLifeLimitDialog extends Vue {
        @Prop() dialogData: CreateRemainingLifeLimitDialogData;

        newRemainingLifeLimit: RemainingLifeLimit = {...emptyRemainingLifeLimit, id: ObjectID.generate()};
        rules: InputValidationRules = {...rules};

        @Watch('dialogData')
        onDialogDataChanged() {
            this.newRemainingLifeLimit.attribute = hasValue(this.dialogData.numericAttributesSelectListItems)
                ? this.dialogData.numericAttributesSelectListItems[0].value.toString() : '';
        }

        /**
         * Whether or not the 'Submit' button should be enabled
         */
        disableSubmitAction() {
            return this.rules['generalRules'].valueIsNotEmpty(this.newRemainingLifeLimit.attribute) !== true ||
                this.rules['generalRules'].valueIsNotEmpty(this.newRemainingLifeLimit.limit) !== true;
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
