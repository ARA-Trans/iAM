<template>
    <v-layout>
        <v-menu v-model="showDatePicker" :close-on-content-click="false" :nudge-right="40" lazy
                transition="slide-y-transition" origin="center center" offset-y full-width min-width="290px">
            <template slot="activator">
                <div v-if="outline">
                    <v-text-field :label="itemLabel" readonly outline v-model="currentYear" append-icon="fas fa-calendar-day"
                                  @click="focusOnActiveDate">
                    </v-text-field>
                </div>
                <div v-else>
                    <v-text-field :label="itemLabel" readonly v-model="currentYear" append-icon="fas fa-calendar-day"
                                  @click="focusOnActiveDate">
                    </v-text-field>
                </div>
            </template>
            <v-date-picker v-model="currentYear" ref="editDialogPicker" min="1950" reactive no-title @input="onSetYear">
            </v-date-picker>
        </v-menu>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {hasValue} from '@/shared/utils/has-value-util';

    @Component
    export default class EditYearDialog extends Vue {
        @Prop() itemYear: string;
        @Prop() itemLabel: string;
        @Prop() outline: boolean;

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

        focusOnActiveDate() {
            this.showDatePicker = false;
            setTimeout(() => {
                const unorderedYearsList: HTMLCollectionOf<HTMLElement> = document
                    .getElementsByClassName('v-date-picker-years') as HTMLCollectionOf<HTMLElement>;
                if (hasValue(unorderedYearsList)) {
                    // @ts-ignore
                    const years: IterableIterator<HTMLUListElement> = unorderedYearsList.item(0).childNodes.values();
                    let foundActive: boolean = false;
                    do {
                        const listElement: HTMLUListElement = years.next().value;
                        if (listElement.className.indexOf('active') !== -1) {
                            listElement.scrollIntoView();
                            foundActive = true;
                        }
                    } while(!foundActive);
                }
                this.showDatePicker = true;
            });
        }
    }
</script>