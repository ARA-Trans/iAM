<template>
    <input class="cost-output" :id="durationId" type="text" value="" readonly pattern="^\$\d{1,3}(,\d{3})*(\.\d+)?$" />
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {hasValue} from '@/shared/utils/has-value-util';

    const currencyPattern = /^\$\d{1,3}(,\d{3})*(\.\d+)?$/;
    @Component
    export default class CashFlowEditorMaxTreatmentCostOutput extends Vue {
        @Prop() maxTreatmentCost: string;
        @Prop() durationId: string;

        inputElement: HTMLInputElement = {} as HTMLInputElement;

        mounted() {
            this.inputElement = document.getElementById(this.durationId) as HTMLInputElement;
            this.formatCurrency();
        }

        @Watch('maxTreatmentCost')
        onMaxTreatmentCostChanged() {
            this.formatCurrency();
        }

        // appends $ to value, validates decimal side
        // and puts cursor back in right position.
        formatCurrency() {
            // don't validate empty input
            if (hasValue(this.maxTreatmentCost)) {
                // check for decimal
                if (this.maxTreatmentCost.indexOf('.') >= 0) {
                    // get position of first decimal
                    // this prevents multiple decimals from
                    // being entered
                    const decimalPosition = this.maxTreatmentCost.indexOf('.');

                    // split number by decimal point
                    let leftSide = this.maxTreatmentCost.substring(0, decimalPosition);
                    let rightSide = this.maxTreatmentCost.substring(decimalPosition);

                    // add commas to left side of number
                    leftSide = this.formatNumber(leftSide);

                    // validate right side
                    rightSide = `${this.formatNumber(rightSide)}00`.substring(0, 2);

                    // join number by .
                    this.inputElement.value = `$${leftSide}.${rightSide}`;
                } else {
                    // no decimal entered
                    // add commas to number
                    // remove all non-digits
                    this.inputElement.value = `$${this.formatNumber(this.maxTreatmentCost)}.00`;
                }
            }
        }

        formatNumber(cost: string) {
            // format number 1000000 to 1,234,567
            return cost.replace(/\D/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        }
    }
</script>

<style>
    .cost-output {
        border-bottom: 1px solid;
    }
</style>