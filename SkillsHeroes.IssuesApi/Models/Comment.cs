using System;

namespace SkillsHeroes.IssuesApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string Text { get; set; }
    }
}
