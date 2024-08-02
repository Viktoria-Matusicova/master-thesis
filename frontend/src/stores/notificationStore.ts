import { defineStore } from "pinia";
import { reactive, ref } from "vue";
import type Notification from "../interfaces/Notification";

//Defines notification store. 
export const useNotificationStore = defineStore("notification", () => {
  const notifications = reactive<Notification[]>([{ action: "", date: "", id: "", parentId: "", ruleName: "", isNew: false, isCustomExisting: false, ruleId: "" }]);
  const lastNotificationId = ref(localStorage.getItem("last_notification"));
  const registrationDate = ref<Date>();

  return {
    notifications,
    lastNotificationId,
    registrationDate
  };
});
