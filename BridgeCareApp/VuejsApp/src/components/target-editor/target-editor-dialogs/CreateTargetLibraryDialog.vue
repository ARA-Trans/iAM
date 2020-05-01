<template>
    <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Target Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" outline v-model="newTargetLibrary.name"
                                  :rules="rules['generalRules'].valueIsNotEmpty"/>
                    <v-textarea label="Description" no-resize outline rows="3"
                                v-model="newTargetLibrary.description"/>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="newTargetLibrary.name ===''" @click="onSubmit(true)"
                           class="ara-blue-bg white--text">
                        Submit
                    </v-btn>
                    <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang='ts'>
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {CreateTargetLibraryDialogData} from '@/shared/models/modals/create-target-library-dialog-data';
    import {emptyTargetLibrary, Target, TargetLibrary} from '@/shared/models/iAM/target';
    import {getUserName} from '@/shared/utils/get-user-info';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateTargetLibraryDialog extends Vue {
        @Prop() dialogData: CreateTargetLibraryDialogData;

        newTargetLibrary: TargetLibrary = {...emptyTargetLibrary, id: ObjectID.generate()};
        rules: InputValidationRules = {...rules};

        /**
         * Sets the newTargetLibrary object's description & targets properties with the dialogData object's
         * description & targets properties
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.newTargetLibrary = {
                ...this.newTargetLibrary,
                description: this.dialogData.description,
                targets: this.dialogData.targets
            };
        }

        /**
         * Emits the newTargetLibrary object or a null value to the parent component and resets the newTargetLibrary
         * object
         * @param submit Flag that determines if the newTargetLibrary is emitted or a null value is emitted
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.setIdsForNewTargetLibrarySubData();
                this.newTargetLibrary.owner = getUserName();
                this.$emit('submit', this.newTargetLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.newTargetLibrary = {...emptyTargetLibrary, id: ObjectID.generate()};
        }

        /**
         * Generates bson ids for the new target library's targets if there are any
         */
        setIdsForNewTargetLibrarySubData() {
            this.newTargetLibrary.targets = this.newTargetLibrary.targets
                .map((target: Target) => ({
                    ...target,
                    id: ObjectID.generate()
                }));
        }
    }
</script>
