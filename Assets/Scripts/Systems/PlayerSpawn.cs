using UnityEngine;

public static class PlayerSpawn
{
    public static bool HasSpawn { get; private set; } = false;
    public static Vector2 LastSpawnPoint { get; private set; } = Vector2.zero;

    public static void SetSpawn(Vector2 pos)
    {
        LastSpawnPoint = pos;
        HasSpawn = true;
    }

    public static void Clear() { HasSpawn = false; LastSpawnPoint = Vector2.zero; }
}
