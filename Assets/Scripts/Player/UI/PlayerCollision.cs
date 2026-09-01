using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameOverManager gameOverManager;

    private void Start()
    {
        gameOverManager = FindObjectOfType<GameOverManager>();

    if (gameOverManager == null)
    {
        Debug.LogWarning("GameOverManager tidak ada pada scene ini.");
    }
    else
    {
        Debug.Log("GameOverManager ditemukan.");
    }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Cek apakah tabrakan terdeteksi sama sekali
        Debug.Log("Player menabrak objek bernama: " + collision.gameObject.name);
        
        // 2. Cek apakah Tag objek tersebut dibaca
        Debug.Log("Tag objek tersebut adalah: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Tag Sesuai! Memanggil Game Over...");
            gameOverManager.TriggerGameOver();
        }
        else
        {
            Debug.Log("Tabrakan terjadi, tapi Tag bukan 'Enemy'.");
        }
    }
}