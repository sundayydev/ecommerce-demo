using System.Text;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Shop.Application.Common.Behaviors;
using Shop.Application.Common.Interfaces;
using Shop.Application.Features.Categories.Commands.CreateCategory;
using Shop.Domain.Constants;
using Shop.Infrastructure.Data;
using Shop.WebApi.Extensions;
using Shop.WebApi.Services;

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


builder.Services.AddValidatorsFromAssembly(typeof(CreateCategoryCommand).Assembly);
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

//Đọc cấu hình JwtSettings từ appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole(Roles.Admin));
    
    options.AddPolicy("RequireUserRole", policy => 
        policy.RequireRole(Roles.User));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors(static builder => 
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

await DatabaseSeeder.SeedAsync(app);

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

app.UseAuthentication(); 
app.UseAuthorization();

app.Run();
