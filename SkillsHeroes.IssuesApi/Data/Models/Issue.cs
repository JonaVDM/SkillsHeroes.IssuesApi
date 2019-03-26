using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SkillsHeroes.IssuesApi.Data.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Urgency Urgency { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime? InProcess { get; set; }
        public DateTime? Completed { get; set; }

        public ICollection<Comment> Comments { get; set; }

        [JsonIgnore]
        public int ApplicationId { get; set; }
        [JsonIgnore]
        public Application Application { get; set; }
    }
}
