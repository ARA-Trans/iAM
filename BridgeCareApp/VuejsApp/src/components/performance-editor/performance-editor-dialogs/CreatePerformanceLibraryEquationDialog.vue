<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="250px">
            <v-card>
                <v-card-title>
                    <v-layout justify-center fill-height>
                        <h3>New Equation</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name" v-model="createdPerformanceLibraryEquation.equationName" outline>
                        </v-text-field>
                        <v-select label="Select Attribute" :items="attributesSelectListItems"
                                  v-model="createdPerformanceLibraryEquation.attribute" outline>
                        </v-select>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit"
                               :disabled="createdPerformanceLibraryEquation.equationName === '' ||
                                          createdPerformanceLibraryEquation.attribute === ''">
                            Submit
                        </v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {emptyEquation, PerformanceLibraryEquation} from '@/shared/models/iAM/performance';
    import {isEmpty} from 'ramda';
    import {SelectItem} from '@/shared/models/vue/select-item';

    @Component
    export default class CreatePerformanceLibraryDialog extends Vue {
        @Prop() showDialog: boolean;

        @State(state => state.attribute.attributes) attributes: string[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getAttributes') getAttributesAction: any;

        attributesSelectListItems: SelectItem[] = [];
        createdPerformanceLibraryEquation: PerformanceLibraryEquation = {...emptyEquation};

        /**
         * Watcher: showDialog
         */
        @Watch('showDialog')
        onShowDialogChanged() {
            if (this.showDialog && isEmpty(this.attributes)) {
                // set isBusy to true, then dispatch action to get attributes
                this.setIsBusyAction({isBusy: true});
                this.getAttributesAction()
                    .then(() => this.setIsBusyAction({isBusy: false}))
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
        }

        /**
         * Watcher: attributes
         */
        @Watch('attributes')
        onAttributesChanged() {
            if (!isEmpty(this.attributes)) {
                // set the attributesSelectListItems property using the list of attributes
                this.attributesSelectListItems = this.attributes.map((attribute: string) => ({
                    text: attribute,
                    value: attribute
                }));
            }
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            this.$emit('submit', this.createdPerformanceLibraryEquation);
            this.createdPerformanceLibraryEquation = {...emptyEquation};
        }

        /**
         * One of the 'Cancel' buttons has been clicked
         */
        onCancel() {
            this.$emit('submit', null);
            this.createdPerformanceLibraryEquation = {...emptyEquation};
        }
    }
</script>