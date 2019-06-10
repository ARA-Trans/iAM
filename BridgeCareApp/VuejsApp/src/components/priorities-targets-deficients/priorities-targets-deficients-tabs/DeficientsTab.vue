<template>
    <v-container fluid grid-list-xl>
        <div>
            <v-layout>
                <v-flex>
                    <v-btn color="info" @click="onAddDeficient">Add</v-btn>
                </v-flex>
            </v-layout>
            <v-layout>
                <v-flex>
                    <div class="deficients-data-table">
                        <v-data-table :headers="deficientDataTableHeaders" :items="deficients"
                                      class="elevation-1 fixed-header v-table__overflow" hide-actions>
                            <template slot="items" slot-scope="props">
                                <td v-for="header in deficientDataTableHeaders">
                                    <div v-if="header.value !== 'criteria'">
                                        <v-edit-dialog :return-value.sync="props.item[header.value]" large lazy persistent>
                                            <v-text-field readonly :value="props.item[header.value]"></v-text-field>
                                            <template slot="input">
                                                <v-text-field v-model="props.item[header.value]" label="Edit" single-line>
                                                </v-text-field>
                                            </template>
                                        </v-edit-dialog>
                                    </div>
                                    <div v-else>
                                        <v-text-field readonly :value="props.item.criteria" append-outer-icon="edit"
                                                      @click:append-outer="onEditCriteria(props.item)">
                                        </v-text-field>
                                    </div>
                                </td>
                            </template>
                        </v-data-table>
                    </div>
                </v-flex>
            </v-layout>
        </div>

        <CreateDeficientDialog :showDialog="showCreateDeficientDialog" @submit="onSubmitNewDeficient" />

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitDeficientCriteria" />

        <v-footer>
            <v-layout class="priorities-targets-deficients-buttons" justify-end row fill-height>
                <v-btn color="info" @click="onSaveDeficients">Save</v-btn>
                <v-btn color="error" @click="onCancelChangesToDeficients">Cancel</v-btn>
            </v-layout>
        </v-footer>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {Deficient} from '@/shared/models/iAM/deficient';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {clone, isNil, append} from 'ramda';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import CreateDeficientDialog from '@/components/priorities-targets-deficients/dialogs/deficients-dialogs/CreateDeficientDialog.vue';
    import {getLatestPropertyValue} from '@/shared/utils/getter-utils';

    @Component({
        components: {CreateDeficientDialog, CriteriaEditorDialog}
    })
    export default class DeficientsTab extends Vue {
        @State(state => state.deficient.deficients) stateDeficients: Deficient[];

        @Action('saveDeficients') saveDeficientsAction: any;

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
        latestDeficientId: number = 0;
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

        /**
         * Sets the deficients list property with a copy of the stateDeficients property when stateDeficients list
         * changes are detected
         */
        @Watch('stateDeficients')
        onStateDeficientsChanged() {
            this.deficients = clone(this.stateDeficients);
        }

        /**
         * Sets the latestDeficientId property when deficients list changes are detected
         */
        @Watch('deficients')
        onDeficientsChanged() {
            this.latestDeficientId = getLatestPropertyValue('id', this.deficients);
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

        /**
         * Sets selectedDeficient property with the specified deficient and sets the criteriaEditorDialogData property
         * values with the selectedDeficient.criteria property
         * @param deficient Deficient object
         */
        onEditCriteria(deficient: Deficient) {
            this.selectedDeficientIndex = this.deficients.findIndex((d: Deficient) => d.id === deficient.id);

            this.criteriaEditorDialogData = {
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
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.deficients[this.selectedDeficientIndex].criteria = criteria;
                this.selectedDeficientIndex = -1;
            }
        }

        /**
         * Sends deficient data changes to the server for upsert
         */
        onSaveDeficients() {
            this.saveDeficientsAction({deficientData: this.deficients});
        }

        /**
         * Discards deficient data changes by resetting the deficients list with a new copy of the stateDeficients list
         */
        onCancelChangesToDeficients() {
            this.deficients = clone(this.stateDeficients);
        }
    }
</script>