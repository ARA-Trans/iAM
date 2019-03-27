<template>
    <v-layout>
        <v-dialog v-model="showDialog" persistent max-width="450px">
            <v-card>
                <v-card-title>
                    <v-layout align-end justify-space-between row fill-height>
                        <h3>New Performance Strategy Library</h3>
                        <v-btn flat icon v-on:click="showDialog = false">
                            <v-icon>clear</v-icon>
                        </v-btn>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column fill-height>
                        <v-text-field label="Name"
                                      v-model="createdPerformanceStrategy.name"
                                      outline>
                        </v-text-field>
                        <v-textarea class="text-area" no-resize outline full-width
                                    :label="createdPerformanceStrategy.description === ''
                                                    ? 'Description' : ''"
                                    v-model="createdPerformanceStrategy.description">
                        </v-textarea>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row fill-height>
                        <v-btn color="info"
                               v-on:click="onCreatePerformanceStrategy(true)"
                               :disabled="createdPerformanceStrategy.name === ''">
                            Submit
                        </v-btn>
                        <v-btn v-on:click="showDialog = false">
                            Cancel
                        </v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop} from 'vue-property-decorator';
    import {CreatedPerformanceStrategy, emptyCreatedPerformanceStrategy} from "@/shared/models/iAM/performance";

    @Component
    export default class CreatePerformanceStrategyDialog extends Vue {
        @Prop() showDialog: boolean;

        createdPerformanceStrategy: CreatedPerformanceStrategy = {...emptyCreatedPerformanceStrategy};

        /**
         * Component has been mounted
         */
        mounted() {
            // set app as busy and call the server to get all networks
            this.setIsBusyAction({isBusy: true});
            this.getNetworksAction()
                .then(() => this.setIsBusyAction({isBusy: false}))
                .catch((error: any) => {
                    this.setIsBusyAction({isBusy: false});
                    console.log(error);
                })
        }
    }
</script>