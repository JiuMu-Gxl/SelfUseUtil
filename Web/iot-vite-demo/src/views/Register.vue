<template>
  <div class="page">
    <div class="glass-card">
      <div class="title-wrap">
        <h1>注册账号</h1>

        <p>创建 MQTT 用户</p>
      </div>

      <div class="form">
        <el-input
          v-model="username"
          size="large"
          placeholder="请输入用户名"
        />

        <el-input
          v-model="password"
          size="large"
          type="password"
          show-password
          placeholder="请输入密码"
        />

        <el-button
          type="primary"
          size="large"
          class="main-btn"
          @click="handleRegister"
        >
          注册账号
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

import { createUserApi } from '@/api/mqttUser'

const router = useRouter()

const username = ref('')

const password = ref('')

async function handleRegister() {
  try {
    await createUserApi(
      username.value,
      password.value,
    )

    ElMessage.success('注册成功')

    router.push('/login')
  } catch {
    ElMessage.error('注册失败')
  }
}
</script>

<style scoped>
@import '@/styles/login.css';
</style>