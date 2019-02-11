<template>
    <v-container fluid grid-list-xl>
        <v-flex xs2>
            <v-select
                    :items="sectionIds"
                    label="Select a Section"
                    v-on:change="onSelectSection"
                    outline>
            </v-select>
        </v-flex>
        <v-flex xs6>
            <v-select :disabled="sectionAttributes.length <= 0" :items="sectionAttributes" v-model="selectedAttributes"
                      v-on:change="setAttributeSelectAllIcon" label="Add/Remove Attributes" multiple outline>
                <v-list-tile slot="prepend-item" ripple @click="onSelectAllAttributes">
                    <v-list-tile-action>
                        <v-icon :color="selectedAttributes.length > 0 ? 'indigo darken-4' : ''">{{attrSelectAllIcon}}</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-title>Select All</v-list-tile-title>
                </v-list-tile>
                <v-divider slot="prepend-item" class="mt-2"></v-divider>
            </v-select>
        </v-flex>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';
    import axios from 'axios';

    //@ts-ignore
    import AppSpinner from '../shared/AppSpinner';
    import {IAttribute, IAttributeYearlyValue, ISection, mockSections} from "@/models/section";
    import * as R from 'ramda';

    axios.defaults.baseURL = process.env.VUE_APP_URL;

    @Component({
        components: {AppSpinner}
    })
    export default class Inventory extends Vue {
        headers: object[] = [{text: 'Attribute', align: 'left', sortable: false, value: 'name'}];
        sections: ISection[] = [];
        sectionIds: number[] = [];
        sectionAttributes: string[] = [];
        selectedAttributes: string[] = [];
        currentDate = new Date();
        startYear: number = 0;
        endYear: number = 0;
        attrSelectAllIcon = 'check_box_outline_blank';
        downloadProgress = false;
        loading = false;

        /*data() {
            return {
                attrSelectAllIcon: 'check_box_outline_blank'
            }
        }*/

        created() {
            this.startYear = this.endYear = this.currentDate.getFullYear();
            this.startProgressStatus();
            // simulate a service call to get the bridge ids
            setTimeout(() => {
                this.sections = mockSections;
                this.sectionIds = this.sections.map((s: ISection) => s.sectionId);
                this.stopProgressStatus();
            }, 2000)
            // TODO: uncomment the following code when web service is in place
            /*axios
                .get('/api/Sections')
                .then(response => (response.data as Promise<ISection[]>))
                .then(
                    data => {
                        this.sections = data;
                        this.sectionIds = this.sections.map((s: ISection) => s.sectionId);
                        this.stopProgressStatus();
                    },
                    error => {
                        this.stopProgressStatus();
                        console.log(error);
                    }
                );*/
        }

        onSelectSection(sectionId: number) {
            // find the section in the list of sections using the given sectionId
            const selectedSection = this.sections.find((s: ISection) => s.sectionId === sectionId);
            // check that the section was found
            if (selectedSection) {
                // reset the list of attributes and create a list for storing the years
                this.sectionAttributes = [];
                const years: number[] = [];
                // loop over the section's attributes
                selectedSection.attributes.forEach((a: IAttribute) => {
                    // push the current attribute name onto the attributes list
                    this.sectionAttributes.push(a.name);
                    // loop over the current attribute's yearly values and push the year onto the years list
                    a.yearlyValues.forEach((val: IAttributeYearlyValue) => years.push(val.year));

                });
                // sort function that returns the difference between a & b
                const diff = (a: number, b: number) => { return a - b; };
                // remove year duplicates then reverse sort the list so years are listed from most recent to past year
                const sortedYears = R.reverse(R.sort(diff, R.uniq(years)));
                // loop over the sorted list of years to add new headers to the headers list using the years as the
                // text/value properties
                sortedYears.forEach((year: number) => {
                    this.headers.push(
                        {text: year.toString(), sortable: false, value: year.toString()}
                    )
                })
            }
        }

        selectedAllAttributes() {
            return this.selectedAttributes.length === this.sectionAttributes.length;
        }

        selectedSomeAttributes() {
            return this.selectedAttributes.length > 0;
        }

        onSelectAllAttributes() {
            if (this.selectedAllAttributes()) {
                this.selectedAttributes = [];
            } else {
                this.selectedAttributes = this.sectionAttributes.slice();
            }
            this.setAttributeSelectAllIcon();
        }

        setAttributeSelectAllIcon() {
            if (this.selectedAllAttributes()) {
                this.attrSelectAllIcon = 'check_box';
            } else if (this.selectedSomeAttributes()) {
                this.attrSelectAllIcon = 'indeterminate_check_box';
            } else {
                this.attrSelectAllIcon = 'check_box_outline_blank';
            }
        }

        startProgressStatus() {
            this.downloadProgress = true;
            this.loading = true;
        }

        stopProgressStatus() {
            this.downloadProgress = false;
            this.loading = false;
        }
    }
</script>