import { createVuestic } from 'vuestic-ui';
import 'vuestic-ui/styles/essential.css';
import 'vuestic-ui/styles/typography.css';

import { createPinia } from 'pinia';
import { createI18n } from 'vue-i18n';
import axios from 'axios';

import './assets/main.css';

import { createApp } from 'vue';
import App from './App.vue';
import router from "@/router/index.js";
import messages from '@/locs/messages.js';

// Set base URL
axios.defaults.baseURL = '/';
// Setting axios defaults with stored cookies
axios.defaults.withCredentials = true;

// create pinia
const pinia = createPinia();

// create I18n 
const i18n = createI18n({
    legacy: false,
    locale: 'ru',
    fallbackLocale: 'ru',
    messages, // import messages
});

// Create app
const app = createApp(App);

/* USE SECTION */

app.use(pinia); // use pinia
app.use(i18n); // use I18n
app.use(createVuestic()); // use vuestic
app.use(router); // use router

/* END USE SECTION */

// mount app to div
app.mount('#app')
