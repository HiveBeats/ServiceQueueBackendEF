using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.Features.Scripting.Requests
{
    public class UpdateScriptSettingsRequest
    {
        [Required]
        public bool IsEnabled { get; set; }
        [Required]
        public LogLevel LogLevel { get; set; }
        [Required]
        public int Priority { get; set; }
    }
    
    public class UpdateScriptBodyRequest
    {
        [Required]
        public string Body { get; set; }
    }
}