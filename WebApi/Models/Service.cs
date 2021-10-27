using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Service
    {
        public long Id { get; private set; }
        
        [Required]
        [MaxLength(256)]
        public string Name { get; private set; }
        
        public Service Parent { get; private set; }
        public long? ParentId { get; private set; }
        public ICollection<Service> Childs { get; } = new List<Service>();
        public ICollection<Topic> Topics { get; private set; }

        public static Service Create(string name, Service parent = null)
        {
            return new Service()
            {
                Name = name,
                Parent = parent
            };
        }
    }
}