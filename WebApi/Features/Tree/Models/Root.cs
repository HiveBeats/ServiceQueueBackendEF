using System.Collections.Generic;

namespace WebApi.Features.Tree.Models
{
    public class Root
    {
        public IEnumerable<TreeNode> root { get; set; }
    }
}