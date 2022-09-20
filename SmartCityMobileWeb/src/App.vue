<script setup lang="ts">
import { RouterView } from 'vue-router'
import {ref} from 'vue'
const active=ref(0)
</script>

<template>
  <van-tabbar v-model="active" route>
  <van-tabbar-item icon="home-o" replace to="/">预约</van-tabbar-item>
  <van-tabbar-item icon="friends-o" replace to="/order">我的订单</van-tabbar-item>
</van-tabbar>

  <RouterView />
<!-- 开启顶部安全区适配 -->
<van-nav-bar safe-area-inset-top />

<!-- 开启底部安全区适配 -->
<van-number-keyboard safe-area-inset-bottom />
</template>

<style scoped>

</style>
