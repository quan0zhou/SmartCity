<script setup lang="ts">
  import {ref} from 'vue'
  import { getOrderList } from '@/utils/http/api/order/index'
  import {useRouter} from 'vue-router'
  const router=useRouter()
  const list = ref<any[]>([]);
  const loading = ref(false);
  const finished = ref(false);
  const refreshing = ref(false);
  let lastId :string='0'
  const onClickLeft=()=>{
  router.push('/')
}
  const onLoad = async () => {
     if (refreshing.value) {
          list.value = [];
          lastId='0'
          refreshing.value = false;
        }
      // 异步更新数据
      // setTimeout 仅做示例，真实场景中一般为 ajax 请求
      let data=(await getOrderList(lastId)).data
      if(data&&data.length>0){
        for (let i = 0; i < data.length; i++) {
          list.value?.push(data[i]);
          if(i==data.length-1){
            lastId=data[i]["orderId"]
          }
        }
      }
      loading.value = false;
      if (data.length <= 0) {
          finished.value = true;
      }
    };
    const onRefresh = async () => {
      // 清空列表数据
      finished.value = false;

      // 重新加载数据
      // 将 loading 设置为 true，表示处于加载状态
      loading.value = true;

      await onLoad();
    };
</script>
<template>
  <van-nav-bar
  title="我的订单"
  left-text="预约"
  left-arrow
  @click-left="onClickLeft"
/>
<van-pull-refresh v-model="refreshing" @refresh="onRefresh">
  <van-list
    v-model:loading="loading"
    :finished="finished"
    finished-text="没有更多了"
    @load="onLoad"
  >
    <van-cell v-for="item in list" :key="item.orderId" :title="item.spaceName" />
  </van-list>
</van-pull-refresh>
</template>

<style lang="less" scoped>

</style>
