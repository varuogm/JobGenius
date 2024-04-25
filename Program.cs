using Newtonsoft.Json;
using lupsSupabaseApi.Models;
using Supabase;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "gouravSupportedOrigins",
        builder => { builder.WithOrigins("http://localhost:5154"); });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string SupabaseURL = config["SupabaseURL"];
string SupabaseAPIKey = config["SupabaseAPIKey"];

builder.Services.AddScoped<Supabase.Client>(_ =>
new Supabase.Client(
  SupabaseURL, SupabaseAPIKey,
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
app.UseCors("gouravSupportedOrigins");

//API Routes
app.MapGet("/jobdescriptions", async (Supabase.Client client) =>
    {
        try
        {
            var response = await client.From<JobDescription>().Get();
            var jobsDescriptionContent = response.Content;
            var allJobsDescriptions = JsonConvert.DeserializeObject<List<JobDescriptionDTO>>(jobsDescriptionContent);

            return Results.Ok(allJobsDescriptions);
        }
        catch (System.Exception ex)
        {
            return Results.BadRequest("Error occured while fetching job descriptions" + ex.Message);
        }
    });

app.MapGet("/jobdescriptions/{id}", async (int id, Supabase.Client client) =>
{
    try
    {
        var response = await client.From<JobDescription>().Where(item => item.Id == id).Get();

        var jobDescription = response.Models.FirstOrDefault();

        if (jobDescription is null)
        {
            return Results.NotFound("Job description for the given id not found.");
        }

        var jobDescriptionResponse = new JobDescriptionResponse
        {
            Id = jobDescription.Id,
            Url = jobDescription.Url,
            JobText = jobDescription.JobText,
            CreatedAt = jobDescription.CreatedAt,
        };
        return Results.Ok(jobDescriptionResponse);
    }
    catch (Exception ex)
    {
        return Results.BadRequest("Error occured while fetching job description via job id " + ex.Message);
    }

});

app.MapPost("/jobdescriptions", async (CreateJobDescriptionRequest request, Supabase.Client client) =>
{
    var jobDescription = new JobDescription
    {
        Url = request.Url,
        JobText = request.JobText,
    };

    try
    {
        var response = await client.From<JobDescription>().Insert(jobDescription);
        var newJobDescription = response.Models.First();

        return Results.Ok("New Job descrption has been added and assigned Id with number :" + newJobDescription.Id);
    }
    catch (Exception e)
    {
        return Results.BadRequest("Could not create a new job listing" + e.Message);
    }

});

app.MapDelete("/jobdescriptions/{id}", async (int jobId, Supabase.Client client) =>
{
    try
    {
        await client.From<JobDescription>().Where(item => item.Id == jobId).Delete();
    }
    catch (Exception ex)
    {
        return Results.BadRequest("Could not delete the job listing" + ex.Message);
    }

    return Results.Ok("Id has been deleted");
});



app.UseHttpsRedirection();

app.Run();
