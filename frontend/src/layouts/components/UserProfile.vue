<script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n({ useScope: 'local' })

const auth0 = useAuth0();
const user = auth0.user.value;
</script>

<i18n lang="yaml">
en:
  logout: Logout
</i18n>

<template>
  <VBadge location="bottom right" dot bordered offset-x="3" offset-y="3" color="success" v-if="user">
    <VAvatar
      style="cursor: pointer;"
      color="primary"
      variant="tonal"
    >
      <VImg :src="user.picture" />

      <!-- SECTION Menu -->
      <VMenu
        activator="parent"
        width="230"
        location="bottom end"
        offset="14px"
      >
        <VList>
          <!-- 👉 User Avatar & Name -->
          <VListItem>
            <template #prepend>
              <VListItemAction start>
                <VBadge dot bordered offset-x="3" offset-y="3" color="success" location="bottom right">
                  <VAvatar
                    color="primary"
                    size="40"
                    variant="tonal"
                  >
                    <VImg :src="user.picture" />
                  </VAvatar>
                </VBadge>
              </VListItemAction>
            </template>

            <VListItemTitle class="font-weight-semibold">
              {{ user.name }}
            </VListItemTitle>
          </VListItem>

          <!-- Divider -->
          <VDivider class="my-2" />

          <!-- 👉 Logout -->
          <VListItem @click="auth0.logout">
            <template #prepend>
              <VIcon
                class="me-2"
                icon="mdi-logout-variant"
                size="22"
              />
            </template>

            <VListItemTitle>{{ t('logout') }}</VListItemTitle>
          </VListItem>
        </VList>
      </VMenu>
      <!-- !SECTION -->
    </VAvatar>
  </VBadge>
</template>
