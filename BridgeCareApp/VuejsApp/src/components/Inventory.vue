<template>
    <v-container fluid grid-list-xl>
        <!--<v-layout row>
            <v-flex xs1>
                <v-slider class="slider" v-model="referenceIndexTypes" :tick-labels="referenceIndexTypesLabels" tick-size="2" ticks="always" step="1" :max="1">
                </v-slider>
            </v-flex>
        </v-layout>-->
        <v-layout justify-space-between row fill-height>
            <v-spacer></v-spacer>
            <v-flex xs2>
                <v-autocomplete :items="bmsIdsSelectList" label="Select by BMS Id" outline v-model="selectedBmsId"
                                @change="onSelectInventoryItemByBMSId" item-text="identifier" item-value="identifier">
                    <template slot="item" slot-scope="data">
                        <template v-if="typeof data.item !== 'object'">
                            <v-list-tile-content v-text="data.item"></v-list-tile-content>
                        </template>
                        <template v-else>
                            <v-list-tile-content>
                                <v-list-tile-title v-html="data.item.identifier"></v-list-tile-title>
                            </v-list-tile-content>
                        </template>
                    </template>
                </v-autocomplete>
            </v-flex>
            <v-flex xs2>
                <v-autocomplete :items="brKeysSelectList" label="Select by BR Key" outline v-model="selectedBrKey"
                                @change="onSelectInventoryItemsByBRKey" item-text="identifier" item-value="identifier">
                    <template slot="item" slot-scope="data">
                        <template v-if="typeof data.item !== 'object'">
                            <v-list-tile-content v-text="data.item"></v-list-tile-content>
                        </template>
                        <template v-else>
                            <v-list-tile-content>
                                <v-list-tile-title v-html="data.item.identifier"></v-list-tile-title>
                            </v-list-tile-content>
                        </template>
                    </template>
                </v-autocomplete>
            </v-flex>
            <v-spacer></v-spacer>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0">
            <v-flex xs12>
                <v-layout justify-space-between row>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Location</h3>
                            </v-layout>
                            <div v-for="locationGrouping in inventoryItemDetail.location"
                                 class="text-field-div">
                                <v-text-field :label="locationGrouping.label" :value="locationGrouping.value" readonly outline>
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
                                <v-text-field :label="ageAndServiceGrouping.label" :value="ageAndServiceGrouping.value" readonly outline>
                                </v-text-field>
                            </div>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0">
            <v-flex xs12>
                <v-layout justify-space-between row>
                    <div class="grouping-div">
                        <v-layout align-center fill-height>                           
                            <iframe class="gmap_canvas" :src="getGMapsUrl()" frameborder="0" scrolling="no" marginheight="0" marginwidth="0"></iframe>
                        </v-layout>
                    </div>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Management</h3>
                            </v-layout>
                            <div v-for="managementGrouping in inventoryItemDetail.management"
                                 class="text-field-div">
                                <v-text-field :label="managementGrouping.label" :value="managementGrouping.value" readonly outline>
                                </v-text-field>
                            </div>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0">
            <v-flex xs12>
                <v-layout justify-space-between row>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Deck Information</h3>
                            </v-layout>
                            <div v-for="deckInformationGrouping in inventoryItemDetail.deckInformation"
                                 class="text-field-div">
                                <v-text-field :label="deckInformationGrouping.label" :value="deckInformationGrouping.value" readonly outline>
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
                                <v-text-field :label="spanInformationGrouping.label" :value="spanInformationGrouping.value" readonly outline>
                                </v-text-field>
                            </div>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0">
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
        <v-divider v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0">
            <v-flex xs12>
                <v-layout justify-center>
                    <div class="grouping-div">
                        <v-layout column>
                            <v-layout justify-center>
                                <h3 class="grouping-header">Risk Scores</h3>
                            </v-layout>
                            <div>
                                <v-layout justify-space-between row>
                                    <v-text-field label="New Risk Score" :value="inventoryItemDetail.riskScores.new" readonly outline>
                                    </v-text-field>
                                    <v-text-field label="Old Risk Score" :value="inventoryItemDetail.riskScores.old" readonly outline>
                                    </v-text-field>
                                </v-layout>
                            </div>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0">
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
                                            <v-text-field :label="ratingRow.operatingRating.label" :value="ratingRow.operatingRating.value" readonly outline>                             
                                            </v-text-field>
                                        </div>
                                        <div class="small-text-field-div">
                                            <v-text-field :label="ratingRow.inventoryRating.label" :value="ratingRow.inventoryRating.value" readonly outline>                                                
                                            </v-text-field>
                                        </div>
                                        <div class="small-text-field-div">
                                            <v-text-field :label="ratingRow.ratioLegalLoad.label" :value="ratingRow.ratioLegalLoad.value" readonly outline>                                                
                                            </v-text-field>
                                        </div>
                                    </v-layout>
                                </div>
                            </v-layout>
                            <v-layout justify-center>
                                <div class="small-text-field-div">
                                    <v-text-field :label="inventoryItemDetail.operatingRatingInventoryRatingGrouping.minRatioLegalLoad.label"
                                                 :value="inventoryItemDetail.operatingRatingInventoryRatingGrouping.minRatioLegalLoad.value" readonly outline>                                        
                                    </v-text-field>
                                </div>
                            </v-layout>
                        </v-layout>
                    </div>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-divider v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0">
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
        <v-divider v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0">
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
        <v-divider v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0"></v-divider>
        <v-layout v-if="inventoryItemDetail.bmsId > 0 || inventoryItemDetail.brKey > 0">
            <v-flex xs12>
                <v-layout justify-center>
                    <div class="grouping-div">
                        <v-layout justify-center column>
                            <v-layout justify-center>
                                <h3>Roadway Info</h3>
                            </v-layout>
                            <div v-for="roadwayInfoGrouping in inventoryItemDetail.roadwayInfo"
                                 class="text-field-div">
                                <v-text-field :label="roadwayInfoGrouping.label" :value="roadwayInfoGrouping.value" readonly outline> 
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
    import {InventoryItem, InventoryItemDetail, LabelValue, NbiLoadRating} from '@/shared/models/iAM/inventory';
    import {uniq, groupBy, contains, concat, isNil} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {DataTableRow} from '@/shared/models/vue/data-table-row';

    @Component
    export default class Inventory extends Vue {
        @State(state => state.inventory.inventoryItems) inventoryItems: InventoryItem[];
        @State(state => state.inventory.inventoryItemDetail) inventoryItemDetail: InventoryItemDetail;
        @State(state => state.inventory.lastFiveBmsIdSearches) stateLastFiveBmsIdSearches: string[];
        @State(state => state.inventory.lastFiveBrKeySearches) stateLastFiveBrKeySearches: number[];

        @Action('getInventory') getInventoryAction: any;
        @Action('getInventoryItemDetailByBMSId') getInventoryItemDetailByBMSIdAction: any;
        @Action('getInventoryItemDetailByBRKey') getInventoryItemDetailByBRKeyAction: any;
        @Action('appendBmsIdSearchString') appendBmsIdSearchStringAction: any;
        @Action('appendBrKeySearchNumber') appendBrKeySearchNumberAction: any;
        @Action('setIsBusy') setIsBusyAction: any;

        /*referenceIndexTypes: number = 0;
        referenceIndexTypesLabels = ['BMS ID', 'BR KEY'];*/
        // bmsIds: any[] = [];
        lastFiveBmsIdSearches: any[] = [];
        bmsIdsSelectList: any[] = [];
        selectedBmsId: string = '';
        // brKeys: any[] = [];
        lastFiveBrKeySearches: any[] = [];
        brKeysSelectList: any[] = [];
        selectedBrKey: number = 0;
        conditionTableHeaders: DataTableHeader[] = [
            {text: '', value: '', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Condition', value: '', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Duration (years)', value: '', align: 'center', sortable: false, class: '', width: ''}
        ];
        nbiLoadRatingTableHeaders: DataTableHeader[] = [];
        nbiLoadRatingTableRows: DataTableRow[] = [];
        postingTableHeaders: DataTableHeader[] = [];
        postingTableRows: DataTableRow[] = [];
        inventorySelectListsWorker: any = null;

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        @Watch('inventoryItems')
        onInventoryItemsChanged() {
            this.setupSelectLists();
            /*if (this.inventoryItems.length > 0) {
                this.setInventorySelectLists();
            }*/
        }

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        @Watch('stateLastFiveBmsIdSearches')
        onLastFiveBmsIdSearchesChanged() {
            if (hasValue(this.stateLastFiveBmsIdSearches)) {
                this.lastFiveBmsIdSearches = this.setLastFiveSearchesForInventorySelectList(this.stateLastFiveBmsIdSearches);
                this.setupSelectLists();
            }

            /*if (this.stateLastFiveBmsIdSearches.length > 0) {
                this.setInventorySelectLists();
            }*/
        }

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        @Watch('stateLastFiveBrKeySearches')
        onLastFiveBrKeySearchesChanged() {
            if (hasValue(this.stateLastFiveBrKeySearches)) {
                this.lastFiveBrKeySearches = this.setLastFiveSearchesForInventorySelectList(this.stateLastFiveBrKeySearches);
                this.setupSelectLists();
            }

            /*if (this.stateLastFiveBrKeySearches.length > 0) {
                this.setInventorySelectLists();
            }*/

        }

        @Watch('inventoryItemDetail')
        onInventoryItemDetailChanged(inventoryItemDetail: InventoryItemDetail) {
            if (inventoryItemDetail.nbiLoadRatings.length > 0) {
                // get the nbiLoadRating column names using the inventoryItemDetail.nbiLoadRatings 1st entry
                const nbiLoadRatingColumns: string[] = uniq(inventoryItemDetail.nbiLoadRatings[0].nbiLoadRatingRow
                    .map((labelValue: LabelValue) => labelValue.label) as string[]
                );
                // set nbiLoadRatingTableHeaders using nbiLoadRatingColumns
                this.nbiLoadRatingTableHeaders = nbiLoadRatingColumns.map((columnName: string) => ({
                    text: columnName, value: columnName, align: 'center', sortable: false, class: '', width: ''
                }) as DataTableHeader);
                // set the nbiLoadRatingTableRows
                this.nbiLoadRatingTableRows = this.createDataTableRowFromNbiLoadRatingGrouping(inventoryItemDetail.nbiLoadRatings);
            }
            else {
                this.nbiLoadRatingTableRows = [];
            }

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

        /**
         * Vue component has been mounted
         */
        mounted() {
            //this.$forceUpdate();
            this.getInventoryAction({network: {}});
        }

        created() {
            this.inventorySelectListsWorker = this.$worker.create(
                [
                    {message: 'setInventorySelectLists', func: (data: any) =>
                        {
                            if(data) {
                                const inventoryItems = data.inventoryItems;
                                const stateLastFiveBmsIdSearches = data.stateLastFiveBmsIdSearches;
                                const stateLastFiveBrKeySearches = data.stateLastFiveBrKeySearches;
                                const lastFiveBmsIdSearches = data.lastFiveBmsIdSearches;
                                const lastFiveBrKeySearches = data.lastFiveBrKeySearches;

                                const bmsIds: any[] = [];
                                const brKeys: any[] = [];

                                inventoryItems.forEach((item: InventoryItem, index: number) => {
                                    if (index === 0) {
                                        bmsIds.push({header: 'BMS Ids'});

                                        brKeys.push({header: 'BR Keys'});
                                    }

                                    if (stateLastFiveBmsIdSearches.indexOf(item.bmsId) === -1) {
                                        bmsIds.push({
                                            identifier: item.bmsId,
                                            group: 'BMS Ids'
                                        });
                                    }

                                    if (stateLastFiveBrKeySearches.indexOf(item.brKey) === -1) {
                                        brKeys.push({
                                            identifier: item.brKey,
                                            group: 'BR Keys'
                                        });
                                    }
                                });

                                const bmsIdsSelectList = lastFiveBmsIdSearches.concat(bmsIds);
                                const brKeysSelectList = lastFiveBrKeySearches.concat(brKeys);

                                return {bmsIdsSelectList: bmsIdsSelectList, brKeysSelectList: brKeysSelectList};
                            }

                            return {bmsIdsSelectList: [], brKeysSelectList: []};
                        }
                    }
                ]
            );
        }

        setupSelectLists() {
            const data: any = {
                inventoryItems: this.inventoryItems,
                stateLastFiveBmsIdSearches: this.stateLastFiveBmsIdSearches,
                stateLastFiveBrKeySearches: this.stateLastFiveBrKeySearches,
                lastFiveBmsIdSearches: this.lastFiveBmsIdSearches,
                lastFiveBrKeySearches: this.lastFiveBrKeySearches,
            };
            this.inventorySelectListsWorker.postMessage('setInventorySelectLists', [data])
            .then((result: any) => {
                this.bmsIdsSelectList = result.bmsIdsSelectList;
                this.brKeysSelectList = result.brKeysSelectList;
            });
        }

        setLastFiveSearchesForInventorySelectList = (searchData: any[]) => {
            const lastFiveSearches: any[] = [];

            searchData.forEach((searchValue: any, index: number) => {
                if (index === 0) {
                    lastFiveSearches.push({header: 'Last Five Searches'});
                }

                lastFiveSearches.push({
                    identifier: searchValue,
                    group: 'Last Five Searches'
                });

                if (index === searchData.length - 1) {
                    lastFiveSearches.push({divider: true});
                }
            });

            return lastFiveSearches;
        };

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

        createDataTableRowFromNbiLoadRatingGrouping(nbiLoadRatingList: NbiLoadRating[]) {
            // create a DataTableRow list
            const dataTableRows: DataTableRow[] = [];
            for (let index = 0; index < nbiLoadRatingList.length; index++) {
                // group the LabelValue list by the label prop
                const groups = groupBy((labelValue: LabelValue) => labelValue.label, nbiLoadRatingList[index].nbiLoadRatingRow);
                // get the list of group keys
                const keys = Object.keys(groups);
                // get the length of the first LabelValue group using the first key in keys if keys has a value
                const groupsLength = hasValue(keys) ? groups[keys[0]].length : 0;

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
            }
            return dataTableRows;
        }

        /**
         * BMS id has been selected
         */
        onSelectInventoryItemByBMSId(bmsId: string) {
            this.selectedBrKey = 0;
            this.getInventoryItemDetailByBMSIdAction({bmsId: bmsId})
                .then(() => setTimeout(() => {
                    this.selectedBmsId = bmsId;
                    this.appendBmsIdSearchStringAction({bmsId: bmsId});
                }));
        }

        /**
         * BR key has been selected
         */
        onSelectInventoryItemsByBRKey(brKey: number) {
            this.selectedBmsId = '';
            this.getInventoryItemDetailByBRKeyAction({brKey: brKey})
                .then(() => setTimeout(() => {
                    this.selectedBrKey = brKey;
                    this.appendBrKeySearchNumberAction({brKey: brKey});
                }));
        }
                
        getGMapsUrl() {
            var url = `https://maps.google.com/maps?q=${this.inventoryItemDetail.name}&t=&z=15&ie=UTF8&iwloc=&output=embed`;
            return encodeURI(url);
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

    .slider{
        width: 150px;
    }
</style>