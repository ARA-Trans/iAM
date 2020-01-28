<template>
    <v-dialog v-model="dialogData.showDialog" persistent max-width="450px">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Investment Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" v-model="newInvestmentLibrary.name" outline></v-text-field>
                    <v-flex xs4>
                        <v-layout justify-space-between>
                            <v-text-field label="Inflation Rate (%)" outline :mask="'##########'"
                                          v-model="newInvestmentLibrary.inflationRate">
                            </v-text-field>
                        </v-layout>
                    </v-flex>
                    <v-textarea rows="3" no-resize outline label="Description"
                                v-model="newInvestmentLibrary.description">
                    </v-textarea>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)"
                           :disabled="newInvestmentLibrary.name === ''">
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
    import {hasValue} from '@/shared/utils/has-value-util';
    import {CreateInvestmentLibraryDialogData} from '@/shared/models/modals/create-investment-library-dialog-data';
    import {
        emptyInvestmentLibrary,
        InvestmentLibrary,
        InvestmentLibraryBudgetYear
    } from '@/shared/models/iAM/investment';
    import {clone} from 'ramda';
    import {sortByProperty} from '@/shared/utils/sorter-utils';
    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateInvestmentStrategyDialog extends Vue {
        @Prop() dialogData: CreateInvestmentLibraryDialogData;

        newInvestmentLibrary: InvestmentLibrary = {...emptyInvestmentLibrary, id: ObjectID.generate()};

        /**
         * Sets the newInvestmentLibrary object's data properties using the dialogData object's data properties
         * if they have value
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            this.newInvestmentLibrary = {
                ...this.newInvestmentLibrary,
                inflationRate: this.dialogData.inflationRate,
                description: this.dialogData.description,
                budgetOrder: this.dialogData.budgetOrder,
                budgetYears: this.dialogData.budgetYears,
                budgetCriteria: this.dialogData.budgetCriteria
            };
        }

        /**
         * Emits the newInvestmentLibrary object or a null value to the parent component and resets the
         * newInvestmentLibrary object
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.setIdsForNewInvestmentLibrarySubData();
                this.$emit('submit', this.newInvestmentLibrary);
            } else {
                this.$emit('submit', null);
            }

            this.newInvestmentLibrary = {...emptyInvestmentLibrary, id: ObjectID.generate()};
        }

        /**
         * Sets the ids for the newInvestmentLibrary object's budgetYears
         */
        setIdsForNewInvestmentLibrarySubData() {
            this.newInvestmentLibrary.budgetYears = this.newInvestmentLibrary.budgetYears
                .map((budgetYear: InvestmentLibraryBudgetYear) => ({...budgetYear, id: ObjectID.generate()}));
        }
    }
</script>