using Supabase;
using Newtonsoft.Json;
using JobGeniusApi.Models;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "gouravSupportedOrigins",
        builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfiguration>(config);

// Add the following code after the line builder.Services.AddScoped<Supabase.Client>(_ => ...
var SupabaseURL = config.GetConnectionString("SupabaseURL");
var SupabaseAPIKey = config.GetConnectionString("SupabaseAPIKey");


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
app.MapGet("/jobs", async (Supabase.Client client) =>
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

app.MapGet("/job/{id}", async (int id, Supabase.Client client) =>
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
            jobUrl = jobDescription.jobUrl,
            company = jobDescription.company,
            creationTime = jobDescription.creationTime,
            comment = jobDescription.comment
        };
        return Results.Ok(jobDescriptionResponse);
    }
    catch (Exception ex)
    {
        return Results.BadRequest("Error occured while fetching job description via job id " + ex.Message);
    }

});

app.MapPost("/add", async (CreateJobDescriptionRequest request, Supabase.Client client) =>
{
    var jobDescription = new JobDescription
    {
        jobUrl = request.jobUrl,
        company = request.company,
        comment = request.comment,
        creationTime = DateTime.Now
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

app.MapDelete("/delete/{id}", async (int id, Supabase.Client client) =>
{
    try
    {
        await client.From<JobDescription>().Where(item => item.Id == id).Delete();
    }
    catch (Exception ex)
    {
        return Results.BadRequest("Could not delete the job listing" + ex.Message);
    }

    return Results.Ok($"Id- {id} has been deleted");
});

app.UseHttpsRedirection();

app.Run();
