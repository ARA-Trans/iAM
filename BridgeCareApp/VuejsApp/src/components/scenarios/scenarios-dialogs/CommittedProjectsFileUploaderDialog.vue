<template>
    <v-dialog v-model="showDialog">
        <v-card>
            <v-layout justify-center fill-height>
                <h3>Committed Projects File Uploader</h3>
            </v-layout>
        </v-card>
        <v-card-text>
            <v-layout column fill-height>
                <div id="file-drag-drop">
                    <form id="file-form">
                        <span class="drop-files">Drop the files here!</span>
                    </form>
                </div>
                <div>
                    <div v-for="(file, key) in files" class="file-listing" :key="key">
                        <v-text-field readonly outline append-icon="delete" :value="file.name"
                                      @click:append="onRemoveFile(key)">
                        </v-text-field>
                    </div>
                </div>
            </v-layout>
        </v-card-text>
        <v-card-actions>
            <v-layout justify-space-between row fill-height>
                <v-btn color="error" v-on:click="onCancel">Cancel</v-btn>
                <v-btn color="info" v-on:click="onUpload">Upload</v-btn>
            </v-layout>
        </v-card-actions>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {Action} from 'vuex-class';
    import {hasValue} from '@/shared/utils/has-value';
    import {getPropertyValues} from '@/shared/utils/getter-utils';
    import {last, any, propEq} from 'ramda';

    @Component
    export default class CommittedProjectsFileUploaderDialog extends Vue {
        @Prop() showDialog: boolean;

        @Action('setErrorMessage') setErrorMessageAction: any;

        dragAndDropCapable: boolean = false;
        dragEvents: string[] = ['drag', 'dragstart', 'dragend', 'dragover', 'dragenter', 'dragleave', 'drop'];
        fileForm: HTMLFormElement = {} as HTMLFormElement;
        files: File[] = [];

        mounted() {
            this.dragAndDropCapable = this.isBrowserDragAndDropCapable();
            if (this.dragAndDropCapable) {
                this.fileForm = document.getElementById('file-form') as HTMLFormElement;
                this.dragEvents.forEach((dragEvent: string) => {
                    this.fileForm.addEventListener(dragEvent, (e: any) => {
                        e.preventDefault();
                        e.stopPropagation();
                    });
                });

                this.fileForm.addEventListener('drop', (e: any) => {
                    if (hasValue(e.dataTransfer.files)) {
                        const extensions: string[] = getPropertyValues('name', e.dataTransfer.files)
                            .map((name: string) => {
                                return last(name.split('.')) as string;
                            });
                        if (any((extension: string) => extension !== 'xlsx', extensions)) {
                            this.setErrorMessageAction({message: 'Only .xlsx file types are allowed'});
                        } else {
                            for (let i = 0; i < e.dataTransfer.files.length; i++) {
                                this.files.push(e.dataTransfer.files[i]);
                            }
                        }
                    }
                });
            }
        }

        isBrowserDragAndDropCapable() {
            const div = document.createElement('div');

            return ('draggable' in div || ('ondragstart' in div && 'ondrop' in div) ) &&
                    'FormData' in window &&
                    'FileReader' in window;
        }

        /**
         * Removes a file from the files array
         */
        onRemoveFile(key: number) {
            this.files.splice(key, 1);
        }

        /**
         * Submits 'null' to EditScenario parent component indicating user canceled file uploading
         */
        onCancel() {
            this.$emit('submit', null);
            this.files = [];
        }

        /**
         * Submits user's selected file to EditScenario parent component for upload
         */
        onUpload() {
            this.$emit('submit', this.files);
            this.files = [];
        }
    }
</script>

<style>
    form {
        display: block;
        height: 400px;
        width: 400px;
        background: #ccc;
        margin: auto;
        margin-top: 40px;
        text-align: center;
        line-height: 400px;
        border-radius: 4px;
    }

    div.file-listing {
        width: 400px;
        margin: auto;
        padding: 10px;
        border-bottom: 1px solid #ddd;
    }
</style>