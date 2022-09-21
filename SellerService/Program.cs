
using EventBus.Messages.Common;
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
builder.Services.AddMassTransit(config => {
    config.AddConsumer<BidDateConsumer>();
    config.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        cfg.ReceiveEndpoint(EventBusConstants.GetBidEndDateQueue, c =>
        {
            c.ConfigureConsumer<BidDateConsumer>(ctx);
        });
    });
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

//app.MapGet("/GetUsers", [Authorize]() =>
//{
//    return new[]
//    {
//        "John.Doe",
//        "Jane.Doe",
//        "Jewel.Doe",
//        "Jayden.Doe",
//    };
//}).WithName("GetUsers");



//app.MapGet("/RandomFail", () =>
//{
//    var randomValue = new Random().Next(0,2);
//    //if(randomValue == 1)
//    {
//        throw new HttpRequestException("Random Failure");
//    }

//    //return "SomeData";
//}).WithName("RandomFail");

//app.MapGet("/RandomTimeout", async () =>
//{
//    var randomValue = new Random().Next(0, 2);
//    //if (randomValue == 1)
//    {
//        await Task.Delay(10000);
//    }

//    //return "SomeData";
//}).WithName("RandomTimeout");

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
