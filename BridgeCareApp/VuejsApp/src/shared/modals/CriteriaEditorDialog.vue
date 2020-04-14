<template>
    <v-dialog persistent v-model="dialogData.showDialog">
        <v-card>
            <v-card-text>
                <v-layout justify-center column>
                    <div>
                        <v-layout justify-center>
                            <v-flex xs4>
                                <v-select :items="criteriaLibrarySelectListItems" clearable clear-icon="fas fa-times"
                                          label="Select a Criteria Library" outline v-model="selectItemValue">
                                </v-select>
                            </v-flex>
                        </v-layout>
                    </div>
                    <div>
                        <CriteriaEditor :criteriaEditorData="criteriaEditorData"
                                        @submitCriteriaEditorResult="onSubmitCriteriaEditorDialogResult"/>
                    </div>
                </v-layout>
            </v-card-text>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import {CriteriaEditorDialogData} from '../models/modals/criteria-editor-dialog-data';
    import CriteriaEditor from '@/shared/components/CriteriaEditor.vue';
    import {
        CriteriaEditorData,
        CriteriaLibrary,
        emptyCriteriaEditorData,
        emptyCriteriaLibrary
    } from '@/shared/models/iAM/criteria';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {equals} from 'ramda';

    @Component({
        components: {CriteriaEditor}
    })
    export default class CriteriaEditorDialog extends Vue {
        @Prop() dialogData: CriteriaEditorDialogData;

        @State(state => state.criteriaEditor.criteriaLibraries) stateCriteriaLibraries: CriteriaLibrary[];
        @State(state => state.criteriaEditor.selectedCriteriaLibrary) stateSelectedCriteriaLibrary: CriteriaLibrary;

        @Action('getCriteriaLibraries') getCriteriaLibrariesAction: any;
        @Action('selectCriteriaLibrary') selectCriteriaLibraryAction: any;

        criteriaLibrarySelectListItems: SelectItem[] = [];
        selectItemValue: string | null = null;
        criteriaEditorData: CriteriaEditorData = {...emptyCriteriaEditorData};

        mounted() {
            this.getCriteriaLibrariesAction();
        }

        @Watch('dialogData')
        onDialogDataChanged() {
            this.criteriaEditorData = {
                ...this.criteriaEditorData,
                mainCriteriaString: this.dialogData.criteria
            };
        }

        @Watch('stateCriteriaLibraries')
        onStateSelectedCriteriaLibrariesChanged() {
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
            if (!equals(this.stateSelectedCriteriaLibrary, emptyCriteriaLibrary)) {
                this.criteriaEditorData = {
                    ...this.criteriaEditorData,
                    mainCriteriaString: this.stateSelectedCriteriaLibrary.criteria
                };
            } else {
                this.criteriaEditorData = {
                    ...this.criteriaEditorData,
                    mainCriteriaString: this.dialogData.criteria
                };
            }
        }

        /**
         * Emits the CriteriaEditor's parsed criteria string to the parent component
         */
        onSubmitCriteriaEditorDialogResult(criteria: string) {
            this.$emit('submitCriteriaEditorDialogResult', criteria);
            this.selectItemValue = null;
        }
    }
</script>

<style>
    .v-dialog:not(.v-dialog--fullscreen) {
        max-height: 100%;
        max-width: 75%;
    }
</style>
