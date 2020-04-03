<template>
    <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Performance Library Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" outline v-model="newPerformanceLibrary.name"></v-text-field>
                    <v-textarea label="Description" no-resize outline rows="3"
                                v-model="newPerformanceLibrary.description">
                    </v-textarea>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="newPerformanceLibrary.name === ''" @click="onSubmit(true)"
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
    import {
        emptyPerformanceLibrary,
        PerformanceLibrary,
        PerformanceLibraryEquation
    } from '@/shared/models/iAM/performance';
    import {CreatePerformanceLibraryDialogData} from '@/shared/models/modals/create-performance-library-dialog-data';
    import {getUserName} from '@/shared/utils/get-user-info';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreatePerformanceLibraryDialog extends Vue {
        @Prop() dialogData: CreatePerformanceLibraryDialogData;

        newPerformanceLibrary: PerformanceLibrary = {...emptyPerformanceLibrary, id: ObjectID.generate()};

        /**
         * Sets the newPerformanceLibrary's description & equation data properties using the dialogData object
         */
        @Watch('dialogData')
        onSelectedPerformanceLibraryChanged() {
            this.newPerformanceLibrary = {
                ...this.newPerformanceLibrary,
                description: this.dialogData.description,
                equations: this.dialogData.equations
            };
        }

        /**
         * Emits the newPerformanceLibrary object or a null value to the parent component and resets the
         * newPerformanceLibrary object
         * @param submit Whether or not to emit the newPerformanceLibrary object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.newPerformanceLibrary.owner = getUserName();
                this.setIdsForNewPerformanceLibrarySubData();
                this.$emit('submit', this.newPerformanceLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.newPerformanceLibrary = {...emptyPerformanceLibrary, id: ObjectID.generate()};
        }

        /**
         * Sets the ids for the newPerformanceLibrary object's equations
         */
        setIdsForNewPerformanceLibrarySubData() {
            this.newPerformanceLibrary.equations = this.newPerformanceLibrary.equations
                .map((equation: PerformanceLibraryEquation) => ({...equation, id: ObjectID.generate()}));
        }
    }
</script>
