import Vue from 'vue';

declare module 'vue/types/vue' {
    interface Vue {
        sockets: any;
    }
}

declare module 'vue/types/options' {
    interface ComponentOptions<V extends Vue> {
        sockets?: any;
    }
}