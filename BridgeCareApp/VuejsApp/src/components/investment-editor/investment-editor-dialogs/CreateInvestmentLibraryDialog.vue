<template>
    <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Investment Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" outline v-model="newInvestmentLibrary.name"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                    <v-flex xs4>
                        <v-layout justify-space-between>
                            <v-text-field :mask="'##########'" label="Inflation Rate (%)" outline
                                          v-model="newInvestmentLibrary.inflationRate"
                                          :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                        </v-layout>
                    </v-flex>
                    <v-textarea label="Description" no-resize outline rows="3"
                                v-model="newInvestmentLibrary.description">
                    </v-textarea>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="newInvestmentLibrary.name === ''" @click="onSubmit(true)"
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
    import {CreateInvestmentLibraryDialogData} from '@/shared/models/modals/create-investment-library-dialog-data';
    import {
        emptyInvestmentLibrary,
        InvestmentLibrary,
        InvestmentLibraryBudgetYear
    } from '@/shared/models/iAM/investment';
    import {getUserName} from '../../../shared/utils/get-user-info';
    import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';
    import {clone} from 'ramda';

    const ObjectID = require('bson-objectid');

    @Component
    export default class CreateInvestmentStrategyDialog extends Vue {
        @Prop() dialogData: CreateInvestmentLibraryDialogData;

        newInvestmentLibrary: InvestmentLibrary = {...emptyInvestmentLibrary, id: ObjectID.generate()};
        rules: InputValidationRules = clone(rules);

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
                this.newInvestmentLibrary.owner = getUserName();
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
