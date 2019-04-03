<template>
    <v-layout>
        <v-dialog v-model="dialogData.showDialog" persistent scrollable max-width="700px">
            <v-layout column fill-height>
                <v-flex xs12>
                    <v-layout justify-space-between row fill-height>
                        <v-flex xs5>
                            <v-card>
                                <v-list>
                                    <v-list-tile v-for="attribute in attributes" @dblclick="onAddAttributeToEquation">
                                        <v-list-tile-content>
                                            <v-list-tile-title>{{attribute}}</v-list-tile-title>
                                        </v-list-tile-content>
                                    </v-list-tile>
                                </v-list>
                            </v-card>
                        </v-flex>

                        <v-flex xs5>
                            <v-list>
                                <v-list-tile v-for="formula in formulas" @dblclick="onAddFormulaToEquation">
                                    <v-list-content>
                                        <v-list-tile-title>{{formula}}</v-list-tile-title>
                                    </v-list-content>
                                </v-list-tile>
                            </v-list>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-flex xs12>
                    <v-layout justify-space-between row fill-height>
                        <v-flex xs5>
                            <v-layout justify-space-between row fill-height>
                                <v-btn>+</v-btn>
                                <v-btn>-</v-btn>
                                <v-btn>*</v-btn>
                                <v-btn>/</v-btn>
                            </v-layout>
                        </v-flex>

                        <v-flex xs5>

                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {EquationEditorDialogData} from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-data';
    import {hasValue} from '@/shared/utils/has-value';

    @Component
    export default class EquationEditor extends Vue {
        @Prop() dialogData: EquationEditorDialogData;

        @State(state => state.attribute.attributes) attributes: string[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getAttributes') getAttributesAction: any;

        @Watch('dialogData')
        onDialogDataChanged() {
            if (this.dialogData.showDialog && !hasValue(this.attributes)) {
                this.setIsBusyAction({isBusy: true});
                this.getAttributesAction()
                    .then(() => this.setIsBusyAction({isBusy: false}))
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
        }

        formulas: string[] = [
            'Abs(x)',
            'Acos(x)',
            'Asin(x)',
            'Atan(x)',
            'Atan(x,y)',
            'Ceiling(x)',
            'Cos(x)',
            'Cosh(x)',
            'E',
            'Exp(x)',
            'Floor(x)',
            'IEEERemain(x,y)',
            'Log(x)',
            'Log10(x)',
            'Max(x,y)',
            'Min(x,y)',
            'PI',
            'Pow(x,y)',
            'Round(x)',
            'Sign(x)',
            'Sin(x)',
            'Sinh(x)',
            'Sqrt(x)',
            'Tan(x)',
            'Tanh(x)',

        ];

        /**
         *
         * @param attribute
         */
        onAddAttributeToEquation(attribute: string) {

        }

        onAddFormulaToEquation(formula: string) {

        }
    }
</script>