using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Variabel statis untuk menyimpan data yang mau di-load sementara
    public static SaveData pendingLoadData;

    public static void LoadFromSlot(int slotIndex)
    {
        SaveData data = SaveSystem.LoadGame(slotIndex);
        
        if (data != null)
        {
            pendingLoadData = data; // Simpan data di memori
            SceneManager.LoadScene(data.sceneName); // Pindah scene
        }
        else
        {
            Debug.Log("Slot kosong, tidak bisa load.");
        }
    }
}