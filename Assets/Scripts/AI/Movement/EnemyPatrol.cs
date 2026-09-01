using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform patrolPointA;
    public Transform patrolPointB;

    [Header("Movement")]
    public float patrolSpeed = 1.5f;

    [Header("Wait")]
    public float waitTime = 1f;

    private Rigidbody2D rb;

    private Transform currentTarget;
    private float waitTimer;
    private bool waiting;

    void Awake()
{
    rb = GetComponent<Rigidbody2D>();

    if (rb == null)
    {
        Debug.LogError(
            "EnemyPatrol: Rigidbody2D tidak ditemukan pada "
            + gameObject.name
        );

        return;
    }

    // Untuk game top-down:
    // Enemy tidak boleh dipengaruhi gravitasi.
    rb.gravityScale = 0f;

    // Enemy tidak boleh berputar.
    rb.constraints =
        RigidbodyConstraints2D.FreezeRotation;

    Debug.Log(
        "EnemyPatrol Ready : "
        + gameObject.name
        + " | Gravity = "
        + rb.gravityScale
    );
}

    void Start()
    {
        if (patrolPointA == null || patrolPointB == null)
        {
            Debug.LogError(
                "EnemyPatrol: Patrol Point belum lengkap pada "
                + gameObject.name
            );

            return;
        }

        currentTarget = patrolPointB;

        Debug.Log(
            "EnemyPatrol Ready : "
            + gameObject.name
        );
    }

    public void Patrol()
    {
        if (rb == null)
            return;

        if (patrolPointA == null || patrolPointB == null)
            return;

        if (waiting)
        {
            rb.velocity = Vector2.zero;

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                waiting = false;
                waitTimer = 0f;

                SwitchTarget();
            }

            return;
        }

        Vector2 direction =
            (currentTarget.position - transform.position).normalized;

        rb.velocity = direction * patrolSpeed;

        Debug.Log(gameObject.name + " PATROL MOVE | Target = " + currentTarget.name +
        " | Direction = " + direction + " | Velocity = " + rb.velocity);

        float distance =
            Vector2.Distance(
                transform.position,
                currentTarget.position
            );

        if (distance <= 0.15f)
        {
            rb.velocity = Vector2.zero;

            waiting = true;
            waitTimer = 0f;

            Debug.Log(
                "Patrol mencapai : "
                + currentTarget.name
            );
        }
    }

    void SwitchTarget()
    {
        if (currentTarget == patrolPointA)
        {
            currentTarget = patrolPointB;
        }
        else
        {
            currentTarget = patrolPointA;
        }

        Debug.Log(
            "Patrol menuju : "
            + currentTarget.name
        );
    }

    public void StopPatrol()
    {
        if (rb == null)
            return;

        rb.velocity = Vector2.zero;

        waiting = false;
        waitTimer = 0f;
    }

    
}