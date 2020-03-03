<template>
    <v-dialog v-model="dialogData.showDialog" persistent max-width="450px">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Deficient Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-text-field label="Name" v-model="newDeficientLibrary.name" outline></v-text-field>
                <v-textarea rows="3" no-resize outline label="Description"
                            v-model="newDeficientLibrary.description">
                </v-textarea>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)" :disabled="newDeficientLibrary.name === ''">
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
    import {clone} from 'ramda';
    import {CreateDeficientLibraryDialogData} from '@/shared/models/modals/create-deficient-library-dialog-data';
    import {Deficient, DeficientLibrary, emptyDeficientLibrary} from '@/shared/models/iAM/deficient';
    import { getUserName } from '../../../shared/utils/get-user-info';
    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateDeficientLibraryDialog extends Vue {
        @Prop() dialogData: CreateDeficientLibraryDialogData;

        newDeficientLibrary: DeficientLibrary = clone({...emptyDeficientLibrary, id: ObjectID.generate()});

        /**
         * Sets the newDeficientLibrary object's description & deficients properties with the dialogData object's
         * description & deficients properties
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.newDeficientLibrary = {
                ...this.newDeficientLibrary,
                description: this.dialogData.description,
                deficients: this.dialogData.deficients
            };
        }

        /**
         * Emits the newDeficientLibrary object or a null value to the parent component
         * @param submit Whether or not to emit the newDeficientLibrary object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.setIdsForNewDeficientLibrarySubData();
                this.newDeficientLibrary.owner = getUserName();
                this.$emit('submit', this.newDeficientLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.newDeficientLibrary = clone({...emptyDeficientLibrary, id: ObjectID.generate()});
        }

        /**
         * Generates bson ids for the new deficient library's deficients if there are any
         */
        setIdsForNewDeficientLibrarySubData() {
            this.newDeficientLibrary.deficients = this.newDeficientLibrary.deficients.map((deficient: Deficient) => ({
                ...deficient,
                id: ObjectID.generate()
            }));
        }
    }
</script>