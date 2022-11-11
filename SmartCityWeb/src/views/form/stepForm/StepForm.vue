<template>
  <page-header-wrapper :title="false">
    <a-card :bordered="false">
      <div class="table-page-search-wrapper">
        <a-form layout="inline">
          <a-row :gutter="48">
            <a-col :md="6" :sm="24">
              <a-form-item label="场地类型">
                <a-select v-model="queryParam.spaceType" placeholder="请选择场地类型" :allowClear="true" @change="changeType">
                  <a-select-option :value="1">网球场</a-select-option>
                  <a-select-option :value="2">篮球场</a-select-option>
                  <a-select-option :value="3">羽毛球场</a-select-option>
                  <a-select-option :value="4">排球场</a-select-option>
                  <a-select-option :value="5">乒乓球场</a-select-option>
                </a-select>
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <a-form-item label="场地">
                <a-select v-model="queryParam.spaceId" placeholder="请选择场地" :allowClear="true" >
                  <a-select-option v-for="(e,i) in spaceList" :key="i" :value="e.spaceId">{{ e.spaceName }}</a-select-option>
                </a-select>
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <a-form-item label="预订人姓名">
                <a-input v-model="queryParam.userName" placeholder="请填写预订人姓名"/>
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <a-form-item label="预订人手机">
                <a-input v-model="queryParam.userPhone" placeholder="请填写预订人手机"/>
              </a-form-item>
            </a-col>
          </a-row>
          <a-row :gutter="48">
            <a-col :md="6" :sm="24">
              <a-form-item label="订单状态">
                <a-select v-model="queryParam.status" placeholder="请选择订单状态" :allowClear="true" >
                  <a-select-option :value="3">退款待确认</a-select-option>
                  <a-select-option :value="0">预订待支付</a-select-option>
                  <a-select-option :value="1">已预订</a-select-option>
                  <a-select-option :value="2">已退款</a-select-option>
                  <a-select-option :value="4">拒绝退款</a-select-option>
                </a-select>
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <a-form-item label="预订日期">
                <a-range-picker v-model="queryParam.date"/>
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <a-form-item label="预订时间段">
                <a-select v-model="queryParam.startTime" placeholder="开始" :allowClear="true" class="time_start" @change="changeStartTime">
                  <a-select-option v-for="(e,i) in startTimeList" :key="i" :value="e.value">{{ e.label }}</a-select-option>
                </a-select>
                至
                <a-select v-model="queryParam.endTime" placeholder="结束" :allowClear="true" class="time_end" @change="changeEndTime">
                  <a-select-option v-for="(e,i) in endTimeList" :key="i" :value="e.value">{{ e.label }}</a-select-option>
                </a-select>
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <span class="table-page-search-submitButtons" >
                <a-button type="primary" @click="$refs.table.refresh(true)">查询</a-button>
                <a-button style="margin-left: 8px" @click="resetQuery">重置</a-button>
              </span>
            </a-col>
          </a-row>
        </a-form>
      </div>
      <div class="table-operator">
        <a-button type="primary" icon="download" @click="handleExport" :loading="exportLoading">导出</a-button>
      </div>
      <s-table
        ref="table"
        size="small"
        rowKey="orderId"
        :columns="columns"
        :data="loadData"
        :pageSize="20"
        showPagination="auto"
        :scroll="{ x: 1600 }"
      >

        <span slot="spaceType" slot-scope="text">
          {{ text|statusFilter }}
        </span>
        <span slot="moneyType" slot-scope="text">
          {{ ('￥'+text) }}
        </span>
        <span slot="statusType" slot-scope="text">
          <template>
            <a-tag color="red" v-if="text==3">退款待确认</a-tag>
            <a-tag color="green" v-else-if="text==0">预订待支付</a-tag>
            <a-tag color="blue" v-else-if="text==1">已预订</a-tag>
            <a-tag color="grey" v-else-if="text==2">已退款</a-tag>
            <a-tag color="purple" v-else-if="text==4">拒绝退款</a-tag>
          </template>
        </span>
        <span slot="action" slot-scope="text, record">
          <template>
            <a-button size="small" @click="handleInfo(record)">查看</a-button>
            <a-divider type="vertical" v-if="record.orderStatus==3" />
            <a-button v-if="record.orderStatus==3" @click="handleRefund(record)" type="danger" size="small">退款</a-button>
          </template>
        </span>
      </s-table>

    </a-card>
    <a-modal
      :title="detailTitle"
      :visible="detailVisible"
      :maskClosable="false"
      width="750px"
      class="detailOrderModal"

      @cancel="handleDetailCancel">
      <template>
        <a-form-model
          :model="currentRow"
          layout="inline"
          :label-col="labelCol"
          :wrapper-col="wrapperCol"
        >
          <a-row>
            <a-col :span="12">
              <a-form-model-item label="最后操作时间" prop="updateTime">
                <a-input v-model="currentRow.updateTime"/>
              </a-form-model-item>
            </a-col>
            <a-col :span="12">
              <a-form-model-item label="订单创建时间" prop="createTime">
                <a-input v-model="currentRow.createTime"/>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="12">
              <a-form-model-item label="场地类型" prop="spaceTypeText">
                <a-input v-model="currentRow.spaceTypeText"/>
              </a-form-model-item>
            </a-col>
            <a-col :span="12">
              <a-form-model-item label="场地名称" prop="spaceName">
                <a-input v-model="currentRow.spaceName"/>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="12">
              <a-form-model-item label="预订日期" prop="reservationDate">
                <a-input v-model="currentRow.reservationDate"/>
              </a-form-model-item>
            </a-col>
            <a-col :span="12">
              <a-form-model-item label="预订时间段" prop="reservationTime">
                <a-input v-model="currentRow.reservationTime"/>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="12">
              <a-form-model-item label="预订人姓名" prop="reservationUserName">
                <a-input v-model="currentRow.reservationUserName"/>
              </a-form-model-item>
            </a-col>
            <a-col :span="12">
              <a-form-model-item label="预订人手机" prop="reservationUserPhone">
                <a-input v-model="currentRow.reservationUserPhone"/>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="12">
              <a-form-model-item label="微信OpenId" prop="openId">
                <a-input v-model="currentRow.openId"/>
              </a-form-model-item>
            </a-col>
            <a-col :span="12">
              <a-form-model-item label="预订金额" prop="money">
                <a-input v-model="currentRow.money"/>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="12">
              <a-form-model-item label="订单商户号" prop="orderNo">
                <a-input v-model="currentRow.orderNo"/>
              </a-form-model-item>
            </a-col>
            <a-col :span="12">
              <a-form-model-item label="订单交易号" prop="paymentNo">
                <a-input v-model="currentRow.paymentNo"/>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="12">
              <a-form-model-item label="订单支付时间" prop="payTime">
                <a-input v-model="currentRow.payTime"/>
              </a-form-model-item>
            </a-col>
            <a-col :span="12">
              <a-form-model-item label="订单状态" prop="orderStatus">
                <a-tag color="red" v-if="currentRow.orderStatus==3">退款待确认</a-tag>
                <a-tag color="green" v-else-if="currentRow.orderStatus==0">预订待支付</a-tag>
                <a-tag color="blue" v-else-if="currentRow.orderStatus==1">已预订</a-tag>
                <a-tag color="grey" v-else-if="currentRow.orderStatus==2">已退款</a-tag>
                <a-tag color="purple" v-else-if="currentRow.orderStatus==4">拒绝退款</a-tag>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="12">
              <a-form-model-item label="退款(或拒绝)时间" prop="refundTime">
                <a-input v-model="currentRow.refundTime"/>
              </a-form-model-item>
            </a-col>
            <a-col :span="12">
              <a-form-model-item label="退款(或拒绝)操作人" prop="refundOptUser">
                <a-input v-model="currentRow.refundOptUser"/>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="24">
              <a-form-model-item label="退款备注" prop="refundRemark" class="refundRemark">
                <a-input type="textarea" v-model="currentRow.refundRemark"/>
              </a-form-model-item>
            </a-col>
          </a-row>
        </a-form-model>
      </template>
      <template slot="footer">
        <a-button type="white" @click="detailVisible=false">取消</a-button>
      </template>
    </a-modal>
    <a-modal
      :title="title"
      :maskClosable="false"
      :width="660"
      :visible="visible"
      @cancel="visible=false"
    >
      <template>
        <a-form-model
          ref="ruleForm"
          :model="form"
          :label-col="labelCol1"
          :wrapper-col="wrapperCol1"
        >
          <a-row>
            <a-col :span="24">
              <a-form-model-item label="预订人姓名" >
                <a-tag color="#2db7f5">
                  {{ currentRow.reservationUserName }}
                </a-tag>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="24">
              <a-form-model-item label="预订人电话" >
                <a-tag color="#87d068">
                  {{ currentRow.reservationUserPhone }}
                </a-tag>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="24">
              <a-form-model-item label="订单号" >
                <a-tag color="pink">
                  商户号： {{ currentRow.orderNo }}
                </a-tag>
                <a-tag color="pink">
                  交易号： {{ currentRow.paymentNo }}
                </a-tag>

              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="24">
              <a-form-model-item label="退款备注" prop="remark">
                <a-input v-model="form.remark" type="textarea" placeholder="请输入退款备注或拒绝退款原因【拒绝退款时必填】" style="height:80px;" />
              </a-form-model-item>
            </a-col>
          </a-row>
        </a-form-model>
      </template>
      <template slot="footer">
        <a-button type="white" @click="visible=false">取消</a-button>
        <a-button type="danger" @click="handleError" :loading="confirmLoading1" >拒绝退款</a-button>
        <a-button type="primary" @click="handleOk" :loading="confirmLoading2" >确认退款</a-button>
      </template>
    </a-modal>
  </page-header-wrapper>
