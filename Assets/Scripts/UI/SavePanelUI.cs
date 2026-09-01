using UnityEngine;
using System.Collections.Generic;

public class SavePanelUI : MonoBehaviour
{
    public static SavePanelUI Instance;
    
    // Array untuk menampung slot UI agar bisa di-refresh
    public SaveSlotUI[] saveSlots; 

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // Sembunyi saat mulai
    }

    public void Open()
    {
        Time.timeScale = 0; // Pause Game
        gameObject.SetActive(true);
        RefreshAllSlots(); // Update tampilan slot saat panel dibuka
    }

    public void Close()
    {
        Time.timeScale = 1; // Resume Game
        gameObject.SetActive(false);
    }

    public void RefreshAllSlots()
    {
        foreach (var slot in saveSlots)
        {
            slot.Refresh();
        }
    }

    public void SaveToSlot(int slotIndex)
    {
        // Ambil Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Ambil Data Quest
        List<string> quests = new List<string>();
        if (GameManager.Instance != null) quests = GameManager.Instance.completedQuestIDs;

        // Bungkus Data
        SaveData data = new SaveData(
            "Slot " + slotIndex,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            player.transform.position,
            quests
        );

        // Simpan
        SaveSystem.SaveGame(data, slotIndex);
        
        // Refresh UI supaya terlihat update-nya
        RefreshAllSlots();
        
        Debug.Log("Game Saved!");
        // Close(); // Opsional: Mau langsung tutup atau biarkan player lihat hasil save?
    }
}