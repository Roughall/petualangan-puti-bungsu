using UnityEngine;

public class BossPlayer : MonoBehaviour
{
    [Header("Movement Stats")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    private bool lastMovingState;
    private bool lastGroundedState;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    [Header("Ground Detection")]
    public Transform groundCheck; // Objek kosong di kaki
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer; // Layer "Ground"

    // Private Variables
    private float horizontalInput;
    private bool isGrounded;
    private bool isJumping;
    private float lastFacingInput;
    private void Awake()
    {
        Debug.Log(
            "[BOSS PLAYER READY] " +
            gameObject.name
        );

        if (rb == null)
        {
            Debug.LogError(
                "[BOSS PLAYER ERROR] Rigidbody2D belum di-assign."
            );
        }

        if (animator == null)
        {
            Debug.LogWarning(
                "[BOSS PLAYER WARNING] Animator belum di-assign."
            );
        }

        if (spriteRenderer == null)
        {
            Debug.LogError(
                "[BOSS PLAYER ERROR] SpriteRenderer belum di-assign."
            );
        }

        if (groundCheck == null)
        {
            Debug.LogError(
                "[BOSS PLAYER ERROR] GroundCheck belum di-assign."
            );
        }

        if (groundLayer.value == 0)
        {
            Debug.LogError(
                "[BOSS PLAYER ERROR] Ground Layer belum di-assign."
            );
        }
    }
    void Update() // Input Player (Setiap Frame)
    {
        // 1. Baca Input (A/D atau Panah Kiri/Kanan)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(horizontalInput) > 0.1f &&
        horizontalInput != lastFacingInput)
        {
            Debug.Log(
                "[BOSS FACING] Direction = " +
                (horizontalInput > 0 ? "RIGHT" : "LEFT") +
                " | FlipX = " +
                spriteRenderer.flipX
            );

            lastFacingInput = horizontalInput;
        }

        // 2. Input Lompat (Spasi)
        // Hanya boleh lompat jika kaki menyentuh tanah (isGrounded)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true; // Kirim sinyal ke FixedUpdate
            
            // Trigger Animasi Lompat (Jika ada)
            if(animator) animator.SetTrigger("Jump");
        }

        // 3. Update Visual (Animasi & Arah Hadap)
        UpdateVisuals();
    }
    void FixedUpdate() // Eksekusi Fisika (Stabil)
    {
        // 4. Cek Apakah Kaki Nempel Tanah?
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 5. Gerakkan Karakter
        // Kita ubah Velocity X sesuai input, tapi biarkan Velocity Y (Gravitasi) apa adanya
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // 6. Eksekusi Lompat
        if (isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = false; // Reset sinyal
        }

        if (Mathf.Abs(rb.velocity.y) > 0.1f)
        {
            Debug.Log(
                "[BOSS ANIMATOR VERTICAL] vSpeed = " +
                rb.velocity.y
            );
        }
    }
    private void UpdateVisuals()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f;

        // --- PLAYER FACING ---
        if (spriteRenderer != null)
        {
            if (horizontalInput > 0.1f)
            {
                spriteRenderer.flipX = true;
            }
            else if (horizontalInput < -0.1f)
            {
                spriteRenderer.flipX = false;
            }
        }

        if (isMoving != lastMovingState)
        {
            Debug.Log(
                "[BOSS ANIMATOR MOVE] IsMoving = " +
                isMoving
            );

            lastMovingState = isMoving;
        }

        if (isGrounded != lastGroundedState)
        {
            Debug.Log(
                "[BOSS ANIMATOR GROUND] IsGrounded = " +
                isGrounded
            );

            lastGroundedState = isGrounded;
        }

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("vSpeed", rb.velocity.y);
        }
    }
    // Menggambar lingkaran merah di Scene untuk debug kaki
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}