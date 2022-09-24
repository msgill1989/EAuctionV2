using BuyerService.RepositoryLayer.Interfaces;
using BuyerService.RepositoryLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BuyerService.Models;
using Microsoft.Extensions.Configuration;
using BuyerService.Data.Interfaces;
using BuyerService.Data;
using MassTransit;
using EventBus.Messages.Events;
using BuyerService.EventBusConsumer;
using EventBus.Messages.Common;
using Serilog;
using Common.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(Serilogger.Configure);
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

builder.Services.AddScoped<IBuyerContext, BuyerContext>();
builder.Services.AddTransient<IBuyerRepository, BuyerRepository>();

//Masstransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config => {
    config.AddConsumer<BidDetailsForSellerConsumer>();
    config.UsingRabbitMq((ctx, cfg) => {
    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    cfg.ConfigureEndpoints(ctx);
    cfg.ReceiveEndpoint(EventBusConstants.GetBidDetailsQueue, c =>
    {
        c.ConfigureConsumer<BidDetailsForSellerConsumer>(ctx);
    });
});
    config.AddRequestClient<GetBidDateRequestEvent>();
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
