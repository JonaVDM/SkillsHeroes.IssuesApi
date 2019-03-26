using System.Collections.Generic;

namespace SkillsHeroes.IssuesApi.Data.Models
{
    public class Application
    {
        public int Id { get; set; }

        public string ApiKey { get; set; }

        public ICollection<Issue> Issues { get; set; }
    }
}
