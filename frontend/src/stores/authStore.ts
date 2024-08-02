import { defineStore } from "pinia";
import { ref } from "vue";

//Defines authentication store.
export const useAuthStore = defineStore("auth", () => {
  const isAuthenticated = ref(false);

  return {
    isAuthenticated,
  };
});
