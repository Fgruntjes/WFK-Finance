<script lang="ts" setup>
import type { NavSectionTitle } from '@layouts/types';

defineProps<{ item: NavSectionTitle }>()
</script>

<template>
  <li class="nav-section-title">
    <div class="title-wrapper">
      <Transition
        name="vertical-nav-section-title"
        mode="out-in"
      >
        <span class="title-text">{{ item.heading }}</span>
      </Transition>
    </div>
  </li>
</template>
