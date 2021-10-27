using WebApi.Features.Tree.Models;
using WebApi.Models;

namespace WebApi.Features.Services.Responses
{
    public class ServiceDto: ITreeItem
    {
        public ServiceDto(Service source)
        {
            Id = source.Id;
            Name = source.Name;
            ParentId = source.ParentId;
        }
        
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }

        #region ITreeItem Members

        long ITreeItem.Id => Id;
        long? ITreeItem.ParentId => ParentId;
        string ITreeItem.Name => Name;
        string ITreeItem.Icon => "pi pi-fw pi-cog";


        #endregion
    }
}