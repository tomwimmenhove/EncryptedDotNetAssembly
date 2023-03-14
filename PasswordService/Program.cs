using PasswordService.BackingStore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

#if false
builder.Services.AddSingleton<IPasswordStorage, FilePasswordStorage>();
#else
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddSingleton<IPasswordStorage, DynamoPasswordStorage>();
#endif

builder.Services.AddSwaggerGen(c => 
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PasswordService API", Version = "v1" }));

var app = builder.Build();
//app.MapControllerRoute(name: "default", pattern: "{controller=PasswordStore}/{action=Index}/{id?}");
app.MapControllerRoute(name: "default", pattern: "{action=Index}/{id?}");

//if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Example API V1"));
}

app.Run();
