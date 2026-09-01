using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance;

    [Header("Database")]
    public PortalDatabase portalDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnterPortal(PortalID portalID)
{
    PortalData portal = portalDatabase.GetPortal(portalID);

    if (portal == null)
    {
        Debug.LogError("Portal tidak ditemukan : " + portalID);
        return;
    }

        Debug.Log("==================================");
        Debug.Log("ENTER PORTAL");
        Debug.Log("Portal ID : " + portal.portalID);
        Debug.Log("Destination World : " + portal.destinationWorld);
        Debug.Log("Destination Spawn : " + portal.destinationSpawn);
        Debug.Log("==================================");

    // simpan spawn tujuan
    SpawnManager.Instance.SetNextSpawn(portal.destinationSpawn);

    // cari world tujuan
    WorldData world = GameManager.Instance.worlds.Find(
        w => w.worldID == portal.destinationWorld);

    if (world == null)
    {
        Debug.LogError("World tidak ditemukan : " + portal.destinationWorld);
        return;
    }

    Debug.Log("[Portal] Loading Scene : " + world.sceneName);

    SceneTransition.Instance.LoadScene(world.sceneName);
}

   
}