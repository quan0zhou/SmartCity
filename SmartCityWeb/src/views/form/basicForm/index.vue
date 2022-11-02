<template>
  <page-header-wrapper :title="false" >
    <a-card :body-style="{padding: '24px 32px'}" :bordered="false">
      <a-row class="btn_operate">
        <a-button icon="lock" type="primary" ghost :loading="loading1" @click="setStatus(0)">设置为不可预订状态 </a-button>
        <a-divider type="vertical" />
        <a-button icon="redo" type="primary" ghost :loading="loading2" @click="setStatus(1)">恢复可预订状态 </a-button>
        <a-divider type="vertical" />
        <a-button icon="transaction" type="primary" ghost :loading="loading3" @click="setMoney">修改预订金额 </a-button>
        <a-divider type="vertical" />
        <a-button icon="sync" type="primary" ghost :loading="loading4" @click="refrsh">刷新所有 </a-button>
        <a-divider type="vertical" />
        <a-tag color="grey">不可预订</a-tag>
        <a-tag color="green">已预订</a-tag>
        <a-tag color="orange">当前选中</a-tag>
      </a-row>
      <a-tabs v-model="activeKey" type="card" @change="tabChange" >
        <a-tab-pane v-for="e in tabList" :key="e.date" >
          <template slot="tab">
            <div>{{ e.date }}</div>
            <div>{{ e.week }}</div>
            <div>
              <a-tag color="#87d068" v-if="e.status==1">可预订</a-tag>
              <a-tag color="#f50" v-else>已满</a-tag>
            </div>
          </template>
          <a-spin :spinning="spinning">
            <table>
              <div>
                <thead>
                  <tr>
                    <td><sub>时间</sub>\<sup>场地</sup></td>
                    <td v-for="(c,i) in e.spaceArray" :key="i">{{ c }}</td>
                  </tr>
                </thead>
              </div>
              <div class="ant-table-body-inner">
                <tbody>
                  <tr v-for="(t,i) in e.timeArray" :key="i">
                    <td>{{ (t.startTimeStr+'~'+t.endTimeStr) }}</td>
                    <td @click="checkItem(item)" v-for="item in e.items.filter(r=>r.startTime==t.startTime)" :key="item.reservationId" :class="[item.isChecked?'ant-tag-orange':(item.isBooked?'ant-tag-green':(item.reservationStatus==0?'ant-tag-grey':''))]">
                      <div v-if="item.isBooked">已预定</div>
                      <div v-else-if="item.reservationStatus==0">不可预订</div>
                      <div>¥ {{ item.money }}</div>

                    </td>
                  </tr>
                </tbody>
              </div>
            </table>
          </a-spin>
        </a-tab-pane>
      </a-tabs>
      <a-modal
        :title="title"
        :maskClosable="false"
        :width="500"
        :visible="visible"
        @cancel="visible=false"
      >
        <template>
          <a-form-model
            ref="ruleForm"
            :model="form"
            :rules="rules"
            :label-col="labelCol"
            :wrapper-col="wrapperCol"
          >
            <a-form-model-item label="预订金额" prop="money">
              <a-input-number v-model="form.money" :min="0.01" :max="9999" placeholder="请输入预订金额" />
            </a-form-model-item>
          </a-form-model>
        </template>
        <template slot="footer">
          <a-button type="white" @click="visible=false">取消</a-button>
          <a-button type="primary" @click="handleOk" :loading="confirmLoading">确定</a-button>
        </template>
      </a-modal>
    </a-card></page-header-wrapper>
</template>

