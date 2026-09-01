using UnityEngine;
using System.Collections.Generic; // PENTING: Untuk List
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Worlds")]
    public List<WorldData> worlds = new List<WorldData>();

    [HideInInspector] public WorldData currentWorld;

    // --- PERBAIKAN 1: Tambahkan variabel ini agar error CS0103 & CS1061 hilang ---
    public List<string> completedQuestIDs = new List<string>(); 
    // --------------------------------------------------------------------------

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
            return; 
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("===== WORLD LIST =====");

    foreach (var world in worlds)
    {
        if(world != null)
            Debug.Log(world.worldID + " -> " + world.sceneName);
    }

    Debug.Log("======================");

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        Debug.Log("===== SCENE LOADED =====");
        Debug.Log(scene.name);
        // Set currentWorld based on scene name
        foreach (var w in worlds)
        {
            if (w.sceneName == scene.name)
            {
                currentWorld = w;

                Debug.Log("Current World = " + currentWorld.worldID);
                Debug.Log("Scene = " + currentWorld.sceneName);
                Debug.Log("Spawn = " + currentWorld.defaultSpawn);

                break;
            }
        }

        // --- PERBAIKAN 2: Pastikan deklarasi 'player' hanya ada SATU kali ---
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // Cek apakah ada data load pending dari SceneLoader?
            // (Asumsi Anda pakai SceneLoader.pendingLoadData dari kodingan sebelumnya)
            if (SceneLoader.pendingLoadData != null)
            {
                // Prioritas 1: Load Game (Posisi dari Save File)
                Vector2 loadPos = new Vector2(SceneLoader.pendingLoadData.playerX, SceneLoader.pendingLoadData.playerY);
                player.transform.position = loadPos;
                
                // Reset pending data setelah dipakai
                SceneLoader.pendingLoadData = null; 
            }
            else if (SpawnManager.Instance != null &&
         SpawnManager.Instance.GetNextSpawn() != SpawnID.None)
{
    Debug.Log("Next Spawn = " + SpawnManager.Instance.GetNextSpawn());

    SpawnPoint point =
        SpawnManager.Instance.GetSpawnPoint(
            SpawnManager.Instance.GetNextSpawn());

            if(point == null)
            {
                Debug.LogError("SpawnPoint TIDAK ditemukan!");
            }
            else
            {
                Debug.Log("SpawnPoint ditemukan : " + point.spawnID);
                Debug.Log("Posisi : " + point.transform.position);
            }

    if (point == null)
    {
        Debug.LogError("SpawnPoint TIDAK ditemukan");
    }
    else
    {
        Debug.Log("Spawn ditemukan : "
            + point.spawnID
            + " Pos = "
            + point.transform.position);
        Debug.Log("Player sebelum = " + player.transform.position);

        player.transform.position = point.transform.position;

        Debug.Log("Player sesudah = " + player.transform.position);

        player.transform.position = point.transform.position;
    }

    SpawnManager.Instance.ClearSpawn();
}
            else if (PlayerSpawn.HasSpawn)
            {
                player.transform.position = PlayerSpawn.LastSpawnPoint;
            }
            else if (currentWorld != null)
            {
                // Prioritas 3: Spawn default world
                player.transform.position = currentWorld.defaultSpawn;
            }
        }

        // Update Camera Bounds
        var cam = Camera.main;
        if (cam != null)
        {
            var controller = cam.GetComponent<CameraController2D>();
            if (controller != null) controller.UpdateBoundsFromCurrentWorld();
        }

        // Play World Music
        if (currentWorld != null)
        {
            AudioManager.Instance.PlayWorld(currentWorld);
        }
    }
}