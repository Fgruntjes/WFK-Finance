<route lang="yaml">
  meta:
    menuIcon: mdi-home-outline
    includeInMenu: true
    requiresAuth: true
</route>

<i18n>
  en:
    hello: Hello bank accounts!
</i18n>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<template>
  <p>{{ t("hello") }}</p>
</template>
