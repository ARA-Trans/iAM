<template>
    <v-dialog v-model="dialogData.showDialog" persistent scrollable max-width="900px">
        <v-card class="equation-container-card">
            <v-card-title>
                <v-layout justify-center>
                    <h3>Equation Editor</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-flex xs12>
                        <div class="validation-message-div">
                            <v-layout justify-center>
                                <p class="invalid-message" v-if="showInvalidMessage">{{invalidMessage}}</p>
                                <p class="valid-message" v-if="showValidMessage">Equation is valid</p>
                            </v-layout>
                        </div>
                    </v-flex>
                    <v-flex xs12>
                        <div>
                            <v-layout justify-start>
                                <v-switch v-model="isPiecewise" :label="isPiecewise ? 'Piecewise' : 'Equation'"></v-switch>
                            </v-layout>
                        </div>
                    </v-flex>
                    <v-flex xs12>
                        <div v-if="!isPiecewise">
                            <v-layout column>
                                <div>
                                    <v-layout justify-space-between row>
                                        <div>
                                            <v-list>
                                                <template>
                                                    <v-subheader>Attributes: Click to add</v-subheader>
                                                    <div class="attributes-list-container">
                                                        <v-list-tile v-for="attribute in attributesList" :key="attribute" class="list-tile"
                                                                     ripple @click="onAddStringToEquation(`[${attribute}]`)">
                                                            <v-list-tile-content>
                                                                <v-list-tile-title>{{attribute}}</v-list-tile-title>
                                                            </v-list-tile-content>
                                                        </v-list-tile>
                                                    </div>
                                                </template>
                                            </v-list>
                                        </div>
                                        <div>
                                            <v-list>
                                                <template>
                                                    <v-subheader>Formulas: Click to add</v-subheader>
                                                    <div class="formulas-list-container">
                                                        <v-list-tile v-for="formula in formulasList" :key="formula" class="list-tile"
                                                                     ripple @click="onAddFormulaToEquation(formula)">
                                                            <v-list-tile-content>
                                                                <v-list-tile-title>{{formula}}</v-list-tile-title>
                                                            </v-list-tile-content>
                                                        </v-list-tile>
                                                    </div>
                                                </template>
                                            </v-list>
                                        </div>
                                    </v-layout>
                                </div>
                                <div>
                                    <v-layout justify-center>
                                        <div class="math-buttons-container">
                                            <v-layout justify-space-between row>
                                                <v-btn class="math-button add" fab small @click="onAddStringToEquation('+')">
                                                    <span>+</span>
                                                </v-btn>
                                                <v-btn class="math-button subtract" fab small @click="onAddStringToEquation('-')">
                                                    <span>-</span>
                                                </v-btn>
                                                <v-btn class="math-button multiply" fab small @click="onAddStringToEquation('*')">
                                                    <span>*</span>
                                                </v-btn>
                                                <v-btn class="math-button divide" fab small @click="onAddStringToEquation('/')">
                                                    <span>/</span>
                                                </v-btn>
                                                <v-btn class="math-button parentheses" fab small @click="onAddStringToEquation('(')">
                                                    <span>(</span>
                                                </v-btn>
                                                <v-btn class="math-button parentheses" fab small @click="onAddStringToEquation(')')">
                                                    <span>)</span>
                                                </v-btn>
                                            </v-layout>
                                        </div>
                                    </v-layout>
                                </div>
                                <div>
                                    <v-layout justify-center>
                                        <v-textarea id="equation_textarea" :rows="5" outline full-width no-resize
                                                    spellcheck="false" v-model="equation" @blur="setCursorPosition"
                                                    @focus="setTextareaCursorPosition">
                                        </v-textarea>
                                    </v-layout>
                                </div>
                            </v-layout>
                        </div>
                        <div v-if="isPiecewise">
                            <v-layout row>
                                <v-flex xs4>
                                    <div>
                                        <v-layout justify-space-between row>
                                            <v-btn class="ara-blue-bg white--text" @click="onAddTimeAttributeDataPoint">
                                                Add
                                            </v-btn>
                                            <v-btn class="ara-blue-bg white--text piecewise-switch-btn" @click="onSwitchDataPointsGridView">
                                                <v-layout justify-space-between row>
                                                    <v-icon>fas fa-sync-alt</v-icon>
                                                    <span>{{dataPointsGridTypeBtnTxt}}</span>
                                                </v-layout>
                                            </v-btn>
                                        </v-layout>
                                        <div class="data-points-grid">
                                            <v-data-table :headers="dataPointsGridHeaders" :items="dataPointsGridData"
                                                          hide-actions :pagination.sync="pagination"
                                                          class="elevation-1 v-table__overflow">
                                                <template slot="items" slot-scope="props">
                                                    <td v-for="header in dataPointsGridHeaders">
                                                        <div v-if="header.value !== ''">
                                                            <v-edit-dialog :return-value.sync="props.item[header.value]" large lazy persistent>
                                                                {{props.item[header.value]}}
                                                                <template slot="input">
                                                                    <v-text-field v-model="props.item[header.value]" label="Edit" single-line>
                                                                    </v-text-field>
                                                                </template>
                                                            </v-edit-dialog>
                                                        </div>
                                                        <div v-else>
                                                            <v-btn icon class="ara-orange" @click="onRemoveTimeAttributeDataPoint(props.item.id)">
                                                                <v-icon>fas fa-trash</v-icon>
                                                            </v-btn>
                                                        </div>
                                                    </td>
                                                </template>
                                            </v-data-table>
                                        </div>
                                        <div>
                                            <v-layout column>
                                                <v-layout justify-center>
                                                    <v-flex xs4>
                                                        <v-select label="Rows per page" :items="rowsPerPageItems"
                                                                  v-model="rowsPerPage">
                                                        </v-select>
                                                    </v-flex>
                                                </v-layout>
                                                <v-pagination v-model="page" :total-visible="5" :length="pages"></v-pagination>
                                            </v-layout>
                                        </div>
                                    </div>
                                </v-flex>
                                <v-flex xs8>
                                    <div>
                                        <kendo-chart :category-axis-title-text="'Time'" :value-axis-title-text="'Attribute'"
                                                     :legend-visible="false" :series-defaults-type="'line'" :series-defaults-style="'smooth'"
                                                     :category-axis-categories="getDataPointsGridDataPropertyValues('timeValue')"
                                                     :category-axis-major-grid-lines-visible="false" :value-axis-major-grid-lines-visible="false"
                                                     :y-axis-title-visible="true" :tooltip-visible="true" :theme="'sass'">
                                            <kendo-chart-series-item :data="getDataPointsGridDataPropertyValues('attributeValue')">
                                            </kendo-chart-series-item>
                                        </kendo-chart>
                                    </div>
                                </v-flex>
                            </v-layout>
                        </div>
                    </v-flex>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout>
                    <v-flex xs12>
                        <div>
                            <v-layout justify-space-between row>
                                <v-btn class="ara-blue-bg white--text" @click="onCheckEquation">Check</v-btn>
                                <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)" :disabled="cannotSubmit">Save</v-btn>
                                <v-btn class="ara-orange-bg white--text" @click="onSubmit(false)">Cancel</v-btn>
                            </v-layout>
                        </div>
                    </v-flex>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {EquationEditorDialogData} from '@/shared/models/modals/equation-editor-dialog-data';
    import {EquationEditorDialogResult} from '@/shared/models/modals/equation-editor-dialog-result';
    import EquationEditorService from '@/services/equation-editor.service';
    import {formulas} from '@/shared/utils/formulas';
    import {AxiosResponse} from 'axios';
    import {getPropertyValues, getPropertyValuesNonUniq} from '@/shared/utils/getter-utils';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {EquationValidation} from '@/shared/models/iAM/equation-validation';
    import {http2XX} from '@/shared/utils/http-utils';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {TimeAttributeDataPoint} from '@/shared/models/iAM/time-attribute-data-point';
    import {isNil, reverse, clone, remove} from 'ramda';
    import {emptyPagination, Pagination} from '@/shared/models/vue/pagination';
    import {SelectItem} from '@/shared/models/vue/select-item';
    const ObjectID = require('bson-objectid');

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
        textareaInput: HTMLTextAreaElement = {} as HTMLTextAreaElement;
        cursorPosition: number = 0;
        showInvalidMessage: boolean = false;
        showValidMessage: boolean = false;
        cannotSubmit: boolean = true;
        invalidMessage: string = '';
        dataPointsGridHeaders: DataTableHeader[] = [
            {text: 'Time', value: 'timeValue', align: 'left', sortable: false, class: '', width: '10px'},
            {text: 'Attribute', value: 'attributeValue', align: 'left', sortable: false, class: '', width: '10px'},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '10px'}
        ];
        dataPointsGridData: TimeAttributeDataPoint[] = [];
        piecewiseRegex: RegExp = /(\(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*\))+/;
        dataPointsGridTypeBtnTxt = 'time-in-rating';
        pagination: Pagination = clone(emptyPagination);
        page: number = 1;
        rowsPerPageItems: SelectItem[] = [
            {text: '5', value: 5}, {text: '10', value: 10}, {text: '15', value: 15}, {text: '20', value: 20}, {text: 'All', value: -1}
        ];
        rowsPerPage: number = 5;
        pages: number = 0;

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
         * Setter: (multiple) => isPiecewise, dataPointsGridData (function call; conditional), equation (conditional)
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            if (this.piecewiseRegex.test(this.dialogData.equation)) {
                this.isPiecewise = true;
                this.onParsePiecewiseEquation();
            } else {
                this.isPiecewise = false;
                this.equation = this.dialogData.equation;
            }
            // set the equation and isPiecewise properties with the dialog data equation
            this.equation = this.dialogData.equation;
            this.isPiecewise = this.dialogData.isPiecewise;
        }

        /**
         * Setter: attributesList (function call; conditional)
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            if (hasValue(this.stateNumericAttributes)) {
                this.setAttributesList();
            }
        }

        /**
         * Setter: (multiple) => showInvalidMessage, showValidMessage, cannotSubmit
         */
        @Watch('equation')
        onEquationChanged() {
            this.showInvalidMessage = false;
            this.showValidMessage = false;

            this.cannotSubmit = !(this.equation === '' && !this.isPiecewise);
        }

        /**
         * Setter: (multiple) => pagination.totalItems, cannotSubmit
         */
        @Watch('dataPointsGridData')
        onDataPointsGridDataChanged() {
            this.pagination = {
                ...this.pagination,
                totalItems: this.dataPointsGridData.length
            };

            this.cannotSubmit = true;
        }

        /**
         * Setter: pagination.rowsPerPage
         */
        @Watch('rowsPerPage')
        onRowsPerPageChanged() {
            this.pagination = {
                ...this.pagination,
                rowsPerPage: this.rowsPerPage
            };
        }

        /**
         * Setter: pagination.page
         */
        @Watch('page')
        onPageChanged() {
            this.pagination = {
                ...this.pagination,
                page: this.page
            };
        }

        /**
         * Setter: pages (conditional)
         */
        @Watch('pagination')
        onPaginationChanged() {
            const pages: number = Math.ceil(this.pagination.totalItems / this.pagination.rowsPerPage);
            if (this.pages !== pages) {
                this.pages = pages;
            }
        }

        /**
         * Setter: attributesList
         */
        setAttributesList() {
            this.attributesList = getPropertyValues('name', this.stateNumericAttributes);
        }

        /**
         * Setter: cursorPosition
         */
        setCursorPosition() {
            this.cursorPosition = this.textareaInput.selectionStart;
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
            } else if (this.cursorPosition === this.equation.length) {
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
            } else if (this.cursorPosition === this.equation.length) {
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
         * Adds a new TimeAttributeDataPoint object to the dataPointsGridData list
         */
        onAddTimeAttributeDataPoint() {
            this.dataPointsGridData.push({id: ObjectID.generate(), timeValue: 0, attributeValue: 0});
        }

        /**
         * Removes a TimeAttributeDataPoint with the specified id from the dataPointsGridData list
         */
        onRemoveTimeAttributeDataPoint(id: string) {
            this.dataPointsGridData = this.dataPointsGridData
                .filter((timeAttributeDataPoint: TimeAttributeDataPoint) => timeAttributeDataPoint.id !== id);
        }

        /**
         * Reverses the dataPointsGridHeaders list & modifies the dataPointsGridTypeBtnTxt value conditionally
         */
        onSwitchDataPointsGridView() {
            const gridHeaders = clone(this.dataPointsGridHeaders);
            this.dataPointsGridHeaders = [
                ...reverse(remove(2, 1, gridHeaders)),
                this.dataPointsGridHeaders[2]
            ];

            this.dataPointsGridTypeBtnTxt = this.dataPointsGridTypeBtnTxt === 'time-in-rating'
                ? 'piecewise' : 'time-in-rating';
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
         * Gets the specified property values from a list of TimeAttributeDataPoint objects then applies pagination
         */
        getDataPointsGridDataPropertyValues(property: string) {
            const gridValues: TimeAttributeDataPoint[] = getPropertyValuesNonUniq(property, this.dataPointsGridData);
            const paginated: TimeAttributeDataPoint[] = [];
            for (let i = (this.pagination.page - 1) * this.pagination.rowsPerPage; i < (this.pagination.page * this.pagination.rowsPerPage) && i < gridValues.length; i++) {
                paginated.push(gridValues[i]);
            }
            return paginated;
        }

        /**
         * Sends an HTTP request to the equation validation API then displays the result of the validation check
         */
        onCheckEquation() {
            const equationValidation: EquationValidation = {
                equation: this.isPiecewise ? this.onParseTimeAttributeDataPoints() : this.equation,
                isPiecewise: this.isPiecewise,
                isFunction: false,
            };

            EquationEditorService.checkEquationValidity(equationValidation)
                .then((response: AxiosResponse<string>) => {
                    // if result is true then set showValidMessage = true, cannotSubmit = false, & showInvalidMessage = false
                    if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                        this.showValidMessage = true;
                        this.showInvalidMessage = false;
                    } else {
                        this.invalidMessage = response.data;
                        // if result is false then set showInvalidMessage = true, cannotSubmit = true, & showValidMessage = false
                        this.showInvalidMessage = true;
                        this.showValidMessage = false;
                    }
                });
        }

        /**
         * Parses a list of TimeAttributeDataPoints objects into a string of (x,y) data points
         */
        onParseTimeAttributeDataPoints() {
            return this.dataPointsGridData.map((timeAttributeDataPoint : TimeAttributeDataPoint) =>
                `(${timeAttributeDataPoint.timeValue},${timeAttributeDataPoint.attributeValue})`
            ).join('');
        }

        /**
         * Parses the equation string of (x,y) data points into a list of TimeAttributeDataPoint objects
         */
        onParsePiecewiseEquation() {
            const regexSplitter = /(\(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*\))/;
            this.dataPointsGridData = this.dialogData.equation.split(regexSplitter)
                .filter((timeAttributeDataPoint: string) =>
                    timeAttributeDataPoint !== '' && !isNil(timeAttributeDataPoint) && timeAttributeDataPoint.indexOf(',') !== -1
                )
                .map((timeAttributeDataPoint: string) => {
                    const splitTimeAttributeDataPoint = timeAttributeDataPoint
                        .replace('(', '').replace(')', '').split(',');

                    return {
                        id: ObjectID.generate(),
                        timeValue: parseFloat(splitTimeAttributeDataPoint[0]),
                        attributeValue: parseFloat(splitTimeAttributeDataPoint[1])
                    };
                });
        }

        /**
         * Submits dialog result or null to the parent component
         */
        onSubmit(submit: boolean) {
            this.resetComponentCalculatedProperties();

            if (submit) {
                const result: EquationEditorDialogResult = {
                    equation: this.equation,
                    isPiecewise: this.isPiecewise,
                    isFunction: false
                };
                this.$emit('submit', result);
            } else {
                this.$emit('submit', null);
            }
        }

        /**
         * Resets component's calculated properties
         */
        resetComponentCalculatedProperties() {
            this.cursorPosition = 0;
            this.showInvalidMessage = false;
            this.showValidMessage = false;
        }
    }
</script>

<style>
    .equation-container-card {
        height: 750px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .validation-message-div {
        height: 21px;
    }

    .invalid-message {
        color: red;
    }

    .attributes-list-container, .formulas-list-container {
        width: 205px;
        height: 250px;
        overflow: auto;
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

    .valid-message {
        color: green;
    }

    .piecewise-switch-btn {
        width: 161px;
    }

    .data-points-grid {
        width: 300px;
        height: 308px;
        overflow: auto;
    }

    .rows-per-page-select .v-input__slot {
        width: 30%;
    }
</style>