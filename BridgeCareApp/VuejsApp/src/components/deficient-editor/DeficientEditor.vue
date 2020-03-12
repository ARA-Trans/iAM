<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onAddNewDeficientLibrary">
                        New Library
                    </v-btn>
                    <v-select v-if="!hasSelectedDeficientLibrary || selectedScenarioId !== '0'" label="Select a Deficient Library"
                              :items="deficientLibrariesSelectListItems" v-model="selectItemValue" outline>
                    </v-select>
                    <v-text-field v-if="hasSelectedDeficientLibrary && selectedScenarioId === '0'" label="Library Name"
                                  v-model="selectedDeficientLibrary.name">
                        <template slot="append">
                            <v-btn class="ara-orange" icon @click="onClearSelectedDeficientLibrary">
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedDeficientLibrary && selectedScenarioId === '0'">
                        Owner: {{selectedDeficientLibrary.owner ? selectedDeficientLibrary.owner : "[ No Owner ]"}}
                    </div>
                    <v-checkbox class="sharing" v-if="hasSelectedDeficientLibrary && selectedScenarioId === '0'" 
                        v-model="selectedDeficientLibrary.shared" label="Shared"/>
                </v-flex>
            </v-layout>
            <v-flex xs3 v-show="hasSelectedDeficientLibrary">
                <v-btn class="ara-blue-bg white--text" @click="showCreateDeficientDialog = true">Add</v-btn>
                <v-btn class="ara-orange-bg white--text" @click="onDeleteDeficients" :disabled="selectedDeficientIds.length === 0">
                    Delete
                </v-btn>
            </v-flex>
        </v-flex>
        <v-flex xs12>
            <div class="deficients-data-table">
                <v-data-table :headers="deficientDataTableHeaders" :items="deficients" v-model="selectedDeficientRows"
                              select-all item-key="id" class="elevation-1 fixed-header v-table__overflow">
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-checkbox v-model="props.selected" primary hide-details></v-checkbox>
                        </td>
                        <td v-for="header in deficientDataTableHeaders">
                            <div v-if="header.value === 'attribute'">
                                <v-edit-dialog @save="onEditDeficientProperty(props.item, 'attribute', props.item.attribute)"
                                               :return-value.sync="props.item.attribute" large lazy persistent>
                                    <input class="output" type="text" :value="props.item.attribute" readonly />
                                    <template slot="input">
                                        <v-select :items="numericAttributes" label="Select an Attribute"
                                                  v-model="props.item.attribute">
                                        </v-select>
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-if="header.value !== 'attribute' && header.value !== 'criteria'">
                                <v-edit-dialog @save="onEditDeficientProperty(props.item, header.value, props.item[header.value])"
                                               :return-value.sync="props.item[header.value]" large lazy persistent>
                                    <input class="output" type="text" :value="props.item[header.value]" readonly />
                                    <template slot="input">
                                        <v-text-field v-model="props.item[header.value]" label="Edit" single-line>
                                        </v-text-field>
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-if="header.value === 'criteria'">
                                <v-layout align-center row style="flex-wrap:nowrap">
                                    <v-menu bottom min-width="500px" min-height="500px">
                                        <template slot="activator">
                                            <input class="output deficient-criteria-output" type="text" :value="props.item.criteria" readonly />
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea rows="5" no-resize readonly full-width outline
                                                            :value="props.item.criteria">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn icon class="edit-icon" @click="onEditDeficientCriteria(props.item)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-layout>
                            </div>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedDeficientLibrary && selectedDeficientLibrary.id !== stateScenarioDeficientLibrary.id">
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea rows="4" no-resize outline label="Description" v-model="selectedDeficientLibrary.description">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedDeficientLibrary">
            <v-layout justify-end row>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-blue-bg white--text" @click="onApplyDeficientLibraryToScenario">
                    Save
                </v-btn>
                <v-btn v-show="selectedScenarioId === '0'" class="ara-blue-bg white--text" @click="onUpdateDeficientLibrary">
                    Update Library
                </v-btn>
                <v-btn class="ara-blue-bg white--text" @click="onAddAsNewDeficientLibrary">
                    Create as New Library
                </v-btn>
                <v-btn v-show="selectedScenarioId === '0'" class="ara-orange-bg white--text" @click="onDeleteDeficientLibrary">
                    Delete Library
                </v-btn>
                <v-btn v-show="selectedScenarioId !== '0'" class="ara-orange-bg white--text" @click="onDiscardDeficientLibraryChanges">
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <Alert :dialogData="alertBeforeDelete" @submit="onSubmitDeleteResponse" />

        <CreateDeficientLibraryDialog :dialogData="createDeficientLibraryDialogData" @submit="onCreateNewDeficientLibrary" />

        <CreateDeficientDialog :showDialog="showCreateDeficientDialog" @submit="onSubmitNewDeficient" />

        <DeficientCriteriaEditor :dialogData="deficientCriteriaEditorDialogData" @submit="onSubmitDeficientCriteria" />
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {State, Action, Getter} from 'vuex-class';
    import {Deficient, DeficientLibrary, emptyDeficient, emptyDeficientLibrary} from '@/shared/models/iAM/deficient';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {clone, isNil, prepend, findIndex, propEq, update, find, contains} from 'ramda';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import CreateDeficientDialog from '@/components/deficient-editor/deficient-editor-dialogs/CreateDeficientDialog.vue';
    import {
        CreateDeficientLibraryDialogData,
        emptyCreateDeficientLibraryDialogData
    } from '@/shared/models/modals/create-deficient-library-dialog-data';
    import {setItemPropertyValue} from '@/shared/utils/setter-utils';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import CreateDeficientLibraryDialog from '@/components/deficient-editor/deficient-editor-dialogs/CreateDeficientLibraryDialog.vue';
    import {Attribute} from '@/shared/models/iAM/attribute';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';

    @Component({
        components: {CreateDeficientLibraryDialog, CreateDeficientDialog, DeficientCriteriaEditor: CriteriaEditorDialog, Alert}
    })
    export default class DeficientEditor extends Vue {
        @State(state => state.deficientEditor.deficientLibraries) stateDeficientLibraries: DeficientLibrary[];
        @State(state => state.deficientEditor.selectedDeficientLibrary) stateSelectedDeficientLibrary: DeficientLibrary;
        @State(state => state.deficientEditor.scenarioDeficientLibrary) stateScenarioDeficientLibrary: DeficientLibrary;
        @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getDeficientLibraries') getDeficientLibrariesAction: any;
        @Action('selectDeficientLibrary') selectDeficientLibraryAction: any;
        @Action('createDeficientLibrary') createDeficientLibraryAction: any;
        @Action('updateDeficientLibrary') updateDeficientLibraryAction: any;
        @Action('deleteDeficientLibrary') deleteDeficientLibraryAction: any;
        @Action('getScenarioDeficientLibrary') getScenarioDeficientLibraryAction: any;
        @Action('saveScenarioDeficientLibrary') saveScenarioDeficientLibraryAction: any;
        @Action('getAttributes') getAttributesAction: any;

        @Getter('getNumericAttributes') getNumericAttributesGetter: any;

        selectedScenarioId: string = '0';
        deficientLibrariesSelectListItems: SelectItem[] = [];
        selectItemValue: string = '';
        selectedDeficientLibrary: DeficientLibrary = clone(emptyDeficientLibrary);
        hasSelectedDeficientLibrary: boolean = false;
        deficients: Deficient[] = [];
        deficientDataTableHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Name', value: 'name', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Deficient Level', value: 'deficient', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Allowed Deficient(%)', value: 'percentDeficient', align: 'left', sortable: false, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '50%'}
        ];
        numericAttributes: string[] = [];
        selectedDeficientRows: Deficient[] = [];
        selectedDeficientIds: string[] = [];
        selectedDeficient: Deficient = clone(emptyDeficient);
        showCreateDeficientDialog: boolean = false;
        deficientCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        createDeficientLibraryDialogData: CreateDeficientLibraryDialogData = clone(emptyCreateDeficientLibraryDialogData);
        alertBeforeDelete: AlertData = clone(emptyAlertData);

        /**
         * Sets onload component UI properties
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/DeficientEditor/Scenario/') {
                    vm.selectedScenarioId = to.query.selectedScenarioId;

                    if (vm.selectedScenarioId === '0') {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                vm.onClearSelectedDeficientLibrary();
                setTimeout(() => {
                    vm.getDeficientLibrariesAction()
                    .then(() => {
                        if (vm.selectedScenarioId !== '0') {
                            vm.getScenarioDeficientLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});
                        }
                    });
                });
            });
        }

        /**
         * Resets onload component UI properties
         */
        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === '/DeficientEditor/Library/') {
                this.selectedScenarioId = '0';
                this.onClearSelectedDeficientLibrary();
                next();
            }
        }

        /**
         * Sets the component's deficientLibrariesSelectListItems array using the deficient libraries data in state
         */
        @Watch('stateDeficientLibraries')
        onStateDeficientLibrariesChanged() {
            this.deficientLibrariesSelectListItems = this.stateDeficientLibraries.map((deficientLibrary: DeficientLibrary) => ({
                text: deficientLibrary.name,
                value: deficientLibrary.id
            }));
        }

        /**
         * Dispatches an action to set the selected deficient library in state
         */
        @Watch('selectItemValue')
        onSelectItemValueChanged() {
            const selectedDeficientLibrary: DeficientLibrary = find(
                propEq('id', this.selectItemValue), this.stateDeficientLibraries
            ) as DeficientLibrary;

            this.selectDeficientLibraryAction({
                selectedDeficientLibrary: hasValue(selectedDeficientLibrary) ? selectedDeficientLibrary : clone(emptyDeficientLibrary)
            });
        }

        /**
         * Sets the component's selectedDeficientLibrary object with the selected deficient library data in state
         */
        @Watch('stateSelectedDeficientLibrary')
        onStateSelectedDeficientLibraryChanged() {
            this.selectedDeficientLibrary = clone(this.stateSelectedDeficientLibrary);
        }

        /**
         * Sets component UI properties based on the selected deficient library data
         */
        @Watch('selectedDeficientLibrary')
        onSelectedDeficientLibraryChanged() {
            this.hasSelectedDeficientLibrary = this.selectedDeficientLibrary.id !== '0';
            this.deficients = clone(this.selectedDeficientLibrary.deficients);
            if (this.numericAttributes.length === 0) {
                this.numericAttributes = getPropertyValues('name', this.getNumericAttributesGetter);
            }
        }

        /**
         * Sets selectedTargetIds using selectedDeficientRows
         */
        @Watch('selectedDeficientRows')
        onSelectedDeficientRowsChanged() {
            this.selectedDeficientIds = getPropertyValues('id', this.selectedDeficientRows) as string[];
        }

        @Watch('stateNumericAttributes')
        onStateNumericAttributesChanged() {
            this.numericAttributes = getPropertyValues('name', this.stateNumericAttributes);
        }

        /**
         * Sets selectItemValue to '' or '0' to de-select the selected deficient library
         */
        onClearSelectedDeficientLibrary() {
            this.selectItemValue = hasValue(this.selectItemValue) ? '' : '0';
        }

        /**
         * Enables the CreateDeficientLibraryDialog
         */
        onAddNewDeficientLibrary() {
            this.createDeficientLibraryDialogData = {
                ...this.createDeficientLibraryDialogData,
                showDialog: true
            };
        }

        /**
         * Adds the CreateDeficientDialog's result to the selected deficient library's deficients list
         * @param newDeficient CreateDeficientDialog's Deficient object result
         */
        onSubmitNewDeficient(newDeficient: Deficient) {
            this.showCreateDeficientDialog = false;

            if (!isNil(newDeficient)) {
                this.selectDeficientLibraryAction({selectedDeficientLibrary: {
                        ...this.selectedDeficientLibrary,
                        deficients: prepend(newDeficient, this.selectedDeficientLibrary.deficients)
                    }
                });
            }
        }

        /**
         * Updates the selected deficient's property with the value
         * @param deficient Selected Deficient object
         * @param property Selected Deficient object's property
         * @param value Value to set on the selected Deficient object's property
         */
        onEditDeficientProperty(deficient: Deficient, property: string, value: any) {
            this.selectDeficientLibraryAction({selectedDeficientLibrary: {
                    ...this.selectedDeficientLibrary,
                    deficients: update(
                        findIndex(propEq('id', deficient.id), this.selectedDeficientLibrary.deficients),
                        setItemPropertyValue(property, value, deficient) as Deficient,
                        this.selectedDeficientLibrary.deficients
                    )
                }});
        }

        /**
         * Enables the CriteriaEditorDialog and sends to it the selected deficient's criteria
         * @param deficient Selected Deficient object
         */
        onEditDeficientCriteria(deficient: Deficient) {
            this.selectedDeficient = clone(deficient);

            this.deficientCriteriaEditorDialogData = {
                showDialog: true,
                criteria: deficient.criteria
            };
        }

        /**
         * Updates the selected deficient's criteria with the CriteriaEditorDialog's result
         * @param criteria CriteriaEditorDialog result
         */
        onSubmitDeficientCriteria(criteria: string) {
            this.deficientCriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectedDeficient.criteria = criteria;

                this.selectDeficientLibraryAction({selectedDeficientLibrary: {
                        ...this.selectedDeficientLibrary,
                        deficients: update(
                            findIndex(propEq('id', this.selectedDeficient.id), this.selectedDeficientLibrary.deficients),
                            this.selectedDeficient,
                            this.selectedDeficientLibrary.deficients
                        )
                    }
                });
            }

            this.selectedDeficient = clone(emptyDeficient);
        }

        /**
         * Enables the CreateDeficientLibraryDialog and sends to it the selected deficient library's description &
         * deficients data
         */
        onAddAsNewDeficientLibrary() {
            this.createDeficientLibraryDialogData = {
                showDialog: true,
                description: this.selectedDeficientLibrary.description,
                deficients: this.selectedDeficientLibrary.deficients
            };
        }

        /**
         * Dispatches an action to create a new deficient library in the mongo database
         * @param createdDeficientLibrary New DeficientLibrary object
         */
        onCreateNewDeficientLibrary(createdDeficientLibrary: DeficientLibrary) {
            this.createDeficientLibraryDialogData = clone(emptyCreateDeficientLibraryDialogData);

            if (!isNil(createdDeficientLibrary)) {
                this.createDeficientLibraryAction({createdDeficientLibrary: createdDeficientLibrary})
                    .then(() => this.selectItemValue = createdDeficientLibrary.id);
            }
        }

        /**
         * Dispatches an action to update the scenario's deficient library data in the sql server database
         */
        onApplyDeficientLibraryToScenario() {
            this.saveScenarioDeficientLibraryAction({saveScenarioDeficientLibraryData: {
                    ...this.selectedDeficientLibrary,
                    id: this.selectedScenarioId
                }
            }).then(() => this.onDiscardDeficientLibraryChanges());
        }

        /**
         * Dispatches an action to remove selected deficients from the selected deficient library
         */
        onDeleteDeficients() {
            this.selectDeficientLibraryAction({selectedDeficientLibrary: {
                    ...this.selectedDeficientLibrary,
                    deficients: this.selectedDeficientLibrary.deficients
                        .filter((deficient: Deficient) => !contains(deficient.id, this.selectedDeficientIds))
                }
            });
        }

        /**
         * Dispatches an action to clear changes made to the selected deficient library
         */
        onDiscardDeficientLibraryChanges() {
            this.onClearSelectedDeficientLibrary();
            setTimeout(() => {
                this.selectDeficientLibraryAction({selectedDeficientLibrary: this.stateScenarioDeficientLibrary});
            });
        }

        /**
         * Dispatches an action to update the selected deficient library data in the mongo database
         */
        onUpdateDeficientLibrary() {
            this.updateDeficientLibraryAction({updatedDeficientLibrary: this.selectedDeficientLibrary});
        }

        onDeleteDeficientLibrary() {
            this.alertBeforeDelete = {
                showDialog: true,
                heading: 'Warning',
                choice: true,
                message: 'Are you sure you want to delete?'
            };
        }

        onSubmitDeleteResponse(response: boolean) {
            this.alertBeforeDelete = clone(emptyAlertData);
            
            if (response) {
                this.deleteDeficientLibraryAction({deficientLibrary: this.selectedDeficientLibrary});
                this.onClearSelectedDeficientLibrary();
            }
        }
    }
</script>

<style>
    .deficients-data-table {
        height: 425px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .deficients-data-table .v-menu--inline, .deficient-criteria-output {
        width: 100%;
    }

    .sharing label {
        padding-top: 0.5em;
    }

    .sharing {
        padding-top: 0;
        margin: 0;
    }
</style>