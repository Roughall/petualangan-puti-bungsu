using UnityEngine;

public class BossCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform player; // Masukkan Player ke sini

    [Header("Settings")]
    public float yOffset = 1.5f; // Agar kamera agak naik dikit (tidak pas di kaki)
    
    // Kita kunci Z di -10 agar gambar tidak hilang
    private float zPosition = -10f; 

    void LateUpdate()
    {
        if (player != null)
        {
            // LOGIKA:
            // X = Ikut Player
            // Y = Ikut Player + Sedikit Offset ke atas
            // Z = Tetap diam di -10
            
            transform.position = new Vector3(player.position.x, player.position.y + yOffset, zPosition);
        }
    }
}