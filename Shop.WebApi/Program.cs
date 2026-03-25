using Asp.Versioning;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shop.Application.Common.Behaviors;
using Shop.Application.Common.Interfaces;
using Shop.Application.Features.Categories.Commands.CreateCategory;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
using Shop.WebApi.Extensions;
using Shop.WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

// Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IApplicationDbContext>(provider => 
    provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddCors();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddCustomHealthChecks(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateCategoryCommand).Assembly);    
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // Mặc định là v1.0
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); 
}).AddApiExplorer(options => 
{
    options.GroupNameFormat = "'v'V";
    
    options.SubstituteApiVersionInUrl = true; 
});

builder.Services.AddAuthentication()
    .AddJwtBearer();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors(static builder => 
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app.UseFileServer();

app.MapOpenApi();
app.MapScalarApiReference();
    
#if (!UseApiOnly)
app.Map("/", () => Results.Redirect("/scalar"));
#endif

app.MapEndpoints(typeof(Program).Assembly);

#if (UseApiOnly)
app.MapFallbackToFile("index.html");
#endif

app.Run();
