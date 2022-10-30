<template>
  <page-header-wrapper :breadcrumb="true" :title="false">
    <a-card :bordered="false">
      <div class="table-page-search-wrapper">
        <a-form layout="inline">
          <a-row :gutter="48">
            <a-col :md="8" :sm="24">
              <a-form-item label="关键字">
                <a-input v-model="queryParam.keyword" placeholder="请输入用户名或账号"/>
              </a-form-item>
            </a-col>
            <a-col :md="8" :sm="24">
              <span class="table-page-search-submitButtons" >
                <a-button type="primary" @click="$refs.table.refresh(true)">查询</a-button>
                <a-button style="margin-left: 8px" @click="() => this.queryParam = {}">重置</a-button>
              </span>
            </a-col>
          </a-row>
        </a-form>
      </div>
      <a-button type="primary" icon="plus" class="editable-add-btn" @click="handleAdd" style="margin-bottom:10px;">
        新增
      </a-button>
      <s-table
        ref="table"
        rowKey="userId"
        size="default"
        :columns="columns"
        :data="loadData"
      >
        <span slot="isAdmin" slot-scope="text,record">
          <template>
            <a-tag color="red" v-if="record.isAdmin">
              管理员
            </a-tag>
            <a-tag color="blue" v-else>
              普通用户
            </a-tag>
          </template>
        </span>
        <span slot="action" slot-scope="text, record">
          <template>
            <a-button size="small" @click="handleInfo(record)">查看</a-button>
            <span v-if="!record.isAdmin">
              <a-divider type="vertical" />
              <a-button @click="handleEdit(record)" type="primary" size="small">编辑</a-button>
              <a-divider type="vertical" />
              <a-button @click="handleReset(record)" type="primary" size="small" ghost>重置密码</a-button>
              <a-divider type="vertical" />
              <a-button @click="handleDelete(record)" type="danger" size="small">删除</a-button>
            </span>
          </template>
        </span>
      </s-table>
    </a-card>
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
              <a-form-model-item ref="userName" label="用户名" prop="userName">
                <a-input
                  v-model="form.userName"
                  placeholder="请输入用户名"
                />
              </a-form-model-item>
            </a-col>
            <a-col :span="12">
              <a-form-model-item label="用户账号" prop="userAccount">
                <a-input
                  v-model="form.userAccount"
                  placeholder="请输入用户账号"/>
              </a-form-model-item>
            </a-col>
          </a-row>
          <a-row>
            <a-col :span="12" v-if="isShowPwd">
              <a-tooltip>
                <template slot="title">
                  默认密码：qwer123
                </template>
                <a-form-model-item label="账号密码" prop="userAccountPwd">
                  <a-input
                    type="password"
                    v-model="form.userAccountPwd"
                    placeholder="请输入用户密码"/>
                </a-form-model-item>
              </a-tooltip>

            </a-col>
            <a-col :span="12">
              <a-form-model-item label="用户手机号" prop="contactPhone">
                <a-input
                  v-model="form.contactPhone"
                  placeholder="请输入用户手机号"/>
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
          <a-divider>用户权限</a-divider>
          <a-row>
            <a-col :span="24">
              <a-checkbox v-model="checkAll" class="checkbox_all" @change="onCheckAllChange">全选</a-checkbox>
              <a-checkbox-group v-model="form.pers" :options="options" @change="onChange" />
            </a-col>
          </a-row>
        </a-form-model>
      </template>
      <template slot="footer">
        <a-button type="white" @click="visible=false">取消</a-button>
        <a-button type="primary" @click="handleOk" :loading="confirmLoading" v-if="isSave">确定</a-button>
      </template>
    </a-modal>
  </page-header-wrapper>
</template>

