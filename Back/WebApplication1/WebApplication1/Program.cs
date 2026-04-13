using BackendApi.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.Json;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbOptions>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDB:ConnectionString")));

builder.Services.AddScoped<IMongoDatabase>(s =>
    s.GetRequiredService<IMongoClient>()
     .GetDatabase(builder.Configuration.GetValue<string>("MongoDB:Database")));

// Services
builder.Services.AddSingleton<MongoDbService>();

// --- CORS CONFIGURATION ---
// Updated to include both React and Angular origins
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:4200") // Added Angular port
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANT: app.UseCors must be called after UseRouting and BEFORE UseAuthorization
app.UseRouting();

app.UseCors(myAllowSpecificOrigins); 

app.UseAuthorization();

app.MapControllers();

// --- MASTER DATABASE AUTO-GENERATION & SEEDING SCRIPT ---
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var client = new MongoClient(config["MongoDB:ConnectionString"]);
    var db = client.GetDatabase(config["MongoDB:Database"]);

    var requiredCollections = new List<string> 
    { 
        "faculties", "Faculty", "counters", "Counters", "Students", 
        "StudentTbl", "Attendance", "StudentAt", "F_Suggestion", 
        "Admin", "AdminNotice", "Department" 
    };

    var existingCollections = db.ListCollectionNames().ToList();
    foreach (var collectionName in requiredCollections)
    {
        if (!existingCollections.Contains(collectionName))
        {
            db.CreateCollection(collectionName);
            Console.WriteLine($"✅ Created missing collection: {collectionName}");
        }
    }

    var countersCollection = db.GetCollection<BsonDocument>("Counters");
    if (countersCollection.CountDocuments(new BsonDocument()) == 0)
    {
        var initialCounters = new List<BsonDocument>
        {
            new BsonDocument { { "_id", "Sid" }, { "Seq", 0L } },
            new BsonDocument { { "_id", "facultyid" }, { "Seq", 0L } },
            new BsonDocument { { "_id", "suggestionid" }, { "Seq", 0L } }
        };
        countersCollection.InsertMany(initialCounters);
    }

    var adminCollection = db.GetCollection<BsonDocument>("Admin");
    if (adminCollection.CountDocuments(new BsonDocument()) == 0)
    {
        adminCollection.InsertOne(new BsonDocument
        {
            { "Aid", 1 },
            { "Username", "admin" },
            { "Password", "adminpassword" }
        });
    }
}

app.Run();