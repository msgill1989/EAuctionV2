
using EventBus.Messages.Common;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SellerService.Data;
using SellerService.Data.Interfaces;
using SellerService.EventBusConsumer;
using SellerService.Models;
using SellerService.RepositoryLayer;
using SellerService.RepositoryLayer.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Aud"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddTransient<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<ISellerContext, SellerContext>();

//Masstransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BidDateForInsertBidConsumer>();
    config.AddConsumer<BidDateForUpdBidAmountConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ConfigureEndpoints(ctx);
        cfg.ReceiveEndpoint(EventBusConstants.GetBidEndDateforInsertBidQueue, c =>
        {
            c.ConfigureConsumer<BidDateForInsertBidConsumer>(ctx);
        });
        cfg.ReceiveEndpoint(EventBusConstants.GetBidEndDateforUpdBidAmountQueue, c =>
        {
            c.ConfigureConsumer<BidDateForUpdBidAmountConsumer>(ctx);
        });
    });
    config.AddRequestClient<GetBidDetailsRequestEvent>();
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
