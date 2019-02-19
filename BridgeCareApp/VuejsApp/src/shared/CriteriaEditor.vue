<template>
    <vue-query-builder :rules="rules" maxDepth="25" v-model="query"></vue-query-builder>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';
    import VueQueryBuilder from 'vue-query-builder/src/VueQueryBuilder.vue';
    import {Criteria, CriteriaType, CriteriaRule, emptyCriteria} from '../models/criteria';

    @Component({
        components: {VueQueryBuilder}
    })
    export default class CriteriaEditor extends Vue {
        query: Criteria = emptyCriteria;
    }
</script>