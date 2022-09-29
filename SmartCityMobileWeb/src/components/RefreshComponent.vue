<script setup lang="ts">
import { ref } from 'vue'
import { getOrderList } from '@/utils/http/api/order/index'

const list = ref<any[]>([]);
const loading = ref(false);
const finished = ref(false);
const refreshing = ref(false);
let lastId: string = '0'
// const activeName = ref('');
type Props ={
    activeName:string
}
const props= defineProps<Props>()
const onLoad = async () => {
    if (refreshing.value) {
        list.value = [];
        lastId = '0'
        refreshing.value = false;
    }
    // 异步更新数据
    // setTimeout 仅做示例，真实场景中一般为 ajax 请求
    let data = (await getOrderList(lastId, props.activeName)).data
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
    <van-pull-refresh v-model="refreshing" @refresh="onRefresh" class="van_refresh">
        <van-list v-model:loading="loading" :finished="finished" finished-text="没有更多了" @load="onLoad">
            <div v-for="item in list" :key="item.orderId" class="order">
                <van-row class="order_head">
                    <van-col span="18"> <label class="title">订单编号:</label> {{item.orderNo}}</van-col>
                    <van-col span="6" class="order_head_status">{{item.orderStatusName}}</van-col>
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
                        <van-button color="#ef0a24" plain size="mini">取消订单</van-button>
                        <van-button type="danger" size="mini" v-if="item.leftTime>0">支付（剩余：
                            <van-count-down :time="item.leftTime" />）
                        </van-button>
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
        box-shadow: 0 1px 3px 0 rgb(0 0 0 / 19%);
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
            border-bottom: 1px solid #ccc;

            &_status {
                text-align: right;
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
    