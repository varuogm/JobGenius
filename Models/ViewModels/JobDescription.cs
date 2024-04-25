using Postgrest.Attributes;
using Postgrest.Models;

namespace lupsSupabaseApi.Models;

[Table("JobDescriptions")]
public class JobDescription : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    [Column("url")]
    public string? Url { get; set; }
    [Column("job_text")]
    public string? JobText { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

