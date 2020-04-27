<template>
    <v-dialog v-model="dialogData.showDialog" max-width="450px" persistent>
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Criteria Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field v-model="newCriteriaLibrary.name" label="Name" outline>
                    </v-text-field>
                    <v-textarea v-model="newCriteriaLibrary.description" label="Description" no-resize outline rows="3">
                    </v-textarea>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn @click="onSubmit(true)" :disabled="newCriteriaLibrary.name === ''"
                           class="ara-blue-bg white--text">
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
    import {CreateCriteriaLibraryDialogData} from '@/shared/models/modals/create-criteria-library-dialog-data';
    import {CriteriaLibrary, emptyCriteriaLibrary} from '@/shared/models/iAM/criteria';
    import {getUserName} from '@/shared/utils/get-user-info';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateCriteriaLibraryDialog extends Vue {
        @Prop() dialogData: CreateCriteriaLibraryDialogData;

        newCriteriaLibrary: CriteriaLibrary = {...emptyCriteriaLibrary, id: ObjectID.generate()};

        /**
         * Sets the newCriteriaLibrary object's description & criteria properties
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.newCriteriaLibrary = {
                ...this.newCriteriaLibrary,
                description: this.dialogData.description,
                criteria: this.dialogData.criteria
            };
        }

        /**
         * Emits the newCriteriaLibrary object or a null value to the parent component
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.newCriteriaLibrary.owner = getUserName();
                this.$emit('submit', this.newCriteriaLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.newCriteriaLibrary = {...emptyCriteriaLibrary, id: ObjectID.generate()};
        }
    }
</script>