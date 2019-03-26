namespace SkillsHeroes.IssuesApi.Models
{
    public class AddIssue
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Data.Models.Urgency Urgency { get; set; }
    }
}
