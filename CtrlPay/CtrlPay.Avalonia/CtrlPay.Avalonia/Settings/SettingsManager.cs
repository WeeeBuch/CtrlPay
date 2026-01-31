using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.Settings
{
    internal class SettingsManager
    {
        private static readonly string fileName = "settings.json";
        private static readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string folder = Path.Combine(appData, "CtrlPay");
        private static string _filePath = Path.Combine(folder, fileName);
        private static bool NeedsOnBoarding = true;

        public static AppSettings Current { get; set; }

        public static bool Init()
        {
            Current = Load(); // Načte se automaticky při prvním přístupu
            return NeedsOnBoarding;
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                // 1. Zjistíme, v jaké složce má soubor být
                string? directory = Path.GetDirectoryName(_filePath);

                // 2. Pokud cesta ke složce není prázdná a složka neexistuje, vytvoříme ji
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 3. Teď už můžeme bezpečně ukládat
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                // Tady je dobré mít aspoň výpis do konzole, abys věděl, co se děje
                System.Diagnostics.Debug.WriteLine($"Chyba při ukládání nastavení: {ex.Message}");
            }
        }

        public static AppSettings Load()
        {
            if (!File.Exists(_filePath)) return new AppSettings(); // Vratí default

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json)) return new AppSettings();

            AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(json);

            if (settings == null) return new AppSettings();

            NeedsOnBoarding = false;
            return settings;
        }
    }
}
