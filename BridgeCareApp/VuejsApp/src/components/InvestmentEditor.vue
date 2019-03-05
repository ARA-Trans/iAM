<template>
    <v-container fluid grid-list-xl>
        <v-layout justify-center>
            <v-flex xsl-6>
                <v-select :items="investmentStrategiesSelect"
                          label="Select an Investment Strategy"
                          item-text="text"
                          item-value="value"
                          v-on:change="onSelectInvestmentStrategy">
                </v-select>
            </v-flex>
        </v-layout>
        <v-divider></v-divider>
        <v-layout column justify-center>
            <v-flex xsl-6>
                <v-layout>
                    <v-flex xsl-3>
                        Inflation Rate (%):
                    </v-flex>
                    <v-flex xsl-3>
                        <v-input v-model="selectedInvestmentStrategy.inflationRate"></v-input>
                    </v-flex>
                </v-layout>
            </v-flex>
            <v-flex xsl-6>
                <v-layout>
                    <v-flex xsl-3>
                        Discount Rate (%):
                    </v-flex>
                    <v-flex xsl-3>
                        <v-input v-model="selectedInvestmentStrategy.discountRate"></v-input>
                    </v-flex>
                </v-layout>
            </v-flex>

            <v-flex xsl-6>
                <v-data-table>

                </v-data-table>
            </v-flex>
        </v-layout>
        <v-divider></v-divider>
        <v-layout justify-center>
            <v-flex xsl-6>

            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';

    import AppSpinner from '../shared/AppSpinner.vue';
    import {InvestmentStrategy, emptyInvestmentStrategy, InvestmentStrategyBudgetYear} from "@/models/iAM/investment";
    import {emptyNetwork} from '@/models/iAM/network';
    import * as R from 'ramda';
    import {VueSelect, vueSelectDefault} from '@/models/vue/vue-select';

    @Component({
        components: {AppSpinner}
    })
    export default class InvestmentEditor extends Vue {
        @State(state => state.investmentEditor.investmentStrategies) investmentStrategies: InvestmentStrategy[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getInvestmentStrategies') getInvestmentStrategiesAction: any;
        @Action('getInvestmentStrategyDetail') getInvestmentStrategyDetailAction: any;

        investmentStrategiesSelect: VueSelect[] = [vueSelectDefault];
        selectedInvestmentStrategy: InvestmentStrategy = emptyInvestmentStrategy;

        @Watch('investmentStrategies')
        onInvestmentStrategiesChanged(investmentStrategies: InvestmentStrategy[]) {
            this.investmentStrategiesSelect = investmentStrategies.map((investmentStrategy: InvestmentStrategy) => (
                {
                    text: investmentStrategy.name,
                    value: investmentStrategy.id.toString()
                }
            ));
        }

        mounted() {
            this.setIsBusyAction({isBusy: true});
            this.getInvestmentStrategiesAction({network: emptyNetwork})
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                })
        }

        onSelectInvestmentStrategy(id: number) {
            this.selectedInvestmentStrategy = this.investmentStrategies
                .find((investmentStrategy: InvestmentStrategy) => investmentStrategy.id === id) as InvestmentStrategy;
            this.selectedInvestmentStrategy.budgetYears = R.sortBy(
                R.prop('year'), this.selectedInvestmentStrategy.budgetYears
            );
        }

        onAddBudgetYear() {
            const latestYear = () => {
                const years = this.selectedInvestmentStrategy.budgetYears
                    .map((budgetYear: InvestmentStrategyBudgetYear) => budgetYear.year);
                const sortedYears = R.sort((year1: number, year2: number) => year1 - year2, [...years]);
                return R.last(sortedYears);
            }
        }
    }
</script>