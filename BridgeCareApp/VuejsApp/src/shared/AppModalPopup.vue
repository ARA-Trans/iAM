<template>
    <v-layout row justify-center>
        <v-dialog v-model="modalData.showModal" persistent
                  max-width="290">
            <v-card>
                <v-card-title class="headline">{{modalData.heading}}</v-card-title>

                <v-card-text>
                    {{modalData.message}}
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
    import { IAlert } from '@/models/IAlert';

    @Component
    export default class AppModalPopup extends Vue {

        @Prop()
        modalData: IAlert

        sendUserSelection(decision: boolean) {
            this.modalData
            this.$emit('decision', decision)
        }
    }
</script>