<template>
    <v-dialog v-model="dialogData.showDialog" persistent max-width="450px">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Investment Strategy Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" v-model="createdInvestmentStrategy.name" outline></v-text-field>
                    <v-flex xs4>
                        <v-layout justify-space-between>
                            <v-text-field label="Inflation Rate (%)" outline :mask="'##########'"
                                          v-model="createdInvestmentStrategy.inflationRate">
                            </v-text-field>
                            <v-text-field label="Discount Rate (%)" outline :mask="'##########'"
                                          v-model="createdInvestmentStrategy.discountRate">
                            </v-text-field>
                        </v-layout>
                    </v-flex>
                    <v-textarea rows="3" no-resize outline label="Description"
                                v-model="createdInvestmentStrategy.description">
                    </v-textarea>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)"
                           :disabled="createdInvestmentStrategy.name === ''">
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
    import {hasValue} from '@/shared/utils/has-value-util';
    import {CreateInvestmentLibraryDialogData} from '@/shared/models/modals/create-investment-library-dialog-data';
    import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';
    import {clone} from 'ramda';

    @Component
    export default class CreateInvestmentStrategyDialog extends Vue {
        @Prop() dialogData: CreateInvestmentLibraryDialogData;

        createdInvestmentStrategy: InvestmentLibrary = clone(emptyInvestmentLibrary);

        /**
         * Sets the createdInvestmentStrategy object's data properties using the dialogData object's data properties
         * if they have value
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.createdInvestmentStrategy = {
                ...this.createdInvestmentStrategy,
                inflationRate: hasValue(this.dialogData.inflationRate) ? this.dialogData.inflationRate : 0,
                discountRate: hasValue(this.dialogData.discountRate) ? this.dialogData.discountRate : 0,
                description: hasValue(this.dialogData.description) ? this.dialogData.description : '',
                budgetOrder: hasValue(this.dialogData.budgetOrder) ? this.dialogData.budgetOrder : [],
                budgetYears: hasValue(this.dialogData.budgetYears) ? this.dialogData.budgetYears : []
            };
        }

        /**
         * Emits the createdInvestmentStrategy object or a null value to the parent component and resets the
         * createdInvestmentStrategy object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.createdInvestmentStrategy);
            } else {
                this.$emit('submit', null);
            }

            this.createdInvestmentStrategy = clone(emptyInvestmentLibrary);
        }
    }
</script>