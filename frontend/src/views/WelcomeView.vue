<template>
  <div class="fill-height flex-container">
    <v-card variant="tonal" class="card-container">
      <v-form ref="form">
        <v-card-title primary-title class="card-title"
          ><v-icon
            icon="mdi-account-circle"
            size="small"
            style="padding-right: 10px"
          ></v-icon
          >Sign In</v-card-title
        >
        <v-card-item style="padding: 16px 23px 0px 23px">
          <v-text-field
            v-model="username"
            :rules="[(v) => !!v || 'Username is required']"
            label="Username"
            required
          ></v-text-field>
        </v-card-item>
        <v-card-item style="padding: 16px 23px 0px 23px">
          <v-text-field
            type="password"
            v-model="password"
            :rules="[(v) => !!v || 'Password is required']"
            label="Password"
            required
          ></v-text-field>
        </v-card-item>
        <v-card-actions
          style="justify-content: flex-end; padding: 16px 23px 23px 23px"
        >
          <v-btn
            prepend-icon="mdi-login"
            variant="outlined"
            rounded="lg"
            @click="login()"
            >Sign In</v-btn
          >
        </v-card-actions>
      </v-form>
    </v-card>
  </div>
  <SnackbarComponent
    :color="color"
    :snackbarModel="snackbarModel"
    :message="message"
    @close="snackbarModel = false"
  >
  </SnackbarComponent>
</template>

<script lang="ts" setup>
import AuthService from "../services/AuthService.js";
import { ref } from "vue";
import router from "../router/index";
import { useAuthStore } from "../stores/authStore";
import SnackbarComponent from "../components/SnackbarComponent.vue";

const form = ref();
const username = ref("user");
const password = ref("user");
const authStore = useAuthStore();
const snackbarModel = ref(false);
const message = ref("");
const color = ref("");

//If credentials are valid, method tries to retrieve token pair from server. If the login is successful, data are retrieved
//and the user is redirected to the application.
async function login() {
  const { valid } = await form.value.validate();
  if (valid) {
    AuthService.getToken(username.value, password.value)
      .then((result) => {
        localStorage.setItem("access_token", result.data.access);
        localStorage.setItem("refresh_token", result.data.refresh);
        authStore.isAuthenticated = true;
        router.replace({ path: "/converter" });
      })
      .catch((error) => {
        color.value = "error";
        if (error.response.status == "401") {
          message.value = "Wrong credentials";
        } else {
          message.value = error.message;
        }
        snackbarModel.value = true;
      });
  }
}
</script>

<style scoped>
.flex-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.card-container {
  height: fit-content;
  width: 30%;
  justify-content: center;
  align-items: center;
  max-width: 600px;
}

.card-title {
  font-size: 2rem;
  display: flex;
  justify-content: center;
  align-items: center !important;
  line-height: 3rem !important;
  padding-bottom: 0;
  padding-top: 16px;
}
</style>
