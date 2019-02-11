import '@babel/polyfill'
import Vue from 'vue'
import 'vuetify/dist/vuetify.min.css'
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css'
import Vuetify from 'vuetify'

import App from './App.vue';
import router from './router'
import store from './store'

Vue.use(Vuetify)

Vue.config.productionTip = false;

new Vue({
    render: h => h(App),
    store,
    router
}).$mount('#app');
