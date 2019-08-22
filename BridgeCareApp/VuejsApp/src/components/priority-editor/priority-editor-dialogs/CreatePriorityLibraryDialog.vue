<template>
    <v-dialog v-model="dialogData.showDialog" persistent max-width="450px">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Priority Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" v-model="createdPriorityLibrary.name" outline></v-text-field>
                    <v-textarea rows="3" no-resize outline label="Description"
                                v-model="createdPriorityLibrary.description">
                    </v-textarea>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)"
                           :disabled="createdPriorityLibrary.name === ''">
                        Submit
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
    import {CreatePriorityLibraryDialogData} from '@/shared/models/modals/create-priority-library-dialog-data';
    import {emptyPriorityLibrary, PriorityLibrary} from '@/shared/models/iAM/priority';
    import {hasValue} from '@/shared/utils/has-value-util';
    const ObjectID = require('bson-objectid');

    @Component
    export default class CreatePriorityLibraryDialog extends Vue {
        @Prop() dialogData: CreatePriorityLibraryDialogData;

        createdPriorityLibrary: PriorityLibrary = {...emptyPriorityLibrary, id: ObjectID.generate()};

        /**
         * Sets the createdPriorityLibrary object's data properties using the dialogData object's data properties, if they
         * have a value
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.createdPriorityLibrary = {
                ...this.createdPriorityLibrary,
                description: hasValue(this.dialogData.description) ? this.dialogData.description : '',
                priorities: hasValue(this.dialogData.priorities) ? this.dialogData.priorities : []
            };
        }

        /**
         * Emits the createdPriorityLibrary object or a null value to the parent component and resets the createdPriorityLibrary
         * object
         * @param submit Flags that determines if the createdPriorityLibrary is emitted back to parent component
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.createdPriorityLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.createdPriorityLibrary = {...emptyPriorityLibrary, id: ObjectID.generate()};
        }
    }
</script>