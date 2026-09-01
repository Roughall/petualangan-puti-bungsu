using UnityEngine;

public class PlayerSideScroll : MonoBehaviour
{
    [Header("Platformer Stats")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck; // Titik di kaki
    public LayerMask groundLayer; // Layer lantai

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Wajib nyalakan Gravitasi untuk Side-Scrolling!
        rb.gravityScale = 3f; // Angka 3 biar lompatnya cepat (snappy)
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. Cek Kaki Nempel Tanah?
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // 2. Gerak Kiri-Kanan
        float x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        // Flip Badan
        if (x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (x < 0) transform.localScale = new Vector3(-1, 1, 1); // Flip pakai Scale lebih aman untuk anak objek

        // 3. Lompat (Hanya kalau di tanah)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }

        // 4. Menunduk (Crouch)
        if (Input.GetKey(KeyCode.DownArrow) && isGrounded)
        {
            animator.SetBool("IsCrouching", true);
            // Opsional: Kecilkan collider agar bisa lewat lorong sempit
        }
        else
        {
            animator.SetBool("IsCrouching", false);
        }

        // Animasi Jalan
        animator.SetBool("IsMoving", x != 0);
        animator.SetBool("IsGrounded", isGrounded);
    }
}