<template>
    <v-layout class="consequences-tab-content">
        <v-flex xs12>
            <v-btn @click="onAddConsequence" class="ara-blue-bg white--text">Add Consequence</v-btn>
            <div class="consequences-data-table">
                <v-data-table :headers="consequencesGridHeaders" :items="consequencesGridData"
                              class="elevation-1 fixed-header v-table__overflow"
                              hide-actions>
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-edit-dialog
                                    :return-value.sync="props.item.attribute"
                                    @save="onEditConsequenceProperty(props.item, 'attribute', props.item.attribute)"
                                    large lazy persistent>
                                <v-text-field readonly single-line class="sm-txt" :value="props.item.attribute"
                                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                <template slot="input">
                                    <v-select :items="attributesSelectListItems" label="Edit"
                                              v-model="props.item.attribute"
                                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-edit-dialog :return-value.sync="props.item.change"
                                           @save="onEditConsequenceProperty(props.item, 'change', props.item.change)"
                                           large lazy persistent>
                                <v-text-field readonly single-line class="sm-txt" :value="props.item.change"
                                              :rules="[rules['treatmentRules'].changeHasEquation(props.item.change, props.item.equation)]"/>
                                <template slot="input">
                                    <v-text-field label="Edit" v-model="props.item.change"
                                                  :rules="[rules['treatmentRules'].changeHasEquation(props.item.change, props.item.equation)]"/>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-textarea full-width no-resize outline readonly rows="3" v-model="props.item.equation">
                                <template slot="append-outer">
                                    <v-btn @click="onEditConsequenceEquation(props.item)" class="edit-icon" icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-textarea full-width no-resize outline readonly rows="3" v-model="props.item.criteria">
                                <template slot="append-outer">
                                    <v-btn @click="onEditConsequenceCriteria(props.item)" class="edit-icon" icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-layout align-start>
                                <v-btn @click="onDeleteConsequence(props.item.id)" class="ara-orange" icon>
                                    <v-icon>fas fa-trash</v-icon>
                                </v-btn>
                            </v-layout>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>

        <EquationEditorDialog :dialogData="equationEditorDialogData" @submit="onSubmitEditedConsequenceEquation"/>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData"
                              @submitCriteriaEditorDialogResult="onSubmitEditedConsequenceCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State} from 'vuex-class';
    import {append, clone, findIndex, isNil, propEq, update} from 'ramda';
    import EquationEditorDialog from '../../../shared/modals/EquationEditorDialog.vue';
    import CriteriaEditorDialog from '../../../shared/modals/CriteriaEditorDialog.vue';
    import {TabData} from '@/shared/models/child-components/tab-data';
    import {
        Consequence,
        emptyConsequence,
        emptyTreatment,
        emptyTreatmentLibrary,
        Treatment,
        TreatmentLibrary
    } from '@/shared/models/iAM/treatment';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {
        emptyEquationEditorDialogData,
        EquationEditorDialogData
    } from '@/shared/models/modals/equation-editor-dialog-data';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {EquationEditorDialogResult} from '@/shared/models/modals/equation-editor-dialog-result';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {setItemPropertyValue} from '@/shared/utils/setter-utils';
    import {InputValidationRules} from '@/shared/utils/input-validation-rules';

    const ObjectID = require('bson-objectid');

    @Component({
        components: {CriteriaEditorDialog, EquationEditorDialog}
    })
    export default class ConsequencesTab extends Vue {
        @Prop() consequencesTabData: TabData;
        @Prop() rules: InputValidationRules;

        @State(state => state.attribute.attributes) stateAttributes: Attribute[];

        consequencesTabTreatmentLibraries: TreatmentLibrary[] = [];
        consequencesTabSelectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
        consequencesTabSelectedTreatment: Treatment = clone(emptyTreatment);
        consequencesGridHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: '200px'},
            {text: 'Change', value: 'change', align: 'left', sortable: false, class: '', width: '125px'},
            {text: 'Equation', value: 'equation', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '100px'}
        ];
        consequencesGridData: Consequence[] = [];
        equationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        selectedConsequence: Consequence = clone(emptyConsequence);
        attributesSelectListItems: SelectItem[] = [];

        /**
         * Component mounted event handler
         */
        mounted() {
            if (hasValue(this.stateAttributes)) {
                this.setAttributeSelectListItems();
            }
        }

        /**
         * Sets the component's data properties
         */
        @Watch('consequencesTabData')
        onConsequencesTabDataChanged() {
            this.consequencesTabTreatmentLibraries = this.consequencesTabData.tabTreatmentLibraries;
            this.consequencesTabSelectedTreatmentLibrary = this.consequencesTabData.tabSelectedTreatmentLibrary;
            this.consequencesTabSelectedTreatment = this.consequencesTabData.tabSelectedTreatment;

            this.setConsequencesGridData();
        }

        /**
         * Calls the setAttributesSelectListItems function if a change to stateAttributes causes it to have a value
         */
        @Watch('stateAttributes')
        onStateAttributesChanged() {
            if (hasValue(this.stateAttributes)) {
                this.setAttributeSelectListItems();
            }
        }

        /**
         * Sets the component's grid data
         */
        setConsequencesGridData() {
            this.consequencesGridData = hasValue(this.consequencesTabSelectedTreatment.consequences)
                ? this.consequencesTabSelectedTreatment.consequences
                : [];
        }

        /**
         * Sets the attribute select items using attributes from state
         */
        setAttributeSelectListItems() {
            this.attributesSelectListItems = this.stateAttributes.map((attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name
            }));
        }

        /**
         * Creates a new Consequence object to add to the selected treatment
         */
        onAddConsequence() {
            const newConsequence: Consequence = {...emptyConsequence, id: ObjectID.generate()};

            this.consequencesTabSelectedTreatmentLibrary = {
                ...this.consequencesTabSelectedTreatmentLibrary,
                treatments: update(
                    findIndex(
                        propEq('id', this.consequencesTabSelectedTreatment.id), this.consequencesTabSelectedTreatmentLibrary.treatments),
                    {
                        ...this.consequencesTabSelectedTreatment,
                        consequences: append(newConsequence, this.consequencesTabSelectedTreatment.consequences)
                    },
                    this.consequencesTabSelectedTreatmentLibrary.treatments
                )
            };

            this.$emit('submit', this.consequencesTabSelectedTreatmentLibrary);
        }

        /**
         * Modifies a consequence's property with a value
         * @param consequence The consequence to modify
         * @param property The property to modify
         * @param value The value to modify with
         */
        onEditConsequenceProperty(consequence: Consequence, property: string, value: any) {
            this.consequencesTabSelectedTreatmentLibrary = {
                ...this.consequencesTabSelectedTreatmentLibrary,
                treatments: update(
                    findIndex(
                        propEq('id', this.consequencesTabSelectedTreatment.id), this.consequencesTabSelectedTreatmentLibrary.treatments),
                    {
                        ...this.consequencesTabSelectedTreatment,
                        consequences: update(
                            findIndex(propEq('id', consequence.id), this.consequencesTabSelectedTreatment.consequences),
                            setItemPropertyValue(property, value, consequence),
                            this.consequencesTabSelectedTreatment.consequences
                        )
                    },
                    this.consequencesTabSelectedTreatmentLibrary.treatments
                )
            };

            this.$emit('submit', this.consequencesTabSelectedTreatmentLibrary);
        }

        /**
         * Sets the selectedConsequence and shows the EquationEditorDialog passing in the selectedConsequence's equation &
         * isFunction data
         * @param consequence The consequence to set as the selectedConsequence
         */
        onEditConsequenceEquation(consequence: Consequence) {
            this.selectedConsequence = clone(consequence);

            this.equationEditorDialogData = {
                ...emptyEquationEditorDialogData,
                showDialog: true,
                equation: this.selectedConsequence.equation
            };
        }

        /**
         * Modifies the selectedConsequence's equation & isFunction data using the EquationEditorDialog result
         * @param result EquationEditorDialog result
         */
        onSubmitEditedConsequenceEquation(result: EquationEditorDialogResult) {
            this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

            if (!isNil(result)) {
                this.consequencesTabSelectedTreatmentLibrary = {
                    ...this.consequencesTabSelectedTreatmentLibrary,
                    treatments: update(
                        findIndex(
                            propEq('id', this.consequencesTabSelectedTreatment.id), this.consequencesTabSelectedTreatmentLibrary.treatments),
                        {
                            ...this.consequencesTabSelectedTreatment,
                            consequences: update(
                                findIndex(propEq('id', this.selectedConsequence.id), this.consequencesTabSelectedTreatment.consequences),
                                {...this.selectedConsequence, equation: result.equation, isFunction: result.isFunction},
                                this.consequencesTabSelectedTreatment.consequences
                            )
                        },
                        this.consequencesTabSelectedTreatmentLibrary.treatments
                    )
                };

                this.$emit('submit', this.consequencesTabSelectedTreatmentLibrary);
            }

            this.selectedConsequence = clone(emptyConsequence);
        }

        /**
         * Sets the selectedConsequence and shows the CriteriaEditorDialog passing in the selectedConsequence's criteria data
         * @param consequence The consequence to set as the selectedConsequence
         */
        onEditConsequenceCriteria(consequence: Consequence) {
            this.selectedConsequence = clone(consequence);

            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.selectedConsequence.criteria
            };
        }

        /**
         * Modifies the selectedConsequence's criteria data using the CriteriaEditorDialog result
         * @param criteria CriteriaEditorDialog result
         */
        onSubmitEditedConsequenceCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.consequencesTabSelectedTreatmentLibrary = {
                    ...this.consequencesTabSelectedTreatmentLibrary,
                    treatments: update(
                        findIndex(
                            propEq('id', this.consequencesTabSelectedTreatment.id), this.consequencesTabSelectedTreatmentLibrary.treatments),
                        {
                            ...this.consequencesTabSelectedTreatment,
                            consequences: update(
                                findIndex(propEq('id', this.selectedConsequence.id), this.consequencesTabSelectedTreatment.consequences),
                                {...this.selectedConsequence, criteria: criteria},
                                this.consequencesTabSelectedTreatment.consequences
                            )
                        },
                        this.consequencesTabSelectedTreatmentLibrary.treatments
                    )
                };

                this.$emit('submit', this.consequencesTabSelectedTreatmentLibrary);
            }

            this.selectedConsequence = clone(emptyConsequence);
        }

        /**
         * Sends a Consequence object that has been marked for deletion to the submitChanges function
         */
        onDeleteConsequence(consequenceId: string) {
            this.consequencesTabSelectedTreatmentLibrary = {
                ...this.consequencesTabSelectedTreatmentLibrary,
                treatments: update(
                    findIndex(
                        propEq('id', this.consequencesTabSelectedTreatment.id), this.consequencesTabSelectedTreatmentLibrary.treatments),
                    {
                        ...this.consequencesTabSelectedTreatment,
                        consequences: this.consequencesTabSelectedTreatment.consequences
                            .filter((consequence: Consequence) => consequence.id !== consequenceId)
                    },
                    this.consequencesTabSelectedTreatmentLibrary.treatments
                )
            };

            this.$emit('submit', this.consequencesTabSelectedTreatmentLibrary);
        }
    }
</script>

<style>
    .consequences-tab-content {
        height: 185px;
    }

    .consequences-data-table {
        height: 215px;
        overflow-y: auto;
    }
</style>
