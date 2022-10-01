<script setup lang="ts">
import { ref, onBeforeMount,computed } from 'vue'
// import { getSpaceInfo } from '@/utils/http/api/custSpace/index'
import { getTagList,getTag } from '@/utils/http/api/reservation/index'
import { Toast } from 'vant';
var activeName=ref<string>()
let tagList=ref<any[]>()
  onBeforeMount(async ()=>{
  tagList.value = (await getTagList()).data;
})
const checkItem=(item:any)=>{
   if(item.reservationStatus==0){
    return
   }else{
     if(item.isChecked){
      item.isChecked=ref(false)
     }else{
      var currentTag=  tagList.value?.find((tag:any)=>{
            return tag.date==activeName.value
      })
      currentTag.items.forEach((e:any) => {
        e.isChecked=ref(false)
      });
      item.isChecked=ref(true)
     }
   }
}

const changeTab=async (name:string)=>{
  const loadingToast=Toast.loading({
    message: '加载中...',
    forbidClick: false,
    loadingType: 'spinner',
    })
   var newTag=(await getTag(name)).data
   console.log(newTag)
   loadingToast.clear()
   var currentTag=ref(tagList.value?.find((tag:any)=>{
       return tag.date==activeName.value
   }))
   Object.assign(currentTag.value,newTag)
}
const currentItem=computed(()=>{
  var currentTag=  tagList.value?.find((tag:any)=>{
      return tag.date==activeName.value
  })
  if(currentTag){
    var item=currentTag.items.find((item:any)=>{
      return item.isChecked==true
     })

     return  {money:item?.money||0,path:'/contact?id='+(item?.reservationId||'') }
  }
  return {money:0,path:'/contact'}
})

</script>
<template>
  <van-notice-bar
  left-icon="volume-o"
  text="可多次预约，单次预约只限一小时，12小时内场地不可取消，否则需要审核退款"
/>
<van-tabs v-model:active="activeName" type="card"  @change="changeTab">
  <van-tab v-for="tag in tagList" :key="tag.date" :name="tag.date">
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
    <table class="dt">
      <thead>
        <tr>
          <td><sub>时间</sub>\<sup>场地</sup></td>
          <td v-for="(c,i) in tag.spaceArray" :key="i">{{ c }}</td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(t,i) in tag.timeArray" :key="i">
                    <td>{{ (t.startTimeStr+'~'+t.endTimeStr) }}</td>
                    <td @click="checkItem(item)"  v-for="item in tag.items.filter((r: any)=>r.startTime==t.startTime)" :key="item.reservationId" :class="[item.isChecked?'ant-tag-orange':(item.reservationStatus==0?'ant-tag-grey':'')]">
                      <div v-if="item.reservationStatus==1"><label class="money_tip">¥</label> {{ item.money }}</div>
                    </td>
                  </tr>
      </tbody>
    </table>
  </van-tab>
</van-tabs>
<van-row class="tip_tag">
  <van-col span="8"><van-tag color="#969799">已预订</van-tag></van-col>
  <van-col span="8"><van-tag color="#fff" class="book">可预订</van-tag></van-col>
  <van-col span="8"><van-tag type="success">当前选中</van-tag></van-col>
</van-row>
<van-action-bar>
  <div class="van-action-bar-icon"><span class="icon_money">¥</span> <span class="money">{{currentItem.money}}</span></div>
  <!-- <van-action-bar-icon icon="chat-o" text="客服" /> -->
  <van-action-bar-button type="danger" text="立即预约" :disabled="currentItem.money<=0"  :to="currentItem.path" replace/>
</van-action-bar>
</template>
<style lang="less" scoped>
  .van-tabs {
     /deep/  .van-tabs__wrap{
      height: 70px;
      .van-tab{
        text-align: center;
        .van-tab__text--ellipsis{
          -webkit-box-orient:horizontal;
        }
      }
      .van-tabs__nav--card{
        height: 70px;
      }
    }
    /deep/ .van-tabs__content{
      .dt{
        border-collapse:collapse;
        width: 100%;
        thead{
          background-color: rgb(36, 104, 230);
          color: aliceblue;
        }
        tr>td{
          text-align: center;
          border: 1px solid #ccc;
          padding: 5px 0px;
          &.ant-tag-grey{
            background:#969799;
            color: aliceblue;
          }
          &.ant-tag-orange{
            background: #07c160;
            color: aliceblue;
          }
        }
        tbody{
          tr>td{
            color: #323233;
            .money_tip{
              font-size: 10px;
            }
          }
          tr>td:first-child{
            background:#2d6ce9;
            color: aliceblue;
          }
        }
      }
    }
  }
  .tip_tag{
    text-align: center;
    margin-top: 20px;
    margin-bottom: 70px;
    .book{
      color: rgb(26, 24, 24);
      border: 1px solid #ccc;
    }
  }
  .van-action-bar-icon{

    min-width: var(--van-action-bar-icon-width);
    height: var(--van-action-bar-icon-height);
    color: var(--van-action-bar-icon-text-color);
    font-size: var(--van-action-bar-icon-font-size);
    line-height: 1;
    text-align: center;
    background: var(--van-action-bar-icon-background-color);
    width: 100px;
    .icon_money{
      font-size: 10px;
      color: #323233;
    }
    .money{
      font-size: 16px;
      color: #323233;
    }
  }
</style>

