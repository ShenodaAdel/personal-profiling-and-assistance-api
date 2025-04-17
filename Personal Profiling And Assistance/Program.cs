using BusinessLogic.Extensions;
using BusinessLogic.Services.Auth;
using BusinessLogic.Services.Choice;
using BusinessLogic.Services.Emails.Dtos;
using BusinessLogic.Services.Emails;
using BusinessLogic.Services.Question;
using BusinessLogic.Services.QuestionChoice;
using BusinessLogic.Services.Test;
using BusinessLogic.Services.User;
using BusinessLogic.Services.UserTest;
using BusinessLogic.ServicesConfigrations;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using BusinessLogic.Services.TokenService;
using BusinessLogic.Services.OTP_Service;

var builder = WebApplication.CreateBuilder(args); 

// Add services to the container.

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.ApplicationContextConfigurator(connectionString);

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IUserTestService, UserTestService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IChoiceService, ChoiceService>();
builder.Services.AddScoped<IQuestionChoiceService, QuestionChoiceService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IOTPService, OTPService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular's local dev URL (or the deployed URL)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed((origin) => true) 
              .AllowCredentials(); // Optional, depending on your setup
    });
});

// Section for EmailSettingsDto
builder.Services.Configure<EmailSettingsDto>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailServices, EmailService>();









builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; 
    });
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders();
// Add services IdentityRole in program file.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomJwtAuth(builder.Configuration);


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp"); // Apply the CORS policy
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