<script>
export default {
    name: 'BasicForm',
  data () {
    return {
      labelCol: { span: 4 },
      wrapperCol: { span: 16 },
      form: { money: '', items: [] },
      rules: {
        money: [
           { required: true, message: '请输入预订金额', trigger: 'blur' }
        ]
      },
      title: '修改预订金额',
      visible: false,
      confirmLoading: false,
      spinning: false,
      loading1: false,
      loading2: false,
      loading3: false,
      loading4: false,
      activeKey: '',
      tabList: []
    }
  },
  methods: {
    // handler
    tabChange (key) {
      this.refreshTag()
    },
    checkItem (item) {
      if (item.isChecked) {
         this.$set(item, 'isChecked', false)
      } else {
       if (!item.isBooked) {
        if (!item.isChecked) {
           this.$set(item, 'isChecked', true)
         }
        }
      }
    },
    setMoney () {
      var tag = this.tabList.find(r => r.date === this.activeKey)
      var selectedItems = tag.items.filter(r => r.isChecked === true)
      if (selectedItems.length <= 0) {
        this.$message.warn('请选择预订记录', 3)
        return
      }
      this.form = this.$options.data().form
      this.form.items = selectedItems
      this.visible = true
    },
    handleOk () {
    this.$refs.ruleForm.validate(valid => {
        if (valid) {
           this.confirmLoading = true
          this.$http.patch('/reservation/setMoney', this.form).then(res => {
            this.confirmLoading = false
            if (res.status) {
              this.$message.success(res.msg, 3)
              this.visible = false
              this.refreshTag()
            } else {
              this.$message.error(res.msg, 3)
            }
          })
        } else {
          return false
        }
      })
    },
    setStatus (status) {
      var tag = this.tabList.find(r => r.date === this.activeKey)
      var selectedItems = tag.items.filter(r => r.isChecked === true)
      if (selectedItems.length <= 0) {
        this.$message.warn('请选择预订记录', 3)
        return
      }
     if (status === 0) {
        this.loading1 = true
        this.$http.patch('/reservation/setUnreservable', selectedItems).then(res => {
          this.loading1 = false
           if (res.status) {
             this.$message.success(res.msg, 3)
             this.refreshTag()
           } else {
             this.$message.error(res.msg, 3)
           }
        })
      } else {
         this.loading2 = true
         this.$http.patch('/reservation/setReservable', selectedItems).then(res => {
           this.loading2 = false
           if (res.status) {
             this.$message.success(res.msg, 3)
             this.refreshTag()
           } else {
             this.$message.error(res.msg, 3)
           }
        })
     }
    },
    refreshTag () {
      var tag = this.tabList.find(r => r.date === this.activeKey)
      tag.items.forEach(element => {
          this.$set(element, 'isChecked', false)
      })
       this.spinning = true
       this.$http.get(`/reservation/tag/${this.activeKey}`).then(res => {
        this.spinning = false
        Object.assign(tag, res || [])
      })
    },
    refrsh () {
      this.loading4 = true
      this.spinning = true
      this.$http.get('/reservation/taglist').then(res => {
       this.loading4 = false
       this.spinning = false
       this.tabList = res || []
      })
    }
  },
  created () {
      this.$http.get('/reservation/taglist').then(res => {
       this.tabList = res || []
       if (this.tabList.length > 0) {
        this.activeKey = this.tabList[0].date
        this.refreshTag()
       }
      })
  }
}
</script>
<style lang="less" scoped>
/* 设置滚动条的样式 */
::-webkit-scrollbar {
    width: 10px;
}
/* 滚动槽 */
::-webkit-scrollbar-track {
    box-shadow: inset006pxrgba(0, 0, 0, 0.3);
    border-radius: 10px;
}
/* 滚动条滑块 */
::-webkit-scrollbar-thumb {
    border-radius: 10px;
    background: rgba(0, 0, 0, 0.1);
    box-shadow: inset006pxrgba(0, 0, 0, 0.5);
}
::-webkit-scrollbar-thumb:window-inactive {
    background: rgba(68, 68, 68, 0.4);
}
.btn_operate{
  margin-bottom: 10px;
}
.ant-card{
  /deep/ .ant-card-body{
    padding: 20px !important;
  }
}
 .ant-input-number{
    width: 100%;
  }
.ant-tabs{
/deep/ .ant-tabs-nav-container{
    height: 66px !important;
}
/deep/ .ant-tabs-tab-active{
  color: #ffffff !important;
  background: #3994e1 !important;
}
/deep/ .ant-tabs-tab{
  height: 66px !important;
  text-align: center;
  line-height:20px !important;
  padding: 0px 10px !important;
  .ant-tag{
    margin-right: 0px;
  }
 }
 table{
  thead{
    tr>td{
     width:150px;
     height: 40px;
     text-align: center;
     font-size: 20px;
     font-weight: 600;

    }
  }
  .ant-table-body-inner{
    height: calc(100vh - 370px);
    overflow-y: scroll;
  tbody{
    height: calc(100% - 200px);
    tr>td:first-child{
       font-size: 20x;
       font-weight: 600;
       border:none
    }
    .ant-tag-grey {
        color: #ffffff;
        background: #808080;
        border-color: #7a7a7a;
    }
    tr>td{
      width: 150px;
      height:50px;
      text-align: center;
      border: 1px solid #CCC;
      cursor: pointer;

    }
  }
  }

 }
}
</style>
