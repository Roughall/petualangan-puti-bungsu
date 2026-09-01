using UnityEngine;
using System.Collections;

public class MinionTopDownAI : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public float runSpeed = 3.5f; // Kecepatan saat kabur
    public float maxHealth = 30f;
    private float currentHealth;

    [Header("AI Sensors")]
    public float detectRange = 6f;    // Jarak melihat player
    public float attackRange = 1.2f;  // Jarak pukul (sedikit lebih jauh drpd side-scroll)
    public float fleeHealthThreshold = 10f; // Batas darah untuk mulai kabur

    [Header("Patrol Settings")]
    public float patrolRadius = 3f;   // Seberapa jauh dia jalan-jalan sendiri
    public float idleTime = 2f;       // Waktu bengong sebelum jalan lagi

    [Header("References")]
    public Transform player;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private EnemyFSM enemyFSM;

    // FSM States
    private enum State { Patrol, Chase, Attack, Flee }
    [SerializeField] private State currentState = State.Patrol;

    // Internal Variables
    private Vector2 patrolTarget;
    private float waitTimer;
    private float attackCooldown = 1.5f;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        Debug.Log("Saya berasal dari : " + gameObject.name);
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        enemyFSM = GetComponent<EnemyFSM>();
        if(enemyFSM == null)
        {
            Debug.LogError("EnemyFSM tidak ditemukan!");
        }
        else
        {
            Debug.Log("EnemyFSM berhasil terhubung.");
        }
        rb.gravityScale = 0; // WAJIB 0 UNTUK TOP DOWN!
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Biar gak muter

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // Set tujuan patroli awal acak
        SetNewPatrolTarget();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // --- OTAK ADAPTIF ---
        // Prioritas Utama: Kalau darah tipis, langsung paksa masuk mode KABUR (Flee)
        if (currentHealth <= fleeHealthThreshold && currentState != State.Flee)
        {
            currentState = State.Flee;
        }

        // --- FSM SWITCHER ---
        switch (currentState)
        {
            case State.Patrol:
                PatrolLogic(distToPlayer);
                break;
            case State.Chase:
                ChaseLogic(distToPlayer);
                break;
            case State.Attack:
                AttackLogic(distToPlayer);
                break;
            case State.Flee:
                FleeLogic(distToPlayer);
                break;
        }

        // Update Animasi
        if (animator)
        {
            // Bergerak jika kecepatan (magnitude) lebih dari 0.1
            animator.SetBool("IsMoving", rb.velocity.magnitude > 0.1f);
        }
        
        // Atur Arah Wajah (Kiri/Kanan)
        HandleFlip();
    }

    // ---------------- LOGIKA STATE ----------------

    void PatrolLogic(float dist)
    {
        // Gerak pelan ke titik acak
        MoveTo(patrolTarget, moveSpeed);

        // Jika sudah sampai di titik tujuan patroli
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            rb.velocity = Vector2.zero;
            waitTimer += Time.deltaTime;

            if (waitTimer >= idleTime)
            {
                SetNewPatrolTarget();
                waitTimer = 0;
            }
        }

        // TRANSISI: Lihat Player -> Kejar
        if (dist < detectRange)
        {
            currentState = State.Chase;
        }
    }

    void ChaseLogic(float dist)
    {
        // Gerak mendekati Player
        MoveTo(player.position, moveSpeed);

        // TRANSISI: Cukup dekat -> Serang
        if (dist <= attackRange)
        {
            currentState = State.Attack;
        }
        
        // TRANSISI: Player kabur jauh -> Balik Patroli
        if (dist > detectRange * 1.5f)
        {
            currentState = State.Patrol;
        }
    }

    void AttackLogic(float dist)
    {
        rb.velocity = Vector2.zero; // Berhenti memukul

        if (Time.time > lastAttackTime + attackCooldown)
        {
            if(animator) animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
            // Masukkan logika damage ke player di sini nanti
            Debug.Log("Minion TopDown: Serang!");
        }

        // Kalau player lari menjauh, kejar lagi
        if (dist > attackRange)
        {
            currentState = State.Chase;
        }
    }

    void FleeLogic(float dist)
    {
        // LOGIKA KABUR TOP-DOWN:
        // Hitung vektor arah DARI Player MENUJU Saya (Menjauh)
        Vector2 fleeDirection = (transform.position - player.position).normalized;
        
        // Lari ke arah tersebut
        rb.velocity = fleeDirection * runSpeed;

        // Jika sudah sangat jauh (aman), mungkin bisa berhenti atau tetap waspada
        // Disini kita buat dia lari terus sampai mati atau player hilang
    }

    // ---------------- FUNGSI PENDUKUNG ----------------

    void MoveTo(Vector2 target, float speed)
    {
        // Menggunakan MoveTowards agar gerakannya linear dan rapi di Top-Down
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
    }

    void SetNewPatrolTarget()
    {
        // Cari titik acak dalam radius X meter dari posisi sekarang
        float randomX = Random.Range(-patrolRadius, patrolRadius);
        float randomY = Random.Range(-patrolRadius, patrolRadius);
        patrolTarget = new Vector2(transform.position.x + randomX, transform.position.y + randomY);
    }

    void HandleFlip()
    {
        // Cek kecepatan horizontal (X)
        if (rb.velocity.x > 0.1f)
            spriteRenderer.flipX = false; // Jalan ke Kanan
        else if (rb.velocity.x < -0.1f)
            spriteRenderer.flipX = true;  // Jalan ke Kiri
    }
    
    // Fungsi dipanggil saat kena pukul
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if(animator) animator.SetTrigger("Hit");

        // Cek langsung apakah perlu kabur?
        if (currentHealth <= fleeHealthThreshold)
        {
            currentState = State.Flee;
            // Visual feedback ketakutan (opsional)
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f); // Jadi agak merah pucat
        }
    }
    
    // Visualisasi Debug di Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange); // Range deteksi
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Range serang
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius); // Area patroli
    }
}