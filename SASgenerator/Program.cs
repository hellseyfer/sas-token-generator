using Microsoft.Extensions.Azure;
using Azure.Identity;
using System;
using Azure.Storage.Blobs;
using Azure.Storage;
using SASgenerator.Services;
namespace SASgenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Configuration.AddJsonFile($"appsettings.json", optional: true).AddEnvironmentVariables();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var storageAccountName = configuration.GetValue<string>("ACCOUNT_STORAGE_NAME");
                var storageAccountUri = $"https://{storageAccountName}.blob.core.windows.net";

            return new BlobServiceClient(
                new Uri(storageAccountUri), new DefaultAzureCredential());
            });

            builder.Services.AddSingleton<StorageService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
