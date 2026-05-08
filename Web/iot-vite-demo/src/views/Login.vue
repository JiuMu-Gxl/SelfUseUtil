<template>
  <div class="page">
    <div class="bg bg1"></div>
    <div class="bg bg2"></div>

    <div class="glass-card">
      <div class="logo-wrap">
        <div class="logo">
          MQTT
        </div>

        <h1>智能控制系统</h1>

        <p>MQTT Device Control Platform</p>
      </div>

      <div class="form">
        <el-input
          v-model="form.username"
          size="large"
          placeholder="请输入用户名"
        >
          <template #prefix>
            <el-icon>
              <User />
            </el-icon>
          </template>
        </el-input>

        <el-input
          v-model="form.password"
          size="large"
          type="password"
          show-password
          placeholder="请输入密码"
          @keyup.enter="handleLogin"
        >
          <template #prefix>
            <el-icon>
              <Lock />
            </el-icon>
          </template>
        </el-input>

        <el-button
          type="primary"
          size="large"
          class="main-btn"
          @click="handleLogin"
        >
          登录系统
        </el-button>

        <div class="links">
          <router-link to="/register">
            注册账号
          </router-link>

          <router-link to="/change-password">
            修改密码
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'

import { useRouter } from 'vue-router'

import { ElMessage } from 'element-plus'

import {
  User,
  Lock,
} from '@element-plus/icons-vue'

import { testConnectApi } from '@/api/mqtt'

import { useUserStore } from '@/stores/user'

const router = useRouter()

const userStore = useUserStore()

const form = reactive({
  username: '',
  password: '',
})

async function handleLogin() {
  if (!form.username || !form.password) {
    ElMessage.warning('请输入用户名和密码')

    return
  }

  try {
    await testConnectApi(form)

    userStore.login(form.username)

    ElMessage.success('登录成功')

    router.push('/')
  } catch {
    ElMessage.error('连接失败')
  }
}
</script>

<style scoped>
.page {
  position: relative;

  width: 100%;
  min-height: 100vh;

  display: flex;
  justify-content: center;
  align-items: center;

  padding: 20px;

  overflow: hidden;
}

.bg {
  position: absolute;

  border-radius: 50%;

  filter: blur(90px);

  opacity: 0.5;
}

.bg1 {
  width: 320px;
  height: 320px;

  background: #3b82f6;

  top: -100px;
  left: -100px;
}

.bg2 {
  width: 280px;
  height: 280px;

  background: #06b6d4;

  right: -80px;
  bottom: -80px;
}

.glass-card {
  position: relative;

  width: 100%;
  max-width: 420px;

  padding: 42px 34px;

  border-radius: 30px;

  backdrop-filter: blur(22px);

  background: rgba(255, 255, 255, 0.08);

  border: 1px solid rgba(255, 255, 255, 0.12);

  box-shadow:
    0 8px 32px rgba(0, 0, 0, 0.28);

  z-index: 10;
}

.logo-wrap {
  text-align: center;

  margin-bottom: 34px;
}

.logo {
  width: 82px;
  height: 82px;

  margin: 0 auto 18px;

  border-radius: 24px;

  background:
    linear-gradient(
      135deg,
      #3b82f6,
      #06b6d4
    );

  display: flex;
  justify-content: center;
  align-items: center;

  color: white;

  font-weight: bold;

  font-size: 24px;
}

.logo-wrap h1 {
  color: white;

  font-size: 30px;

  margin-bottom: 10px;
}

.logo-wrap p {
  color: rgba(255, 255, 255, 0.7);
}

.form {
  display: flex;
  flex-direction: column;

  gap: 22px;
}

.main-btn {
  height: 48px;

  border-radius: 14px;

  font-size: 16px;
}

.links {
  display: flex;

  justify-content: space-between;
}

.links a {
  color: rgba(255, 255, 255, 0.85);

  font-size: 14px;
}

:deep(.el-input__wrapper) {
  height: 48px;

  border-radius: 14px;

  background: rgba(255, 255, 255, 0.08);

  box-shadow: none;
}

:deep(.el-input__inner) {
  color: white;
}

:deep(.el-input__inner::placeholder) {
  color: rgba(255, 255, 255, 0.45);
}

@media (max-width: 768px) {
  .glass-card {
    padding: 30px 22px;
  }

  .logo-wrap h1 {
    font-size: 24px;
  }
}
</style>