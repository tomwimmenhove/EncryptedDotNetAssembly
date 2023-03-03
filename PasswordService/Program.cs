using PasswordService.BackingStore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IPasswordStorage, FilePasswordStorage>();
builder.Services.AddSwaggerGen(c => 
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Example API", Version = "v1" }));

var app = builder.Build();
app.MapControllerRoute(name: "default", pattern: "{controller=PasswordStore}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Example API V1"));
}

app.Run();
