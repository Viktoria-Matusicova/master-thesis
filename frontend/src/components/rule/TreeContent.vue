<template>
  <div class="textarea">
    <div style="width: 85%; min-width: 350px">
      <ConvertArea :isDisabled="true" :rule="storeRule.sigmaRule" />
    </div>
    <div class="buttonArea">
      <v-btn
        prepend-icon="mdi-swap-horizontal"
        variant="outlined"
        rounded="lg"
        class="convertBtn"
        title="Send Rule to the Convert Section"
        @click="convertRule()"
        :disabled="storeRule.sigmaRule == ''"
        >Convert</v-btn
      >
      <v-btn
        prepend-icon="mdi-plus"
        variant="outlined"
        rounded="lg"
        class="convertBtn"
        title="Add Rule to the Custom Rules"
        @click="addToCustomRules()"
        :loading="isLoading"
        :disabled="storeRule.sigmaRule == ''"
        >Custom</v-btn
      >
    </div>
  </div>
  <SnackbarComponent
    :color="color"
    :snackbarModel="snackbar"
    :message="message"
    @close="snackbar = false"
    @navigate="openRule()"
    :navigation="navigation"
  ></SnackbarComponent>
  <v-dialog v-model="dialog" width="auto">
    <v-card>
      <v-card-title class="text-h7 text-center">
        Rule already exists
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text
        >You cannot create a duplicate. Do you want to replace existing Sigma
        value?
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" variant="text" @click="dialog = false"
          >Cancel</v-btn
        >
        <v-btn color="primary" variant="text" @click="closeDialog()"
          >Replace</v-btn
        >
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { useSigmaStore } from "../../stores/sigmaStore";
import { useRuleStore } from "../../stores/ruleStore";
import router from "../../router/index";
import ConvertArea from "../sigma/ConvertArea.vue";
import RuleService from "../../services/RuleService.js";
import { ref } from "vue";
import type CustomRule from "../../interfaces/CustomRule";
import Utility from "../../utility/utility";
import SnackbarComponent from "../SnackbarComponent.vue";
import { useMatrixStore } from "../../stores/matrixStore";

const storeSigma = useSigmaStore();
const storeRule = useRuleStore();
const matrixStore = useMatrixStore();
const snackbar = ref(false);
const isLoading = ref(false);
const addedRuleId = ref();
const dialog = ref();
const message = ref("");
const color = ref("");
const navigation = ref(false);

//Opens converter section and inserts Sigma value into input.
function convertRule() {
  storeSigma.ruleInput = storeRule.sigmaRule;
  storeSigma.ruleOutput = "";
  router.replace({ path: "/converter" });
}

//Opens successfully created custom rule from snackbar. It expands and selects keys in the tree node.
function openRule() {
  snackbar.value = false;
  RuleService.getCustomRule(addedRuleId.value).then((result) => {
    storeRule.customRule = result.data;
    router.replace({ path: "/custom" });

    const node = Utility.findNode(storeRule.customNodes, addedRuleId.value);
    if (node != null && node.key) {
      const arr: string[] = [];
      const parts = (node.key as string).split("-");

      const expandedKeys = ref<{
        [key: string]: any;
      }>({});

      for (let i = 0; i < parts.length - 1; i++) {
        const prev = i === 0 ? "" : arr[i - 1];
        const key = `${prev}${prev ? "-" : ""}${parts[i]}`;
        arr.push(key);
        expandedKeys.value[key] = true;
      }
      storeRule.expandedCustomKeys = { ...expandedKeys.value };
      storeRule.selectedCustomKey = { [node.key]: true };
    }
  });
}

//Closes snackbar's informative dialog and replace value.
function closeDialog() {
  dialog.value = false;
  addCustomRule();
}

//Checks if custom rule already exists. If the value is true, dialog is opened, otherwise custom rule will be added.
function addToCustomRules() {
  if (storeRule.customRuleIds.includes(storeRule.selectedParentId)) {
    dialog.value = true;
  } else {
    addCustomRule();
  }
}

//Adds sigma rule to the custom section. Snackbar is displayed with the information about how the server responded.
function addCustomRule() {
  isLoading.value = true;
  const encodedRule = btoa(storeRule.sigmaRule);
  const rule: CustomRule = {
    sigmaValue: encodedRule,
  };
  RuleService.addCustomRule(rule)
    .then((result) => {
      matrixStore.getLogsourceData();
      matrixStore.getMitreData();
      addedRuleId.value = result.data;
      RuleService.getCustomTreeNodes().then((result) => {
        storeRule.customNodes = result.data;
        storeRule.loadingCustomNodes = false;
        navigation.value = true;
        color.value = "success";
        message.value = "Rule was successfully added to Custom Rules.";
        snackbar.value = true;
        isLoading.value = false;
      });
    })
    .catch((error) => {
      message.value = error.message;
      navigation.value = false;
      color.value = "error";
      snackbar.value = true;
      isLoading.value = false;
    });
}
</script>

<style scoped>
.p-button {
  margin-right: 0.5rem;
}

.convertBtn {
  width: 50%;
  min-width: 160px;
  margin-bottom: 20px;
}

.buttonArea {
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding-top: 7px;
  padding-bottom: 7px;
  min-width: 150px;
  max-width: 250px;
  height: fit-content;
}

.textarea {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-evenly;
  padding: 30px 0px;
  height: 100%;
  width: 100%;
  flex-direction: row;
}

.text:focus-visible {
  border: 0 none #fff;
  overflow: hidden;
  outline: none;
}
</style>
