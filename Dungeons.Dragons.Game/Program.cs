using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Services.AddControllers()
       .AddNewtonsoftJson(options =>
       {
           options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
           options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; // optional
       });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
var assembly = Assembly.GetExecutingAssembly(); // current assembly

var serviceTypes = assembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract)
    .Select(t => new
    {
        Implementation = t,
        Interface = t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name)
    })
    .Where(x => x.Interface != null);

foreach (var type in serviceTypes)
{
    builder.Services.AddSingleton(type.Interface, type.Implementation);
}

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