<script>
import { STable } from '@/components'
import md5 from 'md5'
export default {
  components: {
    STable
  },
  data () {
    return {
      footer: 'OK',
      checkAll: false,
      isSave: false,
      options: [{ label: '预订', value: 2001 }, { label: '订单记录', value: 2002 }, { label: '场地管理', value: 3001 }, { label: '配置管理', value: 3002 }, { label: '用户列表', value: 4001 }],
      labelCol: { },
      wrapperCol: { },
     form: {
        userName: '',
        userAccount: '',
        userAccountPwd: '',
        contactPhone: '',
        remark: '',
        pers: []
      },
      rules: {
        userName: [
           { required: true, message: '请输入用户名称', trigger: 'blur' },
           { max: 50, message: '最大字符50个', trigger: 'blur' }
        ],
        userAccount: [
           { required: true, message: '请输入用户账号', trigger: 'blur' },
           { max: 20, message: '最大字符20个', trigger: 'blur' }
          ],
        userAccountPwd: [
          { required: true, message: '请输入账号密码', trigger: 'blur' },
          { max: 20, message: '最大字符20个', trigger: 'blur' }
        ]
      },
      title: '',
      isShowPwd: false,
      visible: false,
      confirmLoading: false,
      queryParam: {},
      columns: [
        {
          title: '用户名称',
          dataIndex: 'userName',
          key: 'userName'
        },
        {
          title: '用户账号',
          dataIndex: 'userAccount',
          key: 'userAccount'
        },
        {
          title: '手机号',
          dataIndex: 'contactPhone',
          key: 'contactPhone'
        },
        {
          title: '是否管理员',
          dataIndex: 'isAdmin',
          key: 'isAdmin',
           scopedSlots: { customRender: 'isAdmin' }
        },
        {
          title: '备注',
          dataIndex: 'remark',
          key: 'remark',
          align: 'left'
        },
        {
            title: '操作',
            width: 300,
            dataIndex: 'action',
            scopedSlots: { customRender: 'action' }
          }
      ],
      // 加载数据方法 必须为 Promise 对象
      loadData: parameter => {
        const requestParameters = Object.assign({}, parameter, this.queryParam)
        return this.$http.post('/user/list', requestParameters)
          .then(res => {
            return res
          })
      }
    }
  },
  methods: {
     onChange (checkedList) {
      this.indeterminate = !!checkedList.length && checkedList.length < this.options.length
      this.checkAll = checkedList.length === this.options.length
    },
    onCheckAllChange (e) {
      this.form.pers = e.target.checked ? [2001, 2002, 3001, 3002, 4001] : []
    },
    handleReset (row) {
    var $this = this
      this.$confirm({
        title: '确认重置该用户【' + row.userAccount + '】密码吗？',
        okType: 'danger',
        content: h => <div style="color:red;">密码将重置为：qwer123</div>,
        onOk () {
            $this.$http.patch(`/user/reset/${row.userId}`).then(res => {
              if (res.status) {
                 $this.$message.success(res.msg, 3)
              } else {
                 $this.$message.error(res.msg, 3)
              }
            })
          }
        })
    },
    handleEdit (row) {
     if (this.$refs.ruleForm) {
        this.$refs.ruleForm.resetFields()
      }
      this.form.pers = []
      this.checkAll = false
      this.footer = null
      this.isSave = true
      this.isShowPwd = false
      this.title = '编辑用户：' + row.userAccount
      this.visible = true
      this.$http.get(`/user/info/${row.userId}`).then(res => {
       if (res.status) {
         var model = Object.assign({}, res.data) || {}
         model.pers = model.isAdmin ? [2001, 2002, 3001, 3002, 4001] : (res.pers || [])
         if (model.pers.length === this.options.length) {
           this.checkAll = true
         }
         this.form = model
       } else {
         this.$message.error(res.msg, 3)
       }
     })
    },
    handleInfo (row) {
       if (this.$refs.ruleForm) {
        this.$refs.ruleForm.resetFields()
      }
      this.form.pers = []
      this.checkAll = false
      this.footer = null
      this.isSave = false
      this.isShowPwd = false
      this.title = '查看用户：' + row.userAccount
      this.visible = true
      this.$http.get(`/user/${row.userId}`).then(res => {
       if (res.status) {
         var model = Object.assign({}, res.data) || {}
         model.pers = model.isAdmin ? [2001, 2002, 3001, 3002, 4001] : (res.pers || [])
         if (model.pers.length === this.options.length) {
           this.checkAll = true
         }
         this.form = model
       } else {
         this.$message.error(res.msg, 3)
       }
     })
    },
    handleDelete (row) {
      var $this = this
      this.$confirm({
        title: '确认删除该用户【' + row.userAccount + '】吗？',
        okType: 'danger',
        content: h => <div style="color:red;">记录会永久删除</div>,
        onOk () {
            $this.$http.delete(`/user/${row.userId}`).then(res => {
              if (res.status) {
                 $this.$message.success(res.msg, 3)
                 $this.$refs.table.refresh(true)
              } else {
                 $this.$message.error(res.msg, 3)
              }
            })
          }
        })
    },
    handleAdd () {
      if (this.$refs.ruleForm) {
        this.$refs.ruleForm.resetFields()
      }
      this.form = this.$options.data().form
      this.isSave = true
      this.isShowPwd = true
      this.checkAll = true
      this.form.userAccountPwd = 'qwer123'
      this.form.pers = [2001, 2002, 3001, 3002, 4001]
      this.title = '新增用户'
      this.visible = true
    },
    handleOk () {
      this.$refs.ruleForm.validate(valid => {
        if (valid) {
          var model = Object.assign({}, this.form)
          console.log(model)
          if (this.isShowPwd) {
            model.userAccountPwd = md5(model.userAccountPwd)
          }
          this.confirmLoading = true
          this.$http.post('/user/save', model).then(res => {
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
  },
  filters: {
    statusFilter (status) {
      const statusMap = {
        'processing': '进行中',
        'success': '完成',
        'failed': '失败'
      }
      return statusMap[status]
    }
  },
  computed: {
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
