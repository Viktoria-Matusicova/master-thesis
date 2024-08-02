import { defineStore } from "pinia";
import SigmaService from "../services/SigmaService";
import { ref } from "vue";

//Defines Sigma store. 
export const useSigmaStore = defineStore("sigma", () => {
  const ruleInput = ref("");
  const ruleOutput = ref("");
  const backend = ref();
  const config = ref();
  const isBackendLoading = ref(true);
  const isConfigLoading = ref(true);
  const backendItems = ref([]);
  const configItems = ref([]);
  const isConverting = ref(false);

  //Updated backend options.
  function updateBackend(newValue: string) {
    backend.value = newValue;
  }

  //Updated config options.
  function updateConfig(newValue: string) {
    config.value = newValue;
  }

  //get backends.
  function getBackends() {
    SigmaService.getBackends().then((result) => {
      backendItems.value = result.data;
      isBackendLoading.value = false;
    });
  }

  //get configurations.
  function getConfigs() {
    SigmaService.getBackends().then((result) => {
      configItems.value = result.data;
      isConfigLoading.value = false;
    });
  }

  return {
    ruleInput,
    ruleOutput,
    backend,
    config,
    isBackendLoading,
    isConfigLoading,
    backendItems,
    configItems,
    isConverting,
    updateBackend,
    updateConfig,
    getConfigs,
    getBackends
  };
});
