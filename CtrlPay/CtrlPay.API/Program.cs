
using CtrlPay.API.BackgroundServices;
using CtrlPay.DB;
using CtrlPay.Entities;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace CtrlPay.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Logging.ClearProviders();      // smaže defaultní
            builder.Logging.AddConsole();          // přidá console logger
            //builder.Logging.SetMinimumLevel(LogLevel.Information); // úroveň logů

            builder.Services.Configure<MoneroRpcOptions>(
                builder.Configuration.GetSection("MoneroRpc")
            );

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHostedService<XmrComsBackgroundService>();


            // Načtení JWT sekce z configu
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtOptions = jwtSection.Get<JwtOptions>();

            if (string.IsNullOrWhiteSpace(jwtOptions.PrivateKeyPem))
            {
                using var rsa = RSA.Create(2048);
                var privateKeyPem = rsa.ExportRSAPrivateKeyPem();

                var file = "appsettings.json";

                // 1️ Načteme JSON jako Dictionary
                var json = File.ReadAllText(file);
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();

                // 2️ Zajistíme, že sekce existuje
                if (!dict.ContainsKey("Jwt") || dict["Jwt"] is not JsonElement)
                {
                    dict["Jwt"] = new Dictionary<string, object>();
                }

                // 3️ Deserializujeme sekci na Dictionary
                var sectionDict = JsonSerializer.Deserialize<Dictionary<string, object>>(dict["Jwt"].ToString()!) ?? new();

                // 4️ Nastavíme klíč
                sectionDict["PrivateKeyPem"] = privateKeyPem;

                // 5️ Aktualizujeme sekci
                dict["Jwt"] = sectionDict;

                // 6️ Zapíšeme zpět
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(file, JsonSerializer.Serialize(dict, options));
                Console.WriteLine("Vygenerován nový RSA privátní klíč a uložen do appsettings.json");
            }

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddSingleton<TokenService>();

            DatabaseSettings? dbSettings = builder.Configuration
                .GetSection("Database")
                .Get<DatabaseSettings>();

            ValidateDbSettings(dbSettings);

            // inicializace DbContextu
            CtrlPayDbContext.Initialize(dbSettings);

            builder.Services.AddDbContext<CtrlPayDbContext>();

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

                    var rsa = RSA.Create();
                    rsa.ImportFromPem(jwtOptions.PrivateKeyPem.ToCharArray());

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.RsaSecurityKey(rsa),
                    };
                    // Přidáme událost, která se spustí po validaci tokenu
                    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var claims = context.Principal?.Claims;
                            var isTotpClaim = claims?.FirstOrDefault(c => c.Type == "IsTotp")?.Value;

                            if (isTotpClaim != null && bool.Parse(isTotpClaim))
                            {
                                // Token je jen TOTP – zrušíme autentizaci
                                context.Fail("TOTP token cannot be used for this endpoint.");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthentication();
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
