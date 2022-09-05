<template>
  <page-header-wrapper :breadcrumb="true" :title="false">
    <a-card :bordered="false">
      <a-form-model
        ref="ruleForm"
        :model="form"
        :rules="rules"

        :label-col="labelCol"
        :wrapper-col="wrapperCol"
      >
        <a-form-model-item ref="reservationTitle" label="预约标题" prop="reservationTitle">
          <a-input v-model="form.reservationTitle"/>
        </a-form-model-item>
        <a-form-model-item ref="startTime" label="开始时间" prop="startTime">
          <a-time-picker v-model="form.startTime" :default-value="moment('05:00:00', 'HH:mm:ss')" />
        </a-form-model-item>
        <a-form-model-item ref="endTime" label="结束时间" prop="endTime">
          <a-time-picker v-model="form.endTime" :default-value="moment('23:00:00', 'HH:mm:ss')"/>
        </a-form-model-item>
        <a-form-model-item ref="timePeriod" label="时间段（小时）" prop="timePeriod">
          <a-input-number v-model="form.timePeriod" :min="1" :max="24" />
        </a-form-model-item>
        <a-form-model-item ref="settableDays" label="可设置天数" prop="settableDays">
          <a-input-number v-model="form.settableDays" :min="1" />
        </a-form-model-item>
        <a-form-model-item ref="bookableDays" label="可预订天数" prop="bookableDays">
          <a-input-number v-model="form.bookableDays" :min="1" />
        </a-form-model-item>
        <a-form-model-item ref="directRefundPeriod" label="预订时间之外直接退款(小时)" prop="directRefundPeriod">
          <a-input-number v-model="form.directRefundPeriod" :min="1" />
        </a-form-model-item>
        <a-form-model-item ref="appID" label="公众号APPId" prop="appID">
          <a-input v-model="form.appID"/>
        </a-form-model-item>
        <a-form-model-item ref="mchID" label="商户号" prop="mchID">
          <a-input v-model="form.mchID"/>
        </a-form-model-item>
        <a-form-model-item ref="subMchID" label="子商户号" prop="subMchID">
          <a-input v-model="form.subMchID"/>
        </a-form-model-item>
        <a-form-model-item ref="appKey" label="AppKey" prop="appKey">
          <a-input v-model="form.appKey"/>
        </a-form-model-item>
        <a-form-model-item ref="appSecret" label="AppSecret" prop="appSecret">
          <a-input v-model="form.appSecret"/>
        </a-form-model-item>
        <a-form-model-item :wrapper-col="{ span: 14, offset: 4 }">
          <a-button type="primary" @click="onSubmit">
            保存
          </a-button>
          <a-button style="margin-left: 10px;" @click="resetForm">
            重置
          </a-button>
        </a-form-model-item>
      </a-form-model>
    </a-card>

  </page-header-wrapper>
</template>

<script>
// 演示如何使用 this.$dialog 封装 modal 组件
import TaskForm from './modules/TaskForm'
import Info from './components/Info'
import moment from 'moment'

export default {
  name: 'StandardList',
  components: {
    TaskForm,
    Info
  },
  data () {
    return {
      labelCol: { span: 4 },
      wrapperCol: { span: 10 },
      other: '',
      form: {
        reservationTitle: '',
        startTime: '',
        endTime: '',
        timePeriod: '',
        settableDays: '',
        bookableDays: '',
        directRefundPeriod: '',
        appID: '',
        mchID: '',
        subMchID: '',
        appKey: '',
        appSecret: ''
      },
      rules: {
        reservationTitle: [
          { required: true, message: '请输入预约标题', trigger: 'blur' }
        ],
        startTime: [{ required: true, message: '请选择开始时间', trigger: 'change' }],
        endTime: [{ required: true, message: '请选择结束时间', trigger: 'change' }],
         timePeriod: [{ required: true, message: '请选择开始时间', trigger: 'blur' }],
        resource: [
          { required: true, message: 'Please select activity resource', trigger: 'change' }
        ],
        desc: [{ required: true, message: 'Please input activity form', trigger: 'blur' }]
      }
    }
  },
  methods: {
    moment,
    add () {
      this.$dialog(TaskForm,
        // component props
        {
          record: {},
          on: {
            ok () {
              console.log('ok 回调')
            },
            cancel () {
              console.log('cancel 回调')
            },
            close () {
              console.log('modal close 回调')
            }
          }
        },
        // modal props
        {
          title: '新增',
          width: 700,
          centered: true,
          maskClosable: false
        })
    },
    edit (record) {
      console.log('record', record)
      this.$dialog(TaskForm,
        // component props
        {
          record,
          on: {
            ok () {
              console.log('ok 回调')
            },
            cancel () {
              console.log('cancel 回调')
            },
            close () {
              console.log('modal close 回调')
            }
          }
        },
        // modal props
        {
          title: '操作',
          width: 700,
          centered: true,
          maskClosable: false
        })
    }
  }
}
</script>

<style lang="less" scoped>

.ant-avatar-lg {
    width: 48px;
    height: 48px;
    line-height: 48px;
}

.list-content-item {
    color: rgba(0, 0, 0, .45);
    display: inline-block;
    vertical-align: middle;
    font-size: 14px;
    margin-left: 40px;
    span {
        line-height: 20px;
    }
    p {
        margin-top: 4px;
        margin-bottom: 0;
        line-height: 22px;
    }
}
</style>
