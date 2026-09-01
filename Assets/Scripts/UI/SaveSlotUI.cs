using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public int slotIndex; // Set di Inspector: 1, 2, atau 3
    public Text slotText;

    public void Refresh()
    {
        if (SaveSystem.SlotExists(slotIndex))
        {
            SaveData data = SaveSystem.LoadGame(slotIndex);
            slotText.text = $"SLOT {slotIndex}\n{data.sceneName}\n{data.saveTime}";
        }
        else
        {
            slotText.text = $"SLOT {slotIndex}\n[ EMPTY ]";
        }
    }

    // Fungsi ini dipanggil dari Button OnClick di Inspector
    public void OnClickSlot()
    {
        // Cek konteks: Apakah kita sedang di Menu Save (Ingame) atau Menu Load (Main Menu)?
        // Untuk sekarang kita asumsikan ini tombol SAVE di dalam game
        SavePanelUI.Instance.SaveToSlot(slotIndex);
    }
}