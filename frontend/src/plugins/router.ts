import { createAuthGuard } from '@auth0/auth0-vue';
import { setupLayouts } from 'virtual:generated-layouts';
import generatedRoutes from 'virtual:generated-pages';
import { App } from 'vue';
import { Router, createRouter as createVueRouter, createWebHistory } from 'vue-router';

export default function createRouter(app: App): Router {
  const authGuard = createAuthGuard(app);
  let routes = generatedRoutes;
  for (let route of routes) {
    if (route.meta?.requiresAuth) {
      route.beforeEnter = authGuard
    }
  }

  return createVueRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
      ...setupLayouts(routes),
    ],
    scrollBehavior() {
      return { top: 0 }
    },
  })
}