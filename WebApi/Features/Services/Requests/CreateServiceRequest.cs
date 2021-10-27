using System.Diagnostics.CodeAnalysis;

namespace WebApi.Features.Services.Requests
{
    public class CreateServiceRequest
    {
        [NotNull]
        public string Name { get; set; }
        public string ParentId { get; set; }
    }
}