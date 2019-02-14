<template>
    <v-form>
        {{message}}
    </v-form>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';

    @Component
    export default class EditAnalysis extends Vue {
        marker: boolean = true
        message: string = 'Test'

        created() {
            this.marker = true
        }

        toggleMarker() {
            this.marker = !this.marker
        }
        editAnalysis() {
            this.$router.push('EditAnalysis');
        }
    }
</script>

<style scoped>
</style>