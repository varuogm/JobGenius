using Newtonsoft.Json;
using lupsSupabaseApi.Models;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<Supabase.Client>(_ => 
new Supabase.Client(
    //Insert SupabaseURL below
   "SupabaseURL",
   //Insert SupabaseAPIKey below
    "SupabaseAPIKey",
    new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true,
    }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/jobdescriptions", async(
    Supabase.Client client) =>
    {
        var response = await client
            .From<JobDescription>()
            .Get();
        
        var jobdescriptionsString = response.Content;
        var jobdescriptions = JsonConvert.DeserializeObject<List<JobDescriptionObject>>(jobdescriptionsString);

        return Results.Ok(jobdescriptions);
    });

app.MapGet("/jobdescriptions/{id}", async(
    Guid id, 
    Supabase.Client client) =>
{
    var response = await client
        .From<JobDescription>()
        .Where(jd => jd.Id == id)
        .Get();
    
    var jobDescription = response.Models.FirstOrDefault();

    if (jobDescription is null)
    {
        return Results.NotFound();
    }

    var jobDescriptionResponse = new JobDescriptionResponse
    {
        Id = jobDescription.Id,
        Url = jobDescription.Url,
        JobText = jobDescription.JobText,
        CreatedAt = jobDescription.CreatedAt,
    };

    return Results.Ok(jobDescriptionResponse);
});

app.MapPost("/jobdescriptions", async(
    CreateJobDescriptionRequest request,
    Supabase.Client client) =>
{
    var jobDescription = new JobDescription
    {
        Url = request.Url,
        JobText = request.JobText,
    };
    
    var response = await client.From<JobDescription>().Insert(jobDescription);
    
    var newJobDescription = response.Models.First();

    return Results.Ok(newJobDescription.Id);
});

app.MapDelete("/jobdescriptions/{id}", async(
    Guid id, 
    Supabase.Client client) =>
{
    await client
        .From<JobDescription>()
        .Where(jd => jd.Id == id)
        .Delete();
    
    return Results.NoContent();
});

app.UseHttpsRedirection();

app.Run();
