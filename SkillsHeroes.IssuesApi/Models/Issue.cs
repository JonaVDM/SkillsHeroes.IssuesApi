using System;
using System.Collections.Generic;

namespace SkillsHeroes.IssuesApi.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Data.Models.Urgency Urgency { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime? InProcess { get; set; }
        public DateTime? Completed { get; set; }

        public IEnumerable<Comment> Comments { get; set; }
    }
}
