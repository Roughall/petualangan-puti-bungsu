using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldDatabase", menuName = "Game Data/World Database")]
public class WorldDatabase : ScriptableObject
{
    [Header("All Worlds")]
    public List<WorldData> worlds = new List<WorldData>();

    public WorldData GetWorld(WorldID id)
    {
        return worlds.Find(world => world.worldID == id);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < worlds.Count; i++)
        {
            if (worlds[i] == null)
            {
                Debug.LogWarning($"WorldDatabase : Element {i} masih kosong.");
            }
        }
    }
#endif
}