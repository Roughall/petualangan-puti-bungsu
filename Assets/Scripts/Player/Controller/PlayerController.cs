using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 3.5f;

    // Komponen
    private Rigidbody2D rb;
    private Animator animator;        // BARU: Untuk Animasi
    private SpriteRenderer spriteRenderer; // BARU: Untuk Flip Gambar
    private PlayerAnimation playerAnimation;
    private PlayerDirection playerDirection;
    private Vector2 moveInput;

    void Awake()
    {
        Debug.Log("===== PLAYER AWAKE =====");
        Transform[] childs = GetComponentsInChildren<Transform>(true);

        Debug.Log("===== CHILD LIST =====");

        foreach (Transform t in childs)
        {
            Debug.Log(t.name + " Active = " + t.gameObject.activeSelf);
        }
        Debug.Log(gameObject.name);
        Debug.Log(gameObject.GetInstanceID());
        rb = GetComponent<Rigidbody2D>();

        Animator[] animators = GetComponentsInChildren<Animator>(true);

        Debug.Log("========== Animator List ==========");

        foreach (Animator a in animators)
        {
            Debug.Log(a.gameObject.name);
        }

        animator = GetComponentInChildren<Animator>();

        Debug.Log("Animator Dipakai = " + animator.gameObject.name);
        
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerAnimation = GetComponentInChildren<PlayerAnimation>(true);
        Debug.Log("PlayerAnimation Variable = " + playerAnimation);
        playerDirection = GetComponent<PlayerDirection>();
        PlayerAnimation[] animations =GetComponentsInChildren<PlayerAnimation>(true);

        Debug.Log("PlayerAnimation Count = " + animations.Length);

        foreach (PlayerAnimation pa in animations)
        {
            Debug.Log("PlayerAnimation di : " + pa.gameObject.name);
        }

        if (playerAnimation == null)
        {
            Debug.LogError("PlayerAnimation TIDAK ditemukan!");
        }
            else
            {
                Debug.Log("PlayerAnimation ditemukan : " + playerAnimation.name);
            }

        if(playerAnimation != null)
        {
            Debug.Log("========================");
            Debug.Log(animator);
            Debug.Log(animator.runtimeAnimatorController);
            Debug.Log("========================");
            playerAnimation.Initialize(animator, spriteRenderer);
        }

        Debug.Log("Animator : " + animator);
        Debug.Log("SpriteRenderer : " + spriteRenderer);

        if (playerDirection == null)
        {
            Debug.LogError("PlayerDirection TIDAK ditemukan!");
        }
        else
        {
            Debug.Log("PlayerDirection ditemukan : " + playerDirection.name);
        }
    }

   void Update()
{
    float h = Input.GetAxisRaw("Horizontal");
    float v = Input.GetAxisRaw("Vertical");

    if (h != 0 || v != 0)
    {
        Debug.Log("INPUT TERBACA -> H = " + h + "  V = " + v);
    }

    moveInput = new Vector2(h, v).normalized;//--------------------------------

    if (playerDirection != null)
    {
        playerDirection.SetDirection(moveInput);
    }

    //--------------------------------
    // PLAYER ANIMATION
    //----------------------------
    if (playerAnimation != null)
    {
        Debug.Log("PlayerController Kirim = " + moveInput + " Frame = " + Time.frameCount);
        playerAnimation.UpdateAnimation(moveInput);
    }
}

    void FixedUpdate()
    {
        // 3. Fisika Gerakan
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

   // GANTI TOTAL FUNGSI INI
    void AnimatePlayer()
    {
        // --- A. LOGIKA FLIP (Tetap sama) ---
        if (moveInput.x > 0) 
        {
            spriteRenderer.flipX = true; // Kanan -> Flip (karena aset asli Kiri)
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = false; // Kiri -> Normal
        }
        
        // --- B. LOGIKA GERAK VS DIAM (Perbaikan) ---

        // 1. Tentukan apakah ada input gerakan saat ini?
        // (sqrMagnitude > 0 artinya ada tombol panah yang ditekan)
        bool isMovingNow = moveInput.sqrMagnitude > 0.01f;

        // 2. Kirim status ini ke Animator SETIAP FRAME.
        // Ini WAJIB agar Animator tahu kapan harus transisi ke Idle.
        animator.SetBool("IsMoving", isMovingNow);

        // 3. Trik "Memory Arah":
        // HANYA update nilai InputX/Y ke animator JIKA sedang bergerak.
        // Jika pemain melepas tombol (isMovingNow = false), blok 'if' ini dilewati.
        // Akibatnya, Animator akan tetap memegang nilai X/Y terakhir sebelum berhenti.
        if (isMovingNow)
        {
            animator.SetFloat("InputX", Mathf.Abs(moveInput.x)); 
            animator.SetFloat("InputY", moveInput.y);
        }
    }

    // --- FUNGSI LAMA (BATAS KAMERA) TETAP ADA ---
    private void LateUpdate()
    {
        // 1. Cek Kelengkapan Data
        if (GameManager.Instance == null || GameManager.Instance.currentWorld == null) return;

        Vector2 min = GameManager.Instance.currentWorld.cameraMinBounds;
        Vector2 max = GameManager.Instance.currentWorld.cameraMaxBounds;

        Debug.Log("Min : " + min);
        Debug.Log("Max : " + max);
        // Jika batas belum diset (masih 0,0), batalkan
        if (min == Vector2.zero && max == Vector2.zero) return;

        // 2. HITUNG UKURAN BADAN OTOMATIS
        float halfWidth = 0.5f; 
        float halfHeight = 0.5f;

        if (spriteRenderer != null)
        {
            halfWidth = spriteRenderer.bounds.extents.x;
            halfHeight = spriteRenderer.bounds.extents.y;

            Debug.Log("Half Width : " + halfWidth);
            Debug.Log("Half Height : " + halfHeight);
        }
        else 
        {
            var col = GetComponent<Collider2D>();
            if(col != null)
            {
                halfWidth = col.bounds.extents.x;
                halfHeight = col.bounds.extents.y;
            }
        }

        // 3. KUNCI POSISI (CLAMP)
        Vector3 clampedPosition = transform.position;
        
        clampedPosition.x = Mathf.Clamp(transform.position.x, min.x + halfWidth, max.x - halfWidth);
        clampedPosition.y = Mathf.Clamp(transform.position.y, min.y + halfHeight, max.y - halfHeight);

        Debug.Log("Before Clamp : " + transform.position);
        Debug.Log("After Clamp : " + clampedPosition);

        transform.position = clampedPosition;
    }
}