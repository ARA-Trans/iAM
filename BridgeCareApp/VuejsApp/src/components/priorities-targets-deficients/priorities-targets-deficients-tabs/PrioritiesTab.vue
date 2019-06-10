<template>
    <v-container fluid grid-list-xl>
        <div>
            <v-layout>
                <v-flex>
                    <v-btn color="info" @click="onAddPriority">Add</v-btn>
                </v-flex>
            </v-layout>
            <v-layout>
                <v-flex>
                    <div class="priorities-data-table">
                        <v-data-table :headers="priorityDataTableHeaders" :items="priorities"
                                      class="elevation-1 fixed-header v-table__overflow" hide-actions>
                            <template slot="items" slot-scope="props">
                                <td>
                                    <v-edit-dialog :return-value.sync="props.item.priorityLevel" large lazy persistent>
                                        <v-text-field readonly :value="props.item.priorityLevel"></v-text-field>
                                        <template slot="input">
                                            <v-text-field v-model="props.item.priorityLevel" label="Edit" single-line>
                                            </v-text-field>
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td>
                                    <v-edit-dialog :return-value.sync="props.item.year" large lazy persistent>
                                        <v-text-field readonly :value="props.item.year" @click="setPriorityYear(props.item.year)">
                                        </v-text-field>
                                        <template slot="input">
                                            <EditPriorityYearDialog :itemYear="priorityYear.toString()"
                                                                    @editedYear="props.item.year = $event" />
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td>
                                    <v-text-field readonly :value="props.item.criteria" append-outer-icon="edit"
                                                  @click:append-outer="onEditCriteria(props.item)">
                                    </v-text-field>
                                </td>
                            </template>
                        </v-data-table>
                    </div>
                </v-flex>
            </v-layout>
        </div>

        <CreatePriorityDialog :showDialog="showCreatePriorityDialog" @submit="onSubmitNewPriority" />

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitPriorityCriteria" />

        <v-footer>
            <v-layout class="priorities-targets-deficients-buttons" justify-end row fill-height>
                <v-btn color="info" @click="onSavePriorities">Save</v-btn>
                <v-btn color="error" @click="onCancelChangesToPriorities">Cancel</v-btn>
            </v-layout>
        </v-footer>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {Priority} from '@/shared/models/iAM/priority';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {clone, isNil, append} from 'ramda';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import CreatePriorityDialog from '@/components/priorities-targets-deficients/dialogs/priorities-dialogs/CreatePriorityDialog.vue';
    import {getLatestPropertyValue} from '@/shared/utils/getter-utils';
    import {hasValue} from '@/shared/utils/has-value-util';
    import EditYearDialog from '@/components/priorities-targets-deficients/dialogs/shared/EditYearDialog.vue';

    @Component({
        components: {EditPriorityYearDialog: EditYearDialog, CreatePriorityDialog, CriteriaEditorDialog}
    })
    export default class PrioritiesTab extends Vue {
        @State(state => state.priority.priorities) statePriorities: Priority[];

        @Action('savePriorities') savePrioritiesAction: any;

        priorities: Priority[] = [];
        priorityDataTableHeaders: DataTableHeader[] = [
            {text: 'Priority', value: 'priorityLevel', align: 'left', sortable: true, class: '', width: '12.4%'},
            {text: 'Year', value: 'year', align: 'left', sortable: true, class: '', width: '12.4%'},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: true, class: '', width: '75%'}
        ];
        priorityYear: number = -1;
        selectedPriorityIndex: number = -1;
        showCreatePriorityDialog: boolean = false;
        latestPriorityId: number = 0;
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

        /**
         * Sets the priorities list property with a copy of the statePriorities list property when statePriorities list
         * changes are detected
         */
        @Watch('statePriorities')
        onStatePrioritiesChanged() {
            this.priorities = clone(this.statePriorities);
        }

        /**
         * Sets the latestPriorityId property when priorities list changes are detected
         */
        @Watch('priorities')
        onPrioritiesChanged() {
            this.latestPriorityId = getLatestPropertyValue('id', this.priorities);
        }

        /**
         * Sets the showCreatePriorityDialog property to true
         */
        onAddPriority() {
            this.showCreatePriorityDialog = true;
        }

        /**
         * Receives a Priority object result from the CreatePriorityDialog and adds it to the priorities list property
         * @param newPriority Priority object
         */
        onSubmitNewPriority(newPriority: Priority) {
            this.showCreatePriorityDialog = false;

            if (!isNil(newPriority)) {
                newPriority.id = hasValue(this.latestPriorityId) ? this.latestPriorityId + 1 : 1;
                this.priorities = append(newPriority, this.priorities);
            }
        }

        setPriorityYear(year: number) {
            this.priorityYear = year;
        }

        /**
         * Sets selectedPriority property with the specified target and sets the criteriaEditorDialogData property
         * values with the selectedPriority.criteria property
         * @param priority Priority object
         */
        onEditCriteria(priority: Priority) {
            this.selectedPriorityIndex = this.priorities.findIndex((p: Priority) => p.id === priority.id);

            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: priority.criteria
            };
        }

        /**
         * Receives a criteria string result from the CriteriaEditor and sets the selectedPriority.criteria property
         * with it (if present)
         * @param criteria CriteriaEditor criteria string result
         */
        onSubmitPriorityCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.priorities[this.selectedPriorityIndex].criteria = criteria;
                this.selectedPriorityIndex = -1;
            }
        }

        /**
         * Sends priority data changes to the server for upsert
         */
        onSavePriorities() {
            this.savePrioritiesAction({priorityData: this.priorities});
        }

        /**
         * Discards priority data changes by resetting the priorities list with a new copy of the statePriorities list
         */
        onCancelChangesToPriorities() {
            this.priorities = clone(this.statePriorities);
        }
    }
</script>