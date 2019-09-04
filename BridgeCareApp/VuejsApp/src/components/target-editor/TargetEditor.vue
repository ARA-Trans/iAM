<template>
    <v-layout column>
        <v-flex xs12>
            <v-btn class="ara-blue-bg white--text" @click="onAddTarget">Add</v-btn>
        </v-flex>
        <v-flex xs12>
            <div class="targets-data-table">
                <v-data-table :headers="targetDataTableHeaders" :items="targets"
                              class="elevation-1 fixed-header v-table__overflow">
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
                <v-btn class="ara-blue-bg white--text" @click="onSaveTargets" :disabled="targets.length === 0">
                    Save
                </v-btn>
                <v-btn class="ara-orange-bg white--text" @click="onCancelChangesToTargets" :disabled="targets.length === 0">
                    Cancel
                </v-btn>
            </v-layout>
        </v-flex>

        <CreateTargetDialog :showDialog="showCreateTargetDialog" @submit="onSubmitNewTarget" />

        <TargetCriteriaEditor :dialogData="targetCriteriaEditorDialogData" @submit="onSubmitTargetCriteria" />
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {Target} from '@/shared/models/iAM/target';
    import {clone, isNil, append, any, propEq} from 'ramda';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import CreateTargetDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetDialog.vue';
    import EditYearDialog from '@/shared/modals/EditYearDialog.vue';

    @Component({
        components: {EditTargetYearDialog: EditYearDialog, CreateTargetDialog, TargetCriteriaEditor: CriteriaEditorDialog}
    })
    export default class TargetEditor extends Vue {
        @State(state => state.target.targets) stateTargets: Target[];

        @Action('getTargets') getTargetsAction: any;
        @Action('saveTargets') saveTargetsAction: any;

        selectedScenarioId: number = 0;
        targets: Target[] = [];
        targetDataTableHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: true, class: '', width: '15%'},
            {text: 'Name', value: 'name', align: 'left', sortable: true, class: '', width: '15%'},
            {text: 'Year', value: 'year', align: 'left', sortable: true, class: '', width: '7.5%'},
            {text: 'Target', value: 'targetMean', align: 'left', sortable: true, class: '', width: '7.5%'},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '55%'}
        ];
        selectedTargetIndex: number = -1;
        showCreateTargetDialog: boolean = false;
        targetCriteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

        /**
         * Sets component UI properties that triggers cascading UI updates
         */
        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                if (to.path === '/TargetEditor/Scenario/') {
                    vm.selectedScenarioId = isNaN(parseInt(to.query.selectedScenarioId)) ? 0 : parseInt(to.query.selectedScenarioId);
                    if (vm.selectedScenarioId === 0) {
                        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
                        vm.$router.push('/Scenarios/');
                    }
                }

                // vm.onClearSelectedPriorityLibrary();
                setTimeout(() => {
                    vm.getTargetsAction({selectedScenarioId: vm.selectedScenarioId})
                        /*.then(() => {
                            if (vm.selectedScenarioId > 0) {
                                vm.getScenarioTargetLibraryAction({selectedScenarioId: vm.selectedScenarioId});
                            }
                        })*/;
                });
            });
        }

        /**
         * Resets component UI properties that triggers cascading UI updates
         */
        beforeRouteUpdate(to: any, from: any, next: any) {
            if (to.path === '/TargetEditor/Library/') {
                this.selectedScenarioId = 0;
                // this.onClearSelectedTargetLibrary();
                next();
            }
        }

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
            this.showCreateTargetDialog = true;
        }

        /**
         * Receives a Target object result from the CreateTargetDialog and adds it to the targets list property
         * @param newTarget Target object
         */
        onSubmitNewTarget(newTarget: Target) {
            this.showCreateTargetDialog = false;

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

            this.targetCriteriaEditorDialogData = {
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
            this.targetCriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

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