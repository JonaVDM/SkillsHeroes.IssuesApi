using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SkillsHeroes.IssuesApi.Data.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Urgency
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
}
