using System.Collections.Generic;

namespace WebApi.Features.Tree.Models
{
    public class TreeNode
    {
        public long key { get; set; }
        public string label { get; set; }
        public string data { get; set; }
        public string icon { get; set; }
        public IEnumerable<TreeNode> children { get; set; }

        public static TreeNode FromTreeItem(ITreeItem item)
        {
            return new TreeNode()
            {
                key = item.Id,
                label = item.Name,
                icon = item.Icon
            };
        }
    }
}