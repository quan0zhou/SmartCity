<script setup lang="ts">
import { ref,onBeforeMount } from 'vue'
import { getOrderList,cancelOrder,refundOrder, payOrder } from '@/utils/http/api/order/index'
import {useRoute} from 'vue-router'
import { getSpaceInfo,getOpenId } from '@/utils/http/api/custSpace/index'
import { Dialog,Notify  } from 'vant';
const route = useRoute()
// const router = useRouter()
const list = ref<any[]>([]);
const loading = ref(false);
const finished = ref(false);
const refreshing = ref(false);
let lastId: string = '0'
// const activeName = ref('');
type Props ={
    activeName:string
}
onBeforeMount(async ()=>{
  if(!import.meta.env.DEV){
    let openid= localStorage.getItem('openid')
    if((openid||'').trim().length<=0){
      if((route.query.code||'').length>0){
        var openIdResult=await getOpenId(route.query.code as string)
        if(openIdResult.status){
          localStorage.setItem('openid',openIdResult.data)
        }else{
          Notify({ type: 'danger', message: openIdResult.msg });
        }
      }else{
        var settings=(await getSpaceInfo()).data
        window.location.href=`https://open.weixin.qq.com/connect/oauth2/authorize?appid=${settings.appID}&redirect_uri=http%3A%2F%2Fwww.ruanjiezh.cn%2Forder&response_type=code&scope=snsapi_base#wechat_redirect`
      }

    }   
  }else{
    localStorage.setItem('openid','000')
  }
})
const props= defineProps<Props>()
const onLoad = async () => {
    if (refreshing.value) {
        list.value = [];
        lastId = '0'
        refreshing.value = false;
    }
    const openid=localStorage.getItem('openid')||''
    // 异步更新数据
    let data = (await getOrderList(lastId,openid,props.activeName)).data
    if (data && data.length > 0) {
        for (let i = 0; i < data.length; i++) {
            list.value?.push(data[i]);
            if (i == data.length - 1) {
                lastId = data[i]["orderId"]
            }
        }
    }
    loading.value = false;
    if (data.length <= 0) {
        finished.value = true;
    }
};
const cancel= (orderId:string,index:number)=>{
Dialog.confirm({
  title: '取消订单',
  message:
    '确认删除该待支付的订单?',
})
  .then(async () => {
    // on confirm
    let data= await cancelOrder(orderId)
    if(data.status){
       Notify({ type: 'success', message: data.msg })
      list.value.splice(index,1)
    }else{
       Notify({ type: 'danger', message: data.msg })
    }

  })
  .catch(() => {
    // on cancel
  });
}


