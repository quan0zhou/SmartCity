import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

import { Toast } from 'vant';
import 'vant/es/toast/style';

// Dialog
import { Dialog } from 'vant';
import 'vant/es/dialog/style';

// Notify
import { Notify } from 'vant';
import 'vant/es/notify/style';

// ImagePreview
import { ImagePreview } from 'vant';
import 'vant/es/image-preview/style';
// import './assets/main.css'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(Toast)
app.use(Dialog)
app.use(Notify)
app.use(ImagePreview)

app.mount('#app')
