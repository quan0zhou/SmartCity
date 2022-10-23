<script setup lang="ts" >
import { ref,onBeforeMount} from 'vue'
import {useRouter,useRoute} from 'vue-router'
import { getInfo } from '@/utils/http/api/reservation/index'
import { createOrder } from '@/utils/http/api/order/index'
import { Notify } from 'vant';
const route=useRoute()
const router=useRouter()
const model=ref({
  userName:'',
  userPhone:''
})
const reservation=ref({
  spaceName:'',
  reservationDate:'',
  reservationTime:'',
  money:''
})
const isError=ref(true)
const errorMsg=ref('')
const loading = ref(false)
let reservationId=''
onBeforeMount(async ()=>{
  if((route.query.id||'').length>0){
    reservationId=route.query.id as string
    const result = await getInfo(reservationId);
    if(result.data){
      reservation.value=result.data
      Object.assign(reservation.value,{money:"¥ "+result.data.money})
    }
    if(result.status){
      isError.value=false
    }else {
      errorMsg.value=result.msg
    }
  }
 
})
// 校验函数返回 true 表示校验通过，false 表示不通过
const validator = (val:string) => /^1[3456789]\d{9}$/.test(val);
const onClickLeft=()=>{
  router.push('/')
}
const onBridgeReady = (request_data: any): void => {
      (window as any).WeixinJSBridge.invoke(
        "getBrandWCPayRequest",
        request_data.prepayData,
        (res: any) => {
          if (res.err_msg == "get_brand_wcpay_request:ok") {
            //由点金计划控制跳转
            // router.push({
            //   path: "/payResult",
            //   query:{ id:request_data.orderId}
            // });
          }
          else if (res.err_msg == "get_brand_wcpay_request:cancel") { 
            router.push({
              path: "/order"
            });
          }
        }
      )
    }
const onSubmit =  async (values:any) => {
    let openid=localStorage.getItem("openid")
    let data= Object.assign({"openId":openid,"reservationId":reservationId},values)
    if((data.openId||'').length<=0){
      Notify({ type: 'danger', message: 'openId为空' });
      return
    }
    loading.value=true
    let result= await createOrder(data)
    loading.value=false
    if(result.status){
      if (typeof (window as any).WeixinJSBridge == "undefined") {
        if ((document as any).addEventListener) {
          (document as any).addEventListener(
            "WeixinJSBridgeReady",
            onBridgeReady(result.data),
            false
          );
        } else if ((document as any).attachEvent) {
          (document as any).attachEvent(
            "WeixinJSBridgeReady",
            onBridgeReady(result.data)
          );
          (document as any).attachEvent(
            "OnWeixinJSBridgeReady",
            onBridgeReady(result.data)
          );
        }
      } else {
        onBridgeReady(result.data);
      }
    }else{
      Notify({ type: 'danger', message: result.msg });
    }
};
</script>
<template>
  <van-nav-bar
  title="场地预约"
  left-text="返回"
  left-arrow
  @click-left="onClickLeft"
/>
<van-cell-group inset class="reservation_info" >
    <van-field
      v-model="reservation.spaceName"
      label="场地名称"
      :disabled="true"
      
    />
    <van-field
      v-model="reservation.reservationDate"
      label="预约日期"
      :disabled="true"
    />
    <van-field
      v-model="reservation.reservationTime"
      label="预约时间段"
      :disabled="true"
    />
    <van-field
      v-model="reservation.money"
      label="预约金额"
      :disabled="true"
    />
  </van-cell-group>
<van-form   @submit="onSubmit"  v-if="!isError">
  <van-cell-group inset>
    <!-- 通过 pattern 进行正则校验 -->
    <van-field
      v-model="model.userName"
      name="userName"
      label="姓名"
      placeholder="请输入用户名"
      :rules="[{ required: true, message: '请填写用户名' }]"
    />
    <!-- 通过 validator 进行函数校验 -->
    <van-field
      v-model="model.userPhone"
      name="userPhone"
      label="手机号"
      placeholder="请输入用户手机号"
      :rules="[{ validator, message: '请输入正确手机号' }]"
    />
  </van-cell-group>
  <div style="margin: 16px;" >
    <van-button round block type="primary" native-type="submit" :loading="loading">
      支付
    </van-button>
  </div>
</van-form>
<!-- 通用错误 -->
<van-empty image="error" :description="errorMsg" v-if="isError" />
</template>
<style lang="less" scoped>
.reservation_info{
  /deep/ .van-cell__value{
    input{
      text-align: right;
      color: rgb(33, 33, 34);
      -webkit-text-fill-color: rgb(33, 33, 34);
    }
  }
  /deep/ .van-field__label{
    color: #646566;
  }
}
</style>