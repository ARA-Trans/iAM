<template>
    <v-dialog max-width="1000px" persistent scrollable v-model="dialogData.showDialog">
        <CriteriaEditor :mainCriteriaString="dialogData.criteria" @submit="onSubmitCriteria"/>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {CriteriaEditorDialogData} from '../models/modals/criteria-editor-dialog-data';
    import CriteriaEditor from '@/shared/components/CriteriaEditor.vue';

    @Component({
        components: {CriteriaEditor}
    })
    export default class CriteriaEditorDialog extends Vue {
        @Prop() dialogData: CriteriaEditorDialogData;

        /**
         * Emits the CriteriaEditor's parsed criteria string to the parent component
         */
        onSubmitCriteria(criteria: string) {
            this.$emit('submitCriteriaEditorDialogData', criteria);
        }
    }
</script>
