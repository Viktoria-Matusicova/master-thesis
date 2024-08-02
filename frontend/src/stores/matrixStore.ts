import { defineStore } from "pinia";
import type Category from "../interfaces/Category";
import { ref } from "vue";
import MatrixService from "../services/MatrixService";

//Defines matrix store.
export const useMatrixStore = defineStore("matrix", () => {
  const selectedTab = ref("MITRE ATT&CK");
  const showLogsourceLoading = ref(1);
  const showMitreLoading = ref(1);
  const logsourceData = ref<Category[]>([]);
  const mitreData = ref<Category[]>([]);

  //Calls getter for logsource data.
  function getLogsourceData() {
    showLogsourceLoading.value = 1;
    logsourceData.value = [];
    MatrixService.getLogsourceMatrix().then((result) => {
      result.data.forEach(element => {
        logsourceData.value.push(element);
      });
      showLogsourceLoading.value = 0;
    })
  }

  //Calls getter for mitre data.
  function getMitreData() {
    showMitreLoading.value = 1;
    mitreData.value = [];
    MatrixService.getMitreMatrix().then((result) => {
      result.data.forEach(element => {
        mitreData.value.push(element);
      });
      showMitreLoading.value = 0;
    })
  }

  return {
    selectedTab,
    logsourceData,
    mitreData,
    showLogsourceLoading,
    showMitreLoading,
    getLogsourceData,
    getMitreData
  };
});
