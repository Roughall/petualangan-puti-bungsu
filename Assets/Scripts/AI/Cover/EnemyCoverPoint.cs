using UnityEngine;

public class EnemyCoverPoint : MonoBehaviour
{
    [Header("Cover")]
    public bool isActive = true;

    [Header("Cover Quality")]
    [Range(0f, 1f)]
    public float coverScore = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            0.25f
        );
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.up * 0.5f
        );

        Gizmos.DrawWireSphere(
            transform.position,
            0.25f
        );
    }
}