<template>
    <v-layout column>
        <v-flex>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn @click="onNewLibrary" class="ara-blue-bg white--text">
                        New Library
                    </v-btn>
                    <v-select v-if="!hasSelectedCriteriaLibrary" v-model="selectItemValue"
                              :items="criteriaLibrarySelectListItems" label="Select a Criteria Library" outline>
                    </v-select>
                    <v-text-field v-if="hasSelectedCriteriaLibrary"
                                  v-model="selectedCriteriaLibrary.name" @change="canUpdateOrCreate = true">
                        <template slot="append">
                            <v-btn @click="selectItemValue = null" class="ara-orange" icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedCriteriaLibrary">
                        Owner: {{selectedCriteriaLibrary.owner ? selectedCriteriaLibrary.owner : '[ No Owner ]'}}
                    </div>
                    <v-checkbox v-if="hasSelectedCriteriaLibrary"
                                v-model="selectedCriteriaLibrary.shared"
                                class="sharing" label="Shared" @change="canUpdateOrCreate = true"/>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedCriteriaLibrary"/>
        <v-flex v-show="hasSelectedCriteriaLibrary">
            <v-layout justify-center>
                <v-flex xs10>
                    <CriteriaEditor :criteriaEditorData="criteriaEditorData"
                                    @submitCriteriaEditorResult="onSubmitCriteriaEditorResult"/>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedCriteriaLibrary"/>
        <v-flex v-show="hasSelectedCriteriaLibrary">
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea v-model="selectedCriteriaLibrary.description" label="Description" no-resize outline
                                rows="4">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex>
            <v-layout v-show="hasSelectedCriteriaLibrary" justify-end row>
                <v-btn @click="onUpdateLibrary" class="ara-blue-bg white--text" :disabled="!canUpdateOrCreate">
                    Update Library
                </v-btn>
                <v-btn @click="onCreateAsNewLibrary" class="ara-blue-bg white--text" :disabled="!canUpdateOrCreate">
                    Create as New Library
                </v-btn>
                <v-btn @click="onDeleteLibrary" class="ara-orange-bg white--text">
                    Delete Library
                </v-btn>
            </v-layout>
        </v-flex>

        <CreateCriteriaLibraryDialog :dialogData="createCriteriaLibraryDialogData" @submit="onCreateLibrary"/>

        <Alert :dialogData="alertBeforeDeleteDialogData" @submit="onSubmitDeleteResponse"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Action, State} from 'vuex-class';
    import {Watch} from 'vue-property-decorator';
    import CriteriaEditor from '@/shared/components/CriteriaEditor.vue';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {
        CriteriaEditorData,
        CriteriaEditorResult,
        CriteriaLibrary,
        emptyCriteriaEditorData,
        emptyCriteriaLibrary
    } from '@/shared/models/iAM/criteria';

    import {clone, isNil} from 'ramda';
    import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';
    import {
        CreateCriteriaLibraryDialogData,
        emptyCreateCriteriaLibraryDialogData
    } from '@/shared/models/modals/create-criteria-library-dialog-data';
    import CreateCriteriaLibraryDialog from '@/components/criteria-editor/criteria-editor-dialogs/CreateCriteriaLibraryDialog.vue';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';

    @Component({
        components: {Alert, CreateCriteriaLibraryDialog, CriteriaEditor}
    })
    export default class CriteriaLibraryEditor extends Vue {
        @State(state => state.criteriaEditor.criteriaLibraries) stateCriteriaLibraries: CriteriaLibrary[];
        @State(state => state.criteriaEditor.selectedCriteriaLibrary) stateSelectedCriteriaLibrary: CriteriaLibrary;

        @Action('getCriteriaLibraries') getCriteriaLibrariesAction: any;
        @Action('createCriteriaLibrary') createCriteriaLibraryAction: any;
        @Action('selectCriteriaLibrary') selectCriteriaLibraryAction: any;
        @Action('updateCriteriaLibrary') updateCriteriaLibraryAction: any;
        @Action('deleteCriteriaLibrary') deleteCriteriaLibraryAction: any;
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        hasSelectedCriteriaLibrary: boolean = false;
        criteriaLibrarySelectListItems: SelectItem[] = [];
        selectItemValue: string | null = null;
        selectedCriteriaLibrary: CriteriaLibrary = clone(emptyCriteriaLibrary);
        criteriaEditorData: CriteriaEditorData = {
            ...emptyCriteriaEditorData,
            isLibraryContext: true
        };
        createCriteriaLibraryDialogData: CreateCriteriaLibraryDialogData = clone(emptyCreateCriteriaLibraryDialogData);
        alertBeforeDeleteDialogData: AlertData = clone(emptyAlertData);
        canUpdateOrCreate: boolean = false;

        beforeRouteEnter(to: any, from: any, next: any) {
            next((vm: any) => {
                vm.selectItemValue = null;
                vm.getCriteriaLibrariesAction();
            });
        }

        beforeDestroy() {
            this.setHasUnsavedChangesAction({value: false});
        }

        @Watch('stateCriteriaLibraries')
        onStateCriteriaLibrariesChanged() {
            this.criteriaLibrarySelectListItems = this.stateCriteriaLibraries.map((criteriaLibrary: CriteriaLibrary) => ({
                text: criteriaLibrary.name,
                value: criteriaLibrary.id
            }));
        }

        @Watch('selectItemValue')
        onSelectItemValueChanged() {
            this.selectCriteriaLibraryAction({selectedLibraryId: this.selectItemValue});
        }

        @Watch('stateSelectedCriteriaLibrary')
        onStateSelectedCriteriaLibraryChanged() {
            this.canUpdateOrCreate = false;
            this.selectedCriteriaLibrary = clone(this.stateSelectedCriteriaLibrary);
        }

        @Watch('selectedCriteriaLibrary')
        onSelectedCriteriaLibraryChanged() {
            this.setHasUnsavedChangesAction({
                value: hasUnsavedChanges(
                    'criteria', this.selectedCriteriaLibrary, this.stateSelectedCriteriaLibrary, null
                )
            });

            this.hasSelectedCriteriaLibrary = this.selectedCriteriaLibrary.id !== '0';

            this.criteriaEditorData = {
                ...this.criteriaEditorData,
                mainCriteriaString: this.selectedCriteriaLibrary.criteria
            };
        }

        onNewLibrary() {
            this.createCriteriaLibraryDialogData = {
                ...this.createCriteriaLibraryDialogData,
                showDialog: true
            };
        }

        onSubmitCriteriaEditorResult(result: CriteriaEditorResult) {
            this.canUpdateOrCreate = result.validated;

            if (result.validated) {
                this.selectedCriteriaLibrary = {
                    ...this.selectedCriteriaLibrary,
                    criteria: result.criteria!
                };
            }
        }

        onUpdateLibrary() {
            this.updateCriteriaLibraryAction({modifiedCriteriaLibrary: this.selectedCriteriaLibrary});
        }

        onCreateAsNewLibrary() {
            this.createCriteriaLibraryDialogData = {
                showDialog: true,
                criteria: this.selectedCriteriaLibrary.criteria,
                description: this.selectedCriteriaLibrary.description
            };
        }

        onCreateLibrary(newCriteriaLibrary: CriteriaLibrary) {
            this.createCriteriaLibraryDialogData = clone(emptyCreateCriteriaLibraryDialogData);

            if (!isNil(newCriteriaLibrary)) {
                this.createCriteriaLibraryAction({newCriteriaLibrary: newCriteriaLibrary})
                    .then(() => this.selectItemValue = newCriteriaLibrary.id);
            }
        }

        onDeleteLibrary() {
            this.alertBeforeDeleteDialogData = {
                showDialog: true,
                heading: 'Warning',
                choice: true,
                message: 'Are you sure you want to delete?'
            };
        }

        onSubmitDeleteResponse(submit: boolean) {
            this.alertBeforeDeleteDialogData = clone(emptyAlertData);

            if (submit) {
                this.deleteCriteriaLibraryAction({criteriaLibraryId: this.selectedCriteriaLibrary.id})
                    .then(() => this.selectItemValue = null);
            }
        }
    }
</script>