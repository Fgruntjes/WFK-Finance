<route lang="yaml">
  meta:
    requiresAuth: true
</route>

<i18n>
  en:
    hello: Hello World!
</i18n>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<template>
  <p>{{ t("hello") }}</p>
</template>
