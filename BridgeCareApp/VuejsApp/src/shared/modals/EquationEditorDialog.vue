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
                                    <v-card>
                                        <v-card-title>Attributes: Click once to add</v-card-title>
                                        <v-card-text class="list-card-text">
                                            <v-list>
                                                <v-list-tile v-for="attribute in attributesList" :key="attribute" class="list-tile"
                                                             ripple @click="onAddAttributeToEquation(attribute)">
                                                    <v-list-tile-content>
                                                        <v-list-tile-title>{{attribute}}</v-list-tile-title>
                                                    </v-list-tile-content>
                                                </v-list-tile>
                                            </v-list>
                                        </v-card-text>
                                    </v-card>
                                </v-flex>

                                <v-flex xs5>
                                    <v-card>
                                        <v-card-title>Formulas: Click once to add</v-card-title>
                                        <v-card-text class="list-card-text">
                                            <v-list>
                                                <v-list-tile v-for="formula in formulasList" :key="formula" class="list-tile"
                                                             ripple @click="onAddFormulaToEquation(formula)">
                                                    <v-list-tile-content>
                                                        <v-list-tile-title>{{formula}}</v-list-tile-title>
                                                    </v-list-tile-content>
                                                </v-list-tile>
                                            </v-list>
                                        </v-card-text>
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
                                                    <v-checkbox v-show="dialogData.canBePiecewise" label="Is piecewise?"
                                                                v-model="isPiecewise">
                                                    </v-checkbox>
                                                    <v-checkbox class="right-checkbox" label="Is function?" v-model="isFunction">
                                                    </v-checkbox>
                                                </v-layout>
                                            </v-flex>
                                        </v-layout>
                                    </div>
                                    <v-textarea id="equation_textarea" rows="5" outline full-width no-resize spellcheck="false"
                                                v-model="equation" v-on:blur="setCursorPosition" v-on:focus="setTextareaCursorPosition">
                                    </v-textarea>
                                    <div class="validation-message-div">
                                        <v-layout justify-end fill-height>
                                            <p class="invalid-message" v-if="showInvalidMessage">{{invalidMessage}}</p>
                                            <p class="valid-message" v-if="showValidMessage">Equation is valid</p>
                                        </v-layout>
                                    </div>
                                </v-flex>
                            </v-layout>
                        </v-flex>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-spacer></v-spacer>
                        <v-flex xs2>
                            <v-layout justify-end row fill-height>
                                <v-btn color="info lighten-1" v-on:click="onCheckEquation">Check</v-btn>
                                <v-btn color="info" v-on:click="onSubmit" :disabled="cannotSubmit">Submit</v-btn>
                            </v-layout>
                        </v-flex>
                        <v-spacer></v-spacer>
                        <v-flex xs1>
                            <v-btn color="error" v-on:click="onCancel">Cancel</v-btn>
                        </v-flex>
                        <v-spacer></v-spacer>
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
    import {EquationEditorDialogData} from '@/shared/models/modals/equation-editor-dialog-data';
    import {EquationEditorDialogResult} from '@/shared/models/modals/equation-editor-dialog-result';
    import EquationEditorService from '@/services/equation-editor.service';
    import {formulas} from '@/shared/utils/formulas';
    import {isEmpty} from 'ramda';
    import {AxiosResponse} from 'axios';
    import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {EquationValidation} from '@/shared/models/iAM/equation-validation';

    @Component
    export default class EquationEditorDialog extends Vue {
        @Prop() dialogData: EquationEditorDialogData;

        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('getAttributes') getAttributesAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        attributesList: string[] = [];
        formulasList: string[] = formulas;
        equation: string = '';
        isPiecewise: boolean = false;
        isFunction: boolean = false;
        textareaInput: HTMLTextAreaElement = {} as HTMLTextAreaElement;
        cursorPosition: number = 0;
        showInvalidMessage: boolean = false;
        showValidMessage: boolean = false;
        cannotSubmit: boolean = false;
        invalidMessage: string = '';

        /**
         * Component mounted event handler
         */
        mounted() {
            this.textareaInput = document.getElementById('equation_textarea') as HTMLTextAreaElement;
            this.cursorPosition = this.textareaInput.selectionStart;
            if (hasValue(this.stateNumericAttributes)) {
                this.setAttributesList();
            }
        }

        /**
         * Sets equation UI properties dialogData has changed
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            // set the equation, isPiecewise, and isFunction properties with the dialog data equation
            this.equation = this.dialogData.equation;
            this.isPiecewise = this.dialogData.isPiecewise;
            this.isFunction = this.dialogData.isFunction;
        }

        /**
         * Calls the setBenefitAndWeightingAttributes function if a change to stateNumericAttributes causes it to have a value
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            if (hasValue(this.stateNumericAttributes)) {
                this.setAttributesList();
            }
        }

        /**
         * Sets boolean properties to show validated/invalidated equation messages and allow/prevent submitting equation
         * data
         */
        @Watch('equation')
        onEquationChanged() {
            // reset showInvalidMessage & showValidMessage
            this.showInvalidMessage = false;
            this.showValidMessage = false;
            // if equation is an empty string, then allow submission of results
            this.cannotSubmit = !(this.equation === '' || this.dialogData.equation === this.equation);
        }

        /**
         * Sets attributesList with the numeric attributes from state
         */
        setAttributesList() {
            this.attributesList = getPropertyValues('name', this.stateNumericAttributes);
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
         * 'Check' button has been clicked
         */
        onCheckEquation() {
            const equationValidation: EquationValidation = {
                equation: this.equation,
                isFunction: this.isFunction,
                isPiecewise: this.isPiecewise
            };
            EquationEditorService.checkEquationValidity(equationValidation)
                .then((response: AxiosResponse<string>) => {
                    // if result is true then set showValidMessage = true, cannotSubmit = false, & showInvalidMessage = false
                    if (response.data === 'OK') {
                        this.showValidMessage = true;
                        this.cannotSubmit = false;
                        this.showInvalidMessage = false;
                    } else {
                        // if result is false then set showInvalidMessage = true, cannotSubmit = true, & showValidMessage = false
                        this.invalidMessage = response.data;
                        this.showInvalidMessage = true;
                        this.cannotSubmit = true;
                        this.showValidMessage = false;
                    }
                });
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit() {
            // reset component's calculated properties
            this.resetComponentCalculatedProperties();
            // create equation editor dialog result
            const result: EquationEditorDialogResult = {
                equation: this.equation,
                isPiecewise: this.isPiecewise,
                isFunction: this.isFunction
            };
            // submit result
            this.$emit('submit', result);
        }

        /**
         * 'Cancel' button has been clicked
         */
        onCancel() {
            // reset component's calculated properties
            this.resetComponentCalculatedProperties();
            // submit a null result
            this.$emit('submit', null);
        }

        /**
         * Resets component's calculated properties
         */
        resetComponentCalculatedProperties() {
            this.cursorPosition = 0;
            this.showInvalidMessage = false;
            this.showValidMessage = false;
            this.cannotSubmit = false;
        }
    }
</script>

<style>
    .equation-container-card {
        height: 800px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .card-title {
        max-height: 60px;
    }

    .list-card-text {
        height: 300px;
        /*border: 1px solid black !important;*/
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

    .validation-message-div {
        height: 21px;
    }

    .invalid-message {
        color: red;
    }

    .valid-message {
        color: green;
    }
</style>