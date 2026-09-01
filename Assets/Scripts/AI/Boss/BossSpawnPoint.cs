using UnityEngine;

public class BossSpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public string bossID = "BOSS_VILLAGE";

    private void Awake()
    {
        Debug.Log(
            "[BOSS SPAWN POINT READY] " +
            bossID +
            " | Position = " +
            transform.position
        );
    }
}