<template>
  <global-footer class="footer custom-render">
    <template v-slot:links>
      <!-- <a href="https://www.github.com/vueComponent/pro-layout" target="_blank">Pro Layout</a>
      <a href="https://www.github.com/vueComponent/ant-design-vue-pro" target="_blank">Github</a>
      <a href="https://www.github.com/sendya/" target="_blank">@Sendya</a> -->
      <label style="color:#00000073">备案号：</label><a href="https://beian.miit.gov.cn/" target="_blank">京ICP备2022029210号-1</a>
    </template>
    <template v-slot:copyright>
      <!-- <a href="https://beian.miit.gov.cn/" target="_blank">京ICP备2022029210号-1</a> -->
    </template>
  </global-footer>
</template>

<script>
import { GlobalFooter } from '@ant-design-vue/pro-layout'

export default {
  name: 'ProGlobalFooter',
  components: {
    GlobalFooter
  }
}
</script>
