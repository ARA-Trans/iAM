<template>
    <v-layout class="consequences-tab-content">
        <v-flex xs12>
            <v-btn class="ara-blue-bg white--text" @click="onAddConsequence">Add Consequence</v-btn>
            <div class="consequences-data-table">
                <v-data-table :headers="consequencesGridHeaders" :items="consequencesGridData" hide-actions
                              class="elevation-1 fixed-header v-table__overflow">
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-edit-dialog @save="onEditConsequenceProperty(props.item, 'attribute', props.item.attribute)"
                                           :return-value.sync="props.item.attribute" large lazy persistent>
                                <v-text-field readonly :value="props.item.attribute"></v-text-field>
                                <template slot="input">
                                    <v-select :items="attributesSelectListItems" v-model="props.item.attribute"
                                              label="Edit">
                                    </v-select>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-edit-dialog @save="onEditConsequenceProperty(props.item, 'change', props.item.change)"
                                           :return-value.sync="props.item.change" large lazy persistent>
                                <v-text-field readonly :value="props.item.change"></v-text-field>
                                <template slot="input">
                                    <v-text-field v-model="props.item.change" label="Edit"></v-text-field>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-textarea rows="3" readonly no-resize full-width outline v-model="props.item.equation">
                                <template slot="append-outer">
                                    <v-btn class="edit-icon" icon @click="onEditConsequenceEquation(props.item)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-textarea rows="3" readonly no-resize full-width outline v-model="props.item.criteria">
                                <template slot="append-outer">
                                    <v-btn class="edit-icon" icon @click="onEditConsequenceCriteria(props.item)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-layout align-start>
                                <v-btn class="ara-orange" icon @click="onDeleteConsequence(props.item)">
                                    <v-icon>fas fa-trash</v-icon>
                                </v-btn>
                            </v-layout>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>

        <EquationEditorDialog :dialogData="equationEditorDialogData" @submit="onSubmitEditedConsequenceEquation" />

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitEditedConsequenceCriteria"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State} from 'vuex-class';
    import {isNil, findIndex, clone, append} from 'ramda';
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
    const ObjectID = require('bson-objectid');

    @Component({
        components: {CriteriaEditorDialog, EquationEditorDialog}
    })
    export default class ConsequencesTab extends Vue {
        @Prop() consequencesTabData: TabData;

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
            const newConsequence: Consequence = {
                ...clone(emptyConsequence),
                id: ObjectID.generate()
            };
            this.submitChanges(newConsequence, false);
        }

        /**
         * Modifies a consequence's property with a value
         * @param consequence The consequence to modify
         * @param property The property to modify
         * @param value The value to modify with
         */
        onEditConsequenceProperty(consequence: Consequence, property: string, value: any) {
                const updatedConsequence: Consequence = clone(consequence);
                // @ts-ignore
                updatedConsequence[property] = value;
                this.submitChanges(updatedConsequence, false);
        }

        /**
         * Sets the selectedConsequence and shows the EquationEditorDialog passing in the selectedConsequence's equation &
         * isFunction data
         * @param consequence The consequence to set as the selectedConsequence
         */
        onEditConsequenceEquation(consequence: Consequence) {
            this.selectedConsequence = clone(consequence);

            this.equationEditorDialogData = {
                ...clone(emptyEquationEditorDialogData),
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
                const updatedConsequence: Consequence = clone(this.selectedConsequence);
                this.selectedConsequence = clone(emptyConsequence);

                updatedConsequence.equation = result.equation;
                updatedConsequence.isFunction = result.isFunction;

                this.submitChanges(updatedConsequence, false);
            }
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
                const updatedConsequence: Consequence = clone(this.selectedConsequence);
                this.selectedConsequence = clone(emptyConsequence);

                updatedConsequence.criteria = criteria;

                this.submitChanges(updatedConsequence, false);
            }
        }

        /**
         * Sends a Consequence object that has been marked for deletion to the submitChanges function
         */
        onDeleteConsequence(consequence: Consequence) {
            this.submitChanges(consequence, true);
        }

        /**
         * Modifies the selected treatment & selected treatment library with a Consequence object's data changes and
         * emits the modified objects to the parent component
         * @param consequenceData The Consequence object's data to modify the selected treatment library with
         * @param forDelete Whether or not the Consequence object's data is marked for deletion
         */
        submitChanges(consequenceData: Consequence, forDelete: boolean) {
            if (forDelete) {
                this.consequencesTabSelectedTreatment.consequences = this.consequencesTabSelectedTreatment.consequences
                    .filter((consequence: Consequence) => consequence.id !== consequenceData.id);
            } else {
                const updatedConsequenceIndex: number = findIndex((consequence: Consequence) =>
                    consequence.id === consequenceData.id, this.consequencesTabSelectedTreatment.consequences
                );
                if (updatedConsequenceIndex === -1) {
                    this.consequencesTabSelectedTreatment.consequences = append(
                        consequenceData, this.consequencesTabSelectedTreatment.consequences
                    );
                } else {
                    this.consequencesTabSelectedTreatment.consequences[updatedConsequenceIndex] = consequenceData;
                }
            }

            const updatedTreatmentIndex: number = findIndex((treatment: Treatment) =>
                treatment.id === this.consequencesTabSelectedTreatment.id,
                this.consequencesTabSelectedTreatmentLibrary.treatments
            );
            this.consequencesTabSelectedTreatmentLibrary
                .treatments[updatedTreatmentIndex] = this.consequencesTabSelectedTreatment;

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