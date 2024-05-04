using AccountServer.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using SharedDB;

var builder = WebApplication.CreateBuilder(args);

// �����ͺ��̽� ���ؽ�Ʈ ���񽺸� �����̳ʿ� �߰��մϴ�.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<SharedDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SharedConnection")));

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
});

var app = builder.Build();

// API ���� �ּ� ���
var httpProfile = builder.Configuration.GetSection("profiles").GetSection("http");
var httpsProfile = builder.Configuration.GetSection("profiles").GetSection("https");

if (httpProfile != null)
{
    var httpUrl = httpProfile.GetValue<string>("applicationUrl");
    Console.WriteLine($"HTTP ���� �ּ�: {httpUrl}");
}

if (httpsProfile != null)
{
    var httpsUrl = httpsProfile.GetValue<string>("applicationUrl");
    Console.WriteLine($"HTTPS ���� �ּ�: {httpsUrl}");
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
