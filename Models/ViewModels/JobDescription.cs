using Postgrest.Attributes;
using Postgrest.Models;

namespace JobGeniusApi.Models;

[Table("JobDescriptions")]
public class JobDescription : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    [Column("jobUrl")]
    public string? jobUrl { get; set; }
    [Column("company")]
    public string? company { get; set; }
    [Column("creationTime")]
    public DateTime creationTime { get; set; }
}

