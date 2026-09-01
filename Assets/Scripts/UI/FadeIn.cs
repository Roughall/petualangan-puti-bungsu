using System.Collections; // Wajib ada untuk IEnumerator
using UnityEngine;        // Wajib ada untuk MonoBehaviour dan fungsi Unity dasar
using UnityEngine.UI;     // Wajib ada jika berurusan dengan UI

public class FadeIn : MonoBehaviour // Nama Class HARUS SAMA dengan nama file (FadeIn)
{
    public CanvasGroup menuGroup;

    // Fungsi ini akan dipanggil otomatis saat objek aktif/muncul
    private void OnEnable()
    {
        // Memulai animasi fade in
        StartCoroutine(DoFadeIn());
    }

    // Saya ubah nama fungsinya jadi DoFadeIn agar lebih jelas
    IEnumerator DoFadeIn()
    {
        // Pastikan alpha mulai dari 0 (transparan)
        menuGroup.alpha = 0;

        // Loop selama alpha belum mencapai 1 (belum terlihat penuh)
        while (menuGroup.alpha < 1)
        {
            // Menambah nilai alpha perlahan
            // Menggunakan unscaledDeltaTime agar tetap jalan meski game di-Pause (TimeScale = 0)
            menuGroup.alpha += Time.unscaledDeltaTime * 3f;
            
            yield return null; // Tunggu satu frame sebelum lanjut loop
        }
        
        // Pastikan alpha tepat di angka 1 di akhir
        menuGroup.alpha = 1;
    }
}