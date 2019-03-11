<template>

</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {Network} from '@/shared/models/iAM/network';
    import {Simulation} from '@/shared/models/iAM/simulation';
    import {InvestmentStrategy} from '@/shared/models/iAM/investment';

    @Component
    export default class CreateInvestmentStrategyDialog extends Vue  {
        @Prop()
        showDialog: boolean;

        @State(state => state.network.networks) networks: Network[];
        @State(state => state.simulation.simulations) simulations: Simulation[];
        @State(state => state.investmentEditor.investmentStrategies) investmentStrategies: InvestmentStrategy[];

        onSelectNetwork() {}

        onCreateInvestmentStrategy() {}

        onCancel() {}
    }
</script>