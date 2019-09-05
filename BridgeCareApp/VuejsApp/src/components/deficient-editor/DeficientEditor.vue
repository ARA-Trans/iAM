<template>
    <v-layout column>
        <v-flex xs12>
            <v-btn class="ara-blue-bg white--text" @click="onAddDeficient">Add</v-btn>
        </v-flex>
        <v-flex xs12>
            <div class="deficients-data-table">
                <v-data-table :headers="deficientDataTableHeaders" :items="deficients"
                              class="elevation-1 fixed-header v-table__overflow">
                    <template slot="items" slot-scope="props">
                        <td v-for="header in deficientDataTableHeaders">
                            <div v-if="header.value !== 'criteria'">
                                <v-edit-dialog :return-value.sync="props.item[header.value]" large lazy persistent
                                               @save="onEditDeficientProperty(props.item.id, header.value, props.item[header.value])">
                                    <v-text-field readonly :value="props.item[header.value]"></v-text-field>
                                    <template slot="input">
                                        <v-text-field v-model="props.item[header.value]" label="Edit" single-line>
                                        </v-text-field>
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-else>
                                <v-text-field readonly :value="props.item.criteria">
                                    <template slot="append-outer">
                                        <v-icon class="ara-yellow" @click="onEditCriteria(props.item)">
                                            fas fa-edit
                                        </v-icon>
                                    </template>
                                </v-text-field>
                            </div>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>
        <v-flex xs12>
            <v-layout justify-end row>
                <v-btn class="ara-blue-bg white--text" @click="onSaveDeficients" :disabled="deficients.length === 0">
                    Save
                </v-btn>
                <v-btn class="ara-orange-bg white--text" @click="onCancelChangesToDeficients" :disabled="deficients.length === 0">
                    Cancel
                </v-btn>
            </v-layout>
        </v-flex>

        <CreateDeficientDialog :showDialog="showCreateDeficientDialog" @submit="onSubmitNewDeficient" />

        <DeficientCriteriaEditor :dialogData="deficientCriteriaEditorDialogData" @submit="onSubmitDeficientCriteria" />
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {Deficient, DeficientLibrary} from '@/shared/models/iAM/deficient';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {clone, isNil, append, any, propEq} from 'ramda';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import CreateDeficientDialog from '@/components/deficient-editor/deficient-editor-dialogs/CreateDeficientDialog.vue';

    @Component({
        components: {CreateDeficientDialog, DeficientCriteriaEditor: CriteriaEditorDialog}
    })
    export default class DeficientsTab extends Vue {
        @State(state => state.deficient.scenarioDeficientLibrary) stateScenarioDeficientLibrary: DeficientLibrary;

        @Action('getScenarioDeficientLibrary') getScenarioDeficientLibraryAction: any;
        @Action('saveScenarioDeficientLibrary') saveScenarioDeficientLibraryAction: any;

        selectedScenarioId: number = 0;
        deficients: Deficient[] = [];
        deficientDataTableHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: true, class: '', width: '14%'},
            {text: 'Name', value: 'name', align: 'left', sortable: true, class: '', width: '14%'},
            {text: 'Deficient Level', value: 'deficient', align: 'left', sortable: true, class: '', width: '10%'},
            {text: 'Allowed Deficient(%)', value: 'percentDeficient', align: 'left', sortable: true, class: '', width: '11%'},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '50%'}
        ];
        selectedDeficientIndex: number = -1;
        showCreateDeficientDialog: boolean = false;
        deficientCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

        /**
         * Sets component UI properties that triggers cascading UI updates
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

                vm.getScenarioDeficientLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});

                /*vm.onClearSelectedPriorityLibrary();
                setTimeout(() => {
                    vm.getDeficientLibrariesAction()
                    .then(() => {
                        if (vm.selectedScenarioId > 0) {
                            vm.getScenarioDeficientLibraryAction({selectedScenarioId: parseInt(vm.selectedScenarioId)});
                        }
                    });
                });*/
            });
        }

        /**
         * Resets component UI properties that triggers cascading UI updates
         */
        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === '/DeficientEditor/Library/') {
                this.selectedScenarioId = 0;
                // this.onClearSelectedTargetLibrary();
                next();
            }
        }

        /**
         * Sets the deficients list property with a copy of the stateDeficients property when stateDeficients list
         * changes are detected
         */
        @Watch('stateScenarioDeficientLibrary')
        onStateDeficientsChanged() {
            this.deficients = clone(this.stateScenarioDeficientLibrary.deficients);
        }

        /**
         * Sets showCreateDeficientDialog property to true
         */
        onAddDeficient() {
            this.showCreateDeficientDialog = true;
        }

        /**
         * Receives a Deficient object result from the CreateDeficientDialog and adds it to the deficients list property
         * @param newDeficient Deficient object
         */
        onSubmitNewDeficient(newDeficient: Deficient) {
            this.showCreateDeficientDialog = false;

            if (!isNil(newDeficient)) {
                this.deficients = append(newDeficient, this.deficients);
            }
        }

        onEditDeficientProperty(deficientId: any, property: string, value: any) {
            if (any(propEq('id', deficientId), this.deficients)) {
                const index: number = this.deficients.findIndex((deficient: Deficient) => deficient.id === deficientId);
                switch (property) {
                    case 'deficient':
                    case 'percentDeficient':
                        // @ts-ignore
                        this.deficients[index][property] = parseInt(value);
                        break;
                    default:
                        // @ts-ignore
                        this.deficients[index][property] = value;
                }
            }
        }

        /**
         * Sets selectedDeficient property with the specified deficient and sets the criteriaEditorDialogData property
         * values with the selectedDeficient.criteria property
         * @param deficient Deficient object
         */
        onEditCriteria(deficient: Deficient) {
            this.selectedDeficientIndex = this.deficients.findIndex((d: Deficient) => d.id === deficient.id);

            this.deficientCriteriaEditorDialogData = {
                showDialog: true,
                criteria: deficient.criteria
            };
        }

        /**
         * Receives a criteria string result from the CriteriaEditor and sets the selectedDeficient.criteria property
         * with the criteria result (if present)
         * @param criteria CriteriaEditor criteria result
         */
        onSubmitDeficientCriteria(criteria: string) {
            this.deficientCriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.deficients[this.selectedDeficientIndex].criteria = criteria;
                this.selectedDeficientIndex = -1;
            }
        }

        /**
         * Sends deficient data changes to the server for upsert
         */
        onSaveDeficients() {
            this.saveScenarioDeficientLibraryAction({scenarioDeficientLibraryData: {
                    ...this.stateScenarioDeficientLibrary,
                    deficients: clone(this.deficients)
                }
            });
        }

        /**
         * Discards deficient data changes by resetting the deficients list with a new copy of the stateDeficients list
         */
        onCancelChangesToDeficients() {
            this.deficients = clone(this.stateScenarioDeficientLibrary.deficients);
        }
    }
</script>