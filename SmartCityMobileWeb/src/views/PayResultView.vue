<script setup lang="ts">
import { ref, onBeforeMount} from 'vue'
import {useRoute} from 'vue-router'
import paySuccess from '@/assets/paySuccess.png'
import payError from '@/assets/payError.png'
import { getOrderInfo } from '@/utils/http/api/order/index'
const route=useRoute()
let isError=ref(true)
let  money= ref('0.00')
let icon=ref(payError)
let payMsg=ref('')
onBeforeMount(async ()=>{
  if((route.query.id||'').length>0){
    const result = await getOrderInfo(route.query.id as string);
    if(result.data){     
        money.value=result.data.money
    }
    if(result.status){
        isError.value=false
        icon.value=paySuccess
        payMsg.value='支付成功'
    }
    else{
        isError.value=true
        payMsg.value='支付失败'
    }
  }
 
})
</script>
<template>
    <div class="weui-msg">
        <div class="weui-msg__icon-area">
            <van-image  class="img" :src="icon" />
        </div>
        <div class="weui-msg__text-area">
            <p class="text_error" v-if="isError">{{payMsg}}</p>
             <p class="text_success" v-else>{{payMsg}}</p>
        </div>
        <div class="weui-msg__tips-area">
          <p class="tip"><span class="money_tip">¥</span> {{money}}</p>
        </div>
        <div class="weui-msg__opr-area">
      <van-row>
        <van-col span="24" class="btn_row"><van-button type="primary" size="small" style="width:50%;margin-top: 40px;" to="/" replace>返回预约</van-button></van-col>
        <van-col span="24" class="btn_row"><van-button type="success" style="width:50%" size="small"  to="order" replace>我的订单</van-button></van-col>
       </van-row>
            
        </div>
    </div>
</template>
<style lang="less" scoped  >
.weui-msg {
    padding-top: 48px;

    .weui-msg__icon-area {
        text-align: center;
        .img{
            width: 40px;
            height: 40px;
        }
    }
    .weui-msg__text-area{
        .text_success{
            text-align: center;
            font-size: 16px;
            font-weight: 600;
            color: #27935a;
        }
        .text_error{
            text-align: center;
            font-size: 16px;
            font-weight: 600;
            color: #f14747;
        }
    }
    .weui-msg__tips-area{
        text-align: center;
        margin-top: 50px;
        .tip{
            font-size: 32px;
            .money_tip{
                font-size: 16px;
            }
        }
    }
    .weui-msg__opr-area{
    .btn_row{
        margin-top: 10px;
        text-align: center;
    }
    }
}
</style>