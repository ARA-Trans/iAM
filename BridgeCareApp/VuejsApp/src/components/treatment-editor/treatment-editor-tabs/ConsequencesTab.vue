<template>
    <v-container fluid grid-list-xl>
        <v-layout fill-height>
            <v-flex xs12>
                <v-btn color="info" v-on:click="onAddConsequence">Add Consequence</v-btn>
                <div class="consequences-data-table">
                    <v-data-table :headers="consequencesGridHeaders" :items="consequencesGridData" hide-actions
                                  class="elevation-1 fixed-header v-table__overflow">
                        <template slot="items" slot-scope="props">
                            <td>
                                <v-edit-dialog @save="onEditConsequenceProperty(props.item, 'attribute', props.item.attribute)"
                                               :return-value.sync="props.item.attribute" large lazy persistent>
                                    <v-text-field readonly :value="props.item.attribute"></v-text-field>
                                    <template slot="input">
                                        <v-select :items="attributes" v-model="props.item.attribute"
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
                                <v-textarea rows="3" readonly no-resize full-width outline append-outer-icon="edit"
                                            @click:append-outer="onEditConsequenceEquation(props.item)"
                                            v-model="props.item.equation">
                                </v-textarea>
                            </td>
                            <td>
                                <v-textarea rows="3" readonly no-resize full-width outline append-outer-icon="edit"
                                            @click:append-outer="onEditConsequenceCriteria(props.item)"
                                            v-model="props.item.criteria">
                                </v-textarea>
                            </td>
                            <td>
                                <v-layout align-start fill-height>
                                    <v-btn color="error" icon v-on:click="onDeleteConsequence(props.item)">
                                        <v-icon>delete</v-icon>
                                    </v-btn>
                                </v-layout>
                            </td>
                        </template>
                    </v-data-table>
                </div>
            </v-flex>
        </v-layout>

        <EquationEditor :dialogData="equationEditorDialogData" @submit="onSubmitEditedConsequenceEquation" />

        <CriteriaEditor :dialogData="criteriaEditorDialogData" @submit="onSubmitEditedConsequenceCriteria"/>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {isNil, findIndex, uniq} from 'ramda';
    import EquationEditor from '../../../shared/dialogs/EquationEditor.vue';
    import CriteriaEditor from '../../../shared/dialogs/CriteriaEditor.vue';
    import {TabData} from '@/shared/models/child-components/treatment-editor/tab-data';
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
    } from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-data';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/dialogs/criteria-editor-dialog/criteria-editor-dialog-data';
    import {hasValue} from '@/shared/utils/has-value';
    import {getLatestPropertyValue} from '@/shared/utils/getter-utils';
    import {EquationEditorDialogResult} from '@/shared/models/dialogs/equation-editor-dialog/equation-editor-dialog-result';

    @Component({
        components: {CriteriaEditor, EquationEditor}
    })
    export default class ConsequencesTab extends Vue {
        @Prop() consequencesTabData: TabData;

        @State(state => state.attribute.attributes) attributes: string[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getAttributes') getAttributesAction: any;

        consequencesTabTreatmentStrategies: TreatmentLibrary[] = [];
        consequencesTabSelectedTreatmentStrategy: TreatmentLibrary = {...emptyTreatmentLibrary};
        consequencesTabSelectedTreatment: Treatment = {...emptyTreatment};

        consequencesGridHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: '200px'},
            {text: 'Change', value: 'change', align: 'left', sortable: false, class: '', width: '125px'},
            {text: 'Equation', value: 'equation', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''},
            {text: '', value: '', align: 'left', sortable: false, class: '', width: '100px'}
        ];
        consequencesGridData: Consequence[] = [];
        equationEditorDialogData: EquationEditorDialogData = {...emptyEquationEditorDialogData};
        criteriaEditorDialogData: CriteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
        selectedConsequence: Consequence = {...emptyConsequence};
        allConsequences: Consequence[] = [];

        /**
         * Sets the ConsequencesTab's required UI functionality properties
         */
        @Watch('consequencesTabData')
        onConsequencesTabDataChanged() {
            this.consequencesTabTreatmentStrategies = this.consequencesTabData.tabTreatmentLibraries;
            this.consequencesTabSelectedTreatmentStrategy = this.consequencesTabData.tabSelectedTreatmentLibrary;
            this.consequencesTabSelectedTreatment = this.consequencesTabData.tabSelectedTreatment;
            this.setAllConsequences();
            this.setConsequencesGridData();
        }

        /**
         * Component has been mounted
         */
        mounted() {
            // set isBusy to true, then dispatch action to get all attributes
            this.setIsBusyAction({isBusy: true});
            this.getAttributesAction()
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }

        /**
         * Sets allConsequences property based on costsTabTreatmentStrategies data
         */
        setAllConsequences() {
            this.consequencesTabTreatmentStrategies.forEach((treatmentStrategy: TreatmentLibrary) => {
                if (treatmentStrategy.id === this.consequencesTabSelectedTreatmentStrategy.id) {
                    this.consequencesTabSelectedTreatmentStrategy.treatments.forEach((treatment: Treatment) => {
                        if (hasValue(treatment.consequences)) {
                            this.allConsequences.push(...treatment.consequences);
                        }
                    });
                } else {
                    treatmentStrategy.treatments.forEach((treatment: Treatment) => {
                        if (hasValue(treatment.consequences)) {
                            this.allConsequences.push(...treatment.consequences);
                        }
                    });
                }
            });
            this.allConsequences = uniq(this.allConsequences);
        }

        /**
         * Sets consequencesGridData property based on costsTabSelectedTreatment data
         */
        setConsequencesGridData() {
            if (this.consequencesTabSelectedTreatment.id !== 0 && this.consequencesTabSelectedTreatment.consequences.length > 0) {
                this.consequencesGridData = [...this.consequencesTabSelectedTreatment.consequences];
            } else {
                this.consequencesGridData = [];
            }
        }

        /**
         * Creates a new Consequence object to add to the selected treatment
         */
        onAddConsequence() {
            const latestId: number = getLatestPropertyValue('id', this.allConsequences);
            const newConsequence: Consequence = {
                ...emptyConsequence,
                treatmentId: this.consequencesTabSelectedTreatment.id,
                id: hasValue(latestId) ? latestId + 1 : 1
            };
            this.submitChanges(newConsequence, false);
        }

        /**
         * Edits the given consequence's specified property with the specified value
         */
        onEditConsequenceProperty(consequence: Consequence, property: string, value: any) {
                const updatedConsequence: Consequence = {...consequence};
                // @ts-ignore
                updatedConsequence[property] = value;
                this.submitChanges(updatedConsequence, false);
        }

        /**
         * Sets selectedConsequence with the given consequence, then sets equationEditorDialogData with
         * selectedConsequence data
         * @param consequence The consequence to set as selectedConsequence
         */
        onEditConsequenceEquation(consequence: Consequence) {
            this.selectedConsequence = {...consequence};
            this.equationEditorDialogData = {
                ...emptyEquationEditorDialogData,
                showDialog: true,
                equation: this.selectedConsequence.equation,
                isFunction: this.selectedConsequence.isFunction
            };
        }

        /**
         * Updates the selectedConsequence.equation & selectedConsequence.isFunction based on the user submitted result
         * from the EquationEditor
         * @param result User's submitted EquationEditor result
         */
        onSubmitEditedConsequenceEquation(result: EquationEditorDialogResult) {
            this.equationEditorDialogData = {...emptyEquationEditorDialogData};
            if (!isNil(result)) {
                const updatedConsequence: Consequence = {...this.selectedConsequence};
                this.selectedConsequence = {...emptyConsequence};
                updatedConsequence.equation = result.equation;
                updatedConsequence.isFunction = result.isFunction;
                this.submitChanges(updatedConsequence, false);
            }
        }

        /**
         * Sets selectedConsequence with the given consequence, then sets criteriaEditorDialogData with
         * selectedConsequence data
         * @param consequence The consequence to set as selectedConsequence
         */
        onEditConsequenceCriteria(consequence: Consequence) {
            this.selectedConsequence = {...consequence};
            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.selectedConsequence.criteria
            };
        }

        /**
         * Updates the selectedConsequence.criteria based on the user submitted result from the CriteriaEditor
         * @param criteria User's submitted CriteriaEditor result
         */
        onSubmitEditedConsequenceCriteria(criteria: string) {
            this.criteriaEditorDialogData = {...emptyCriteriaEditorDialogData};
            if (!isNil(criteria)) {
                const updatedConsequence: Consequence = {...this.selectedConsequence};
                this.selectedConsequence = {...emptyConsequence};
                updatedConsequence.criteria = criteria;
                this.submitChanges(updatedConsequence, false);
            }
        }

        /**
         * A Consequence 'Delete' button has been clicked
         */
        onDeleteConsequence(consequence: Consequence) {
            this.submitChanges(consequence, true);
        }

        /**
         * Submits consequence data changes
         * @param consequenceData The consequence data to submit changes on
         * @param forDelete Whether or not the consequence data is to be used for deleting a consequence
         */
        submitChanges(consequenceData: Consequence, forDelete: boolean) {
            // update selected treatment data
            const updatedTreatment: Treatment = {...this.consequencesTabSelectedTreatment};
            if (forDelete) {
                updatedTreatment.consequences = updatedTreatment.consequences
                    .filter((consequence: Consequence) => consequence.id !== consequenceData.id);
            } else {
                const updatedConsequenceIndex: number = findIndex((consequence: Consequence) =>
                    consequence.id === consequenceData.id, updatedTreatment.consequences
                );
                if (updatedConsequenceIndex === -1) {
                    updatedTreatment.consequences = [...updatedTreatment.consequences, consequenceData];
                } else {
                    updatedTreatment.consequences[updatedConsequenceIndex] = consequenceData;
                }
            }
            // update selected treatment strategy data
            const updatedTreatmentStrategy: TreatmentLibrary = {...this.consequencesTabSelectedTreatmentStrategy};
            const updatedTreatmentIndex: number = findIndex(
                (treatment: Treatment) => treatment.id === updatedTreatment.id, updatedTreatmentStrategy.treatments
            );
            updatedTreatmentStrategy.treatments[updatedTreatmentIndex] = updatedTreatment;
            this.$emit('submit', updatedTreatmentStrategy);
        }
    }
</script>

<style>
    .consequences-data-table {
        height: 245px;
        overflow-y: auto;
    }
</style>