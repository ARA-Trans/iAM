<template>
    <v-layout>
        <v-dialog v-model="dialogData.showDialog" persistent scrollable max-width="700px">
            <v-card class="equation-container-card">
                <v-card-title class="card-title">
                    <v-layout justify-center fill-height>
                        <h3>Equation Editor</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-flex xs12>
                            <v-layout justify-space-between row fill-height>
                                <v-flex xs5>
                                    <v-card class="list-card">
                                        <v-list>
                                            <v-list-tile v-for="attribute in attributes" :key="attribute" class="list-tile"
                                                         ripple @click="onAddAttributeToEquation(attribute)">
                                                <v-list-tile-content>
                                                    <v-list-tile-title>{{attribute}}</v-list-tile-title>
                                                </v-list-tile-content>
                                            </v-list-tile>
                                        </v-list>
                                    </v-card>
                                </v-flex>

                                <v-flex xs5>
                                    <v-card class="list-card">
                                        <v-list>
                                            <v-list-tile v-for="formula in formulas" :key="formula" class="list-tile"
                                                         ripple @click="onAddFormulaToEquation(formula)">
                                                <v-list-tile-content>
                                                    <v-list-tile-title>{{formula}}</v-list-tile-title>
                                                </v-list-tile-content>
                                            </v-list-tile>
                                        </v-list>
                                    </v-card>
                                </v-flex>
                            </v-layout>
                        </v-flex>
                        <v-flex xs12>
                            <v-layout justify-center fill-height>
                                <v-flex xs6>
                                    <v-layout justify-space-between row fill-height>
                                        <v-btn class="math-button add" fab small v-on:click="onAddStringToEquation('+')">
                                            <span>+</span>
                                        </v-btn>
                                        <v-btn class="math-button subtract" fab small v-on:click="onAddStringToEquation('-')">
                                            <span>-</span>
                                        </v-btn>
                                        <v-btn class="math-button multiply" fab small v-on:click="onAddStringToEquation('*')">
                                            <span>*</span>
                                        </v-btn>
                                        <v-btn class="math-button divide" fab small v-on:click="onAddStringToEquation('/')">
                                            <span>/</span>
                                        </v-btn>
                                        <v-btn class="math-button parentheses" fab small v-on:click="onAddStringToEquation('(')">
                                            <span>(</span>
                                        </v-btn>
                                        <v-btn class="math-button parentheses" fab small v-on:click="onAddStringToEquation(')')">
                                            <span>)</span>
                                        </v-btn>
                                    </v-layout>
                                </v-flex>
                            </v-layout>
                        </v-flex>
                        <v-flex xs12>
                            <v-layout justify-center fill-height>
                                <v-flex xs11>
                                    <div>
                                        <v-layout justify-center fill-height>
                                            <v-flex xs5>
                                                <v-layout justify-space-between row fill-height>
                                                    <v-checkbox label="Is piecewise?" v-model="isPiecewise"></v-checkbox>
                                                    <v-checkbox class="right-checkbox" label="Is function?" v-model="isFunction">
                                                    </v-checkbox>
                                                </v-layout>
                                            </v-flex>
                                        </v-layout>
                                    </div>
                                    <v-textarea id="equation_textarea" rows="5" outline full-width no-resize spellcheck="false"
                                                v-model="equation" v-on:blur="setCursorPosition" v-on:focus="setTextareaCursorPosition">
                                    </v-textarea>
                                </v-flex>
                            </v-layout>
                        </v-flex>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn v-on:click="onCancel">Cancel</v-btn>
                        <v-btn color="info" v-on:click="onSubmit">Submit</v-btn>
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
    import {EquationEditorDialogData} from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-data';
    import {hasValue} from '@/shared/utils/has-value';
    import {EquationEditorDialogResult} from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-result';

    @Component
    export default class EquationEditor extends Vue {
        @Prop() dialogData: EquationEditorDialogData;

        @State(state => state.attribute.attributes) attributes: string[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getAttributes') getAttributesAction: any;

        equation: string = '';
        isPiecewise: boolean = false;
        isFunction: boolean = false;
        formulas: string[] = [
            'Abs()',
            'Acos()',
            'Asin()',
            'Atan()',
            'Atan(,)',
            'Ceiling()',
            'Cos()',
            'Cosh()',
            'E',
            'Exp()',
            'Floor()',
            'IEEERemain(,)',
            'Log()',
            'Log10()',
            'Max(,)',
            'Min(,)',
            'PI',
            'Pow(,)',
            'Round()',
            'Sign()',
            'Sin()',
            'Sinh()',
            'Sqrt()',
            'Tan()',
            'Tanh()',

        ];
        cursorPosition: number = 0;
        textareaInput: HTMLTextAreaElement = null;

        @Watch('dialogData')
        onDialogDataChanged() {
            // set the equation, isPiecewise, and isFunction properties with the dialog data equation
            this.equation = this.dialogData.equation;
            this.isPiecewise = this.dialogData.isPiecewise;
            this.isFunction = this.dialogData.isFunction;
            // if the dialog is shown but there are no attributes in state...
            if (this.dialogData.showDialog && !hasValue(this.attributes)) {
                // dispatch isBusy action and dispatch getAttributes action
                this.setIsBusyAction({isBusy: true});
                this.getAttributesAction()
                    .then(() => this.setIsBusyAction({isBusy: false}))
                    .catch((error: any) => {
                        this.setIsBusyAction({isBusy: false});
                        console.log(error);
                    });
            }
        }

        mounted() {
            this.textareaInput = document.getElementById('equation_textarea') as HTMLTextAreaElement;
            this.cursorPosition = this.textareaInput.selectionStart;
        }

        /**
         * Sets cursor position property when the equation textarea element loses focus
         */
        setCursorPosition() {
            this.cursorPosition = this.textareaInput.selectionStart;
        }

        /**
         * One of the attribute list items in the list of attributes has been clicked
         */
        onAddAttributeToEquation(attribute: string) {
            this.onAddStringToEquation(`[${attribute}]`);
        }

        /**
         * One of the formula list items in the list of formulas has been clicked
         * @param formula The formula string to add to the equation string
         */
        onAddFormulaToEquation(formula: string) {
            if (this.cursorPosition === 0) {
                this.equation = `${formula}${this.equation}`;
                this.cursorPosition = formula !== 'E' && formula !== 'PI'
                    ? formula.indexOf('(') + 1
                    : formula.length;
            } else if (this.equation.length === this.cursorPosition) {
                this.equation = `${this.equation}${formula}`;
                if (formula !== 'E' && formula !== 'PI') {
                    let i = this.equation.length;
                    while (this.equation.charAt(i) !== '(') {
                        i--;
                    }
                    this.cursorPosition = i + 1;
                } else {
                    this.cursorPosition = this.equation.length;
                }
            } else {
                const output = `${this.equation.substr(0, this.cursorPosition)}${formula}`;
                this.equation = `${output}${this.equation.substr(this.cursorPosition)}`;
                if (formula !== 'E' && formula !== 'PI') {
                    let i = output.length;
                    while (output.charAt(i) !== '(') {
                        i--;
                    }
                    this.cursorPosition = i + 1;
                } else {
                    this.cursorPosition = output.length;
                }
            }
            this.textareaInput.focus();
        }

        /**
         * User has clicked on an operator or parentheses button to add to the equation
         * @param value The string value to add to the equation string
         */
        onAddStringToEquation(value: string) {
            if (this.cursorPosition === 0) {
                this.cursorPosition = value.length;
                this.equation = `${value}${this.equation}`;
            } else if (this.equation.length === this.cursorPosition) {
                this.equation = `${this.equation}${value}`;
                this.cursorPosition = this.equation.length;
            } else {
                const output = `${this.equation.substr(0, this.cursorPosition)}${value}`;
                this.equation = `${output}${this.equation.substr(this.cursorPosition)}`;
                this.cursorPosition = output.length;
            }
            this.textareaInput.focus();
        }

        /**
         * Sets the cursor position of the equation textarea element using the cursorPosition property
         */
        setTextareaCursorPosition() {
            setTimeout(() =>
                this.textareaInput.setSelectionRange(this.cursorPosition, this.cursorPosition)
            );
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            const result: EquationEditorDialogResult = {
                equation: this.equation,
                isPiecewise: this.isPiecewise,
                isFunction: this.isFunction
            };
            this.$emit('submit', result);
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {
            // submit a null value
            this.$emit('submit', null);
        }
    }
</script>

<style>
    .equation-container-card {
        height: 768px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .card-title {
        max-height: 60px;
    }

    .list-card {
        height: 300px;
        border: 1px solid black !important;
        margin: 10px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .list-tile {
        cursor: pointer;
    }

    .math-button {
        border: 1px solid black;
        font-size: 1.5em;
    }

    .parentheses {
        font-size: 1.25em;
    }

    .add, .divide {
        font-size: 1.5em;
    }

    .multiply {
        font-size: 1.75em;
    }

    .subtract {
        font-size: 2em;
    }

    .right-checkbox {
        margin-left: 40px;
    }
</style>