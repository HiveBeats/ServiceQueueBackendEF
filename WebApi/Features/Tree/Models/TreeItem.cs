using System.Collections.Generic;
using System.Linq;

namespace WebApi.Features.Tree.Models
{
    public interface ITreeItem
    {
        long Id { get; }
        long? ParentId { get; }
        string Name { get; }
        string Icon { get; }
    }

    public static class TreeItemExtensions
    {
        public static IEnumerable<TreeNode> GenerateTree(
            this IEnumerable<ITreeItem> collection,
            long? root_id = null)
        {
            foreach (var c in collection.Where(c => string.Equals(c.ParentId, root_id)))
            {
                var treeNode = TreeNode.FromTreeItem(c);
                treeNode.children = collection.GenerateTree(c.Id);
                yield return treeNode;
            }
        }
    }
}