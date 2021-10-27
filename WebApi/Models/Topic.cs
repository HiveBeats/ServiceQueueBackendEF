using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Topic
    {
        public long Id { get; private set; }
        
        [Required]
        [MaxLength(128)]
        public string Name { get; private set; }
        public long ServiceId { get; private set; }
        public Service Service { get; private  set; }
        
        [Required]
        public bool SolveByReading { get; private set; }

        public ICollection<Message> Messages { get; private set; }

        public static Topic Create(string name, Service service, bool solveByReading)
        {
            return new Topic()
            {
                Name = name,
                Service = service,
                SolveByReading = solveByReading
            };
        }
    }
}