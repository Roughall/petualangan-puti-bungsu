using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public SavePanelUI savePanel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Lapor ke Console siapa yang nabrak
        Debug.Log("ADA YANG NABRAK! Nama objek: " + other.name + " | Tag objek: " + other.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Ini Player! Membuka menu...");
            if(savePanel != null) savePanel.Open();
        }
        else
        {
            Debug.LogError("❌ BUKAN Player! Ganti Tag objek '" + other.name + "' menjadi 'Player' di Inspector!");
        }
    }
}