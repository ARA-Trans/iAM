<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Priority</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Priority" v-model="newPriority.priorityLevel" outline></v-text-field>

                        <v-text-field label="Year" v-model="newPriority.year" outline></v-text-field>
                    </v-layout>
                </v-card-text>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {emptyPriority, Priority} from "@/shared/models/iAM/priority";
    import {clone} from 'ramda';

    @Component
    export default class CreatePriorityDialog extends Vue {
        @Prop() showDialog: boolean;

        newPriority: Priority = clone(emptyPriority);

        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.newPriority);
            } else {
                this.$emit('submit', null);
            }

            this.newPriority = clone(emptyPriority);
        }
    }
</script>