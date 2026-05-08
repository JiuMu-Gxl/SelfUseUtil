<template>
  <div class="page">
    <div class="glass-card">
      <div class="title-wrap">
        <h1>修改密码</h1>

        <p>修改 MQTT 用户密码</p>
      </div>

      <div class="form">
        <el-input
          v-model="username"
          size="large"
          placeholder="请输入用户名"
        />

        <el-input
          v-model="newPassword"
          size="large"
          type="password"
          show-password
          placeholder="请输入新密码"
        />

        <el-button
          type="primary"
          size="large"
          class="main-btn"
          @click="handleChangePassword"
        >
          修改密码
        </el-button>

        <router-link
          class="back-link"
          to="/login"
        >
          返回登录
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

import { useRouter } from 'vue-router'

import { ElMessage } from 'element-plus'

import { changePasswordApi } from '@/api/mqttUser'

const router = useRouter()

const username = ref('')

const newPassword = ref('')

async function handleChangePassword() {
  try {
    await changePasswordApi(
      username.value,
      newPassword.value,
    )

    ElMessage.success('修改成功')

    router.push('/login')
  } catch {
    ElMessage.error('修改失败')
  }
}
</script>

<style scoped>
@import '@/styles/login.css';
</style>