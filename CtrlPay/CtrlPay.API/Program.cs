
using CtrlPay.API.BackgroundServices;
using CtrlPay.DB;
using Microsoft.EntityFrameworkCore;
using CtrlPay.Entities;

namespace CtrlPay.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Logging.ClearProviders();      // smaže defaultní
            builder.Logging.AddConsole();          // pøidá console logger
            //builder.Logging.SetMinimumLevel(LogLevel.Information); // úroveò logù

            builder.Services.Configure<MoneroRpcOptions>(
                builder.Configuration.GetSection("MoneroRpc")
            );

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHostedService<XmrComsBackgroundService>();

            var dbSettings = builder.Configuration
                .GetSection("Database")
                .Get<DatabaseSettings>();

            ValidateDbSettings(dbSettings);

            // inicializace DbContextu
            CtrlPayDbContext.Initialize(dbSettings);

            builder.Services.AddDbContext<CtrlPayDbContext>();


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
        }

        public static void ValidateDbSettings(DatabaseSettings? dbSettings)
        {
            string[] allowedDbTypes = ["mysql"];
            if (string.IsNullOrWhiteSpace(dbSettings.Type) || !allowedDbTypes.Contains(dbSettings.Type.ToLower()))
                throw new Exception("Database Type missing or invalid");
            if (string.IsNullOrWhiteSpace(dbSettings.ProviderIp))
                throw new Exception("Database ProviderIp missing");
            if (string.IsNullOrWhiteSpace(dbSettings.ProviderPort))
                throw new Exception("Database ProviderPort missing");
            if (string.IsNullOrWhiteSpace(dbSettings.DbName))
                throw new Exception("Database DbName missing");
            if (string.IsNullOrWhiteSpace(dbSettings.DbLogin))
                throw new Exception("Database DbLogin missing");
            if (string.IsNullOrWhiteSpace(dbSettings.DbPassword))
                throw new Exception("Database DbPassword missing");
        }
    }
}
