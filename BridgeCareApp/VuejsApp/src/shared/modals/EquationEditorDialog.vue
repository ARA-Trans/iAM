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
                            <v-tabs v-model="selectedTab">
                                <v-tab :key="0" @click="isPiecewise = false">Equation</v-tab>
                                <v-tab :key="1" @click="isPiecewise = true">Piecewise</v-tab>
                                <v-tab :key="2" @click="isPiecewise = true">Time In Rating</v-tab>
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
                                                        <v-btn class="ara-blue-bg white--text" @click="showAddDataPointMultiPopup = true">
                                                            Add Multi
                                                        </v-btn>
                                                    </v-layout>
                                                    <div class="data-points-grid">
                                                        <v-data-table :headers="piecewiseGridHeaders" :items="piecewiseGridData"
                                                                      hide-actions class="elevation-1 v-table__overflow">
                                                            <template slot="items" slot-scope="props">
                                                                <td v-for="header in piecewiseGridHeaders">
                                                                    <div v-if="header.value !== ''">
                                                                        <div v-if="props.item.timeValue === 0">
                                                                            {{props.item[header.value]}}
                                                                        </div>
                                                                        <div v-else class="edit-data-point-span"
                                                                              @click="onEditDataPoint(props.item, header.value)">
                                                                            {{props.item[header.value]}}
                                                                        </div>
                                                                    </div>
                                                                    <div v-else>
                                                                        <v-btn v-if="props.item.timeValue !== 0" icon class="ara-orange"
                                                                               @click="onRemoveTimeAttributeDataPoint(props.item.id)">
                                                                            <v-icon>fas fa-trash</v-icon>
                                                                        </v-btn>
                                                                    </div>
                                                                </td>
                                                            </template>
                                                        </v-data-table>
                                                    </div>
                                                </div>
                                            </v-flex>
                                            <v-flex xs8>
                                                <div class="kendo-chart-container">
                                                    <kendo-chart :data-source="piecewiseGridData"
                                                                 :series="series"
                                                                 :pannable-lock="'y'"
                                                                 :zoomable-mousewheel-lock="'y'"
                                                                 :zoomable-selection-lock="'y'"
                                                                 :category-axis="categoryAxis"
                                                                 :theme="'sass'"
                                                                 :value-axis-title-text="'Condition'"
                                                                 :tooltip="tooltip">
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
                                                        <v-btn class="ara-blue-bg white--text" @click="showAddDataPointMultiPopup = true">
                                                            Add Multi
                                                        </v-btn>
                                                    </v-layout>
                                                    <div class="data-points-grid">
                                                        <v-data-table :headers="timeInRatingGridHeaders" :items="timeInRatingGridData"
                                                                      hide-actions class="elevation-1 v-table__overflow">
                                                            <template slot="items" slot-scope="props">
                                                                <td v-for="header in timeInRatingGridHeaders">
                                                                    <div v-if="header.value !== ''">
                                                                        <div class="edit-data-point-span"
                                                                             @click="onEditDataPoint(props.item, header.value)">
                                                                            {{props.item[header.value]}}
                                                                        </div>
                                                                    </div>
                                                                    <div v-else>
                                                                        <v-btn icon class="ara-orange"
                                                                               @click="onRemoveTimeAttributeDataPoint(props.item.id)">
                                                                            <v-icon>fas fa-trash</v-icon>
                                                                        </v-btn>
                                                                    </div>
                                                                </td>
                                                            </template>
                                                        </v-data-table>
                                                    </div>
                                                </div>
                                            </v-flex>
                                            <v-flex xs8>
                                                <div class="kendo-chart-container">
                                                    <kendo-chart :data-source="piecewiseGridData"
                                                                 :series="series"
                                                                 :pannable-lock="'y'"
                                                                 :zoomable-mousewheel-lock="'y'"
                                                                 :zoomable-selection-lock="'y'"
                                                                 :category-axis="categoryAxis"
                                                                 :theme="'sass'"
                                                                 :value-axis-title-text="'Condition'"
                                                                 :tooltip="tooltip">
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

        <v-dialog v-model="showAddDataPointPopup" persistent max-width="250px">
            <v-card>
                <v-card-text>
                    <v-layout justify-center column>
                        <div>
                            <v-text-field outline v-model="newDataPoint.timeValue" label="Time Value"
                                          type="number" :rules="[timeValueIsNotEmpty, timeValueIsNotZero, timeValueIsNew]">
                            </v-text-field>
                        </div>
                        <div>
                            <v-text-field outline v-model="newDataPoint.attributeValue" label="Attribute Value"
                                          type="number" :rules="[conditionValueIsNotEmpty, conditionValueIsNew]">
                            </v-text-field>
                        </div>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn class="ara-blue-bg white--text" @click="submitNewDataPoint(true)"
                               :disabled="disableDataPointSubmit()">
                            Save
                        </v-btn>
                        <v-btn class="ara-orange-bg white--text" @click="submitNewDataPoint(false)">Cancel</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-dialog v-model="showAddDataPointMultiPopup" persistent max-width="200px">
            <v-card>
                <v-card-text>
                    <v-layout justify-center column>
                        <p>Data point entries must follow the format <span class="format-span"><strong>#,#</strong></span> (time,attribute) with each entry on a separate line.</p>
                        <v-flex xs2>
                            <v-textarea rows="20" no-resize outline v-model="multiDataPoints"
                                        :rules="[multiDataPointsFormIsNotEmpty, isCorrectMultiDataPointsFormat, timeValueIsNotZero, multiDataPointsAreNew]">
                            </v-textarea>
                        </v-flex>

                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn class="ara-blue-bg white--text" @click="submitNewDataPointMulti(true)"
                               :disabled="disableMultiDataPointsSubmit()">
                            Save
                        </v-btn>
                        <v-btn class="ara-orange-bg white--text" @click="submitNewDataPointMulti(false)">Cancel</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-dialog v-model="showEditDataPointPopup" persistent max-width="250px">
            <v-card>
                <v-card-text>
                    <v-layout justify-center>
                        <div v-if="editedDataPointProperty === 'timeValue'">
                            <v-text-field outline v-model="editedDataPoint.timeValue" label="Time Value"
                                          type="number" :rules="[timeValueIsNotEmpty, timeValueIsNotZero, timeValueIsNew]">
                            </v-text-field>
                        </div>
                        <div v-else>
                            <v-text-field outline v-model="editedDataPoint.attributeValue" label="Attribute Value"
                                          type="number" :rules="[conditionValueIsNotEmpty, conditionValueIsNew]">
                            </v-text-field>
                        </div>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn class="ara-blue-bg white--text" @click="onSubmitEditedDataPointValue(true)"
                               :disabled="disableEditDataPointSubmit()">
                            Save
                        </v-btn>
                        <v-btn class="ara-orange-bg white--text" @click="onSubmitEditedDataPointValue(false)">Cancel</v-btn>
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
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {Equation, EquationValidationResult} from '@/shared/models/iAM/equation';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {emptyTimeAttributeDataPoint, TimeAttributeDataPoint} from '@/shared/models/iAM/time-attribute-data-point';
    import {clone, reverse, isEmpty, add, insert, findIndex, propEq, update} from 'ramda';
    import {sortByProperty} from '@/shared/utils/sorter-utils';
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
            {text: 'Condition', value: 'attributeValue', align: 'left', sortable: false, class: '', width: '10px'},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '10px'}
        ];
        timeInRatingGridHeaders: DataTableHeader[] = [
            {text: 'Condition', value: 'attributeValue', align: 'left', sortable: false, class: '', width: '10px'},
            {text: 'Time', value: 'timeValue', align: 'left', sortable: false, class: '', width: '10px'}
        ];
        piecewiseGridData: TimeAttributeDataPoint[] = [];
        timeInRatingGridData: TimeAttributeDataPoint[] = [];
        showAddDataPointPopup: boolean = false;
        newDataPoint: TimeAttributeDataPoint = clone(emptyTimeAttributeDataPoint);
        series: any[] = [{type: 'line', field: 'attributeValue', categoryField: 'timeValue', markers: {visible: false}}];
        categoryAxis: any = {min: 0, max: 0, labels: {step: 1}, title: {text: 'Time', visible: true}};
        tooltip: any = {visible: true, template: '#= category #, #= value #'};
        showAddDataPointMultiPopup: boolean = false;
        multiDataPoints: string = '';
        selectedTab: number = 0;
        showEditDataPointPopup: boolean = false;
        editedDataPointProperty: string = '';
        editedDataPoint: TimeAttributeDataPoint = clone(emptyTimeAttributeDataPoint);

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
         * Setter: attributesList (function call; conditional)
         */
        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            if (hasValue(this.stateNumericAttributes)) {
                this.setAttributesList();
            }
        }

        /**
         * Setter: (multiple) => isPiecewise, piecewiseGridData (function call; conditional), equation (conditional)
         */
        @Watch('dialogData')
        onDialogDataChanged() {
            if ((/(\(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*\))+/).test(this.dialogData.equation)) {
                this.isPiecewise = true;
                this.selectedTab = 1;
                this.onParsePiecewiseEquation();
            } else {
                this.isPiecewise = false;
                this.equation = this.dialogData.equation;
            }
        }

        /**
         * Setter: (multiple) => pagination.totalItems, cannotSubmit
         */
        @Watch('piecewiseGridData')
        onPiecewiseGridDataChanged() {
            this.cannotSubmit = true;
            this.showInvalidMessage = false;
            this.showValidMessage = false;

            this.categoryAxis.max = getPropertyValues('timeValue', this.piecewiseGridData).length;

            if (this.categoryAxis.max <= 10) {
                this.categoryAxis.labels.step = 1;
            } else {
                this.categoryAxis.labels.step = Math.trunc(this.categoryAxis.max / 10);
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
         * Parses the equation string of (x,y) data points into a list of TimeAttributeDataPoint objects
         */
        onParsePiecewiseEquation() {
            const regexSplitter = /(\(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*\))/;
            let dataPoints: TimeAttributeDataPoint[] = [];

            const dataPointStrings: string[] = this.dialogData.equation.split(regexSplitter)
                .filter((dataPoint: string) => hasValue(dataPoint) && dataPoint.indexOf(',') !== -1);

            dataPointStrings.forEach((dataPoint: string) => {
                const splitDataPoint = dataPoint
                    .replace('(', '')
                    .replace(')', '')
                    .split(',');

                dataPoints.push({
                    id: ObjectID.generate(),
                    timeValue: parseInt(splitDataPoint[0]),
                    attributeValue: parseFloat(splitDataPoint[1])
                });
            });

            dataPoints = sortByProperty('timeValue', dataPoints);

            this.syncDataGridLists(dataPoints);
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
         * Sets the cursor position of the equation textarea element using the cursorPosition property
         */
        setTextareaCursorPosition() {
            setTimeout(() =>
                this.textareaInput.setSelectionRange(this.cursorPosition, this.cursorPosition)
            );
        }

        /**
         * Shows the new data point popup
         */
        onAddTimeAttributeDataPoint() {
            this.newDataPoint = {
                ...this.newDataPoint,
                id: ObjectID.generate()
            };
            this.showAddDataPointPopup = true;
        }

        /**
         * Creates a new data point from the new data point popup
         */
        submitNewDataPoint(submit: boolean) {
            this.showAddDataPointPopup = false;

            if (submit) {
                const dataPoints: TimeAttributeDataPoint[] = this.selectedTab === 1
                    ? [...this.piecewiseGridData, this.newDataPoint]
                    : [...this.timeInRatingGridData, this.newDataPoint];

                this.syncDataGridLists(dataPoints);
            }

            this.newDataPoint = clone(emptyTimeAttributeDataPoint);
        }

        /**
         * Syncs data between the two data grid lists
         */
        syncDataGridLists(dataPoints: TimeAttributeDataPoint[]) {
            let piecewiseData: TimeAttributeDataPoint[] = [];
            let timeInRatingData: TimeAttributeDataPoint[] = [];

            if (this.selectedTab === 1) {
                piecewiseData = sortByProperty('timeValue', dataPoints)
                    .filter((dataPoint: TimeAttributeDataPoint) => dataPoint.timeValue !== 0);

                piecewiseData.forEach((dataPoint: TimeAttributeDataPoint, index: number) => {
                    timeInRatingData.push({
                        ...dataPoint,
                        timeValue: index === 0
                            ? piecewiseData[index].timeValue
                            : Math.abs(piecewiseData[index - 1].timeValue - piecewiseData[index].timeValue)
                    });
                });

                timeInRatingData = reverse(sortByProperty('attributeValue', timeInRatingData));

                if (hasValue(timeInRatingData)) {
                    const n1: TimeAttributeDataPoint = {
                        id: ObjectID.generate(),
                        timeValue: 0,
                        attributeValue: Math.trunc(add(1, timeInRatingData[0].attributeValue))
                    };
                    piecewiseData = insert(0, n1, piecewiseData);
                }
            } else {
                timeInRatingData = reverse(sortByProperty('attributeValue', dataPoints));

                let cumulativeTimeValue: number = 0;
                timeInRatingData.forEach((dataPoint: TimeAttributeDataPoint) => {
                    const timeValue: number = add(cumulativeTimeValue, dataPoint.timeValue);
                    cumulativeTimeValue = timeValue;

                    piecewiseData.push({
                        ...dataPoint,
                        timeValue: timeValue
                    });
                });

                piecewiseData = sortByProperty('timeValue', piecewiseData);

                if (hasValue(timeInRatingData)) {
                    const n1: TimeAttributeDataPoint = {
                        id: ObjectID.generate(),
                        timeValue: 0,
                        attributeValue: Math.trunc(add(1, timeInRatingData[0].attributeValue))
                    };
                    piecewiseData = insert(0, n1, piecewiseData);
                }
            }

            this.piecewiseGridData = piecewiseData;
            this.timeInRatingGridData = timeInRatingData;
        }

        /**
         * Creates new data points from the multiple data points popup result
         */
        submitNewDataPointMulti(submit: boolean) {
            if (submit) {
                const parsedMultiDataPoints: TimeAttributeDataPoint[] = this.parseMultiDataPoints();

                const dataPoints = this.selectedTab === 1
                    ? [...this.piecewiseGridData, ...parsedMultiDataPoints]
                    : [...this.timeInRatingGridData, ...parsedMultiDataPoints];

                this.syncDataGridLists(dataPoints);
            }

            this.showAddDataPointMultiPopup = false;
            this.multiDataPoints = '';
        }

        /**
         * Parses the multi data points string into a list of TimeAttributeDataPoint objects
         */
        parseMultiDataPoints() {
            const splitDataPoints: string[] = this.multiDataPoints
                .split(/\r?\n/).filter((dataPoints: string) => dataPoints !== '');

            if (hasValue(splitDataPoints)) {
                const dataPoints: TimeAttributeDataPoint[] = splitDataPoints.map((dataPoints: string) => {
                    const splitValues: string[] = dataPoints.split(',');

                    return {
                        id: ObjectID.generate(),
                        timeValue: parseInt(splitValues[0]),
                        attributeValue: parseFloat(splitValues[1])
                    };
                });

                return dataPoints;
            }

            return [];
        }

        onEditDataPoint(dataPoint: TimeAttributeDataPoint, property: string) {
            this.editedDataPoint = clone(dataPoint);
            this.editedDataPointProperty = property;
            this.showEditDataPointPopup = true;
        }

        onSubmitEditedDataPointValue(submit: boolean) {
            if (submit) {
                let dataPoints = this.selectedTab === 1 ? clone(this.piecewiseGridData) : clone(this.timeInRatingGridData);
                dataPoints = update(
                    findIndex(propEq('id', this.editedDataPoint.id), dataPoints), this.editedDataPoint, dataPoints
                );

                this.syncDataGridLists(dataPoints);
            }

            this.editedDataPoint = clone(emptyTimeAttributeDataPoint);
            this.editedDataPointProperty = '';
            this.showEditDataPointPopup = false;
        }

        /**
         * Removes a TimeAttributeDataPoint with the specified id from a data grid list
         */
        onRemoveTimeAttributeDataPoint(id: string) {
            const dataPoints: TimeAttributeDataPoint[] = this.selectedTab === 1
                ? this.piecewiseGridData.filter((dataPoint: TimeAttributeDataPoint) => dataPoint.id !== id)
                : this.timeInRatingGridData.filter((dataPoint: TimeAttributeDataPoint) => dataPoint.id !== id);

            this.syncDataGridLists(dataPoints);
        }

        /**
         * Sends an HTTP request to the equation validation API then displays the result of the validation check
         */
        onCheckEquation() {
            const equation: Equation = {
                equation: this.isPiecewise ? this.onParseTimeAttributeDataPoints() : this.equation,
                isPiecewise: this.isPiecewise,
                isFunction: false,
            };

            EquationEditorService.checkEquationValidity(equation)
                .then((response: AxiosResponse<EquationValidationResult>) => {
                    if (hasValue(response, 'data')) {
                        const validationResult: EquationValidationResult = response.data;
                        if (validationResult.isValid) {
                            this.showValidMessage = true;
                            this.showInvalidMessage = false;
                            this.cannotSubmit = false;
                        } else {
                            this.invalidMessage = validationResult.message;
                            this.showInvalidMessage = true;
                            this.showValidMessage = false;
                            this.cannotSubmit = true;
                        }
                    }
                });
        }

        /**
         * Parses a list of TimeAttributeDataPoints objects into a string of (x,y) data points
         */
        onParseTimeAttributeDataPoints() {
            return this.piecewiseGridData.map((timeAttributeDataPoint : TimeAttributeDataPoint) =>
                `(${timeAttributeDataPoint.timeValue},${timeAttributeDataPoint.attributeValue})`
            ).join('');
        }

        /**
         * Submits dialog result or null to the parent component
         */
        onSubmit(submit: boolean) {
            this.resetComponentCalculatedProperties();

            if (submit) {
                const result: EquationEditorDialogResult = {
                    equation: this.isPiecewise ? this.onParseTimeAttributeDataPoints() : this.equation,
                    isPiecewise: this.isPiecewise,
                    isFunction: false
                };
                this.$emit('submit', result);
            } else {
                this.$emit('submit', null);
            }

            this.piecewiseGridData = [];
            this.timeInRatingGridData = [];
            this.selectedTab = 0;
        }

        /**
         * Resets component's calculated properties
         */
        resetComponentCalculatedProperties() {
            this.cursorPosition = 0;
            this.showInvalidMessage = false;
            this.showValidMessage = false;
        }

        /**
         * Disables the new data point popup 'SAVE' button
         */
        disableDataPointSubmit() {
            return this.timeValueIsNotEmpty(this.newDataPoint.timeValue.toString()) !== true ||
                   this.timeValueIsNotZero(this.newDataPoint.timeValue.toString()) !== true ||
                   this.timeValueIsNew(this.newDataPoint.timeValue.toString()) !== true ||
                   this.conditionValueIsNotEmpty(this.newDataPoint.attributeValue.toString()) !== true ||
                   this.conditionValueIsNew(this.newDataPoint.attributeValue.toString()) !== true;
        }

        /**
         * Disables the multiple data points popup 'SAVE' button
         */
        disableMultiDataPointsSubmit() {
            return this.multiDataPoints === '' ||
                   this.isCorrectMultiDataPointsFormat() !== true ||
                   this.multiDataPointsAreNew() !== true;
        }

        /**
         * Disables the edit data point popup 'SAVE' button
         */
        disableEditDataPointSubmit() {
            if (this.editedDataPointProperty === 'timeValue') {
                return this.timeValueIsNotEmpty(this.editedDataPoint.timeValue.toString()) !== true ||
                       this.timeValueIsNotZero(this.editedDataPoint.timeValue.toString()) !== true ||
                       this.timeValueIsNew(this.editedDataPoint.timeValue.toString()) !== true;
            } else {
                return this.conditionValueIsNotEmpty(this.editedDataPoint.attributeValue.toString()) !== true ||
                       this.conditionValueIsNew(this.editedDataPoint.attributeValue.toString()) !== true;
            }
        }

        /**
         * Rule: Checks if a given time value is 0
         * @param value
         */
        timeValueIsNotZero(value: string) {
            return parseInt(value) > 0 || 'Time values cannot be less than or equal to 0';
        }

        /**
         * Rule: Checks if a given time value is new
         * @param value
         */
        timeValueIsNew(value: string) {
            if (this.selectedTab === 1) {
                const timeValues: number[] = getPropertyValues('timeValue', this.piecewiseGridData);

                return timeValues.indexOf(parseInt(value)) === -1 || 'Time value already exists';
            }

            return true;
        }

        /**
         * Rule: Checks if a given time value is not empty
         * @param value
         */
        timeValueIsNotEmpty(value: string) {
            return hasValue(value) || 'A value must be entered';
        }

        /**
         * Rule: Checks if a given condition value is new
         * @param value
         */
        conditionValueIsNew(value: string) {
            const conditionValues: number[] = this.selectedTab === 1
                ? getPropertyValues('attributeValue', this.piecewiseGridData)
                : getPropertyValues('attributeValue', this.timeInRatingGridData);

            return conditionValues.indexOf(parseFloat(value)) === -1 || 'Condition value already exists';
        }

        /**
         * Rule: Checks if a given condition value is not empty
         * @param value
         */
        conditionValueIsNotEmpty(value: string) {
            return hasValue(value) || "A value must be entered";
        }

        /**
         * Rule: Checks if the multi data point popup textarea is not empty
         */
        multiDataPointsFormIsNotEmpty() {
            return this.multiDataPoints !== '' || 'Values must be entered';
        }

        /**
         * Rule: Checks if the multi data point popup textarea has correctly formatted data
         */
        isCorrectMultiDataPointsFormat() {
            const eachDataPointIsValid = this.multiDataPoints
                .split(/\r?\n/).filter((dataPoints: string) => dataPoints !== '')
                .every((dataPoints: string) => {
                    return (/\d+(\.{1}\d+)*,\d+(\.{1}\d+)*/).test(dataPoints) &&
                           dataPoints.split(',').every((value: string) => !isNaN(parseFloat(value)));
                });

            return eachDataPointIsValid || 'Incorrect format';
        }

        /**
         * Rule: Checks if the multi data point popup textarea data has all new values for times & conditions
         */
        multiDataPointsAreNew() {
            const dataPoints: TimeAttributeDataPoint[] = this.parseMultiDataPoints();
            const existingConditionValues: number[] = [];
            const existingTimeValues: number[] = [];

            const eachDataPointIsNew = dataPoints.every((dataPoint: TimeAttributeDataPoint) => {
                const conditionValueIsNew = this.conditionValueIsNew(dataPoint.attributeValue.toString()) === true;
                const timeValueIsNew: boolean = this.timeValueIsNew(dataPoint.timeValue.toString()) === true;

                if (!conditionValueIsNew) {
                    existingConditionValues.push(dataPoint.attributeValue);
                }

                if (this.selectedTab === 1 && !timeValueIsNew) {
                    existingTimeValues.push(dataPoint.timeValue);
                }

                return this.selectedTab === 1
                    ? conditionValueIsNew && timeValueIsNew
                    : conditionValueIsNew;
            });

            let conditionValuesAlreadyExistsMessage: string = '';
            if (!isEmpty(existingConditionValues)) {
                conditionValuesAlreadyExistsMessage = 'The following condition values already exist: ';

                existingConditionValues.forEach((value: number, index: number) => {
                    conditionValuesAlreadyExistsMessage += index > 0 ? `, ${value}` : `${value}`;
                });
            }

            let timeValuesAlreadyExistsMessage: string = '';
            if (!isEmpty(existingTimeValues)) {
                timeValuesAlreadyExistsMessage = 'The following time values already exist: ';

                existingTimeValues.forEach((value: number, index: number) => {
                    timeValuesAlreadyExistsMessage += index > 0 ? `, ${value}` : `${value}`;
                });
            }

            return eachDataPointIsNew || `${conditionValuesAlreadyExistsMessage}\n${timeValuesAlreadyExistsMessage}`;
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

    .format-span {
        color: red;
    }

    .edit-data-point-span {
        cursor: pointer;
    }
</style>