const refund=(item:any)=>{
  console.log(item)
  Dialog.confirm({
  title: '确认退款',
  message:'如果距离预约开始时间大于12小时，则直接退款；否则需要后台审核退款',
})
.then(async () => {
    await onRefresh()
    // on confirm
    let data= await refundOrder(item.orderId)
    if(data.status){
       Notify({ type: 'success', message: data.msg })
       refreshing.value=true
       await onRefresh()
    }else{
       Notify({ type: 'danger', message: data.msg })
    }
  })
  .catch(() => {
    // on cancel
  });
}
const onBridgeReady = (request_data: any): void => {
      (window as any).WeixinJSBridge.invoke(
        "getBrandWCPayRequest",
        request_data.prepayData,
        (res: any) => {
          if (res.err_msg == "get_brand_wcpay_request:ok") {
            // router.push({
            //   path: "/payResult",
            //   query:{ id:request_data.orderId}
            // })
          }
        }
      )
}
const pay= async(item:any)=>{
    Object.assign(item,{payLoading:true})
    let result= await payOrder(item.orderId)
    Object.assign(item,{payLoading:false})
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
}
const onRefresh = async () => {
    // 清空列表数据
    finished.value = false;

    // 重新加载数据
    // 将 loading 设置为 true，表示处于加载状态
    loading.value = true;

    await onLoad();
};
const finishTime=(item:any)=>{
    Object.assign(item,{leftTime:0})
}
</script>
<template>
    <van-pull-refresh v-model="refreshing" @refresh="onRefresh" class="van_refresh">
        <van-list v-model:loading="loading" :finished="finished" finished-text="没有更多了" @load="onLoad">
            <div v-for="(item,index) in list" :key="item.orderId" class="order">
                <van-row class="order_head">
                    <van-col span="24"> <label class="title">订单编号：</label>{{item.orderNo}}</van-col>
                    <van-col span="24" class="order_head_status"><label class="title">订单状态：</label>{{item.orderStatusName}}</van-col>
                </van-row>
                <van-row>
                    <van-col span="12"><label class="title">场地:</label> {{item.spaceName}}</van-col>
                    <van-col span="12" class="row_right"><label class="title">预约日期:</label> {{item.reservationDate}}
                    </van-col>
                </van-row>
                <van-row>
                    <van-col span="12"><label class="title">预约时间段:</label> {{item.reservationTime}}</van-col>
                    <van-col span="12" class="row_right"><label class="title">预约金额:</label> <label>¥</label>
                        {{item.money}}
                    </van-col>
                </van-row>
                <van-row>
                    <van-col span="12"><label class="title">预约人:</label> {{item.reservationUserName}}</van-col>
                    <van-col span="12" class="row_right"><label class="title">预约手机:</label>
                        {{item.reservationUserPhone}}
                    </van-col>
                </van-row>
                <van-row v-if="item.orderStatus>0">
                    <van-col span="24"><label class="title">支付时间:</label> {{item.payTime}}</van-col>
                </van-row>
                <van-row v-if="item.orderStatus==2||item.orderStatus==4">
                    <van-col span="24"><label class="title">{{(item.orderStatus==2?'退款时间:':'拒绝时间:')}}</label>
                        {{item.refundTime}}</van-col>
                </van-row>
                <van-row v-if="(item.refundRemark||'').trim().length>0">
                    <van-col span="24"><label class="title">{{(item.orderStatus==2?'退款备注:':'拒绝备注:')}}</label>
                        {{item.refundRemark}}</van-col>
                </van-row>
                <van-row class="order_foot" v-if="item.orderStatus==0">
                    <van-col span="24">
                        <van-button color="#ef0a24" plain size="mini" @click="cancel(item.orderId,index)">取消订单</van-button>
                        <van-button type="danger" size="mini" :loading="item.payLoading||false" loading-text="支付中……" v-if="item.leftTime>0" @click="pay(item)">支付（剩余：<van-count-down :time="item.leftTime" @finish="finishTime(item)" format="mm:ss"/>）
                        </van-button>
                    </van-col>
                </van-row>
                <van-row class="order_foot" v-else-if="item.orderStatus==1&&item.isCanRefund">
                    <van-col span="24">
                        <van-button type="danger" size="mini" @click="refund(item)">确认退款</van-button>
                    </van-col>
                </van-row>
            </div>
        </van-list>
    </van-pull-refresh>
</template>
    
<style lang="less" scoped>
.van_refresh {
    .order {
        font-size: 13px;
        margin: 10px 5px 0px 5px;
        padding: 10px;
        border-radius: 10px;
        box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.521);
        background: #fff;

        .van-row {
            padding: 10px 0px;

            .title {
                color: rgb(37, 36, 36);
            }

            .row_right {
                text-align: right;
            }
        }

        &_head {
            border-bottom: 1px dashed rgb(209, 208, 208);

            &_status {
                text-align: left;
                margin-top: 10px;
                color: rgb(240, 49, 49);
            }
        }

        &_foot {
            text-align: right;

            .van-count-down {
                display: inline-block;
                color: #fff;
                font-size: 12px;
            }
        }
    }
}
</style>
    