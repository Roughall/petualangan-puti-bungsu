using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public SpawnID spawnID;

    public bool faceRight = true;

    public bool playSpawnAnimation = true;

    public string description;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position,0.25f);
    }
#endif


}