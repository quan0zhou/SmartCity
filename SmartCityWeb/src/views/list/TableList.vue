<template>
  <page-header-wrapper :breadcrumb="true" :title="false">
    <a-card :bordered="false">
      <div class="table-page-search-wrapper">
        <a-form layout="inline">
          <a-row :gutter="48">
            <a-col :md="6" :sm="24">
              <a-form-item label="场地名称">
                <a-input v-model="queryParam.spaceName" placeholder="请填写场地名称"/>
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <a-form-item label="场地联系人">
                <a-input v-model="queryParam.contactName" placeholder="请填写场地联系人"/>
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <a-form-item label="场地类型">
                <a-select v-model="queryParam.spaceType" placeholder="请选择场地类型" >
                  <a-select-option value="0">全部</a-select-option>
                  <a-select-option value="1">网球场</a-select-option>
                  <a-select-option value="2">篮球场</a-select-option>
                  <a-select-option value="3">羽毛球场</a-select-option>
                  <a-select-option value="4">排球场</a-select-option>
                  <a-select-option value="5">乒乓球场</a-select-option>
                </a-select>
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <span class="table-page-search-submitButtons" >
                <a-button type="primary" @click="$refs.table.refresh(true)">查询</a-button>
                <a-button style="margin-left: 8px" @click="() => this.queryParam = {}">重置</a-button>
              </span>
            </a-col>
          </a-row>
        </a-form>
      </div>

      <div class="table-operator">
        <a-button type="primary" icon="plus" @click="handleAdd">新增</a-button>
      </div>

      <s-table
        ref="table"
        size="default"
        rowKey="key"
        :columns="columns"
        :data="loadData"
        showPagination="auto"
      >

        <span slot="spaceType" slot-scope="text">
          {{ text|statusFilter }}
        </span>

        <span slot="action" slot-scope="text, record">
          <template>
            <a @click="handleEdit(record)">配置</a>
            <a-divider type="vertical" />
            <a @click="handleSub(record)">订阅报警</a>
          </template>
        </span>
      </s-table>

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
            :rules="rules"
            layout="inline"
            :label-col="labelCol"
            :wrapper-col="wrapperCol"
          >
            <a-row>
              <a-col :span="12">
                <a-form-model-item ref="spaceName" label="场地名称" prop="spaceName">
                  <a-input
                    v-model="form.spaceName"
                    placeholder="请输入场地名称"
                  />
                </a-form-model-item>
              </a-col>
              <a-col :span="12">
                <a-form-model-item label="场地类型" prop="spaceType" >
                  <a-select v-model="form.spaceType" placeholder="请选择场地类型" style="width:183px;" >
                    <a-select-option value="1">网球场</a-select-option>
                    <a-select-option value="2">篮球场</a-select-option>
                    <a-select-option value="3">羽毛球场</a-select-option>
                    <a-select-option value="4">排球场</a-select-option>
                    <a-select-option value="5">乒乓球场</a-select-option>
                  </a-select>
                </a-form-model-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="12">
                <a-form-model-item ref="contactName" label="场地联系人" prop="contactName">
                  <a-input
                    v-model="form.contactName"
                    placeholder="请输入场地联系人"
                  />
                </a-form-model-item>
              </a-col>
              <a-col :span="12">
                <a-form-model-item label="联系人电话" prop="contactPhone">
                  <a-input
                    v-model="form.contactPhone"
                    placeholder="请输入联系人电话"/>
                </a-form-model-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="24">
                <a-form-model-item label="场地地址" prop="spaceAddress">
                  <a-input
                    style="width:489px"
                    v-model="form.spaceAddress"
                    placeholder="请输入场地地址"
                  />
                </a-form-model-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="24">
                <a-form-model-item label="备注" prop="remark">
                  <a-input v-model="form.remark" type="textarea" placeholder="请输入备注" />
                </a-form-model-item>
              </a-col>
            </a-row>
          </a-form-model>
        </template>
        <template slot="footer">
          <a-button type="white" @click="visible=false">取消</a-button>
          <a-button type="primary" @click="handleOk" :loading="confirmLoading" v-if="isSave">确定</a-button>
        </template>
      </a-modal>
    </a-card>
  </page-header-wrapper>
