import '@babel/polyfill';
import '@fortawesome/fontawesome-free/css/all.css';
import Vue from 'vue';
import 'vuetify/dist/vuetify.min.css';
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import Vuetify from 'vuetify';
import VueFire from 'vuefire';

import App from './App.vue';
import router from './router';
import store from './store/root-store';

Vue.use(Vuetify, {
    iconfont: 'fa'
}, VueFire);

Vue.config.productionTip = false;

new Vue({
    store,
    router,
    render: h => h(App)
}).$mount('#app');
