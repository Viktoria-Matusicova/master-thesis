<template>
  <div class="border-right">
    <div class="button-container">
      <v-btn
        variant="text"
        @click="collapseAll"
        icon="mdi-collapse-all"
        size="small"
        style="height: 30px; width: 30px"
        title="Collapse All"
      >
      </v-btn>
    </div>
    <div style="display: flex; overflow: auto; height: 100%">
      <Tree
        :value="store.sigmaNodes"
        class="tree-menu"
        scrollHeight="flex"
        :expandedKeys="expandedKeys"
        :loading="store.loadingSigmaNodes"
        selectionMode="single"
        @node-select="onNodeSelect"
        @node-expand="onNodeExpand"
        @node-collapse="onNodeCollapse"
        :selection-keys="selectedKey"
        @update:selection-keys="onSelection"
      ></Tree>
    </div>
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
import Tree, { type TreeNode } from "primevue/tree";
import RuleService from "../../services/RuleService.js";
import { useRuleStore } from "../../stores/ruleStore";
import "../../styles/_variables.scss";
import YAML from "yaml";
import { ref, watch } from "vue";
import SnackbarComponent from "../SnackbarComponent.vue";

const store = useRuleStore();
const expandedKeys = ref(store.expandedSigmaKeys);
const selectedKey = ref(store.selectedSigmaKey);
const snackbarModel = ref(false);
const message = ref("");
const color = ref("");

//Watches for changes of the expanded sigma keys property. Whenever is the value changed, the watcher updates local value.
watch(
  () => store.expandedSigmaKeys,
  () => {
    expandedKeys.value = { ...store.expandedSigmaKeys };
  }
);

//Watches for changes of the selected sigma key property. Whenever is the value changed, the watcher updates local value.
watch(
  () => store.selectedSigmaKey,
  () => {
    selectedKey.value = { ...store.selectedSigmaKey };
  }
);

//Collapses all expanded keys from local and stored value.
function collapseAll() {
  expandedKeys.value = {};
  store.collapseAllExpandedKeys();
}

//Function is called whenever selected tree node is updated. If node doesn't have any children,
//sigma value is retrieved from the server, otherwise node is expanded.
function onNodeSelect(node: TreeNode) {
  if (node.children && node.children.length == 0 && node.id != undefined) {
    RuleService.getSigmaRule(node.id)
      .then((result) => {
        store.selectedParentId = node.id;
        const yamlObj = YAML.parse(result.data);
        yamlObj["description"] = yamlObj["description"].replace(/\n/g, "");
        const ymlString = YAML.stringify(yamlObj, null, 6);
        store.sigmaRule = ymlString;
      })
      .catch((e) => {
        color.value = "error";
        message.value = e.message;
        snackbarModel.value = true;
      });
  } else if (node.key) {
    if (expandedKeys.value[node.key]) {
      expandedKeys.value[node.key] = false;
    } else {
      expandedKeys.value[node.key] = true;
    }
    expandedKeys.value = { ...expandedKeys.value };
    store.expandedSigmaKeys = { ...expandedKeys.value };
  }
}

//Function is called whenever any node is expanded. It updates stored and local value.
function onNodeExpand(node: TreeNode) {
  if (node.key) {
    expandedKeys.value[node.key] = true;
    store.expandedSigmaKeys.value = { ...expandedKeys.value };
  }
}

//Function is called whenever any node is collapsed. It updates stored and local value.
function onNodeCollapse(node: TreeNode) {
  if (node.key) {
    expandedKeys.value[node.key] = false;
    store.expandedSigmaKeys.value = { ...expandedKeys.value };
  }
}

//Function is called when selected key is updated. It updates stored and local value.
function onSelection(node: any) {
  selectedKey.value = node;
  store.selectedSigmaKey = node;
}
</script>

<style scoped lang="scss">
.tree-menu {
  max-height: 100%;
  min-width: 15vw;
  max-width: 25vw;
  padding-right: 1px;
  background-color: #1b1b1b;
  color: rgb(var(--v-theme-dark));
  overflow: auto;
}

.button-container {
  display: flex;
  justify-content: flex-end;
  padding-bottom: 0.2rem;
  padding-right: 0.2rem;
}

.border-right {
  background-color: #1b1b1b;
  height: 100%;
  display: flex;
  flex-direction: column;
  width: fit-content;
}

.p-button {
  margin-right: 0.5rem;
}

.v-container--fluid {
  padding-left: 0px;
}

::v-deep(.v-main) {
  .v-container--fluid {
    padding-left: 0px;
  }
}

::v-deep(.p-tree) {
  border: 0px;
  padding: 0px;
  width: fit-content;

  .p-tree-container .p-treenode .p-treenode-content.p-highlight {
    color: black;
  }

  .p-tree-container .p-treenode .p-treenode-content .p-treenode-icon {
    color: white;
    font-size: 0.8rem;
  }

  .p-tree-loading-overlay {
    background-color: #1b1b1b;
  }

  .p-tree-wrapper {
    padding: 5px;

    .p-tree-container {
      font-size: 15px;
      height: 100%;
      background-color: #1b1b1b;
      width: fit-content;
      overflow: unset;

      .p-treenode {
        padding: 0px;
        width: fit-content;

        .p-treenode-content {
          padding: 0px;
          padding-right: 10px;
          width: fit-content;

          .p-tree-toggler {
            width: 1.1rem;
            height: 1.1rem;
            margin-right: 0rem;
            color: #ffffff;
          }
        }
      }
    }
  }
}
</style>
