

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Data.Entities.BaseEntity;

    [Serializable]
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
            CreateAt = DateTime.Now;
        }
        
        [Key] 
        public string Id { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime? UpdateAt { get; set; }
        [JsonIgnore]
        public string? UpdateBy { get; set; }
        
        public bool IsDeleted { get; set; } 
        [JsonIgnore]
        public DateTime? DeleteAt { get; set; }
        [JsonIgnore]
        public string? DeleteBy { get; set; }

    }
