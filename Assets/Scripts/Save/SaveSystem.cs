using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static string GetSavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_{slot:D2}.json");
    }

    public static bool Save(SaveData data, int slot)
    {
        try
        {
            data.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string json = JsonUtility.ToJson(data, prettyPrint: true);
            string path = GetSavePath(slot);

            File.WriteAllText(path, json);

            Debug.Log($"[SaveSystem] Сохранено в {path} ({json.Length} символов)");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Ошибка сохранения: {e.Message}");
            return false;
        }
    }

    public static SaveData Load(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] Файл не найден: {path}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log($"[SaveSystem] Загружено из {path}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Ошибка загрузки: {e.Message}");
            return null;
        }
    }

    public static bool Exists(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    public static void Delete(int slot)
    {
        string path = GetSavePath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] Удалено: {path}");
        }
    }
}