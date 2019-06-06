<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Target</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="newTarget.name" outline></v-text-field>

                        <v-select-list label="Select Attribute" :items="numericAttributes"
                                       v-model="newTarget.attribute" outline>
                        </v-select-list>

                        <v-text-field label="Year" v-model="newTarget.year" outline></v-text-field>

                        <v-text-field label="Target" v-model="newTarget.targetMean" outline>
                        </v-text-field>
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
    import {Target, emptyTarget} from '@/shared/models/iAM/target';
    import {clone} from 'ramda';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {getPropertyValues} from "@/shared/utils/getter-utils";
    import {hasValue} from "@/shared/utils/has-value-util";

    @Component
    export default class CreateTargetDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        newTarget: Target = clone(emptyTarget);
        numericAttributes: string[] = [];

        mounted() {
            if (hasValue(this.stateNumericAttributes)) {
                this.numericAttributes = getPropertyValues('name', this.stateNumericAttributes);
            }
        }

        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            if (hasValue(this.stateNumericAttributes)) {
                this.numericAttributes = getPropertyValues('name', this.stateNumericAttributes);
            }
        }

        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.newTarget);
            } else {
                this.$emit('submit', null);
            }

            this.newTarget = clone(emptyTarget);
        }
    }
</script>