</template>

<script>
import { STable } from '@/components'
const columns = [
  {
    title: '最后操作时间',
    width: '160px',
     align: 'center',
    dataIndex: 'updateTime',
    key: 'updateTime'
  },
  {
    title: '状态',
    align: 'center',
    dataIndex: 'orderStatus',
    scopedSlots: { customRender: 'statusType' },
    key: 'orderStatus'
  },
  {
    title: '场地类型',
    dataIndex: 'spaceType',
    align: 'center',
    scopedSlots: { customRender: 'spaceType' },
    key: 'spaceType'
  },
  {
    title: '场地',
     align: 'center',
    dataIndex: 'spaceName',
    key: 'spaceName'
  },
  {
    title: '预订日期',
     align: 'center',
    dataIndex: 'reservationDate',
    key: 'reservationDate'
  },
  {
    title: '时间段',
    align: 'center',
    dataIndex: 'reservationTime',
    key: 'reservationTime'
  },
    {
    title: '预订金额',
    align: 'center',
    dataIndex: 'money',
    scopedSlots: { customRender: 'moneyType' },
    key: 'money'
  },
  {
    title: '预订人姓名',
    align: 'center',
    dataIndex: 'reservationUserName',
    key: 'reservationUserName'
  },
   {
    title: '预订人手机号',
    align: 'center',
    dataIndex: 'reservationUserPhone',
    key: 'reservationUserPhone'
  },
  // {
  //   title: '微信openId',
  //    align: 'center',
  //   dataIndex: 'openId',
  //   key: 'openId'
  // },
    {
    title: '支付时间',
    width: '160px',
     align: 'center',
    dataIndex: 'payTime',
    key: 'payTime'
  },
  // {
  //   title: '商户订单号',
  //   dataIndex: 'orderNo',
  //    align: 'center',
  //   key: 'orderNo'
  // },
  // {
  //   title: '交易订单号',
  //   dataIndex: 'paymentNo',
  //    align: 'center',
  //   key: 'paymentNo'
  // },
  {
    title: '退款(或拒绝)时间',
    align: 'center',
    width: '160px',
    dataIndex: 'refundTime',
    key: 'refundTime'
  },
  {
    title: '退款(或拒绝)操作人',
    align: 'center',
    width: '160px',
    dataIndex: 'refundOptUser',
    key: 'refundOptUser'
  },
  {
    title: '操作',
    dataIndex: 'action',
    width: '150px',
    align: 'center',
    fixed: 'right',
    scopedSlots: { customRender: 'action' }
  }
]
const spaceTypeMap = {
  1: {
    text: '网球场'
  },
 2: {
    text: '篮球场'
  },
  3: {
    text: '羽毛球场'
  },
  4: {
    text: '排球场'
  },
  5: {
    text: '乒乓球场'
  }
}
export default {
  components: {
    STable
  },
  name: 'StepForm',
  data () {
    this.columns = columns
    return {
      pagination: {
           defaultPageSize: 20
      },
      form: {},
      visible: false,
      title: '',
      labelCol: {},
      wrapperCol: {},
      labelCol1: { span: 4 },
      wrapperCol1: { span: 16 },
      confirmLoading1: false,
      confirmLoading2: false,
      exportLoading: false,
      detailTitle: '',
      detailVisible: false,
      queryParam: {
        spaceType: 1,
        status: 3,
        spaceId: undefined,
        startTime: undefined,
        endTime: undefined
      },
      // 加载数据方法 必须为 Promise 对象
      loadData: parameter => {
        const requestParameters = Object.assign({}, parameter, this.queryParam)
        if (requestParameters.date && requestParameters.date.length >= 2) {
           requestParameters.startDate = requestParameters.date[0].format('YYYY-MM-DD')
           requestParameters.endDate = requestParameters.date[1].format('YYYY-MM-DD')
        }
        return this.$http.post('/order/list', requestParameters)
          .then(res => {
            return res
          })
      },
      currentRow: {},
      timeList: [],
      startTimeList: [],
      endTimeList: [],
      spaceList: []
    }
  },
  filters: {
    statusFilter (type) {
      return spaceTypeMap[type].text
    }
  },
  methods: {
    handleExport () {
       const requestParameters = Object.assign({}, this.queryParam)
       if (requestParameters.date && requestParameters.date.length >= 2) {
           requestParameters.startDate = requestParameters.date[0].format('YYYY-MM-DD')
           requestParameters.endDate = requestParameters.date[1].format('YYYY-MM-DD')
        }
        this.exportLoading = true
        this.$http({ url: '/order/downLoad', data: requestParameters, method: 'post', responseType: 'blob' }).then(res => {
           this.exportLoading = false
            this.downloadFile('订单记录.xlsx', res)
          })
    },
    downloadFile (fileName, data) {
      if (!data) { return }
      var url = window.URL.createObjectURL(new Blob([data]))
      var link = document.createElement('a')
      link.style.display = 'none'
      link.href = url
      link.setAttribute('download', fileName)
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
	},
    changeType (val) {
      this.queryParam.spaceId = undefined
      this.spaceList = []
      if (val) {
         this.$http.get('/custSpace/list/' + val).then(res => {
               this.spaceList = res || []
         })
      }
    },
    changeStartTime (val) {
      if (val) {
          this.endTimeList = this.timeList.filter(r => r.value >= val)
      } else {
          this.endTimeList = this.timeList
      }
    },
    changeEndTime (val) {
      if (val) {
          this.startTimeList = this.timeList.filter(r => r.value <= val)
      } else {
          this.startTimeList = this.timeList
      }
    },
    resetQuery () {
      this.queryParam = {}
      this.endTimeList = this.timeList
      this.startTimeList = this.timeList
    },
    handleInfo (row) {
      this.detailTitle = '订单详情：' + row.spaceName + '【' + row.reservationDate + ' ' + row.reservationTime + '】'
      Object.assign(this.currentRow, row)
      this.currentRow.spaceTypeText = spaceTypeMap[this.currentRow.spaceType].text
      this.detailVisible = true
    },
    handleRefund (row) {
      Object.assign(this.currentRow, row)
      this.form = { orderId: row.orderId, remark: '' }
      this.title = '退款：' + row.spaceName + '【' + row.reservationDate + ' ' + row.reservationTime + '】'
      this.visible = true
    },
    handleDetailCancel () {
       this.detailVisible = false
    },
    handleOk () {
      this.confirmLoading2 = true
      this.$http.patch(`/order/refund`, this.form).then(res => {
          this.confirmLoading2 = false
         if (res.status) {
            this.$message.success(res.msg, 3)
            this.visible = false
            this.$refs.table.refresh(true)
         } else {
            this.$message.error(res.msg, 3)
         }
       })
    },
    handleError () {
        if ((this.form.remark || '').trim().length <= 0) {
         this.$message.warn('拒绝退款，请填写退款备注', 3)
         return
        }
        this.confirmLoading1 = true
        this.$http.patch(`/order/refuseRefund`, this.form).then(res => {
        this.confirmLoading1 = false
         if (res.status) {
            this.$message.success(res.msg, 3)
            this.visible = false
            this.$refs.table.refresh(true)
         } else {
            this.$message.error(res.msg, 3)
         }
       })
    }
  },
  created () {
    for (let i = 7; i <= 21; i++) {
      if (i < 10) {
        this.timeList.push({ label: '0' + i, value: i })
      } else {
        this.timeList.push({ label: i.toString(), value: i })
      }
    }
    this.startTimeList = this.timeList
    this.endTimeList = this.timeList
    this.changeType(this.queryParam.spaceType)
  }
}
</script>

<style lang="less" scoped>
.table-page-search-wrapper{
  /deep/ .ant-form-item-label{
      width: 80px !important;
  }
  .time_start,.time_end{
    width: calc((100% - 23px) / 2);
  }
}
// .table-wrapper{
//  /deep/ th{
//     padding: 5px;
//   }
//   /deep/ td{
//     padding: 5px;
//   }
// }
.detailOrderModal{
  /deep/ .ant-form-item-label{
    width: 135px;
  }
  .refundRemark{
    margin-top: 5px;
    /deep/ .ant-form-item-control{
      width: 534px;
    }
  }
}
</style>