</template>

<script>
import { STable } from '@/components'

const columns = [
  {
    title: '最后操作时间',
    dataIndex: 'updateTime',
    key: 'updateTime'
  },
  {
    title: '场地名称',
    dataIndex: 'spaceName',
    key: 'spaceName'
  },
  {
    title: '场地类型',
    dataIndex: 'spaceType',
    scopedSlots: { customRender: 'spaceType' },
    key: 'spaceType'
  },
  {
    title: '场地联系人',
    dataIndex: 'contactName',
    key: 'contactName'
  },
  {
    title: '场地联系人',
    dataIndex: 'contactPhone',
    key: 'contactPhone'
  },
  {
    title: '场地地址',
    dataIndex: 'spaceAddress',
    key: 'spaceAddress'
  },
  {
    title: '备注',
    dataIndex: 'remark',
    key: 'remark'
  },
  {
    title: '操作',
    dataIndex: 'action',
    width: '150px',
    scopedSlots: { customRender: 'action' }
  }
]

const spaceTypeMap = {
  1: {
    spaceType: '1',
    text: '网球场'
  },
 2: {
    spaceType: '2',
    text: '篮球场'
  },
  3: {
    spaceType: '3',
    text: '羽毛球场'
  },
  4: {
    spaceType: '4',
    text: '排球场'
  },
  5: {
    spaceType: '5',
    text: '乒乓球场'
  }
}

export default {
  components: {
    STable
  },
  data () {
    this.columns = columns
    return {
      labelCol: {},
      wrapperCol: {},
      form: {
       spaceName: '',
       spaceType: undefined,
       contactName: '',
       contactPhone: '',
       spaceAddress: '',
       remark: ''
      },
      rules: {
        spaceName: [
           { required: true, message: '请输入场地名称', trigger: 'blur' },
           { max: 50, message: '最大字符50个', trigger: 'blur' }
        ],
        spaceType: [
           { required: true, message: '请输入场地类型', trigger: 'change' }
          ]
      },
      title: '',
      isSave: false,
      visible: false,
      confirmLoading: false,
      // 查询参数
      queryParam: {},
      // 加载数据方法 必须为 Promise 对象
      loadData: parameter => {
        const requestParameters = Object.assign({}, parameter, this.queryParam)
        console.log('loadData request parameters:', requestParameters)
        return this.$http.post('/custSpace/list', requestParameters)
          .then(res => {
            return res
          })
      }
    }
  },
  filters: {
    statusFilter (type) {
      return spaceTypeMap[type].text
    },
    statusTypeFilter (type) {
      return spaceTypeMap[type].spaceType
    }
  },
  created () {

  },
  computed: {

  },
  methods: {
    handleAdd () {
      if (this.$refs.ruleForm) {
        this.$refs.ruleForm.resetFields()
      }
      this.form = this.$options.data().form
      this.isSave = true
      this.title = '新增场地'
      this.visible = true
    },
    handleEdit (record) {
      this.visible = true
    },
    handleOk () {
    this.$refs.ruleForm.validate(valid => {
        if (valid) {
          var model = Object.assign({}, this.form)
          console.log(model)
          this.confirmLoading = true
          this.$http.post('/custSpace/save', model).then(res => {
            this.confirmLoading = false
            if (res.status) {
              this.visible = false
              this.$message.success(res.msg, 3)
              this.$refs.table.refresh(true)
            } else {
              this.$message.error(res.msg, 3)
            }
          })
        } else {
          return false
        }
      })
    }
  }
}
</script>
<style lang="less" scoped>
.ant-modal-root{
  /deep/ .ant-form-item-label  {
     width: 80px;
  }
  .ant-row{
    .ant-form-item{
      height: 60px;
      margin-bottom: 0px;
      textarea{
        width: 490px;
        height: 60px;
      }
    }
    .checkbox_all{
      margin-right: 8px;
    }
  }

}

</style>
