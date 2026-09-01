using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // Menggunakan persistentDataPath agar aman di Windows/Android/Mac
    private static string savePath => Application.persistentDataPath + "/saves/";

    public static void SaveGame(SaveData data, int slotIndex)
    {
        // Pastikan folder ada
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath + $"slot_{slotIndex}.json", json);
        
        Debug.Log($"Game Saved to Slot {slotIndex} at {savePath}");
    }

    public static SaveData LoadGame(int slotIndex)
    {
        string file = savePath + $"slot_{slotIndex}.json";

        if (!File.Exists(file))
        {
            Debug.LogWarning($"Save file not found: {file}");
            return null;
        }

        string json = File.ReadAllText(file);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool SlotExists(int slotIndex)
    {
        return File.Exists(savePath + $"slot_{slotIndex}.json");
    }
}