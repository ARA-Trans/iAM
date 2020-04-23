<template>
    <v-dialog max-width="500px" persistent v-model="showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>Committed Projects</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-layout column>
                        <form class="ara-light-gray-bg" id="file-form" v-show="dragAndDropCapable">
                            <v-layout align-center fill-height justify-center>
                                <div>Drag & Drop Files Here</div>
                            </v-layout>
                        </form>
                        <v-flex xs12>
                            <v-layout justify-start>
                                <v-checkbox label="No Treatment" v-model="applyNoTreatment"></v-checkbox>
                            </v-layout>
                        </v-flex>
                        <v-flex xs12>
                            <v-layout justify-start>
                                <v-btn @click="fileSelect.click()" class="ara-blue-bg white--text">
                                    Select File(s)
                                </v-btn>
                            </v-layout>
                        </v-flex>
                        <div v-show="false">
                            <input @change="onSelect($event.target.files)" id="file-select" multiple type="file"/>
                        </div>
                    </v-layout>
                    <div class="files-table">
                        <v-data-table :headers="filesTableHeaders" :items="files" class="elevation-1 fixed-header v-table__overflow"
                                      hide-actions>
                            <template slot="items" slot-scope="props">
                                <td>{{props.item.name}}</td>
                                <td>{{convertBytesToMegabytes(props.item.size)}}</td>
                                <td>
                                    <v-btn @click="onRemoveFile(props.item.name)" class="ara-orange" icon>
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </td>
                            </template>
                        </v-data-table>
                    </div>
                </v-layout row>
                Warning: Uploading new committed projects will override ALL previous commitments.
                    Committed projects may take a few minutes to process. You will receive an email when this process is complete.
                <v-layout>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn @click="onUpload" class="ara-blue-bg white--text">Upload</v-btn>
                    <v-btn @click="onExport" class="ara-blue-bg white--text">Export</v-btn>
                    <v-btn @click="onCancel" class="ara-orange-bg white--text">Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {Action} from 'vuex-class';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {any, clone, last, propEq} from 'ramda';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {bytesToMegabytes} from '@/shared/utils/math-utils.ts';
    import {CommittedProjectsDialogResult} from '@/shared/models/modals/committed-projects-dialog-result';

    @Component
    export default class CommittedProjectsFileUploaderDialog extends Vue {
        @Prop() showDialog: boolean;

        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setIsBusy') setIsBusyAction: any;

        dragAndDropCapable: boolean = false;
        dragEvents: string[] = ['drag', 'dragstart', 'dragend', 'dragover', 'dragenter', 'dragleave', 'drop'];
        fileSelect: HTMLInputElement = {} as HTMLInputElement;
        fileForm: HTMLFormElement = {} as HTMLFormElement;
        files: File[] = [];
        filesTableHeaders: DataTableHeader[] = [
            {text: 'Name', value: 'name', align: 'left', sortable: true, class: '', width: '100px'},
            {text: 'Size', value: 'size', align: 'left', sortable: true, class: '', width: '50px'},
            {text: '', value: '', align: 'center', sortable: false, class: '', width: '25px'}
        ];
        applyNoTreatment: boolean = true;

        /**
         * Component has been mounted
         */
        mounted() {
            // link fileSelect to the file input 'file-select'
            this.fileSelect = document.getElementById('file-select') as HTMLInputElement;
            // set dragAndDropCapable flag
            this.dragAndDropCapable = this.isBrowserDragAndDropCapable();
            if (this.dragAndDropCapable) {
                // link fileForm to the form element 'file-form'
                this.fileForm = document.getElementById('file-form') as HTMLFormElement;
                // add event listeners to 'file-form' for all drag & drop events
                this.dragEvents.forEach((dragEvent: string) => {
                    this.fileForm.addEventListener(dragEvent, (e: any) => {
                        e.preventDefault();
                        e.stopPropagation();
                    });
                });
                // add the drop event to the 'file-form' event listeners to allow users to add dropped files to the files
                // list
                this.fileForm.addEventListener('drop', (e: any) => {
                    this.onSelect(e.dataTransfer.files);
                });
            }
        }

        /**
         * Determines whether or not the user's current browser allows drag & drop functionality
         */
        isBrowserDragAndDropCapable() {
            const div = document.createElement('div');

            return ('draggable' in div || ('ondragstart' in div && 'ondrop' in div)) &&
                'FormData' in window &&
                'FileReader' in window;
        }

        /**
         * Adds a selected file or files to the files list if the selected file(s) have the .xlsx extension, otherwise a
         * toastr error is raised alerting the user that only .xlsx files are allowed
         */
        onSelect(newFiles: File[]) {
            if (hasValue(newFiles)) {
                this.setIsBusyAction({isBusy: true});
                const extensions: string[] = getPropertyValues('name', newFiles)
                    .map((name: string) => {
                        return last(name.split('.')) as string;
                    });
                if (any((extension: string) => extension !== 'xlsx', extensions)) {
                    this.setErrorMessageAction({message: 'Only .xlsx file types are allowed'});
                    newFiles = [...newFiles].filter((file: File) => file.name.indexOf('xlsx') !== -1);
                }
                const oldFiles = this.files.filter((file: File) => !any(propEq('name', file.name), newFiles));
                this.files = [...oldFiles, ...newFiles];
                this.setIsBusyAction({isBusy: false});
            }
        }

        /**
         * Returns a formatted string of a file's bytes converted to megabytes
         */
        convertBytesToMegabytes(bytes: number) {
            return `${bytesToMegabytes(bytes)} MB`;
        }

        /**
         * Removes a file from the files array
         */
        onRemoveFile(name: string) {
            if (any(propEq('name', name), this.files)) {
                this.files = this.files.filter((file: File) => file.name !== name);
            }
        }

        /**
         * Submits 'null' to EditScenario parent component indicating user canceled file uploading
         */
        onCancel() {
            this.$emit('submit', null, false);
            this.files = [];
        }

        /**
         * Submits user's selected file(s) to EditScenario parent component for upload
         */
        onUpload() {
            const results: CommittedProjectsDialogResult = {
                applyNoTreatment: this.applyNoTreatment,
                files: this.files
            };
            this.$emit('submit', results, false);
            this.files = [];
        }

        /**
         * Submits user's action to EditScenario parent component for Export
         */
        onExport() {
            this.$emit('submit', null, true);
            this.files = [];
        }
    }
</script>

<style>
    form {
        /*display: block;*/
        height: 100px;
        /*width: 100%;
        margin: auto;
        margin-top: 40px;
        text-align: center;
        line-height: 400px;*/
        border-radius: 4px;
    }

    .file-select-btn {
        width: 100px;
    }

    .files-table {
        height: 215px;
        overflow-y: auto;
    }
</style>
