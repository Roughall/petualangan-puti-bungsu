using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [Tooltip("Target world (WorldData asset)")]
    public WorldData targetWorld;

    [Tooltip("Optional offset from world spawn")]
    public Vector2 offset = Vector2.zero;

    [Tooltip("Play fade transition?")]
    public bool useFade = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // set spawn for next scene
        if (targetWorld != null)
        {
            Vector2 spawn = targetWorld.defaultSpawn + offset;
            PlayerSpawn.SetSpawn(spawn);

            if (useFade)
                SceneTransition.Instance.LoadScene(targetWorld.sceneName);
            else
                SceneManager.LoadScene(targetWorld.sceneName);
        }
        else
        {
            Debug.LogWarning("Teleporter has no targetWorld assigned.");
        }
    }
}
