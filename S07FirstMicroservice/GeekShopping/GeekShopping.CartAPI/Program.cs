using GeekShopping.CartAPI.Data.DTO;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", _ =>
    {
        _.Authority = "https://localhost:4435";
        _.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };

        _.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                (message, cert, chain, errors) => true
        };
    });

builder.Services.AddAuthorization(_ =>
{
    _.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "geek_shopping");
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(_ =>
{
    _.SwaggerDoc("v1", new OpenApiInfo { Title = "GeekShopping.CartAPI", Version = "v1" });
    _.EnableAnnotations();
    _.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Enter 'Bearer' [space] and your token!",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    _.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var connection = builder.Configuration["MySQLConnection:MySQLConnectionString"];
builder.Services.AddDbContext<MySqlContext>(options => options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 37))));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<ProductDTO, Product>().ReverseMap();
    cfg.CreateMap<CartHeaderDTO, CartHeader>().ReverseMap();
    cfg.CreateMap<CartDetailDTO, CartDetail>().ReverseMap();
    cfg.CreateMap<CartDTO, Cart>().ReverseMap();
});
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IRabbitMQMessageSender, RabbitMQMessageSender>();

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