using Postgrest.Attributes;
using Postgrest.Models;

namespace lupsSupabaseApi.Models;

[Table("JobDescriptions")]
public class JobDescription : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id {get; set;}
    [Column("url")]
    public String? Url {get; set;}
    [Column("job_text")]
    public String? JobText {get; set;}
    [Column("created_at")]
    public DateTime CreatedAt {get; set;}
}

