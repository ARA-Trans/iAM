<template>
    <v-layout row justify-center>
        <v-dialog v-model="warningData.showModal" persistent
                  max-width="290">
            <v-card>
                <v-card-title class="headline">{{warningData.heading}}</v-card-title>

                <v-card-text>
                    {{warningData.message}}
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>

                    <v-btn color="green darken-1"
                           flat="flat"
                           @click="sendUserSelection(false)">
                        Cancel
                    </v-btn>

                    <v-btn color="green darken-1"
                           flat="flat"
                           @click="sendUserSelection(true)">
                        Proceed
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop } from 'vue-property-decorator';
    import { IWarning } from '@/models/IWarning';

    @Component
    export default class AppModalPupup extends Vue {

        @Prop()
        warningData: IWarning

        sendUserSelection(decision: boolean) {
            this.warningData
            this.$emit('decision', decision)
        }
    }
</script>