<template>
    <v-progress-linear :indeterminate="true"></v-progress-linear>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';

    @Component
    export default class AppShowProgress extends Vue {
    }
</script>
