<template>
    <v-form>
        <v-card>
            <v-container>
                <v-flex xs12>
                    <v-menu ref="menu"
                            v-model="menu"
                            :close-on-content-click="false"
                            :nudge-right="40"
                            lazy
                            transition="scale-transition"
                            offset-y
                            full-width
                            min-width="290px">
                        <v-text-field slot="activator"
                                      v-model="date"
                                      label="Start year"
                                      prepend-icon="event"
                                      readonly></v-text-field>
                        <v-date-picker ref="picker"
                                       v-model="date"
                                        min="1950"
                                       reactive
                                       no-title
                                       @input="save"></v-date-picker>
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
    import { Component, Watch } from 'vue-property-decorator';

    import moment from 'moment';

    @Component
    export default class EditAnalysis extends Vue {
        menu: boolean = false
        date: string = ''
        maxDate: string = moment().year().toString()

        data() {
            return {
                analysisPeriod: 0,
                benefitLimit: 0,
                optimizationType: ['Incremental benefit/cost', 'Another one', 'The better one'],
                budgetType: ['As budget permits', 'Another one', 'The better one'],
            }
        }
        
        @Watch('menu')
        onDateChanged(val: string) {
            //@ts-ignore
            val && this.$nextTick(() => (this.$refs.picker.activePicker = 'YEAR'))
        }
        save(date: string) {
            this.date = date.substring(0, 4)
            //@ts-ignore
            this.$refs.picker.activePicker = 'YEAR'
            this.menu = false;
        }
        cancel() {
            this.$router.push('EditScenario')
        }
    }
</script>

<style scoped>
</style>