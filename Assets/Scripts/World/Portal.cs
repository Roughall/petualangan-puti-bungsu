using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Portal : MonoBehaviour
{
    [Header("Portal")]
    public PortalID portalID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PortalManager.Instance.EnterPortal(portalID);
    }
}