<script setup lang="ts">
import logo from '@/assets/logo.svg?raw';
import { VerticalNavLink } from '@layouts';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<i18n lang="yaml">
  en:
    dashboard: Dashboard
</i18n>
  

<template>
  <!-- 👉 Nav header -->
  <div class="nav-header">
    <RouterLink
      to="/"
      class="app-logo d-flex align-center gap-x-3 app-title-wrapper"
    >
      <!-- ℹ️ You can also use img tag or VImg here -->
      <div v-html="logo" />

      <Transition name="vertical-nav-app-title">
        <h1 class="font-weight-semibold leading-normal text-xl text-uppercase">
          WFK Finance
        </h1>
      </Transition>
    </RouterLink>
  </div>

  <!-- 👉 Nav items -->
  <ul>
    <VerticalNavLink
      :item="{
        title: t('dashboard'),
        to: 'index',
        icon: { icon: 'mdi-home-outline' }
      }"
    />
    <!-- 👉 Pages
    <VerticalNavSectionTitle :item="{ heading: 'Pages' }" />
    <VerticalNavLink
      :item="{
        title: 'Login',
        to: 'login',
        target: '_blank',
        icon: { icon: 'mdi-login' }
      }"
    /> -->
  </ul>
</template>
