using UnityEngine;

public class RecoveryPoint : MonoBehaviour
{
    [Header("Recovery Position")]
    public Transform hidePoint;
    public Transform exitPoint;

    [Header("Recovery")]
    public bool isActive = true;

    private void OnDrawGizmos()
    {
        if (hidePoint != null)
        {
            Gizmos.DrawWireSphere(
                hidePoint.position,
                0.2f
            );
        }

        if (exitPoint != null)
        {
            Gizmos.DrawWireSphere(
                exitPoint.position,
                0.2f
            );
        }
    }
}