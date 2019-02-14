<template>
    <v-form>
        <v-card>
            <v-container>
                <v-flex xs12>
                    <v-menu ref="menu"
                            v-model="menu"
                            :close-on-content-click="false"
                            :nudge-right="40"
                            :return-value.sync="date"
                            lazy
                            transition="scale-transition"
                            offset-y
                            full-width
                            min-width="290px">
                        <v-text-field slot="activator"
                                        v-model="date"
                                        label="Start year"
                                        prepend-icon="event"
                                        readonly
                                        placeholder="Start year"></v-text-field>
                        <v-date-picker v-model="date" no-title scrollable>
                            <v-spacer></v-spacer>
                            <v-btn flat color="primary" @click="menu = false">Cancel</v-btn>
                            <v-btn flat color="primary" @click="$refs.menu.save(date)">OK</v-btn>
                        </v-date-picker>
                    </v-menu>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model.number="analysisPeriod"
                                    label="Analysis period"
                                    type="number"></v-text-field>
                </v-flex>
                <v-divider inset></v-divider>
                <v-flex xs12>
                    <v-select :items="optimizationType"
                                label="Optimization type"
                                outline></v-select>
                </v-flex>
                <v-flex xs12>
                    <v-select :items="budgetType"
                                label="Budget type"
                                outline></v-select>
                </v-flex>
                <v-flex xs12>
                    <v-text-field v-model.number="benefitLimit"
                                    label="Benefit limit" outline
                                    type="number"></v-text-field>
                </v-flex>
                <v-divider inset></v-divider>
                <v-flex xs12>
                    <v-textarea outline
                                name="input-7-4"
                                label="Description"
                                value="The Woodman set to work at once, and so sharp was his axe that the tree was soon chopped nearly through."></v-textarea>
                </v-flex>
                <v-layout row wrap>
                    <v-flex xs12>
                        <v-btn depressed color="primary">Apply</v-btn>
                        <v-btn depressed color="grey" @click="cancel">Cancel</v-btn>
                    </v-flex>
                </v-layout>
            </v-container>
        </v-card>
    </v-form>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';

    @Component
    export default class EditAnalysis extends Vue {
        message: string = 'Test'

        data() {
            return {
                date: new Date().toISOString().substr(0, 10),
                menu: false,
                modal: false,
                analysisPeriod: 0,
                benefitLimit: 0,
                optimizationType: ['Incremental benefit/cost', 'Another one', 'The better one'],
                budgetType: ['As budget permits', 'Another one', 'The better one'],
            }
        }
        cancel() {
            this.$router.push('EditScenario')
        }
    }
</script>

<style scoped>
</style>