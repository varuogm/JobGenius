public class JobDescriptionResponse
{
    public int Id { get; set; }
    public string? jobUrl { get; set; }

    public string? company { get; set; }

    public string? comment { get; set; }

    public DateTime creationTime { get; set; }
}