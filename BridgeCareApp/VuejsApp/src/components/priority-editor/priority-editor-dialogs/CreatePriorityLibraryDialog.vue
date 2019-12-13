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
                    <v-text-field label="Name" v-model="newPriorityLibrary.name" outline></v-text-field>
                    <v-textarea rows="3" no-resize outline label="Description"
                                v-model="newPriorityLibrary.description">
                    </v-textarea>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)"
                           :disabled="newPriorityLibrary.name === ''">
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
    import {CreatePriorityLibraryDialogData} from '@/shared/models/modals/create-priority-library-dialog-data';
    import {
        emptyPriority,
        emptyPriorityLibrary,
        Priority,
        PriorityFund,
        PriorityLibrary
    } from '@/shared/models/iAM/priority';
    import {clone} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';
    import moment from 'moment';
    const ObjectID = require('bson-objectid');

    @Component
    export default class CreatePriorityLibraryDialog extends Vue {
        @Prop() dialogData: CreatePriorityLibraryDialogData;

        newPriorityLibrary: PriorityLibrary = clone({...emptyPriorityLibrary, id: ObjectID.generate()});

        /**
         * Sets the newPriorityLibrary object's description & priorities properties with the dialogData object's
         * description & priority properties
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.newPriorityLibrary = {
                ...this.newPriorityLibrary,
                description: this.dialogData.description,
                priorities: this.dialogData.priorities
            };
        }

        /**
         * Emits the newPriorityLibrary object or a null value to the parent component
         * @param submit Whether or not to emit the newPriorityLibrary object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                if (!hasValue(this.newPriorityLibrary.priorities)) {
                    this.newPriorityLibrary.priorities.push(clone({...emptyPriority, year: moment().year()}));
                }
                this.setIdsForNewPriorityLibrarySubData();
                this.$emit('submit', this.newPriorityLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.newPriorityLibrary = clone({...emptyPriorityLibrary, id: ObjectID.generate()});
        }

        /**
         * Generates bson ids for the new priority library's priorities & priority funds if there are any
         */
        setIdsForNewPriorityLibrarySubData() {
            this.newPriorityLibrary.priorities = this.newPriorityLibrary.priorities.map((priority: Priority) => ({
                ...priority,
                id: ObjectID.generate(),
                priorityFunds: priority.priorityFunds.map((priorityFund: PriorityFund) => ({
                    ...priorityFund,
                    id: ObjectID.generate()
                }))
            }));
        }
    }
</script>