using UnityEngine;

public class BossCamera_stabil : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Follow")]
    public float smoothSpeed = 5f;

    [Header("Arena Boundaries")]
    public Transform leftBoundary;
    public Transform rightBoundary;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        Debug.Log("[BOSS CAMERA] Awake");
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("[BOSS CAMERA] Target belum terpasang.");
            return;
        }

        if (leftBoundary == null || rightBoundary == null)
        {
            Debug.LogWarning("[BOSS CAMERA] Boundary belum terpasang.");
            return;
        }

        float targetX = target.position.x;

        float halfWidth = cam.orthographicSize * cam.aspect;

        float minX = leftBoundary.position.x + halfWidth;
        float maxX = rightBoundary.position.x - halfWidth;

        float clampedX = Mathf.Clamp(targetX, minX, maxX);

        float newX = Mathf.Lerp(
            transform.position.x,
            clampedX,
            smoothSpeed * Time.deltaTime
        );

        transform.position = new Vector3(
            newX,
            transform.position.y,
            transform.position.z
        );
    }
}