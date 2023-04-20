<route lang="yaml">
  meta:
    menuIcon: mdi-bank
    includeInMenu: true
    requiresAuth: true
</route>

<i18n>
  en:
    hello: Hello bank accounts!
</i18n>

<script setup lang="ts">
import { BankAccountsService } from '@/api/services';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const { data: registeredBankAccounts, loading, error } = useRequest(() => BankAccountsService.getBankAccounts());
</script>

<template>
  <p>{{ t("hello") }}</p>
  <p>{{ registeredBankAccounts }}</p>
</template>
