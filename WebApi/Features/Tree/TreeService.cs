using System.Collections.Generic;
using System.Linq;
using WebApi.Features.Tree.Models;

namespace WebApi.Features.Tree
{
    public class TreeService
    {
        public static Root GetTreeFromList(IEnumerable<ITreeItem> list)
        {
            return new Root()
            {
                root = list.GenerateTree().ToList()
            };
        }
    }
}