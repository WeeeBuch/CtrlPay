
using CtrlPay.DB;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;

namespace CtrlPay.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            DatabaseSettings databaseSettings = builder.Configuration.GetSection("Database").Get<DatabaseSettings>();
            builder.Services.AddLinqToDBContext<AppDataConnection>((provider, options) =>
            {
                options
                    .UseMySql($"Server={databaseSettings.ProviderIp};Port={databaseSettings.ProviderPort};Database={databaseSettings.DbName};Uid={databaseSettings.DbLogin};Pwd={databaseSettings.DbPassword};");
                    //.AddDefaultLogging(provider);                                    // loguje SQL do ILogger (volitelné)
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
