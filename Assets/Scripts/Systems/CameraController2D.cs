using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.08f;
    private Vector3 velocity = Vector3.zero;

    private Camera cam;
    private Vector2 minBounds, maxBounds;
    private float halfHeight, halfWidth;

    // GANTI: Gunakan Awake() bukan Start() untuk inisialisasi komponen
    // Ini menjamin 'cam' sudah siap SEBELUM GameManager memanggilnya.
    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main; // Backup jika script tidak ditempel di kamera
    }

    void Start()
    {
        Debug.Log("[Camera] Target = " + target);
        FindPlayer();

        UpdateBoundsFromCurrentWorld();
    }

    public void UpdateBoundsFromCurrentWorld()
    {
        // PENGAMAN: Cek lagi jika cam masih null (untuk menghindari error baris 26)
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null) return; // Jika masih null, batalkan fungsi agar tidak crash

        if (GameManager.Instance != null && GameManager.Instance.currentWorld != null)
        {
            minBounds = GameManager.Instance.currentWorld.cameraMinBounds;
            maxBounds = GameManager.Instance.currentWorld.cameraMaxBounds;
        }

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    void LateUpdate()
{
    //------------------------------------
    // Cari Player bila belum ada
    //------------------------------------

    if (target == null)
    {
        FindPlayer();
        return;
    }

    //------------------------------------
    // Hitung posisi kamera yang diinginkan
    //------------------------------------

    Vector3 desiredPosition = target.position;

    desiredPosition.z = transform.position.z;

    //------------------------------------
    // Clamp berdasarkan WorldData
    //------------------------------------

    desiredPosition.x = Mathf.Clamp(
        desiredPosition.x,
        minBounds.x + halfWidth,
        maxBounds.x - halfWidth);

    desiredPosition.y = Mathf.Clamp(
        desiredPosition.y,
        minBounds.y + halfHeight,
        maxBounds.y - halfHeight);

    //------------------------------------
    // Smooth Follow
    //------------------------------------

    transform.position = Vector3.SmoothDamp(
        transform.position,
        desiredPosition,
        ref velocity,
        smoothTime);
}
    private void FindPlayer()
    {
        if (target != null)
        return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;

            Debug.Log("[Camera] Player ditemukan : " + target.name);
        }
    }
}