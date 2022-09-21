<script setup lang="ts">
import { ref, onMounted } from 'vue'
// import { getSpaceInfo } from '@/utils/http/api/custSpace/index'
import { getTagList } from '@/utils/http/api/reservation/index'

var active=ref('')
let tagList=ref()
onMounted(async ()=>{
  tagList.value = await getTagList();
  console.log(tagList,active)
})

</script>
<template>
<van-tabs v-model:active="active" type="card" sticky>
  <van-tab v-for="tag in tagList" :key="tag.date">
    <template #title> 
      <div>
        <div>{{tag.week}}</div>
        <div>{{tag.date.substring(5)}}</div>
        <div>
          <van-tag type="danger" v-if="tag.status==0">已满</van-tag>
          <van-tag type="success" v-else>可预订</van-tag>
        </div>
      </div> 
     </template>
    <table>
      <thead>
        <tr>
          <td><sub>时间</sub>\<sup>场地</sup></td>
          <td v-for="(c,i) in tag.spaceArray" :key="i">{{ c }}</td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(t,i) in tag.timeArray" :key="i">
                    <td>{{ (t.startTimeStr+'~'+t.endTimeStr) }}</td>
                    <td  v-for="item in tag.items.filter((r: any)=>r.startTime==t.startTime)" :key="item.reservationId" :class="[item.isChecked?'ant-tag-orange':(item.isBooked?'ant-tag-green':(item.reservationStatus==0?'ant-tag-grey':''))]">
                      <div>¥ {{ item.money }}</div>
                    </td>
                  </tr>
      </tbody>
    </table>
  </van-tab>
</van-tabs>
</template>
<style lang="less" scoped>
  .van-tabs {
     /deep/  .van-tabs__wrap{
      height: 70px;
      .van-tab{
        text-align: center;
      }
      .van-tabs__nav--card{
        height: 70px;
      }
    }
  }

</style>

