<template>
    <v-container fluid grid-list-xl>
        <v-layout row>
            <v-flex xs1>
                <v-slider v-model="referenceIndexTypes" :tick-labels="referenceIndexTypesLabels" tick-size="2" ticks="always"
                          step="1" :max="1" v-on:change="onToggleReferenceIndexTypeSelect">
                </v-slider>
            </v-flex>
            <v-flex xs2>
                <v-select v-if="referenceIndexTypes === 0" :items="referenceIds" label="Select a BMS Id"
                          v-on:change="onSelectInventoryItemByRefId" outline>
                </v-select>

                <v-select v-if="referenceIndexTypes === 1" :items="referenceKeys" label="Select a BR Key"
                          v-on:change="onSelectInventoryItemsByRefKey" outline>
                </v-select>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.simulationId > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.simulationId > 0">
            <v-flex xs12>
                <v-layout justify-space-between row>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Location</h3>
                            </v-layout>
                            <div v-for="locationGrouping in inventoryItemDetail.location"
                                 class="text-field-div" >
                                <v-text-field :label="locationGrouping.label" readonly outline>
                                    {{locationGrouping.value}}
                                </v-text-field>
                            </div>
                        </v-layout>
                    </div>
                    <div class="grouping-div">
                        <v-layout justify-center column class="text-field-div">
                            <v-layout justify-center>
                                <h3>Age and Service</h3>
                            </v-layout>
                            <div v-for="ageAndServiceGrouping in inventoryItemDetail.ageAndService"
                                 class="text-field-div">
                                <v-text-field :label="ageAndServiceGrouping.label" readonly outline>
                                    {{ageAndServiceGrouping.value}}
                                </v-text-field>
                            </div>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.simulationId > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.simulationId > 0">
            <v-flex xs12>
                <v-layout justify-space-between row>
                    <div class="grouping-div">
                        <v-layout align-center fill-height>
                            <iframe class="gmap_canvas"
                                    src="https://maps.google.com/maps?q=ben%20franklin%20bridge%20pennsylvania&t=&z=15&ie=UTF8&iwloc=&output=embed"
                                    frameborder="0" scrolling="no" marginheight="0" marginwidth="0">
                            </iframe>
                        </v-layout>
                    </div>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Management</h3>
                            </v-layout>
                            <div v-for="managementGrouping in inventoryItemDetail.management"
                                 class="text-field-div">
                                <v-text-field :label="managementGrouping.label" readonly outline>
                                    {{managementGrouping.value}}
                                </v-text-field>
                            </div>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.simulationId > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.simulationId > 0">
            <v-flex xs12>
                <v-layout justify-space-between row>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Deck Information</h3>
                            </v-layout>
                            <div v-for="deckInformationGrouping in inventoryItemDetail.deckInformation"
                                 class="text-field-div">
                                <v-text-field :label="deckInformationGrouping.label" readonly outline>
                                    {{deckInformationGrouping.value}}
                                </v-text-field>
                            </div>
                        </v-layout>
                    </div>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Span Information</h3>
                            </v-layout>
                            <div v-for="spanInformationGrouping in inventoryItemDetail.spanInformation"
                                 class="text-field-div">
                                <v-text-field :label="spanInformationGrouping.label" readonly outline>
                                    {{spanInformationGrouping.value}}
                                </v-text-field>
                            </div>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.simulationId > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.simulationId > 0">
            <v-flex xs12>
                <v-layout justify-space-between row>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Current</h3>
                            </v-layout>
                            <v-data-table :headers="conditionTableHeaders"
                                          :items="inventoryItemDetail.currentConditionDuration"
                                          class="elevation-1">
                                <template slot="items" slot-scope="props">
                                    <td class="text-align-center">{{props.item.name}}</td>
                                    <td class="text-align-center">{{props.item.condition}}</td>
                                    <td class="text-align-center">{{props.item.duration}}</td>
                                </template>
                            </v-data-table>
                        </v-layout>
                    </div>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Previous</h3>
                            </v-layout>
                            <v-data-table :headers="conditionTableHeaders"
                                          :items="inventoryItemDetail.previousConditionDuration"
                                          class="elevation-1">
                                <template slot="items" slot-scope="props">
                                    <td class="text-align-center">{{props.item.name}}</td>
                                    <td class="text-align-center">{{props.item.condition}}</td>
                                    <td class="text-align-center">{{props.item.duration}}</td>
                                </template>
                            </v-data-table>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.simulationId > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.simulationId > 0">
            <v-flex xs12>
                <v-layout justify-center>
                    <div class="grouping-div">
                        <v-layout column>
                            <v-layout justify-center>
                                <h3 class="grouping-header">Risk Scores</h3>
                            </v-layout>
                            <div>
                                <v-layout justify-space-between row>
                                    <v-text-field label="New Risk Score" readonly outline>
                                        {{inventoryItemDetail.riskScores.new}}
                                    </v-text-field>
                                    <v-text-field label="Old Risk Score" readonly outline>
                                        {{inventoryItemDetail.riskScores.old}}
                                    </v-text-field>
                                </v-layout>
                            </div>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.simulationId > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.simulationId > 0">
            <v-flex xs12>
                <v-layout justify-center>
                    <div class="unsized-grouping-div">
                        <v-layout column>
                            <v-layout justify-center>
                                <h3 class="grouping-header">Operating Rating (OR) vs Inventory Rating (IR)</h3>
                            </v-layout>
                            <v-layout column>
                                <div v-for="ratingRow in inventoryItemDetail.operatingRatingInventoryRatingGrouping.ratingRows">
                                    <v-layout justify-space-between row>
                                        <div class="small-text-field-div">
                                            <v-text-field :label="ratingRow.operatingRating.label" readonly outline>
                                                {{ratingRow.operatingRating.value}}
                                            </v-text-field>
                                        </div>
                                        <div class="small-text-field-div">
                                            <v-text-field :label="ratingRow.inventoryRating.label" readonly outline>
                                                {{ratingRow.inventoryRating.value}}
                                            </v-text-field>
                                        </div>
                                        <div class="small-text-field-div">
                                            <v-text-field :label="ratingRow.ratioLegalLoad.label" readonly outline>
                                                {{ratingRow.ratioLegalLoad.value}}
                                            </v-text-field>
                                        </div>
                                    </v-layout>
                                </div>
                            </v-layout>
                            <v-layout justify-center>
                                <div class="small-text-field-div">
                                    <v-text-field :label="inventoryItemDetail.operatingRatingInventoryRatingGrouping.minRatioLegalLoad.label"
                                                  readonly outline>
                                        {{inventoryItemDetail.operatingRatingInventoryRatingGrouping.minRatioLegalLoad.value}}
                                    </v-text-field>
                                </div>
                            </v-layout>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.simulationId > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.simulationId > 0">
            <v-flex xs12>
                <v-layout justify-center>
                    <div class="unsized-grouping-div">
                        <v-layout justify-center>
                            <h3>NBI Load Rating</h3>
                        </v-layout>
                        <v-data-table :items="nbiLoadRatingTableRows"
                                      :headers="nbiLoadRatingTableHeaders"
                                      class="elevation-1">
                            <template slot="items" slot-scope="props">
                                <td v-for="header in nbiLoadRatingTableHeaders" class="text-align-center">
                                    {{props.item[header.value]}}
                                </td>
                            </template>
                        </v-data-table>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.simulationId > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.simulationId > 0">
            <v-flex xs12>
                <v-layout justify-center>
                    <div class="unsized-grouping-div">
                        <v-layout justify-center>
                            <h3>Posting</h3>
                        </v-layout>
                        <v-data-table :items="postingTableRows"
                                      :headers="postingTableHeaders"
                                      class="elevation-1">
                            <template slot="items" slot-scope="props">
                                <td v-for="header in postingTableHeaders" class="text-align-center">
                                    {{props.item[header.value]}}
                                </td>
                            </template>
                        </v-data-table>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.simulationId > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.simulationId > 0">
            <v-flex xs12>
                <v-layout justify-center>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Roadway Info</h3>
                            </v-layout>
                            <div v-for="roadwayInfoGrouping in inventoryItemDetail.roadwayInfo"
                                 class="text-field-div">
                                <v-text-field :label="roadwayInfoGrouping.label" readonly outline>
                                    {{roadwayInfoGrouping.value}}
                                </v-text-field>
                            </div>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import axios from 'axios';

    import {
        InventoryItem,
        InventoryItemDetail,
        LabelValue
    } from '@/shared/models/iAM/inventory';
    import {uniq, groupBy} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import { DataTableRow } from '@/shared/models/vue/data-table-row';
    import { Network } from '@/shared/models/iAM/network';

    axios.defaults.baseURL = process.env.VUE_APP_URL;

    @Component
    export default class Inventory extends Vue {
        @State(state => state.busy.isBusy) isBusy: boolean;
        @State(state => state.inventory.inventoryItems) inventoryItems: InventoryItem[];
        @State(state => state.inventory.inventoryItemDetail) inventoryItemDetail: InventoryItemDetail;
        @State(state => state.network.networks) networks: Network[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getInventory') getInventoryAction: any;
        @Action('getInventoryItemDetail') getInventoryItemDetailAction: any;
        @Action('getNetworks') getNetworksAction: any;

        referenceIndexTypes: number = 0;
        referenceIndexTypesLabels = ['BMS ID', 'BR KEY'];
        referenceIds: number[] = [];
        referenceKeys: number[] = [];
        conditionTableHeaders: DataTableHeader[] = [
            {text: '', value: '', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Condition', value: '', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Duration (years)', value: '', align: 'center', sortable: false, class: '', width: ''}
        ];
        nbiLoadRatingTableHeaders: DataTableHeader[] = [];
        nbiLoadRatingTableRows: DataTableRow[] = [];
        postingTableHeaders: DataTableHeader[] = [];
        postingTableRows: DataTableRow[] = [];

        /**
         * inventoryItems state has changed
         */
        @Watch('inventoryItems')
        onInventoryItemsChanged(inventoryItems: InventoryItem[]) {
            this.referenceIds = inventoryItems.map((item: InventoryItem) => item.referenceId);
            this.referenceKeys = inventoryItems.map((item: InventoryItem) => item.referenceKey);
        }

        @Watch('inventoryItemDetail')
        onInventoryItemDetailChanged(inventoryItemDetail: InventoryItemDetail) {
            // get the nbiLoadRating column names using the inventoryItemDetail.nbiLoadRating LabelValue list
            const nbiLoadRatingColumns: string[] = uniq(inventoryItemDetail.nbiLoadRating
                .map((labelValue: LabelValue) => labelValue.label) as string[]
            );
            // set nbiLoadRatingTableHeaders using nbiLoadRatingColumns
            this.nbiLoadRatingTableHeaders = nbiLoadRatingColumns.map((columnName: string) => ({
                text: columnName, value: columnName, align: 'center', sortable: false, class: '', width: ''
            }) as DataTableHeader);
            // set the nbiLoadRatingTableRows using the inventoryItemDetail.nbiLoadRating LabelValue list
            this.nbiLoadRatingTableRows = this.createDataTableRowFromGrouping(inventoryItemDetail.nbiLoadRating);
            // get the posting column names using the inventoryItemDetail.posting LabelValue list
            const postingColumns: string[] = uniq(inventoryItemDetail.posting
                .map((labelValue: LabelValue) => labelValue.label) as string[]
            );
            // set postingTableHeaders using postingColumns
            this.postingTableHeaders = postingColumns.map((columnName: string) => ({
                text: columnName, value: columnName, align: 'center', sortable: false, class: '', width: ''
            }) as DataTableHeader);
            // set the postingTableRows using the createDataTableRowFromGrouping func.
            this.postingTableRows = this.createDataTableRowFromGrouping(inventoryItemDetail.posting);

        }

        createDataTableRowFromGrouping(labelValueList: LabelValue[]) {
            // group the LabelValue list by the label prop
            const groups = groupBy((labelValue: LabelValue) => labelValue.label, labelValueList);
            // get the list of group keys
            const keys = Object.keys(groups);
            // get the length of the first LabelValue group using the first key in keys if keys has a value
            const groupsLength = hasValue(keys) ? groups[keys[0]].length : 0;
            // create a DataTableRow list
            const dataTableRows: DataTableRow[] = [];
            // use a for loop to create a DataTableRow to add to dataTableRows
            for (let i = 0; i < groupsLength; i++) {
                // create an empty DataTableRow object
                const dataTableRow: DataTableRow = {};
                // loop over each postingGroups key, adding the key as a property to postingTableRow
                // and then getting the value of the LabelValue object at the current iteration for the current group
                Object.keys(groups).forEach((key: string) => dataTableRow[key] = hasValue(groups[key][i]) ? groups[key][i].value : '');
                // push the created postingTableRow to postingTableRows
                dataTableRows.push(dataTableRow);
            }
            return dataTableRows;
        }

        /**
         * Vue component has been mounted
         */
        mounted() {
            this.setIsBusyAction({isBusy: true});
            this.getInventoryAction({
                network: {}
            }).then(() =>
                this.setIsBusyAction({isBusy: false})
            ).catch((error: any) => {
                this.setIsBusyAction({isBusy: false});
                console.log(error);
            });
        }

        /**
         * Reference index type toggle has changed
         */
        onToggleReferenceIndexTypeSelect() {

        }

        /**
         * Reference id has been selected
         */
        onSelectInventoryItemByRefId(refId: number) {
            // find the inventory item in the list of inventory items using the refId
            const inventoryItem: InventoryItem = this.inventoryItems
                .find((item: InventoryItem) => item.referenceId === refId) as InventoryItem;
            // dispatch action to get inventory item detail
            this.setIsBusyAction({isBusy: true});
            this.getInventoryItemDetailAction({inventoryItem: inventoryItem})
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }

        /**
         * Reference key has been selected
         */
        onSelectInventoryItemsByRefKey(refKey: number) {
            // find the inventory item in the list of inventory items using the refKey
            const inventoryItem: InventoryItem = this.inventoryItems
                .find((item: InventoryItem) => item.referenceKey === refKey) as InventoryItem;
            // dispatch action to get inventory item detail
            this.setIsBusyAction({isBusy: true});
            this.getInventoryItemDetailAction({inventoryItem: inventoryItem})
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                });
        }
    }
</script>

<style>
    .grouping-div, .text-field-div {
        width: 500px;
    }

    .small-text-field-div {
        width: 300px;
    }

    .grouping-div, .unsized-grouping-div {
        margin: 30px 0;
    }

    .grouping-header {
        margin-bottom: 20px;
    }

    .gmap_canvas {
        width: 100%;
        min-height: 786px;
        height: 100%;
    }

    .text-align-center {
        text-align: center;
    }
</style>