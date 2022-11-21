using CLOEmergency;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(mvcOptions =>
{
    // Response.Body string formatter
    mvcOptions.InputFormatters.Add(new TextSingleValueFormatter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Emergency API",
        Description = "Emergency Employee Phone Book",
        TermsOfService = new Uri("https://www.clo3d.com/ko/legal/terms-of-service"),
        Contact = new OpenApiContact
        {
            Name = "CLO Contact Us",
            Url = new Uri("https://www.clo3d.com/ko/support/contactus")
        },
        License = new OpenApiLicense
        {
            Name = "CLO Plans",
            Url = new Uri("https://www.clo3d.com/ko/plans")
        }
    });
});



builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
