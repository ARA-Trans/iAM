import '@babel/polyfill'
import Vue from 'vue'
import 'vuetify/dist/vuetify.min.css'
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css'
import Vuetify from 'vuetify'
//import { default as Adal, AxiosAuthHttp } from 'vue-adal'
//const Adal = require('vue-adal')

import App from './App.vue';
import router from './router'

Vue.use(Vuetify)

//Vue.use(Adal, {
//    config: {
//        tenant: 'aradomain.onmicrosoft.com',
//        clientId: '6b4fef89-350c-4dfe-8aa2-5ed5fddb9c5b',
//        redirectUri: 'http://localhost:1337/',
//        cacheLocation: 'localStorage'
//    },
//    requireAuthOnInitialize: true,
//    router: router
//})

Vue.config.productionTip = false;

new Vue({
    render: h => h(App),
    router,
}).$mount('#app');
