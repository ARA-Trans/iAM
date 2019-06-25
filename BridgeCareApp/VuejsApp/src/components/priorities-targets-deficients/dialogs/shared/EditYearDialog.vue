<template>
    <v-layout>
        <v-menu v-model="showDatePicker" :close-on-content-click="false" :nudge-right="40" lazy
                transition="scale-transition" offset-y full-width min-width="290px">
            <template slot="activator">
                <v-text-field label="Year" v-model="currentYear" append-icon="event">
                </v-text-field>
            </template>
            <v-date-picker v-model="currentYear" ref="editDialogPicker" min="1950" reactive no-title
                           @input="onSetYear">
            </v-date-picker>
        </v-menu>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';

    @Component
    export default class EditYearDialog extends Vue {
        @Prop() itemYear: string;

        showDatePicker: boolean = false;
        currentYear: string = this.itemYear;

        @Watch('itemYear')
        onItemYearChanged() {
            this.currentYear = this.itemYear;
        }

        /**
         * Sets the date picker to show year options on nextTrick when showDatePicker has changed
         */
        @Watch('showDatePicker')
        onShowDatePickerChanged() {
            if (this.showDatePicker) {
                // @ts-ignore
                this.$nextTick(() => this.$refs.editDialogPicker.activePicker = 'YEAR');
            }
        }

        /**
         * Sets the showDatePicker property to false then sets the currentYear property with the parsed parameter
         * @param year Currently selected year as a string
         */
        onSetYear(year: string) {
            this.showDatePicker = false;
            this.currentYear = year.substr(0, 4);
            this.$emit('editedYear', this.currentYear);
        }
    }
</script>