import { defineStore } from "pinia";
import type Rules from "../interfaces/Rules";
import RuleService from "../services/RuleService";
import { reactive, ref } from "vue";

//Defines rule store. 
export const useRuleStore = defineStore("rule", () => {
  const sigmaRule = ref("");
  const selectedParentId = ref("");
  const sigmaNodes = ref();
  const Rules: Rules = {
    ruleId: "",
    parentId: "",
    siems: [],
    sigma: { value: "", framework: { generatedName: "", category: "", reference: "", subcategory: "", name: "" } },
  };

  const customRule = reactive(Rules);
  const customNodes = ref();
  const customRuleIds = ref<string[]>([]);
  const loadingSigmaNodes = ref(true);
  const expandedSigmaKeys = ref<{
    [key: string]: any;
  }>({});
  const selectedSigmaKey = ref<{
    [key: string]: any;
  }>({});
  const selectedCustomKey = ref<{
    [key: string]: any;
  }>({});
  const loadingCustomNodes = ref(true);
  const expandedCustomKeys = ref<{
    [key: string]: any;
  }>({});
  const customRuleId = ref();

  //Collapses all expanded keys in Sigma section.
  function collapseAllExpandedKeys() {
    expandedSigmaKeys.value = {};
  }

  //Collapses all expanded keys in Custom section.
  function collapseAllExpandedCustomKeys() {
    expandedCustomKeys.value = {};
  }

  //get tree nodes.
  function getTreeNodes() {
    RuleService.getTreeNodes().then((result) => {
      sigmaNodes.value = result.data;
      loadingSigmaNodes.value = false;
    });
  }

  return {
    sigmaRule,
    sigmaNodes,
    customRule,
    customNodes,
    loadingSigmaNodes,
    selectedSigmaKey,
    loadingCustomNodes,
    expandedCustomKeys,
    selectedCustomKey,
    customRuleId,
    expandedSigmaKeys,
    customRuleIds,
    selectedParentId,
    collapseAllExpandedKeys,
    collapseAllExpandedCustomKeys,
    getTreeNodes
  };
});
