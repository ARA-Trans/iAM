<template>
    <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Cash Flow Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" outline v-model="createdCashFlowLibrary.name"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                    <v-textarea label="Description" no-resize outline rows="3"
                                v-model="createdCashFlowLibrary.description"/>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="createdCashFlowLibrary.name === ''" @click="onSubmit(true)"
                           class="ara-blue-bg white--text">
                        Submit
                    </v-btn>
                    <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">
                        Cancel
                    </v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Prop, Watch} from 'vue-property-decorator';
    import {CreateCashFlowLibraryDialogData} from '@/shared/models/modals/create-cash-flow-library-dialog-data';
    import {
        CashFlowLibrary,
        emptyCashFlowLibrary,
        SplitTreatment,
        SplitTreatmentLimit
    } from '@/shared/models/iAM/cash-flow';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {getUserName} from '../../../shared/utils/get-user-info';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';
    import {clone} from 'ramda';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateCashFlowLibraryDialog extends Vue {
        @Prop() dialogData: CreateCashFlowLibraryDialogData;

        createdCashFlowLibrary: CashFlowLibrary = {...emptyCashFlowLibrary, id: ObjectID.generate()};
        rules: InputValidationRules = clone(rules);

        /**
         * Sets createdCashFlowLibrary class property using dialogData class property
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.createdCashFlowLibrary = {
                ...this.createdCashFlowLibrary,
                splitTreatments: this.dialogData.splitTreatments
            };
        }

        /**
         * Emits a new cash flow library object or null to the parent component and resets createdCashFlowLibrary
         * class property
         * @param submit Boolean
         */
        onSubmit(submit: boolean) {
            if (hasValue(this.createdCashFlowLibrary.splitTreatments)) {
                this.setIdsForNewLibrarySubData();
            }

            if (submit) {
                this.createdCashFlowLibrary.owner = getUserName();
                this.$emit('submit', this.createdCashFlowLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.createdCashFlowLibrary = {...emptyCashFlowLibrary, id: ObjectID.generate()};
        }

        /**
         * Generates bson ids for library's parameters & durations data
         */
        setIdsForNewLibrarySubData() {
            this.createdCashFlowLibrary.splitTreatments = this.createdCashFlowLibrary.splitTreatments
                .map((splitTreatment: SplitTreatment) => {
                    return {
                        ...splitTreatment,
                        id: ObjectID.generate(),
                        splitTreatmentLimits: splitTreatment.splitTreatmentLimits
                            .map((splitTreatmentLimit: SplitTreatmentLimit) => {
                                splitTreatmentLimit.id = ObjectID.generate();
                                return splitTreatmentLimit;
                            })
                    };
                });
        }
    }
</script>
