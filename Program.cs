using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using PostSQLgreAPI.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
 options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
//var account = new Account(
//    cloudinarySettings["CloudName"],
//    cloudinarySettings["ApiKey"],
//    cloudinarySettings["ApiSecret"]
//);


var account = new Account(
    cloud: Environment.GetEnvironmentVariable("CloudName"),
    apiKey: Environment.GetEnvironmentVariable("ApiKey"),
    apiSecret: Environment.GetEnvironmentVariable("ApiSecret")
);

builder.Services.AddSingleton(new Cloudinary(account));

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
