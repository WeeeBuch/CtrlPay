using CtrlPay.Repos.Frontend;
using System;
using System.IO;
using System.Text.Json;

namespace CtrlPay.Avalonia.Settings;

internal class SettingsManager
{
    private static readonly string fileName = "settings.json";
    private static readonly string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CtrlPay");
    private static readonly string _filePath = Path.Combine(folder, fileName);

    private static bool NeedsOnBoarding = true;
    public static AppSettings Current { get; set; } = new(); // Defaultní inicializace

    public static bool Init()
    {
        AppLogger.Info("Settings Init started.");
        Current = Load();
        return NeedsOnBoarding;
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            // 1. Validace složky (zkráceno)
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // 2. Serializace (Objekt -> String)
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });

            // 3. Kontrola, jestli se data vůbec změnila (optimization)
            // Pokud soubor existuje a obsah je stejný, nic neukládáme a nelogujeme spam
            if (File.Exists(_filePath) && File.ReadAllText(_filePath) == json)
            {
                return;
            }

            AppLogger.Info("Settings change detected, saving to disk.");
            File.WriteAllText(_filePath, json);
            AppLogger.Info("Settings successfully serialized and saved.");
        }
        catch (Exception ex)
        {
            AppLogger.Error("Settings saving failed.", ex);
        }
    }

    public static AppSettings Load()
    {
        AppLogger.Info("Loading settings from file...");

        if (!File.Exists(_filePath))
        {
            AppLogger.Warning("Settings file not found. User is new.");
            return new AppSettings();
        }

        try
        {
            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                AppLogger.Warning("Settings file is empty.");
                return new AppSettings();
            }

            // Deserializace (String -> Objekt)
            AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(json);

            if (settings == null)
            {
                AppLogger.Error("Settings deserialization returned NULL.");
                return new AppSettings();
            }

            AppLogger.Info("Settings loaded successfully.");
            NeedsOnBoarding = false;
            return settings;
        }
        catch (Exception ex)
        {
            AppLogger.Error("Error while loading settings.", ex);
            return new AppSettings();
        }
    }
}