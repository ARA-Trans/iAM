<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Deficient</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="newDeficient.name" outline></v-text-field>
                        <v-select-list :items="numericAttributes" v-model="newDeficient.attribute"></v-select-list>
                    </v-layout>
                </v-card-text>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State} from 'vuex-class';
    import {Deficient, emptyDeficient} from '@/shared/models/iAM/deficient';
    import {clone} from 'ramda';

    @Component
    export default class CreateDeficientDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[]

        newDeficient: Deficient = clone(emptyDeficient);

        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.newDeficient);
            } else {
                this.$emit('submit', null);
            }

            this.newDeficient = clone(emptyDeficient);
        }
    }
</script>