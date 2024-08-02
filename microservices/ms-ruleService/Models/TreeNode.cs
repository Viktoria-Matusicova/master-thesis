namespace ruleService.Models
{

    /// <summary>Represents a tree node structure.</summary>
    public class TreeNode
    {
        /// <summary>The key of the tree node.</summary>
        public string key { get; set; }

        /// <summary>The label of the tree node.</summary>
        public string label { get; set; }

        /// <summary>The icon of the tree node.</summary>
        public string icon { get; set; }

        /// <summary>The ID of the tree node.</summary>
        public string id { get; set; }

        /// <summary>The  ID of the parent tree node.</summary>
        public string parentId { get; set; }

        /// <summary>The list of children in the current tree node./summary>
        public List<TreeNode> children { get; set; }

        public TreeNode(string key, string label, string id = "", string parentId = "")
        {
            this.key = key;
            this.label = label;
            if (label.Contains(".yml"))
            {
                this.icon = "pi pi-file";
            }
            this.icon = icon;
            this.id = id;
            this.parentId = parentId;
            children = new List<TreeNode>();
        }
        public TreeNode(TreeNode oldNode)
        {
            this.key = oldNode.key;
            this.label = oldNode.label;
            this.icon = oldNode.icon;
            this.id = oldNode.id;
            this.parentId = oldNode.parentId;
            this.children = new List<TreeNode>();
            foreach (var item in oldNode.children)
            {
                this.children.Add(new TreeNode(item));
            }
        }

        /// <summary>Adds a child tree node to the current node.</summary>
        /// <param name="key">The key of the child tree node.</param>
        /// <param name="label">The label of the child tree node.</param>
        /// <param name="id">The ID of the child tree node (optional).</param>
        /// <param name="parentId">The parent ID of the child tree node (optional).</param>
        public void AddChild(string key, string label, string id = "", string parentId = "")
        {
            children.Add(new TreeNode(key, label, id, parentId));
        }

        /// <summary>Finds a child tree node with the specified label.</summary>
        /// <param name="label">The label of the child tree node to find.</param>
        /// <returns>The found child tree node or null if not found.</returns>
        public TreeNode FindChild(string label)
        {
            foreach (TreeNode child in children)
                if (child.label == label)
                    return child;
            return null;
        }

        /// <summary>Gets the number of children in current tree node.</summary>
        /// <returns>The number of children in current tree node.</returns>
        public int GetChildrenCount()
        {
            return children.Count;
        }
    }
}



