<script setup lang="ts">
import logo from '@/assets/logo.svg?raw';
import { VerticalNavLink, VerticalNavSectionTitle } from '@layouts';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const routes = useRouter()
  .getRoutes()
  .filter((route) => route.meta?.includeInMenu);

</script>

<i18n lang="yaml">
  en:
    header:
      configuration: Configuration
    link:
      dashboard: Dashboard
      bank_accounts: Bank accounts
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
        title: t('link.dashboard'),
        to: 'index',
        icon: { icon: 'mdi-home-outline' }
      }"
    />

    <VerticalNavSectionTitle :item="{ heading: t('header.configuration') }" />
    <VerticalNavLink
      v-for="route in routes"
      :item="{
        title: t(`link.${String(route.name)}`),
        to: route.name as string,
        icon: { icon: route.meta?.menuIcon }
      }"
    /> 
    <!-- 👉 Pages
    
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
