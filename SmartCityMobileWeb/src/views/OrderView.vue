<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import Refresh    from '@/components/RefreshComponent.vue'
const router = useRouter()
const activeName = ref('');

const onClickLeft = () => {
  router.push('/')
}
</script>
<template>
  <van-nav-bar title="我的订单" left-text="预约" left-arrow @click-left="onClickLeft" />

  <van-tabs v-model:active="activeName" sticky>
    <van-tab title="全部" name="">
      <Refresh :activeName="activeName"></Refresh>
    </van-tab>
    <van-tab title="预订待支付" name="0">
      <Refresh :activeName="activeName"></Refresh>
    </van-tab>
    <van-tab title="已预订" name="1">
      <Refresh :activeName="activeName"></Refresh>
    </van-tab>
    <van-tab title="退款待确认" name="3">
      <Refresh :activeName="activeName"></Refresh>
    </van-tab>
    <van-tab title="已退款" name="2">
      <Refresh :activeName="activeName"></Refresh>
    </van-tab>
    <van-tab title="拒绝退款" name="4">
      <Refresh :activeName="activeName"></Refresh>
    </van-tab>
  </van-tabs>
 
</template>

<style lang="less" scoped>

</style>
