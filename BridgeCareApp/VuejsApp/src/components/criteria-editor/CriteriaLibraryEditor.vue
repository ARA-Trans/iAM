<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-btn @click="onNewLibrary" class="ara-blue-bg white--text">
                    New Library
                </v-btn>
                <v-select v-if="!hasSelectedCriteriaLibrary" v-model="selectItemValue"
                          :items="criteriaLibrarySelectListItems" label="Select a Criteria Library" outline>
                </v-select>
                <v-text-field v-if="hasSelectedCriteriaLibrary"
                              v-model="selectedCriteriaLibrary.name">
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
                            class="sharing" label="Shared"/>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedCriteriaLibrary"/>
        <v-flex v-show="hasSelectedCriteriaLibrary">
            <CriteriaEditor :mainCriteriaString="selectedCriteriaLibrary.criteria" @submit="onSubmitCriteria"/>
        </v-flex>
        <v-divider v-show="hasSelectedCriteriaLibrary"/>
        <v-flex v-show="hasSelectedCriteriaLibrary" xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea v-model="selectedCriteriaLibrary.description" label="Description" no-resize outline
                                rows="4">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout v-show="hasSelectedCriteriaLibrary" justify-end row>
                <v-btn @click="onUpdateLibrary" class="ara-blue-bg white--text">
                    Update Library
                </v-btn>
                <v-btn @click="onCreateAsNewLibrary" class="ara-blue-bg white--text">
                    Create as New Library
                </v-btn>
                <v-btn @click="onDeleteLibrary" class="ara-orange-bg white--text">
                    Delete Library
                </v-btn>
            </v-layout>
        </v-flex>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Action, State} from 'vuex-class';
    import {Watch} from 'vue-property-decorator';
    import CriteriaEditor from '@/shared/components/CriteriaEditor.vue';
    import {SelectItem} from '@/shared/models/vue/select-item';
    import {CriteriaLibrary, emptyCriteriaLibrary} from '@/shared/models/iAM/criteria';

    import {clone} from 'ramda';
    import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';
    import {
        CreateCriteriaLibraryDialogData,
        emptyCreateCriteriaLibraryDialogData
    } from '@/shared/models/modals/create-criteria-library-dialog-data';

    @Component({
        components: {CriteriaEditor}
    })
    export default class CriteriaLibraryEditor extends Vue {
        @State(state => state.criteriaEditor.criteriaLibraries) stateCriteriaLibraries: CriteriaLibrary[];
        @State(state => state.criteriaEditor.selectedCriteriaLibrary) stateSelectedCriteriaLibrary: CriteriaLibrary;

        @Action('createCriteriaLibrary') createCriteriaLibraryAction: any;
        @Action('selectCriteriaLibrary') selectCriteriaLibraryAction: any;
        @Action('updateCriteriaLibrary') updateCriteriaLibraryAction: any;
        @Action('deleteCriteriaLibrary') deleteCriteriaLibraryAction: any;
        @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

        hasSelectedCriteriaLibrary: boolean = false;
        criteriaLibrarySelectListItems: SelectItem[] = [];
        selectItemValue: string | null = null;
        selectedCriteriaLibrary: CriteriaLibrary = clone(emptyCriteriaLibrary);
        createCriteriaLibraryDialogData: CreateCriteriaLibraryDialogData = clone(emptyCreateCriteriaLibraryDialogData);

        @Watch('stateCriteriaLibraries')
        onStateCriteriaLibrariesChanged() {
            this.criteriaLibrarySelectListItems = this.stateCriteriaLibraries.map((criteriaLibrary: CriteriaLibrary) => ({
                text: criteriaLibrary.name,
                value: criteriaLibrary.id
            }));
        }

        @Watch('stateSelectedCriteriaLibrary')
        onStateSelectedCriteriaLibraryChanged() {
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
        }

        onNewLibrary() {
            this.createCriteriaLibraryDialogData = {
                ...this.createCriteriaLibraryDialogData,
                showDialog: true
            };
        }

        onSubmitCriteria(criteria: string) {
            this.selectedCriteriaLibrary = {
                ...this.selectedCriteriaLibrary,
                criteria: criteria
            };
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

        onCreateLibrary() {

        }

        onDeleteLibrary() {

        }
    }
</script>