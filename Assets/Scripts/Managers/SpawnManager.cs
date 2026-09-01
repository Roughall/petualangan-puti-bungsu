using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private SpawnID nextSpawn = SpawnID.None;

    private void Awake()
{
    Debug.Log("SpawnManager Awake");
    Debug.Log(gameObject.name);
    Debug.Log(transform.root.name);
    Debug.Log(GetInstanceID());

    if (Instance == null)
    {
        Instance = this;

        DontDestroyOnLoad(transform.root.gameObject);
    }
    else
    {
        Debug.Log("SpawnManager Duplicate");

        Destroy(gameObject);
    }
}

    public void SetNextSpawn(SpawnID spawnID)
    {
        nextSpawn = spawnID;

        Debug.Log("[SpawnManager] Save Next Spawn = " + nextSpawn);
    }

    public SpawnID GetNextSpawn()
    {
        return nextSpawn;
    }

    public void ClearSpawn()
    {
        nextSpawn = SpawnID.None;
    }

    public SpawnPoint GetSpawnPoint(SpawnID id)
    {
        SpawnPoint[] points = FindObjectsOfType<SpawnPoint>();

        foreach (SpawnPoint point in points)
            {
                if (point.spawnID == id)
                return point;
            }

        return null;
    }

    public bool HasSpawn()
    {
        return nextSpawn != SpawnID.None;
    }

    public SpawnID debugSpawn = SpawnID.None;
    
}