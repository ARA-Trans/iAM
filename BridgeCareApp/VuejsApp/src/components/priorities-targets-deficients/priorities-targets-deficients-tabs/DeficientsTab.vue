<template>
    <v-container fluid grid-list-xl>
        <div class="">
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
                                        <v-layout row>
                                            <v-flex>
                                                <v-text-field readonly :value="props.item[header.value]"></v-text-field>
                                            </v-flex>
                                            <v-btn flat icon @click="onEditCriteria(props.item)">
                                                <v-icon></v-icon>
                                            </v-btn>
                                        </v-layout>
                                    </div>
                                </td>
                            </template>
                        </v-data-table>
                    </div>
                </v-flex>
            </v-layout>
        </div>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitDeficientCriteria" />

        <v-footer>
            <v-layout justify-end row fill-height>
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
    import {Deficient, emptyDeficient} from "@/shared/models/iAM/deficient";
    import {DataTableHeader} from "@/shared/models/vue/data-table-header";
    import {clone, isNil} from 'ramda';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from "@/shared/models/modals/criteria-editor-dialog-data";
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    @Component({
        components: {CriteriaEditorDialog}
    })
    export default class DeficientsTab extends Vue {
        @State(state => state.deficient.deficients) stateDeficients: Deficient[];

        @Action('saveDeficients') saveDeficientsAction: any;

        deficients: Deficient[] = [];
        deficientDataTableHeaders: DataTableHeader[] = [
            {text: 'Attribute', value: 'attribute', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Name', value: 'name', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Deficient Level', value: 'deficient', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Allowed Deficient(%)', value: 'deficientPercent', align: 'left', sortable: true, class: '', width: ''},
            {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: ''}
        ];
        selectedDeficient: Deficient = clone(emptyDeficient);
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

        @Watch('stateDeficients')
        onStateDeficientsChanged() {
            this.deficients = clone(this.stateDeficients);
        }

        /**
         * Sets selectedDeficient property with the specified deficient and sets the criteriaEditorDialogData property
         * values with the selectedDeficient.criteria property
         * @param deficient Deficient object
         */
        onEditCriteria(deficient: Deficient) {
            this.selectedDeficient = clone(deficient);

            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: this.selectedDeficient.criteria
            };
        }

        /**
         * Receives the criteria result from the CriteriaEditor and sets the selectedDeficient.criteria property
         * with the criteria result (if present)
         * @param criteria CriteriaEditor criteria result
         */
        onSubmitDeficientCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                this.selectedDeficient.criteria = criteria;

                const index: number = this.deficients
                    .findIndex((deficient: Deficient) => deficient.id === this.selectedDeficient.id);
                this.deficients[index] = this.selectedDeficient;

                this.selectedDeficient = clone(emptyDeficient);
            }
        }

        /**
         * Sends deficient data changes to the server for upsert
         */
        onSaveDeficients() {
            this.saveDeficientsAction({deficientsData: this.deficients});
        }

        /**
         * Discards deficient data changes by resetting the deficients array with a copy of stateDeficients
         */
        onCancelChangesToDeficients() {
            this.deficients = clone(this.stateDeficients);
        }
    }
</script>