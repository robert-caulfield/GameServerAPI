using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using GameServerAPI;
using GameServerAPI.BackgroundServices;
using GameServerAPI.Configuration;
using GameServerAPI.Data;
using GameServerAPI.Models;
using GameServerAPI.Models.Token;
using GameServerAPI.Models.TokenObjects;
using GameServerAPI.Repository;
using GameServerAPI.Repository.IRepository;
using GameServerAPI.Services;
using GameServerAPI.Services.Auth;
using GameServerAPI.Services.Auth.IServices;
using GameServerAPI.Services.Token;
using GameServerAPI.Services.Token.IServices;
using GameServerAPI.SwaggerExamples;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using GameServerAPI.Models.API;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>();

// Set Identity config options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    options.User.RequireUniqueEmail = true;
});

// Bind GameServerManager settings
builder.Services.Configure<GameServerManagerSettings>(builder.Configuration.GetSection("GameServerManagerSettings"));

// Make game server memory store a singleton
builder.Services.AddSingleton<GameServerMemoryStore>();



// Start game server cleanup service
builder.Services.AddHostedService<GameServerCleanupService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameServerRepository, GameServerRepository>();
builder.Services.AddScoped<GameServerManager>();

// Register and provide ITokenService<PlayerJoinToken> with secret key
// This service is responsible for creating and validating player join tokens
builder.Services.AddScoped<ITokenService<PlayerJoinToken>>(p =>
{
    var secretKey = builder.Configuration["ApiSettings:PlayerJoinTokenSecret"];
    return new PlayerJoinTokenService(secretKey);
});

// Register and provide ITokenService<AuthToken> with secret key
// This service is responsible for creating and validating user authentication tokens
builder.Services.AddScoped<ITokenService<AuthToken>>(p =>
{
    var secretKey = builder.Configuration["ApiSettings:Secret"];
    return new AuthTokenService(secretKey);
});

// Add auto mapper config
builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddControllers(option =>
{
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters()
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values.SelectMany(v => v.Errors)
        .Select(v => v.ErrorMessage).ToList();
        var apiResponse = new APIResponse
        {
            StatusCode = System.Net.HttpStatusCode.BadRequest,
            IsSuccess = false,
            Errors = errors
        };
        return new BadRequestObjectResult(apiResponse)
        {
            ContentTypes = {"application/json"}
        };
    };
});

// Grab auth secret from appsettings.json
var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
// Add Authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    
    options.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Game Server API",
        Version = "v1",
        Description = "An API that facilitates a secure multiplayer gaming environment. Allows for " +
        "game server management, authentication of players, and access to game server information."
    });
    // Create example responses
    options.ExampleFilters();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);


    // Swagger gen for both authentication and server key
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme \r\n\r\n" +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "bearer",
                Name = "Authorization",
                In = ParameterLocation.Header
            },
            new List<String>
            {

            }

        }
        
    });
});


// Add swagger examples
builder.Services.AddSwaggerExamplesFromAssemblyOf<BadRequestExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ServerErrorExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ForbiddenExample>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
