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

                        <v-select-list label="Select Attribute" :items="numericAttributes"
                                       v-model="newDeficient.attribute" outline>
                        </v-select-list>

                        <v-text-field label="Deficient Level" v-model="newDeficient.deficient" outline></v-text-field>

                        <v-text-field label="Allowed Deficient(%)" v-model="newDeficient.percentDeficient" outline>
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
    import {Deficient, emptyDeficient} from '@/shared/models/iAM/deficient';
    import {clone} from 'ramda';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {getPropertyValues} from "@/shared/utils/getter-utils";
    import {hasValue} from "@/shared/utils/has-value-util";

    @Component
    export default class CreateDeficientDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        newDeficient: Deficient = clone(emptyDeficient);
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
                this.$emit('submit', this.newDeficient);
            } else {
                this.$emit('submit', null);
            }

            this.newDeficient = clone(emptyDeficient);
        }
    }
</script>