using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string slotName;       // Contoh: "Slot 1"
    public string sceneName;      // Contoh: "World_Village"
    public string saveTime;       // Waktu save
    
    // Posisi Player (Dipeceh jadi float agar aman di JSON)
    public float playerX;
    public float playerY;

    // Data Quest dari GameManager
    public List<string> completedQuests; 

    // Constructor kosong (Penting untuk JsonUtility)
    public SaveData() {}

    public SaveData(string _slotName, string _scene, UnityEngine.Vector2 _pos, List<string> _quests)
    {
        slotName = _slotName;
        sceneName = _scene;
        saveTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        
        playerX = _pos.x;
        playerY = _pos.y;
        
        completedQuests = _quests;
    }
}