<template>
  <div>
    <a-dropdown v-if="currentUser && currentUser.userName" placement="bottomRight">
      <span class="ant-pro-account-avatar">
        <a-avatar size="small" :src="person" class="antd-pro-global-header-index-avatar" />
        <span>{{ (currentUser.userName+"("+currentUser.userAccount+")") }}</span>
      </span>
      <template v-slot:overlay>
        <a-menu class="ant-pro-drop-down menu" :selected-keys="[]">
          <a-menu-item v-if="menu" key="settings" @click="handleToSettings">
            <a-icon type="setting" />
            {{ $t('menu.account.settings') }}
          </a-menu-item>
          <a-menu-divider v-if="menu" />
          <a-menu-item key="logout" @click="handleLogout">
            <a-icon type="logout" />
            {{ $t('menu.account.logout') }}
          </a-menu-item>
        </a-menu>
      </template>
    </a-dropdown>
    <span v-else>
      <a-spin size="small" :style="{ marginLeft: 8, marginRight: 8 }" />
    </span>
    <a-modal
      :visible="visible"
      :title="title"
      :confirm-loading="confirmLoading"
      @ok="handleOk"
      @cancel="visible=false"
    >
      <a-form :form="form" :label-col="{ span: 5 }" :wrapper-col="{ span: 16 }" >
        <a-form-item label="原密码">
          <a-input
            v-decorator="['oldPassword', { rules: [{ required: true, message: '请输入原密码!' }] }]"
            type="password"
            placeholder="请输入原密码"
          />
        </a-form-item>
        <a-form-item label="新密码">
          <a-input
            v-decorator="['password', { rules: [{ required: true, message: '请输入新密码!' }, {
              validator: validateToNextPassword,
            }] }]"
            type="password"
            placeholder="请输入新密码"
          />
        </a-form-item>
        <a-form-item label="确认密码">
          <a-input
            v-decorator="['confirmPassword', { rules: [{ required: true, message: '请再次输入新密码!' },{
              validator: compareToFirstPassword,
            }] }]"
            type="password"
            @blur="handleConfirmBlur"
            placeholder="请再次输入新密码"
          />
        </a-form-item>
      </a-form></a-modal>
  </div>
</template>

<script>
import { Modal } from 'ant-design-vue'
import md5 from 'md5'
import person from '/public/person.png'
export default {
  name: 'AvatarDropdown',
  data () {
    return {
    confirmDirty: false,
    form: this.$form.createForm(this, { name: 'form' }),
    visible: false,
    confirmLoading: false,
    title: '修改密码',
     person: person
    }
  },
  props: {
    currentUser: {
      type: Object,
      default: () => null
    },
    menu: {
      type: Boolean,
      default: true
    }
  },
  methods: {
    handleOk (e) {
      e.preventDefault()
      this.form.validateFields((err, values) => {
        if (!err) {
          values['oldPassword'] = md5(values.oldPassword)
          values['password'] = md5(values.password)
          values['confirmPassword'] = md5(values.confirmPassword)
          this.$http.post('/user/changePwd', values).then(res => {
            if (res.status) {
              this.visible = false
              this.$message.success(res.msg, 3)
            } else {
              this.$message.error(res.msg, 3)
            }
          })
        }
      })
    },
      handleConfirmBlur (e) {
      const value = e.target.value
      this.confirmDirty = this.confirmDirty || !!value
    },
    compareToFirstPassword (rule, value, callback) {
      const form = this.form
      if (value && value !== form.getFieldValue('password')) {
        callback('两次输入的密码不一致')
      } else {
        callback()
      }
    },
    validateToNextPassword (rule, value, callback) {
      const form = this.form
      if (value && this.confirmDirty) {
        form.validateFields(['confirmPassword'], { force: true })
      }
      callback()
    },
    handleToCenter () {
      this.$router.push({ path: '/account/center' })
    },
    handleToSettings () {
      this.form.resetFields()
      this.visible = true
    },
    handleLogout (e) {
      Modal.confirm({
        title: this.$t('layouts.usermenu.dialog.title'),
        content: this.$t('layouts.usermenu.dialog.content'),
        onOk: () => {
          return this.$store.dispatch('Logout').then(() => {
            this.$router.push({ name: 'login' })
          })
        },
        onCancel () {}
      })
    }
  }
}
</script>

<style lang="less" scoped>
.ant-pro-drop-down {
  /deep/ .action {
    margin-right: 8px;
  }
  /deep/ .ant-dropdown-menu-item {
    min-width: 160px;
  }
}
</style>
