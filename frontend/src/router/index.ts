/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { createRouter, createWebHistory } from "vue-router";
import RuleView from "../views/RuleView.vue";
import WelcomeView from "../views/WelcomeView.vue";
import NotFound from "../components/NotFound.vue";
import AuthService from "../services/AuthService";
import { useAuthStore } from "../stores/authStore";
import Utility from "../utility/utility";

//Create router. Defines possible routes with component to load and specifies routes where authentication is required.
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: "/login", name: "Login", component: WelcomeView },
    {
      path: "/rules",
      component: RuleView,
      meta: { requiresAuth: true },
    },
    { path: "/:pathMatch(.*)", component: NotFound },
  ],
});

//checks before every route change if user is authorized to visit the page.
router.beforeEach(async (to, from, next) => {
  const store = useAuthStore();
  // Check if the page requires authentication.
  if (to.matched.some((record) => record.meta.requiresAuth)) {
    // Check if the user is authenticated
    if (!AuthService.isAuthenticated()) {
      store.isAuthenticated = false;
      // If the user is not authenticated, redirect to the login page.
      next({
        name: "Login",
      });
    } else {
      // Check if the access token is valid.
      try {
        const IsJWTExpired = Utility.IsJWTExpired(
          localStorage.getItem("access_token")!
        );
        if (!IsJWTExpired) {
          // If the access token is valid, continue to the next page.
          store.isAuthenticated = true;
          next();
        }
      } catch (error) {
        // If the access token is not valid, try to refresh the access token.
        try {
          await AuthService.sendRefreshToken();
          store.isAuthenticated = true;
          // If the access token is successfully refreshed, continue to the next page.
          next();
        } catch (error) {
          store.isAuthenticated = false;
          // If the access token cannot be refreshed, redirect to the login page.
          next({
            name: "Login",
          });
        }
      }
    }
  } else {
    //If user is authenticated, cannot access login page again.
    if (store.isAuthenticated && to.path == "/login") {
      next({
        path: from.path,
        params: { nextUrl: from.fullPath },
      });
    } else {
      // If the page does not require authentication, continue to the next page.
      next();
    }
  }
});

export default router;
