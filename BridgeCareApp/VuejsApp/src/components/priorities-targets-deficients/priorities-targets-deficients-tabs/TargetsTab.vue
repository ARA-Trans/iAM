<template>
    <v-container fluid grid-list-xl>
        <div>
            <v-layout>
                <v-flex>
                    <v-btn color="info" @click="onAddTarget">Add</v-btn>
                </v-flex>
            </v-layout>
            <v-layout>
                <v-flex>
                    <div class="targets-data-table">
                        <v-data-table :headers="targetDataTableHeaders" :items="targets"
                                      class="elevation-1 fixed-header v-table__overflow" hide-actions>
                            <template slot="items" slot-scope="props">
                                <td v-for="header in targetDataTableHeaders">
                                    <div v-if="header.value !== 'criteria' && header.value !== 'year'">
                                        <v-edit-dialog :return-value.sync="props.item[header.value]" large lazy persistent
                                                       @save="onEditTargetProperty(props.item.id, header.value, props.item[header.value])">
                                            <v-text-field readonly :value="props.item[header.value]"></v-text-field>
                                            <template slot="input">
                                                <v-text-field v-model="props.item[header.value]" label="Edit" single-line
                                                              :type="header.value === 'attribute' || header.value === 'name' ? 'text' : 'number'">
                                                </v-text-field>
                                            </template>
                                        </v-edit-dialog>
                                    </div>
                                    <div v-if="header.value === 'year'">
                                        <v-edit-dialog :return-value.sync="props.item.year" large lazy persistent
                                                       @save="onEditTargetProperty(props.item.id, header.value, props.item[header.value])">
                                            <v-text-field readonly :value="props.item.year"></v-text-field>
                                            <template slot="input">
                                                <EditTargetYearDialog :itemYear="props.item.year.toString()" :itemLabel="'Year'"
                                                                @editedYear="props.item.year = $event" />
                                            </template>
                                        </v-edit-dialog>
                                    </div>
                                    <div v-if="header.value === 'criteria'">
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

        <CreateTargetDialog :dialogData="createTargetDialogData" @submit="onSubmitNewTarget" />

        <TargetsCriteriaEditor :dialogData="targetscriteriaEditorDialogData" @submit="onSubmitTargetCriteria" />

        <v-footer>
            <v-layout class="priorities-targets-deficients-buttons" justify-end row fill-height>
                <v-btn color="info" @click="onSaveTargets" :disabled="targets.length === 0">Save</v-btn>
                <v-btn color="error" @click="onCancelChangesToTargets" :disabled="targets.length === 0">Cancel</v-btn>
            </v-layout>
        </v-footer>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch, Prop} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {Target} from '@/shared/models/iAM/target';
    import {clone, isNil, append, any, propEq} from 'ramda';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import CreateTargetDialog from '@/components/priorities-targets-deficients/dialogs/targets-dialogs/CreateTargetDialog.vue';
    import EditYearDialog from '@/shared/modals/EditYearDialog.vue';
    import {
        CreatePrioritizationDialogData,
        emptyCreatePrioritizationDialogData
    } from '@/shared/models/modals/create-prioritization-dialog-data';

    @Component({
        components: {EditTargetYearDialog: EditYearDialog, CreateTargetDialog, TargetsCriteriaEditor: CriteriaEditorDialog}
    })
    export default class TargetsTab extends Vue {
        @Prop() selectedScenarioId: number;

        @State(state => state.target.targets) stateTargets: Target[];

        @Action('saveTargets') saveTargetsAction: any;

        targets: Target[] = [];
        targetDataTableHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: true, class: '', width: '15%'},
            {text: 'Name', value: 'name', align: 'left', sortable: true, class: '', width: '15%'},
            {text: 'Year', value: 'year', align: 'left', sortable: true, class: '', width: '7.5%'},
            {text: 'Target', value: 'targetMean', align: 'left', sortable: true, class: '', width: '7.5%'},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '55%'}
        ];
        selectedTargetIndex: number = -1;
        createTargetDialogData: CreatePrioritizationDialogData = clone(emptyCreatePrioritizationDialogData);
        targetscriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

        /**
         * Sets the targets list property with a copy of the stateTargets list property when stateTargets list changes
         * are detected
         */
        @Watch('stateTargets')
        onStateTargetsChanged() {
            this.targets = clone(this.stateTargets);
        }

        /**
         * Sets the showCreateTargetDialog property to true
         */
        onAddTarget() {
            this.createTargetDialogData = {
                showDialog: true,
                scenarioId: this.selectedScenarioId
            };
        }

        /**
         * Receives a Target object result from the CreateTargetDialog and adds it to the targets list property
         * @param newTarget Target object
         */
        onSubmitNewTarget(newTarget: Target) {
            this.createTargetDialogData = clone(emptyCreatePrioritizationDialogData);

            if (!isNil(newTarget)) {
                this.targets = append(newTarget, this.targets);
            }
        }

        onEditTargetProperty(targetId: any, property: string, value: any) {
            if (any(propEq('id', targetId), this.targets)) {
                const index: number = this.targets.findIndex((target: Target) => target.id === targetId);
                // @ts-ignore
                this.targets[index][property] = value;
            }
        }

        /**
         * Sets the selectedTargetIndex property with the specified target's index in the targets list
         * values with the selectedTarget.criteria property
         * @param target Target object
         */
        onEditCriteria(target: Target) {
            this.selectedTargetIndex = this.targets.findIndex((t: Target) => t.id === target.id);

            this.targetscriteriaEditorDialogData = {
                showDialog: true,
                criteria: target.criteria
            };
        }

        /**
         * Receives a criteria string result from the CriteriaEditor and sets the selectedTarget.criteria property
         * with it (if present)
         * @param criteria CriteriaEditor criteria string result
         */
        onSubmitTargetCriteria(criteria: string) {
            this.targetscriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.targets[this.selectedTargetIndex].criteria = criteria;
                this.selectedTargetIndex = -1;
            }
        }

        /**
         * Sends target data changes to the server for upsert
         */
        onSaveTargets() {
            this.saveTargetsAction({selectedScenarioId: this.selectedScenarioId, targetData: this.targets});
        }

        /**
         * Discards target data changes by resetting the targets list with a new copy of the stateTargets list
         */
        onCancelChangesToTargets() {
            this.targets = clone(this.stateTargets);
        }
    }
</script>