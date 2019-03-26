using Newtonsoft.Json;
using System;

namespace SkillsHeroes.IssuesApi.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        [JsonIgnore]
        public int IssueId { get; set; }
        [JsonIgnore]
        public Issue Issue { get; set; }
    }
}
