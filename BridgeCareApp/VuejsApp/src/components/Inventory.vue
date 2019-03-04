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
                          v-on:change="onSelectAttribute" label="Add/Remove Attributes" multiple outline>
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
        <v-layout row wrap>
            <v-flex xs12>
                <div v-if="hasValue(selectedSectionGridData)">
                    <iframe class="gmap_canvas"
                            src="https://maps.google.com/maps?q=ben%20franklin%20bridge%20pennsylvania&t=&z=15&ie=UTF8&iwloc=&output=embed"
                            frameborder="0" scrolling="no" marginheight="0" marginwidth="0">
                    </iframe>
                </div>
            </v-flex>
        </v-layout>
        <v-layout row wrap>
            <v-flex v-if="hasValue(selectedSectionGridData)" v-for="img in images">
                <v-img :src="img.src" aspect-ratio="1"></v-img>
            </v-flex>
        </v-layout>
        <v-layout row wrap>
            <v-flex xs12>
                <AppSpinner/>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from "vue";
    import {Component, Watch} from "vue-property-decorator";
    import {Action, State} from "vuex-class";
    import axios from "axios";

    import AppSpinner from "../shared/AppSpinner.vue";
    import {Attribute, AttributesWithYearlyValues, Section} from "@/models/section";
    import * as R from "ramda";
    import * as moment from "moment";
    import {hasValue} from "@/shared/utils/has-value";

    axios.defaults.baseURL = process.env.VUE_APP_URL;

    @Component({
        components: {AppSpinner}
    })
    export default class Inventory extends Vue {
        @State(state => state.busy.isBusy) isBusy: boolean;
        @State(state => state.inventory.sections) sections: Section[];

        @Action("setIsBusy") setIsBusyAction: any;
        @Action("getNetworkInventory") getNetworkInventoryAction: any;

        sectionGridHeaders: object[] = [{text: 'Attribute', align: 'left', sortable: false, value: 'name'}];
        sectionIds: number[] = [];
        selectedSection: Section;
        sectionAttributes: string[] = [];
        selectedAttributes: string[] = [];
        selectAllAttrIcon = 'check_box_outline_blank';
        startYears: number[] = [];
        endYears: number[] =[];
        startYear: number = new Date().getFullYear();
        endYear: number = new Date().getFullYear();
        selectedSectionGridData: any[] = [];
        images: object[] = [
            {src: require("@/assets/images/inventory_mock_bridge_images/1.jpg")},
            {src: require("@/assets/images/inventory_mock_bridge_images/2.jpg")},
            {src: require("@/assets/images/inventory_mock_bridge_images/3.jpg")},
            {src: require("@/assets/images/inventory_mock_bridge_images/4.jpg")},
            {src: require("@/assets/images/inventory_mock_bridge_images/5.jpg")},
            {src: require("@/assets/images/inventory_mock_bridge_images/6.jpg")},
            {src: require("@/assets/images/inventory_mock_bridge_images/7.jpg")},
            {src: require("@/assets/images/inventory_mock_bridge_images/8.jpg")}
        ];

        @Watch("sections")
        onSectionsChanged(val: Section[]) {
            this.sectionIds = val.map((s: Section) => s.sectionId);
        }
        /**
         * Vue component has been created
         */
        mounted() {
            this.setIsBusyAction({isBusy: true});
            this.getNetworkInventoryAction({
                network: {}
            }).then(() =>
                this.setIsBusyAction({isBusy: false})
            ).catch((error: any) => {
                this.setIsBusyAction({isBusy: false});
                console.log(error);
            });
        }
        /**
         * Section has been selected; Sets up the list of values for attribute & start/end years filters and the initial
         * data grid headers
         * @param sectionId Selected section's id
         */
        onSelectSection(sectionId: number) {
            // find the section in the list of sections using the given sectionId
            //@ts-ignore
            this.selectedSection = this.sections.find((s: Section) => s.sectionId === sectionId);
            // if a section was found...
            if (hasValue(this.selectedSection)) {
                // reset the list of attributes and create a placeholder list for the years
                this.sectionAttributes = [];
                const years: number[] = [];
                // sort function that sorts by the 'year' property of attribute yearly values
                const sortByYear = R.sortBy(R.prop('year'));
                // for each of the attributes...
                this.selectedSection.attributes.forEach((a: Attribute) => {
                    // push the current attribute name onto the attributes list
                    this.sectionAttributes.push(a.name);
                    // sort the yearly values for the given attribute in ascending order
                    const sortedYearlyValuesAsc = sortByYear(a.yearlyValues);
                    // for each sorted yearly value push the yearly value year onto the years list
                    sortedYearlyValuesAsc.forEach((attributesWithYearlyValues: AttributesWithYearlyValues) =>
                        years.push(attributesWithYearlyValues.year)
                    );

                });
                // set all section attributes as selected by default
                this.selectedAttributes = this.sectionAttributes;
                // update the selectAllAttrIcon
                this.setAttributeSelectAllIcon();
                // set the start years list while removing any duplicate years
                this.startYears = R.uniq(years);
                this.startYear = hasValue(this.startYears) ? this.startYears[0] : 0;
                // set the end years list with a copy of the start years list in desc order
                this.endYears = R.reverse(this.startYears);
                this.endYear = hasValue(this.endYears) ? this.endYears[0] : 0;
                // set the selected section's grid data
                this.setSelectedSectionGridData();
            }
        }
        /**
         * Sets the selected section's grid data
         */
        setSelectedSectionGridData() {
            // get the selected attributes
            const filteredAttributes = this.selectedSection.attributes.filter((a: Attribute) =>
                this.selectedAttributes.indexOf(a.name) !== -1
            );
            // get the selected year range
            const yearRange = this.getYearRange();
            if (hasValue(filteredAttributes) && hasValue(yearRange)) {
                //reset the sectionGridHeaders list
                this.sectionGridHeaders = R.filter(h => R.propEq("value", "name", h), this.sectionGridHeaders);
                // for each of the years in the yearRange list add a new header to the sectionGridHeaders list using
                // the years as the text/value properties
                yearRange.forEach((year: number) => {
                    this.sectionGridHeaders.push(
                        {text: year.toString(), sortable: false, value: year.toString()}
                    )
                });
                // set the selected section grid data
                this.selectedSectionGridData = filteredAttributes.map((a: Attribute) => {
                    const row = {
                        name: a.name
                    };
                    yearRange.forEach((n: number) => {
                        if (R.any(R.propEq("year", n), a.yearlyValues)) {
                            //@ts-ignore
                            row[`${n}`] = a.yearlyValues.find((val: AttributesWithYearlyValues) => val.year === n).value;
                        } else {
                            //@ts-ignore
                            row[`${n}`] = "N/A";
                        }
                    });
                    return row;
                });
            } else {
                this.selectedSectionGridData = [];
            }
        }

        /**
         * Sets the selectAllAttrIcon based on selected section's selected attributes
         */
        setAttributeSelectAllIcon() {
            if (this.selectedAllAttributes()) {
                this.selectAllAttrIcon = "check_box";
            } else if (this.selectedSomeAttributes()) {
                this.selectAllAttrIcon = "indeterminate_check_box";
            } else {
                this.selectAllAttrIcon = "check_box_outline_blank";
            }
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
            this.setSelectedSectionGridData();
        }
        /**
         * An attribute has been selected/unselected; Set the selectAllAttrIcon & selectedSectionGridData
         */
        onSelectAttribute() {
            this.setAttributeSelectAllIcon();
            this.setSelectedSectionGridData();
        }
        /**
         * A start year has been selected; Sets the end year to be equal to or greater than the start year depending on
         * which start year has been selected
         */
        onChangeStartYear() {
            if (hasValue(this.startYear) && hasValue(this.endYear)) {
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
                this.setSelectedSectionGridData();
            }
        }
        /**
         * An end year has been selected; Sets the start year to be equal to or less than the start year depending on
         * which end year has been selected
         */
        onChangeEndYear() {
            if (hasValue(this.startYear) && hasValue(this.endYear)) {
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
                this.setSelectedSectionGridData();
            }
        }

        getYearRange() {
            if (this.startYear === this.endYear) {
                return [this.endYear];
            } else {
                const range: number[] = [];
                let currentYear = this.endYear;
                while (currentYear >= this.startYear) {
                    range.push(currentYear);
                    //@ts-ignore
                    currentYear = parseInt(moment().year(currentYear).subtract(1, "year").format("YYYY"));
                }
                return range;
            }
        }

        /**
         * Calls the utility hasValue function
         * @param item
         */
        hasValue(item: any) {
            return hasValue(item);
        }
    }
</script>

<style>
    .gmap_canvas {
        width: 100%;
        height: 300px;
    }
</style>