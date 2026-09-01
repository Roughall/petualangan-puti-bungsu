using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnDatabase",menuName = "Game Data/Spawn Database")]
public class SpawnDatabase : ScriptableObject
{
    [Header("All Spawn Points")]
    public List<SpawnData> spawns = new List<SpawnData>();

    public SpawnData GetSpawn(SpawnID id)
    {
        foreach (SpawnData spawn in spawns)
        {
            if (spawn.spawnID == id)
                return spawn;
        }

        Debug.LogWarning("Spawn tidak ditemukan : " + id);
        return null;
    }

    public bool Contains(SpawnID id)
    {
        return GetSpawn(id) != null;
    }
}