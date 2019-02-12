<template>
    <v-container fluid grid-list-xl>
        <v-layout row wrap>
            <v-flex xs2>
                <v-select
                        :items="sectionIds"
                        label="Select a Section"
                        v-on:change="onSelectSection"
                        outline>
                </v-select>
            </v-flex>
            <v-flex xs1></v-flex>
            <v-flex xs4>
                <v-select :disabled="sectionAttributes.length <= 0" :items="sectionAttributes" v-model="selectedAttributes"
                          v-on:change="setAttributeSelectAllIcon" label="Add/Remove Attributes" multiple outline>
                    <template slot="selection" slot-scope="{item, index}">
                        <span v-if="index === 0">{{item}}</span>
                        <span v-if="index === 1">, {{item}}</span>
                        <span v-if="index === 2">, {{item}}</span>
                        <span v-if="index === 3" class="grey--text caption">(+{{selectedAttributes.length - 3}} others)</span>
                    </template>
                    <v-list-tile slot="prepend-item" ripple @click="onSelectAllAttributes">
                        <v-list-tile-action>
                            <v-icon :color="selectedAttributes.length > 0 ? 'indigo darken-4' : ''">{{selectAllAttrIcon}}</v-icon>
                        </v-list-tile-action>
                        <v-list-tile-title>Select All</v-list-tile-title>
                    </v-list-tile>
                    <v-divider slot="prepend-item" class="mt-2"></v-divider>
                </v-select>
            </v-flex>
            <v-flex xs1></v-flex>
            <v-flex xs2>
                <v-select :disabled="sectionAttributes.length <= 0" :items="startYears" v-model="startYear"
                          v-on:change="onChangeStartYear" outline label="Select Start Year"></v-select>
            </v-flex>
            <v-flex xs2>
                <v-select :disabled="sectionAttributes.length <= 0" :items="endYears" v-model="endYear"
                          v-on:change="onChangeEndYear" outline label="Select End Year"></v-select>
            </v-flex>
        </v-layout>
        <v-layout row wrap>
            <v-flex xs12>
                <v-data-table v-if="hasValue(selectedSectionGridData)" :headers="sectionGridHeaders"
                              :items="selectedSectionGridData" class="elevation-1">
                    <template slot="items" slot-scope="props">
                        <td v-for="sectionGridHeader in sectionGridHeaders">
                            {{props.item[sectionGridHeader.value]}}
                        </td>
                    </template>
                </v-data-table>
            </v-flex>
        </v-layout>
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
        sectionGridHeaders: object[] = [{text: 'Attribute', align: 'left', sortable: false, value: 'name'}];
        sections: ISection[] = [];
        sectionIds: number[] = [];
        selectedSection: ISection;
        sectionAttributes: string[] = [];
        selectedAttributes: string[] = [];
        selectAllAttrIcon = 'check_box_outline_blank';
        startYears: number[] = [];
        endYears: number[] =[];
        startYear: number = new Date().getFullYear();
        endYear: number = new Date().getFullYear();
        selectedSectionGridData: any[] = [];
        downloadProgress = false;
        loading = false;
        /**
         * Vue component has been created
         */
        created() {
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
        /**
         * Section has been selected; Sets up the list of values for attribute & start/end years filters and the initial
         * data grid headers
         * @param sectionId Selected section's id
         */
        onSelectSection(sectionId: number) {
            // find the section in the list of sections using the given sectionId
            //@ts-ignore
            this.selectedSection = this.sections.find((s: ISection) => s.sectionId === sectionId);
            // if a section was found...
            if (this.hasValue(this.selectedSection)) {
                // reset the list of attributes and create a placeholder list for the years
                this.sectionAttributes = [];
                const years: number[] = [];
                // sort function that sorts by the 'year' property of attribute yearly values
                const sortByYear = R.sortBy(R.prop('year'));
                // for each of the attributes...
                this.selectedSection.attributes.forEach((a: IAttribute) => {
                    // push the current attribute name onto the attributes list
                    this.sectionAttributes.push(a.name);
                    // sort the yearly values for the given attribute in ascending order
                    const sortedYearlyValuesAsc = sortByYear(a.yearlyValues);
                    // for each sorted yearly value push the yearly value year onto the years list
                    sortedYearlyValuesAsc.forEach((val: IAttributeYearlyValue) => years.push(val.year));

                });
                // set all section attributes as selected by default
                this.selectedAttributes = this.sectionAttributes;
                // update the selectAllAttrIcon
                this.setAttributeSelectAllIcon();
                // set the start years list while removing any duplicate years
                this.startYears = R.uniq(years);
                this.startYear = this.hasValue(this.startYears) ? this.startYears[0] : 0;
                // set the end years list with a copy of the start years list in desc order
                this.endYears = R.reverse(this.startYears);
                this.endYear = this.hasValue(this.endYears) ? this.endYears[0] : 0;
                // for each of the end years add a new header to the sectionGridHeaders list using the years as the
                // text/value properties
                this.endYears.forEach((year: number) => {
                    this.sectionGridHeaders.push(
                        {text: year.toString(), sortable: false, value: year.toString()}
                    )
                });
                this.setSelectedSectionGridData();
            }
        }
        /**
         * Sets the selected section's grid data
         */
        setSelectedSectionGridData() {
            //TODO: apply logic to use the select filters (attributes & start/end years)
            this.selectedSectionGridData = this.selectedSection.attributes.map((a: IAttribute) => {
                const row = {
                    name: a.name
                };
                this.endYears.forEach((n: number) => {
                    if (R.any(R.propEq('year', n), a.yearlyValues)) {
                        //@ts-ignore
                        row[`${n}`] = a.yearlyValues.find((val: IAttributeYearlyValue) => val.year === n).value;
                    } else {
                        //@ts-ignore
                        row[`${n}`] = 'N/A';
                    }
                });
                return row;
            });
        }
        /**
         * Whether or not all attributes for a selected section have been selected
         */
        selectedAllAttributes() {
            return this.selectedAttributes.length === this.sectionAttributes.length;
        }
        /**
         * Whether or not some attributes for a selected section have been selected
         */
        selectedSomeAttributes() {
            return this.selectedAttributes.length > 0;
        }
        /**
         * Select all selected section attributes checkbox has been clicked; Sets the list of selected attributes & sets
         * the selectAllAttrIcon
         */
        onSelectAllAttributes() {
            if (this.selectedAllAttributes()) {
                this.selectedAttributes = [];
            } else {
                this.selectedAttributes = this.sectionAttributes.slice();
            }
            this.setAttributeSelectAllIcon();
        }
        /**
         * Sets the selectAllAttrIcon based on selected section's selected attributes
         */
        setAttributeSelectAllIcon() {
            if (this.selectedAllAttributes()) {
                this.selectAllAttrIcon = 'check_box';
            } else if (this.selectedSomeAttributes()) {
                this.selectAllAttrIcon = 'indeterminate_check_box';
            } else {
                this.selectAllAttrIcon = 'check_box_outline_blank';
            }
        }
        /**
         * A start year has been selected; Sets the end year to be equal to or greater than the start year depending on
         * which start year has been selected
         */
        onChangeStartYear() {
            if (this.hasValue(this.startYear) && this.hasValue(this.endYear)) {
                if (this.startYear > this.endYear) {
                    if (this.startYear === this.endYears[0]) {
                        this.endYear = this.startYear;
                    } else {
                        let index = 0;
                        const last = this.startYears.length - 1;
                        while (index <= last || this.endYear <= this.startYear) {
                            if (this.startYears[index] > this.endYear) {
                                this.endYear = this.startYears[index];
                            }
                            index++;
                        }
                    }
                }
            }
        }
        /**
         * An end year has been selected; Sets the start year to be equal to or less than the start year depending on
         * which end year has been selected
         */
        onChangeEndYear() {
            if (this.hasValue(this.startYear) && this.hasValue(this.endYear)) {
                if (this.endYear < this.startYear) {
                    if (this.endYear === this.startYears[0]) {
                        this.startYear = this.endYear;
                    } else {
                        let index = 0;
                        const last = this.endYears.length - 1;
                        while (index <= last || this.startYear >= this.endYear) {
                            if (this.endYears[index] < this.startYear) {
                                this.startYear = this.endYears[index];
                            }
                            index++;
                        }
                    }
                }
            }
        }
        /**
         * Sets downloadProgress & loading properties to true
         */
        startProgressStatus() {
            this.downloadProgress = true;
            this.loading = true;
        }
        /**
         * Sets downloadProgress & loading properties to false
         */
        stopProgressStatus() {
            this.downloadProgress = false;
            this.loading = false;
        }
        /**
         * Whether or not the specified item has a value (is not null, is not undefined, and is not considered empty)
         * @param item
         */
        hasValue(item: any) {
            return !R.isNil(item) && !R.isEmpty(item);
        }
    }
</script>