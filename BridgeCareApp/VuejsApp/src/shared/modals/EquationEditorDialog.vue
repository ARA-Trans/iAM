<template>
    <v-layout>
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
                            <v-tabs>
                                <v-tab @click="isPiecewise = false">Equation</v-tab>
                                <v-tab @click="isPiecewise = true">Piecewise</v-tab>
                                <v-tab @click="isPiecewise = true">Time In Rating</v-tab>
                                <v-tab-item>
                                    <div class="equation-container-div">
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
                                </v-tab-item>
                                <v-tab-item>
                                    <div class="equation-container-div">
                                        <v-layout row>
                                            <v-flex xs4>
                                                <div>
                                                    <v-layout justify-space-between row>
                                                        <v-btn class="ara-blue-bg white--text" @click="onAddTimeAttributeDataPoint">
                                                            Add
                                                        </v-btn>
                                                        <!--<v-btn class="ara-blue-bg white&#45;&#45;text piecewise-switch-btn" @click="onSwitchDataPointsGridView">
                                                            <v-layout justify-space-between row>
                                                                <v-icon>fas fa-sync-alt</v-icon>
                                                                <span>{{dataPointsGridTypeBtnTxt}}</span>
                                                            </v-layout>
                                                        </v-btn>-->
                                                    </v-layout>
                                                    <div class="data-points-grid">
                                                        <v-data-table :headers="piecewiseGridHeaders" :items="dataPointsGridData"
                                                                      hide-actions :pagination.sync="pagination"
                                                                      class="elevation-1 v-table__overflow">
                                                            <template slot="items" slot-scope="props">
                                                                <td v-for="header in piecewiseGridHeaders">
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
                                                            <v-layout justify-center>
                                                                <v-pagination v-show="rowsPerPage !== -1" v-model="page"
                                                                              :total-visible="5" :length="pages">
                                                                </v-pagination>
                                                            </v-layout>
                                                        </v-layout>
                                                    </div>
                                                </div>
                                            </v-flex>
                                            <v-flex xs8>
                                                <div class="kendo-chart-container">
                                                    <!--<kendo-chart :category-axis-title-text="'Time'" :value-axis-title-text="'Attribute'"
                                                                 :legend-visible="false" :series-defaults-type="'line'" :series-defaults-style="'smooth'"
                                                                 :category-axis-categories="getDataPointsGridDataPropertyValues('timeValue')"
                                                                 :category-axis-major-grid-lines-visible="false" :value-axis-major-grid-lines-visible="false"
                                                                 :category-axis-visible="showCategoryAxis"
                                                                 :y-axis-title-visible="true" :tooltip-visible="true" :theme="'sass'">
                                                        <kendo-chart-series-item :data="getDataPointsGridDataPropertyValues('attributeValue')">
                                                        </kendo-chart-series-item>
                                                    </kendo-chart>-->
                                                    <kendo-chart :data-source="chartData"
                                                                 :series="series"
                                                                 :pannable-lock="'y'"
                                                                 :zoomable-mousewheel-lock="'y'"
                                                                 :zoomable-selection-lock="'y'"
                                                                 :category-axis="categoryAxis"
                                                                 :theme="'sass'"
                                                                 :category-axis-title-text="'Time'"
                                                                 :value-axis-title-text="'Attribute'"
                                                                 :tooltip="tooltip"
                                                                 :plot-area="plotArea">
                                                        <kendo-chart-plot-area :border-dash-type="'solid'"></kendo-chart-plot-area>
                                                    </kendo-chart>
                                                </div>
                                            </v-flex>
                                        </v-layout>
                                    </div>
                                </v-tab-item>
                                <v-tab-item>
                                    <div class="equation-container-div">
                                        <v-layout row>
                                            <v-flex xs4>
                                                <div>
                                                    <v-layout justify-space-between row>
                                                        <v-btn class="ara-blue-bg white--text" @click="onAddTimeAttributeDataPoint">
                                                            Add
                                                        </v-btn>
                                                    </v-layout>
                                                    <div class="data-points-grid">
                                                        <v-data-table :headers="timeInRatingGridHeaders" :items="dataPointsGridData"
                                                                      hide-actions :pagination.sync="pagination"
                                                                      class="elevation-1 v-table__overflow">
                                                            <template slot="items" slot-scope="props">
                                                                <td v-for="header in timeInRatingGridHeaders">
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
                                                            <v-layout justify-center>
                                                                <v-pagination v-show="rowsPerPage !== -1" v-model="page"
                                                                              :total-visible="5" :length="pages">
                                                                </v-pagination>
                                                            </v-layout>
                                                        </v-layout>
                                                    </div>
                                                </div>
                                            </v-flex>
                                            <v-flex xs8>
                                                <div class="kendo-chart-container">
                                                    <!--<kendo-chart :category-axis-title-text="'Time'" :value-axis-title-text="'Attribute'"
                                                                 :legend-visible="false" :series-defaults-type="'line'" :series-defaults-style="'smooth'"
                                                                 :category-axis-categories="getDataPointsGridDataPropertyValues('timeValue')"
                                                                 :category-axis-major-grid-lines-visible="false" :value-axis-major-grid-lines-visible="false"
                                                                 :category-axis-visible="showCategoryAxis"
                                                                 :y-axis-title-visible="true" :tooltip-visible="true" :theme="'sass'" :pannable="true">
                                                        <kendo-chart-series-item :data="getDataPointsGridDataPropertyValues('attributeValue')">
                                                        </kendo-chart-series-item>
                                                    </kendo-chart>-->
                                                    <kendo-chart :data-source="chartData"
                                                                 :series="series"
                                                                 :pannable-lock="'y'"
                                                                 :zoomable-mousewheel-lock="'y'"
                                                                 :zoomable-selection-lock="'y'"
                                                                 :category-axis="categoryAxis"
                                                                 :theme="'sass'"
                                                                 :category-axis-title-text="'Time'"
                                                                 :value-axis-title-text="'Attribute'"
                                                                 :tooltip="tooltip"
                                                                 :plotarea="plotArea">
                                                    </kendo-chart>
                                                </div>
                                            </v-flex>
                                        </v-layout>
                                    </div>
                                </v-tab-item>
                            </v-tabs>
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

        <v-dialog :class="'data-points-popup'" v-model="showAddDataPointPopup" persistent max-width="250px">
            <v-card>
                <v-card-text>
                    <v-layout justify-center column>
                        <div>
                            <v-text-field outline v-model="newDataPoint.timeValue" label="Time Value"></v-text-field>
                        </div>
                        <div>
                            <v-text-field outline v-model="newDataPoint.attributeValue" label="Attribute Value">
                            </v-text-field>
                        </div>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn class="ara-blue-bg white--text" @click="submitNewDataPoint(true)">Save</v-btn>
                        <v-btn class="ara-orange-bg white--text" @click="submitNewDataPoint(false)">Cancel</v-btn>
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
    import {AxiosResponse} from 'axios';
    import {getPropertyValues, getPropertyValuesNonUniq} from '@/shared/utils/getter-utils';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {EquationValidation} from '@/shared/models/iAM/equation-validation';
    import {http2XX} from '@/shared/utils/http-utils';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {emptyTimeAttributeDataPoint, TimeAttributeDataPoint} from '@/shared/models/iAM/time-attribute-data-point';
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
        piecewiseGridHeaders: DataTableHeader[] = [
            {text: 'Time', value: 'timeValue', align: 'left', sortable: false, class: '', width: '10px'},
            {text: 'Attribute', value: 'attributeValue', align: 'left', sortable: false, class: '', width: '10px'},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '10px'}
        ];
        timeInRatingGridHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attributeValue', align: 'left', sortable: false, class: '', width: '10px'},
            {text: 'Time', value: 'timeValue', align: 'left', sortable: false, class: '', width: '10px'},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '10px'}
        ];
        dataPointsGridData: TimeAttributeDataPoint[] = [];
        piecewiseRegex: RegExp = /(\(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*\))+/;
        pagination: Pagination = clone(emptyPagination);
        page: number = 1;
        rowsPerPageItems: SelectItem[] = [
            {text: '5', value: 5}, {text: '10', value: 10}, {text: '15', value: 15}, {text: '20', value: 20}, {text: 'All', value: -1}
        ];
        rowsPerPage: number = 5;
        pages: number = 0;
        showAddDataPointPopup: boolean = false;
        newDataPoint: TimeAttributeDataPoint = clone(emptyTimeAttributeDataPoint);
        series: any[] = [{
            type: 'line', field: 'attributeValue', categoryField: 'timeValue'
        }];
        categoryAxis: any = {
            min: 0, max: 10, labels: {rotation: 'auto'}
        };
        tooltip: any = {
            visible: true, template: '#= category #, #= value #'
        };
        plotArea: any = {
            border: {dashType: 'solid'}
        };
        chartData: TimeAttributeDataPoint[] = [];

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
            // if (this.piecewiseRegex.test(this.dialogData.equation)) {
            if (true) {
                this.isPiecewise = true;
                this.onParsePiecewiseEquation();
                this.getDataPointsGridDataPropertyValues();
            } else {
                this.isPiecewise = false;
            }

            this.equation = this.dialogData.equation;
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

            this.getDataPointsGridDataPropertyValues();
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
         *
         */
        onAddTimeAttributeDataPoint() {
            this.newDataPoint = {
                ...this.newDataPoint,
                id: ObjectID.generate()
            };
            this.showAddDataPointPopup = true;
        }

        /**
         * Removes a TimeAttributeDataPoint with the specified id from the dataPointsGridData list
         */
        onRemoveTimeAttributeDataPoint(id: string) {
            this.dataPointsGridData = this.dataPointsGridData
                .filter((timeAttributeDataPoint: TimeAttributeDataPoint) => timeAttributeDataPoint.id !== id);
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
        getDataPointsGridDataPropertyValues() {
            this.chartData = [];
            if (this.pagination.rowsPerPage === -1) {
                this.chartData.push(...this.dataPointsGridData);
            } else {
                for (let i = (this.pagination.page - 1) * this.pagination.rowsPerPage; i < (this.pagination.page * this.pagination.rowsPerPage) && i < this.dataPointsGridData.length; i++) {
                    this.chartData.push(this.dataPointsGridData[i]);
                }
            }
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
            var test: string = '(0,10)(1,9.8)(2,9.6)(3,9.4)(4,9.2)(5,9)(6,8.875)(7,8.75)(8,8.625)(9,8.5)(10,8.375)(11,8.25)(12,8.125)(13,8)(14,7.917)(15,7.834)(16,7.751)(17,7.668)(18,7.585)(19,7.502)(20,7.419)(21,7.336)(22,7.253)(23,7.17)(24,7.087)(25,7.004)(26,6.959)(27,6.914)(28,6.869)(29,6.824)(30,6.779)(31,6.734)(32,6.689)(33,6.644)(34,6.599)(35,6.554)(36,6.509)(37,6.464)(38,6.419)(39,6.374)(40,6.329)(41,6.284)(42,6.239)(43,6.194)(44,6.149)(45,6.104)(46,6.059)(47,6.014)(48,5.977)(49,5.94)(50,5.903)(51,5.866)(52,5.829)(53,5.792)(54,5.755)(55,5.718)(56,5.681)(57,5.644)(58,5.607)(59,5.57)(60,5.533)(61,5.496)(62,5.459)(63,5.422)(64,5.385)(65,5.348)(66,5.311)(67,5.274)(68,5.237)(69,5.2)(70,5.163)(71,5.126)(72,5.089)(73,5.052)(74,5.015)(75,4.915)(76,4.815)(77,4.715)(78,4.615)(79,4.515)(80,4.415)(81,4.315)(82,4.215)(83,4.115)(84,4.015)(85,3.815)(86,3.615)(87,3.415)(88,3.215)(89,3.015)(90,2.815)(91,2.615)(92,2.415)(93,2.215)(94,2.015)(95,1.815)(96,1.615)(97,1.415)(98,1.215)(99,1.015)(100,0.815)(101,0.615)(102,0.415)(103,0.215)(104,0)';
            this.dataPointsGridData = test.split(regexSplitter)//this.dialogData.equation.split(regexSplitter)
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

        submitNewDataPoint(submit: boolean) {
            this.showAddDataPointPopup = false;

            if (submit) {
                this.dataPointsGridData.push(this.newDataPoint);
            }

            this.newDataPoint = clone(emptyTimeAttributeDataPoint);
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

    .data-points-grid {
        width: 300px;
        height: 308px;
        overflow: auto;
    }

    .rows-per-page-select .v-input__slot {
        width: 30%;
    }

    .equation-container-div {
        height: 505px;
    }

    /*.kendo-chart-container {
        height: 999px;
        width: 999px;
        overflow: scroll;
    }*/
</style>